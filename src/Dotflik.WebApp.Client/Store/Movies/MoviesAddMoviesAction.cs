using System;
using System.Collections.Generic;
using Dotflik.Domain.Aggregates;

namespace Dotflik.WebApp.Client.Store.Movies
{
  /// <summary>
  /// Add movies to the store action.
  /// </summary>
  public record MoviesAddMoviesAction(IEnumerable<Movie> Movies);
}
