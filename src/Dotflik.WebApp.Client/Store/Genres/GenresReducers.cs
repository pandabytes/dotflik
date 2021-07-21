using System;
using Fluxor;

namespace Dotflik.WebApp.Client.Store.Genres
{
  /// <summary>
  /// Contain reducers that manipulate the <see cref="GenresState"/>.
  /// </summary>
  public static class GenresReducers
  {
    /// <summary>
    /// Set the genre color mappings.
    /// </summary>
    /// <param name="state">Current state.</param>
    /// <param name="action">Action to set the genre color mappings.</param>
    /// <returns>Updated state with the set genre color mappings.</returns>
    [ReducerMethod]
    public static GenresState OnSetGenreColorMappingsAction(GenresState state, GenresSetGenreColorMappingsAction action)
      => state with { GenreColorMappings = action.GenreColorMappings };
  }
}
