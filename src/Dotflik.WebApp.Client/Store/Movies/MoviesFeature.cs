using System;
using System.Collections.Generic;

using Fluxor;
using Dotflik.Domain.Aggregates;

namespace Dotflik.WebApp.Client.Store.Movies
{
  /// <summary>
  /// The feature describing the <see cref="MoviesState"/> state.
  /// </summary>
  public class MoviesFeature : Feature<MoviesState>
  {
    /// <inheritdoc/>
    public override string GetName() => "Movies";

    /// <inheritdoc/>
    protected override MoviesState GetInitialState()
      => new MoviesState(Movies: new List<Movie>(), PageSize: 10, PageToken: string.Empty);

  }
}
