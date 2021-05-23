using System;

namespace Dotflik.Domain.Entities
{
  public class Movie
  {
    public string Id { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public int Year { get; init; }

    public string Director { get; init; } = string.Empty;

    public string? BannerUrl { get; init; }
  }
}
