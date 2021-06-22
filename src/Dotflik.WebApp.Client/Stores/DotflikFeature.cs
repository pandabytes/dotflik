using System;
using System.Collections.Generic;

using Fluxor;
using Dotflik.Domain.Aggregates;

namespace Dotflik.WebApp.Client.Stores
{
  /// <summary>
  /// The feature describing the <see cref="DotflikState"/> state
  /// </summary>
  public class DotflikFeature : Feature<DotflikState>
  {
    /// <inheritdoc/>
    public override string GetName() => "Dotflik";

    /// <inheritdoc/>
    protected override DotflikState GetInitialState()
    {
      var emptyMovieService = new MoviesState(new List<Movie>(), 5);
      return new DotflikState(MovieState: emptyMovieService);
    }

  }
}
