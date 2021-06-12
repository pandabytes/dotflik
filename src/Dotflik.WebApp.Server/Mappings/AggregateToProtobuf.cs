using System;
using System.Linq;

namespace Dotflik.WebApp.Server.Mappings
{
  /// <summary>
  /// Provide extension methods that map aggregate object to protobuf object
  /// </summary>
  public static class AggregateToProtobuf
  {
    /// <summary>
    /// Convert entity Movie object to protobuf Movie object
    /// </summary>
    /// <param name="movie">Entity movie</param>
    /// <returns>Protobuf movie</returns>
    private static Protobuf.Resources.Movie ToProtobuf(this Domain.Entities.Movie movie)
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

    /// <summary>
    /// Convert entity Genre object to protobuf Genre object
    /// </summary>
    /// <param name="genre">Entity genre</param>
    /// <returns>Protobuf genre</returns>
    private static Protobuf.Resources.Genre ToProtobuf(this Domain.Entities.Genre genre)
    {
      var protobufGenre = new Protobuf.Resources.Genre
      {
        Id = genre.Id,
        Name = genre.Name
      };
      return protobufGenre;
    }

    /// <summary>
    /// Convert entity Star object to protobuf Star object
    /// </summary>
    /// <param name="star">Entity star</param>
    /// <returns>Protobuf star</returns>
    private static Protobuf.Resources.Star ToProtobuf(this Domain.Entities.Star star)
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

    /// <summary>
    /// Convert aggregate Movie object to protobuf aggregate Movie object
    /// </summary>
    /// <param name="movie">Aggregate movie</param>
    /// <returns>Protobuf aggregate movie</returns>
    public static Protobuf.MovieAggregate ToProtobuf(this Domain.Aggregates.Movie movie)
    {
      var protobufMovieAgg = new Protobuf.MovieAggregate();
      protobufMovieAgg.Movie = ((Domain.Entities.Movie)movie).ToProtobuf();

      protobufMovieAgg.Stars.AddRange(movie.Stars.Select(s => s.ToProtobuf()));
      protobufMovieAgg.Genres.AddRange(movie.Genres.Select(g => g.ToProtobuf()));

      return protobufMovieAgg;
    }

  }
}
