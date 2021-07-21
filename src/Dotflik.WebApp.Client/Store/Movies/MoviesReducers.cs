using System;
using System.Linq;
using Fluxor;

namespace Dotflik.WebApp.Client.Store.Movies
{
  /// <summary>
  /// Contain reducers that manipulate the <see cref="MoviesState"/>
  /// </summary>
  public static class MoviesReducers
  {
    /// <summary>
    /// Add movies to the store. If movies in <paramref name="action"/> are
    /// already in the store, then they will be skipped.
    /// </summary>
    /// <param name="state">Current state.</param>
    /// <param name="action">Action to add movies to the store.</param>
    /// <returns>Updated state with added movies.</returns>
    [ReducerMethod]
    public static MoviesState OnAddMovies(MoviesState state, MoviesAddMoviesAction action)
    {
      var currentMovies = state.Movies;

      // Only add movies that aren't in the store yet
      var moviesToAdd = action.Movies.Where(
        m => currentMovies.FirstOrDefault(x => x.Id == m.Id) is null);

      var updatedMoviesList = currentMovies.ToList();
      updatedMoviesList.AddRange(moviesToAdd);

      return state with { Movies = updatedMoviesList };
    }

    /// <summary>
    /// Set a new page token in the store. This new page token
    /// will be used request for the next batch of movies and it should
    /// be updated with the page token received from the movie service.
    /// </summary>
    /// <param name="state">Current state.</param>
    /// <param name="action">Action to set a new page token to the store.</param>
    /// <returns>Updated state with a new page token.</returns>
    [ReducerMethod]
    public static MoviesState OnSetPageToken(MoviesState state, MoviesSetPageTokenAction action)
      => state with { PageToken = action.PageToken };

  }
}
