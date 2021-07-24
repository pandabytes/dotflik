using Dotflik.Domain.Aggregates;
using Dotflik.Domain.Collections;

namespace Dotflik.WebApp.Client.Store.Movies
{
  /// <summary>
  /// The state updated by fetching data from the 
  /// gRPC movie service.
  /// </summary>
  public record MoviesState(ReadonlyOrderedDictionary<string, Movie> Movies, int PageSize, string PageToken);
}
