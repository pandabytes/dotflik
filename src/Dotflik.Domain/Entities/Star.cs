using System;

namespace Dotflik.Domain.Entities
{
  public class Star
  {
    public string Id { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public int BirthYear { get; init; }

    public string? Headshot { get; init; }
  }
}
