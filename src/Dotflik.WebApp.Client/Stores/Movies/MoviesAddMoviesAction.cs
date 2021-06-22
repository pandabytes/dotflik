using System;
using System.Collections.Generic;
using Dotflik.Domain.Aggregates;

namespace Dotflik.WebApp.Client.Stores.Movies
{
  /// <summary>
  /// Add movies to the global store action
  /// </summary>
  public record MoviesAddMoviesAction(IEnumerable<Movie> Movies);
}
