using System;
using System.Collections.Generic;
using Dotflik.Domain.Aggregates;

namespace Dotflik.WebApp.Client.Store.Movies
{
  /// <summary>
  /// The state updated by fetching data from the 
  /// gRPC movie service.
  /// </summary>
  public record MoviesState(IReadOnlyList<Movie> Movies, int PageSize, string PageToken);
}
