using System;
using System.Collections.Generic;
using Dotflik.Domain.Entities;

namespace Dotflik.Domain.Aggregates
{
  public sealed class Star : Entities.Star
  {
    public List<Entities.Movie> Movies { get; set; } = new List<Entities.Movie>();
  }
}
