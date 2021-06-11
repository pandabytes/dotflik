using System;
using System.Collections.Generic;

namespace Dotflik.Domain.Entities
{
  public class Genre
  {
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public override bool Equals(object? obj)
    {
      if (ReferenceEquals(this, obj))
      {
        return true;
      }

      var otherGenre = obj as Genre;
      if (otherGenre is not null)
      {
        return Id == otherGenre.Id && Name == otherGenre.Name;
      }

      return false;
    }

    public override int GetHashCode()
    {
      unchecked
      {
        int hashcode = 1430287 * Id.GetHashCode() * Name.GetHashCode();
        return hashcode * 17;
      }
    }

    public static bool operator ==(Genre g1, Genre g2) => g1.Equals(g2);

    public static bool operator !=(Genre g1, Genre g2) => g1 != g2;


  }
}
