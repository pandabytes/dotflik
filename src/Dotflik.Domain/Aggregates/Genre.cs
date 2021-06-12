using System;
using System.Collections.Generic;

namespace Dotflik.Domain.Aggregates
{
  public sealed class Genre : Entities.Genre
  {
    public List<Entities.Movie> Movies { get; init; } = new List<Entities.Movie>();
  }
}
