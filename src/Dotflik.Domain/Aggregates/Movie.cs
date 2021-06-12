using System;
using System.Collections.Generic;

namespace Dotflik.Domain.Aggregates
{
  public sealed class Movie : Entities.Movie
  {
    public List<Entities.Genre> Genres { get; init; } = new List<Entities.Genre>();

    public List<Entities.Star> Stars { get; init; } = new List<Entities.Star>();
  }
}
