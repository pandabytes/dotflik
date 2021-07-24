using System.Collections.Specialized;
using System.Linq;
using Fluxor;

using Dotflik.Domain.Collections;
using Dotflik.Domain.Aggregates;

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

      // Populate with existing movies
      var updatedMoviesDict = new OrderedDictionary();
      foreach (var pair in currentMovies)
      {
        updatedMoviesDict.Add(pair.Key, pair.Value);
      }

      // Only add movies that aren't in the store yet
      var newMovies = action.Movies.Where(m => !updatedMoviesDict.Contains(m.Id));
      foreach (var movie in newMovies)
      {
        updatedMoviesDict.Add(movie.Id, movie);
      }

      return state with { Movies = new ReadonlyOrderedDictionary<string, Movie>(updatedMoviesDict) };
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
