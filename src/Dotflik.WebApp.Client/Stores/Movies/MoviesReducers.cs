using System;
using System.Linq;
using Fluxor;

namespace Dotflik.WebApp.Client.Stores.Movies
{
  /// <summary>
  /// Contain reducers that manipulate the <see cref="MoviesState"/>
  /// </summary>
  public static class MoviesReducers
  {
    /// <summary>
    /// Add movies to the store. If movies in <paramref name="action"/> are
    /// already in the store, then they will be skipped
    /// </summary>
    /// <param name="state">Current state of the store</param>
    /// <param name="action">Action to add movies to the store</param>
    /// <returns>Updated state with added movies</returns>
    [ReducerMethod]
    public static DotflikState OnAddMovies(DotflikState state, MoviesAddMoviesAction action)
    {
      var currentMovies = state.MovieState.Movies;

      // Only add movies that aren't in the store yet
      var moviesToAdd = action.Movies.Where(
        m => currentMovies.FirstOrDefault(x => x.Id == m.Id) is null);

      var updatedMoviesList = currentMovies.ToList();
      updatedMoviesList.AddRange(moviesToAdd);

      var newMovieState = state.MovieState with { Movies = updatedMoviesList };
      return state with { MovieState = newMovieState };
    }

  }
}
