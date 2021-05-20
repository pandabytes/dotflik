using System;

namespace Fabflix.Domain.Entities
{
  public record Star(string Id, string Name, int BirthYear, string? Headshot);
}
