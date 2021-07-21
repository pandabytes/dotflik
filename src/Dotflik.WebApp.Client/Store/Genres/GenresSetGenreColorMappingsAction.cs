using System;
using System.Collections.Generic;

namespace Dotflik.WebApp.Client.Store.Genres
{
  /// <summary>
  /// Set the genre color mappings in the store action.
  /// </summary>
  public record GenresSetGenreColorMappingsAction(Dictionary<string, string> GenreColorMappings);
}
