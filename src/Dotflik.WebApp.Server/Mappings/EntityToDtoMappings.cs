using System;
using System.Linq;

namespace Dotflik.WebApp.Server.Mappings
{
  /// <summary>
  /// Provide extension methods that map entity object to dto object
  /// </summary>
  public static class EntityToDtoMappings
  {
    /// <summary>
    /// Convert entity Movie object to protobuf Movie object
    /// </summary>
    /// <param name="movie">Entity movie</param>
    /// <returns>Protobuf movie</returns>
    public static Protobuf.Resources.Movie ToProtobuf(this Domain.Entities.Movie movie)
    {
      var protobufMovie = new Protobuf.Resources.Movie
      {
        Id = movie.Id,
        Title = movie.Title,
        Year = movie.Year,
        Director = movie.Director,
        BannerUrl = movie.BannerUrl ?? string.Empty
      };
      
      return protobufMovie;
    }

    public static Protobuf.Resources.Genre ToProtobuf(this Domain.Entities.Genre genre)
    {
      var protobufGenre = new Protobuf.Resources.Genre
      {
        Id = genre.Id,
        Name = genre.Name
      };
      return protobufGenre;
    }

    public static Protobuf.Resources.Star ToProtobuf(this Domain.Entities.Star star)
    {
      var protobufStar = new Protobuf.Resources.Star
      {
        Id = star.Id,
        Name = star.Name,
        BirthYear = star.BirthYear,
        Headshot = star.Headshot ?? string.Empty
      };
      return protobufStar;
    }

    //public static Protobuf.Movie.Movie ToProtobuf(this Domain.Aggregates.Movie movie)
    //{
    //  var protobufMovie = ((Domain.Entities.Movie)movie).ToProtobuf();

    //  var protobufGenres = movie.Genres.Select(g => g.ToProtobuf());
    //  var protobufStars = movie.Stars.Select(s => s.ToProtobuf());

    //  protobufMovie.Genres.AddRange(protobufGenres);
    //  protobufMovie.Stars.AddRange(protobufStars);

    //  return protobufMovie;
    //}

  }
}
