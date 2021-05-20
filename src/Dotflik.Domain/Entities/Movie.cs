using System;

namespace Fabflix.Domain.Entities
{
  public record Movie(string Id, string Title, int Year, string Director, string? BannerUrl);
}
