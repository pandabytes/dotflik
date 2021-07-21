using System;
using System.Collections.Generic;

using Fluxor;

namespace Dotflik.WebApp.Client.Store.Genres
{
  /// <summary>
  /// The feature describing the <see cref="GenresState"/> state.
  /// </summary>
  public class GenresFeature : Feature<GenresState>
  {
    /// <inheritdoc/>
    public override string GetName() => "Genres";

    /// <inheritdoc/>
    protected override GenresState GetInitialState()
      => new GenresState(new Dictionary<string, string>());
  }
}
