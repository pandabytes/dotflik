using System;

namespace Dotflik.Domain.Entities
{
  //public record Movie(string Id, string Title, int Year, string Director, string? BannerUrl);

  public class Movie
  {
    public string Id { get; init; } = null!;

    public string Title { get; init; } = null!;

    public int Year { get; init; }

    public string Director { get; init; } = null!;

    public string? BannerUrl { get; init; }
  }
}
