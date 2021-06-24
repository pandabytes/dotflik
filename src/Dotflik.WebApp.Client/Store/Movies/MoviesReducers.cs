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
    /// already in the store, then they will be skipped
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="action">Action to add movies to the store</param>
    /// <returns>Updated state with added movies</returns>
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

  }
}
