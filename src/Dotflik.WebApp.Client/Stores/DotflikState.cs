using System;
using System.Collections.Generic;
using Dotflik.Domain.Aggregates;

namespace Dotflik.WebApp.Client.Stores
{
  /// <summary>
  /// The global state of the Dotflik web application
  /// </summary>
  public record DotflikState(MoviesState MovieState);

  /// <summary>
  /// The state updated by fetching data from the 
  /// gRPC movie service
  /// </summary>
  public record MoviesState(IReadOnlyList<Movie> Movies, int PageSize);
}
