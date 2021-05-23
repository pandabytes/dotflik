using System;

namespace Dotflik.WebApp.Server.Mappings
{
  /// <summary>
  /// Provide extension methods that map entity object to dto object
  /// </summary>
  internal static class EntityToDtoMappings
  {
    /// <summary>
    /// Convert entity Movie object to protobuf Movie object
    /// </summary>
    /// <param name="movie">Entity movie</param>
    /// <returns>Protobuf movie</returns>
    public static Protobuf.Movie.Movie ToProtobuf(this Domain.Entities.Movie movie)
    {
      var protobufMovie = new Protobuf.Movie.Movie
      {
        Id = movie.Id,
        Title = movie.Title,
        Year = movie.Year,
        Director = movie.Director,
        BannerUrl = movie.BannerUrl ?? string.Empty
      };

      return protobufMovie;
    }

  }
}
