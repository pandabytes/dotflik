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
    /// <summary>
    /// The currently selected movie.
    /// </summary>
    private Domain.Aggregates.Movie? m_selectMovie;

    /// <summary>
    /// The current page token to request 
    /// for the next set of movies.
    /// </summary>
    private string m_pageToken = string.Empty;

    /// <summary>
    /// The error modal that displays an error message telling
    /// user it is unable to fetch movies when the app first loads.
    /// </summary>
    /// <remarks>
    /// Should not be null since it is referenced from the razor code.
    /// </remarks>
    private ErrorModal UnableToFetchErrorModal { get; set; } = null!;

    /// <summary>
    /// The movie modal that contains more details of a movie.
    /// </summary>
    /// <remarks>
    /// Should not be null since it is referenced from the razor code.
    /// </remarks>
    private MovieDetailsModal MovieDetailsModal { get; set; } = null!;

    /// <summary>
    /// The text of the load more movies button.
    /// </summary>
    private string LoadMoreMoviesButtonText { get; set; } = "Load more movies";

    /// <summary>
    /// Determine whether the load movie button is enabled.
    /// </summary>
    private bool LoadMoreMoviesButtonEnable => LoadMoreMoviesButtonText == "Load more movies";

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
        await LoadMovies();
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
      await MovieDetailsModal.OpenAsync();
    }

    /// <summary>
    /// Load movies using the token <see cref="m_pageToken"/>
    /// to the <see cref="MoviesState"/>. This method will also
    /// update <see cref="m_pageToken"/> to the next page token
    /// received from the movie service.
    /// </summary>
    /// <remarks>
    /// If the method is unable to load movies, it will display
    /// the <see cref="UnableToFetchErrorModal"/> modal.
    /// </remarks>
    /// <returns>Empty task</returns>
    private async Task LoadMovies()
    {
      var tuple = await GetMovies(MoviesState.Value.PageSize, m_pageToken);
      if (tuple is not null)
      {
        var (movies, newPageToken) = tuple.Value;
        m_pageToken = newPageToken;
        Dispatcher.Dispatch(new MoviesAddMoviesAction(movies));
      }
      else
      {
        await UnableToFetchErrorModal.OpenAsync();
      }      
    }

    /// <summary>
    /// Load more movies when the button is clicked and also
    /// disable the button so that it can't be clicked
    /// multiple times when it's already clicked.
    /// </summary>
    /// <returns>Empty task</returns>
    private async Task LoadMoreMoviesButtonClicked()
    {
      LoadMoreMoviesButtonText = "Loading...";

      await LoadMovies();

      LoadMoreMoviesButtonText = "Load more movies";
    }

  }
}
