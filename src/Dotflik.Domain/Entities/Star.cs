using System;

namespace Dotflik.Domain.Entities
{
  public class Star
  {
    public string Id { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public int? BirthYear { get; init; }

    public string? Headshot { get; init; }

    public override bool Equals(object? obj)
    {
      if (ReferenceEquals(this, obj))
      {
        return true;
      }

      var otherStar = obj as Star;
      if (otherStar is not null)
      {
        return Id == otherStar.Id && Name == otherStar.Name &&
               BirthYear == otherStar.BirthYear && Headshot == otherStar.Headshot;
      }

      return false;
    }

    public override int GetHashCode()
    {
      unchecked
      {
        int hashcode = 1430287 * Id.GetHashCode() * Name.GetHashCode() *
                       (BirthYear?.GetHashCode() ?? 1) * (Headshot?.GetHashCode() ?? 1);
        return hashcode * 17;
      }
    }

    public static bool operator ==(Star s1, Star s2) => s1.Equals(s2);

    public static bool operator !=(Star s1, Star s2) => s1 != s2;



  }
}
