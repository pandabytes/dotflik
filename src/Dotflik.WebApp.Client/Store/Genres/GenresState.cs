using System;
using System.Collections.Generic;

namespace Dotflik.WebApp.Client.Store.Genres
{
  /// <summary>
  /// The state updated by fetching data from the 
  /// gRPC genre service.
  /// </summary>
  public record GenresState(IReadOnlyDictionary<string, string> GenreColorMappings);
}
