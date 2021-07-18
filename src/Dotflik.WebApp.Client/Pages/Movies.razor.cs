using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Fluxor;
using Fluxor.Blazor.Web.Components;
using Grpc.Core;
using Microsoft.AspNetCore.Components;

using Dotflik.Protobuf;
using Dotflik.Protobuf.Pagination;
using Dotflik.WebApp.Client.Mappings;
using Dotflik.WebApp.Client.Store.Movies;
using Dotflik.WebApp.Client.Components.Modals;

namespace Dotflik.WebApp.Client.Pages
{
  public partial class Movies : FluxorComponent
  {
    private const string MovieModalId = "movieModal";

    /// <summary>
    /// The currently selected movie.
    /// </summary>
    private Domain.Aggregates.Movie? m_selectMovie;

    /// <summary>
    /// The error modal that displays an error message telling
    /// user it is unable to fetch movies when the app first loads.
    /// </summary>
    private ErrorModal? UnableToFetchErrorModal { get; set; }

    /// <summary>
    /// The movie modal that contains more details of a movie.
    /// </summary>
    private MovieDetailsModal? MovieDetailsModal { get; set; }

    [Inject]
    protected MovieService.MovieServiceClient MovieService { get; set; } = null!;

    [Inject]
    protected GenreService.GenreServiceClient GenreService { get; set; } = null!;

    [Inject]
    protected IDispatcher Dispatcher { get; set; } = null!;

    [Inject]
    protected IState<MoviesState> MoviesState { get; set; } = null!;

    /// <inheritdoc/>
    protected async override Task OnInitializedAsync()
    {
      // Movies are not available initially so fetch them
      if (MoviesState.Value.Movies.Count == 0)
      {
        var tuple = await GetMovies(MoviesState.Value.PageSize);
        if (tuple is not null)
        {
          var (movies, _) = tuple.Value;
          Dispatcher.Dispatch(new MoviesAddMoviesAction(movies));
        }
        else
        {
          if (UnableToFetchErrorModal is not null)
          {
            await UnableToFetchErrorModal.OpenAsync();
          }
        }
      }
    }

    /// <summary>
    /// Get the movies via gRPC movie service.
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
    /// Open the selected movie.
    /// </summary>
    /// <param name="movie">Selected movie</param>
    private async Task OpenSelectedMovie(Domain.Aggregates.Movie movie)
    {
      m_selectMovie = movie;
      if (MovieDetailsModal is not null)
      {
        await MovieDetailsModal.OpenAsync();
      }
    }

  }
}
