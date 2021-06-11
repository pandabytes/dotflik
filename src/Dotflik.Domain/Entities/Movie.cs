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

    public override bool Equals(object? obj)
    {
      if (ReferenceEquals(this, obj))
      {
        return true;
      }

      var otherMovie = obj as Movie;
      if (otherMovie is not null)
      {
        return Id == otherMovie.Id && Director == otherMovie.Director &&
               Year == otherMovie.Year && BannerUrl == otherMovie.BannerUrl;
      }

      return false;
    }

    public override int GetHashCode()
    {
      unchecked
      {
        int hashcode = 1430287 * Id.GetHashCode() * Director.GetHashCode() 
                               * Year.GetHashCode() * (BannerUrl?.GetHashCode() ?? 1);
        return hashcode * 17;
      }
    }

    public static bool operator ==(Movie m1, Movie m2) => m1.Equals(m2);

    public static bool operator !=(Movie m1, Movie m2) => m1 != m2;

  }
}
