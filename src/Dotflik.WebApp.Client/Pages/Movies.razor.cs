using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Grpc.Core;
using Dotflik.Protobuf;
using Dotflik.Protobuf.Pagination;
using Dotflik.WebApp.Client.Mappings;
using Dotflik.WebApp.Client.Store;
using Dotflik.WebApp.Client.Store.Movies;
using Microsoft.AspNetCore.Components;

namespace Dotflik.WebApp.Client.Pages
{
  public partial class Movies : FluxorComponent
  {
    private const string MovieModalId = "movieModal";

    [Inject]
    protected MovieService.MovieServiceClient MovieService { get; set; } = null!;

    [Inject]
    protected GenreService.GenreServiceClient GenreService { get; set; } = null!;

    [Inject]
    protected IDispatcher Dispatcher { get; set; } = null!;

    [Inject]
    protected IState<MoviesState> MoviesState { get; set; } = null!;

    /// <summary>
    /// Indicate an error has occured on this page
    /// </summary>
    private bool m_errorOccurred;

    /// <summary>
    /// The currently selected movie
    /// </summary>
    private Domain.Aggregates.Movie? m_selectMovie;

    /// <inheritdoc/>
    protected async override Task OnInitializedAsync()
    {
      var tuple = await GetMovies(5);
      if (tuple is not null)
      {
        var (movies, _) = tuple.Value;
        Dispatcher.Dispatch(new MoviesAddMoviesAction(movies));
      }

      m_errorOccurred = tuple is null;
    }

    /// <summary>
    /// Get the movies via gRPC movie service
    /// </summary>
    /// <param name="pageSize"></param>
    /// <param name="token"></param>
    /// <returns>Tuple where 1st item is the list of movies and
    /// 2nd item is the next token</returns>
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

    /// <summary>
    /// Save the selected movie
    /// </summary>
    /// <param name="movie">Selected movie</param>
    private void SelectMovie(Domain.Aggregates.Movie movie)
      => m_selectMovie = movie;

  }
}
