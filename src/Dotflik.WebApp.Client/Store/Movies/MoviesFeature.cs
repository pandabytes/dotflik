using Fluxor;
using Dotflik.Domain.Aggregates;
using Dotflik.Domain.Collections;

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
      => new MoviesState(Movies: new ReadonlyOrderedDictionary<string, Movie>(), 
                         PageSize: 10, 
                         PageToken: string.Empty);
    
  }
}

