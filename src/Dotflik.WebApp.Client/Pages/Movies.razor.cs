using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

using Grpc.Core;
using Dotflik.Protobuf;
using Dotflik.Protobuf.Pagination;
using Dotflik.WebApp.Client.Mappings;

namespace Dotflik.WebApp.Client.Pages
{
  public partial class Movies : ComponentBase
  {
    private const string MovieModalId = "movieModal";

    private bool m_errorOccurred;
    private List<Domain.Aggregates.Movie>? m_movies;
    private Domain.Aggregates.Movie? m_selectMovie;

    [Inject]
    protected MovieService.MovieServiceClient MovieService { get; set; } = null!;

    [Inject]
    protected GenreService.GenreServiceClient GenreService { get; set; } = null!;

    /// <inheritdoc/>
    protected async override Task OnInitializedAsync()
    {
      var tuple = await GetMovies(5);
      if (tuple is not null)
      {
        var (movies, _) = tuple.Value;
        m_movies = movies;
      }

      m_errorOccurred = tuple is null;
    }

    private async Task PageSizeUpdateHandler(ChangeEventArgs events)
    {
      m_movies = null;

      var isNumber = int.TryParse(events.Value?.ToString(), out int pageSize);
      if (isNumber)
      {
        var tuple = await GetMovies(pageSize);

        if (tuple is not null)
        {
          var (movies, _) = tuple.Value;
          m_movies = movies;
        }
      }
    }

    private async Task<(List<Domain.Aggregates.Movie>, string)?> GetMovies(int pageSize, string token = "")
    {
      try
      {
        var request = new PaginationRequest { PageSize = pageSize, PageToken = token };
        var response = await MovieService.ListMoviesAsync(request);

        var movies = response.Movies.Select(m => m.ToDomainAggregate()).ToList();
        return (movies, response.PaginationResponse.NextPageToken);
      }
      catch (RpcException ex)
      {
        Console.Error.WriteLine(ex.Message);
      }
      return null;
    }

    private void SelectMovie(Domain.Aggregates.Movie movieAggr)
    {
      m_selectMovie = movieAggr;
    }

  }
}
