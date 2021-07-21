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
    /// Number of movies to be rendered per row.
    /// </summary>
    private const int NumMoviesPerRow = 5;

    /// <summary>
    /// Number of rows that would contain
    /// at most <see cref="NumMoviesPerRow"/>.
    /// </summary>
    private int NumMovieRows
    {
      get
      {
        var movieCount = MoviesState.Value.Movies.Count;
        if (movieCount == 0)
        {
          return 0;
        }
        if (movieCount <= NumMoviesPerRow)
        {
          return 1;
        }
        
        var remainer = movieCount % NumMoviesPerRow;
        return (movieCount / NumMoviesPerRow) + remainer;
      }
    }

    /// <summary>
    /// Get a shallow list of movies within the given range.
    /// If the range is out of bound then an empty list is returned
    /// or the number of movies up until <paramref name="count"/> 
    /// are returned.
    /// </summary>
    /// <param name="index">Start index of the range</param>
    /// <param name="count">Number of movies to get</param>
    /// <returns>Shallow list of movies</returns>
    private List<Domain.Aggregates.Movie> GetMoviesRange(int index, int count)
    {
      var subList = new List<Domain.Aggregates.Movie>();
      var movies = MoviesState.Value.Movies;
      var movieCount = movies.Count;

      if (movieCount > 0)
      {
        for (int i = index; (i < index + count) && (i < movieCount); i++)
        {
          subList.Add(movies[i]);
        }
      }

      return subList;
    }

    /// <summary>
    /// The currently selected movie.
    /// </summary>
    private Domain.Aggregates.Movie? m_selectMovie;

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
    protected IDispatcher Dispatcher { get; set; } = null!;

    [Inject]
    protected IState<MoviesState> MoviesState { get; set; } = null!;

    [Inject]
    protected GetGenreColor GetGenreColor { get; set; } = null!;

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
    /// <param name="pageSize">Number of movies to request.</param>
    /// <param name="token">The token to get the next batch of movies.</param>
    /// <param name="timeout">Number of seconds to wait for the service to return.</param>
    /// <returns>Tuple where 1st item is the list of movies and
    /// 2nd item is the next token.</returns>
    private async Task<(List<Domain.Aggregates.Movie>, string)?> GetMovies(int pageSize, string token = "", int timeout = 3)
    {
      try
      {
        var grpcCallOptions = new CallOptions(deadline: DateTime.UtcNow.AddSeconds(timeout));
        var request = new PaginationRequest { PageSize = pageSize, PageToken = token };
        var response = await MovieService.ListMoviesAsync(request, grpcCallOptions);

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
    /// <returns>Empty task.</returns>
    private async Task LoadMovies()
    {
      var (pageSize, pageToken) = (MoviesState.Value.PageSize, MoviesState.Value.PageToken);
      var tuple = await GetMovies(pageSize, pageToken);
      if (tuple is not null)
      {
        var (movies, newPageToken) = tuple.Value;
        Dispatcher.Dispatch(new MoviesSetPageTokenAction(newPageToken));
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
