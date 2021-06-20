using System;
using System.Linq;

namespace Dotflik.WebApp.Client.Mappings
{
  /// <summary>
  /// Provide extension methods that map protobuf object to aggregate object
  /// </summary>
  public static class ProtobufToAggregate
  {
    /// <summary>
    /// Convert protobuf Movie object to entity Movie object
    /// </summary>
    /// <remarks>
    /// If banner url in the protobuf object is empty, then a default
    /// url will be used to indicate the movie banner is not available
    /// </remarks>
    /// <param name="protobufMovie">Protobuf movie</param>
    /// <returns>Entity movie</returns>
    private static Domain.Entities.Movie ToEntity(this Protobuf.Resources.Movie protobufMovie)
    {
      const string unavailableBannerUrl = "http://www.interlog.com/~tfs/images/posters/TFSMoviePosterUnavailable.jpg";
      var bannerUrl = string.IsNullOrWhiteSpace(protobufMovie.BannerUrl) ? unavailableBannerUrl : protobufMovie.BannerUrl;

      var movie = new Domain.Entities.Movie
      {
        Id = protobufMovie.Id,
        Title = protobufMovie.Title,
        Year = protobufMovie.Year,
        Director = protobufMovie.Director,
        BannerUrl = bannerUrl
      };
      
      return movie;
    }

    /// <summary>
    /// Convert protobuf Genre object to entity Genre object
    /// </summary>
    /// <param name="protobufGenre">Protobuf genre</param>
    /// <returns>Protobuf genreEntity genre</returns>
    private static Domain.Entities.Genre ToEntity(this Protobuf.Resources.Genre protobufGenre)
    {
      var genre = new Domain.Entities.Genre
      {
        Id = protobufGenre.Id,
        Name = protobufGenre.Name
      };
      return genre;
    }

    /// <summary>
    /// Convert protobuf Star object to entity Star object 
    /// </summary>
    /// <remarks>
    /// If headshot url in the protobuf object is empty, then a default
    /// url will be used to indicate the headshot is not available
    /// </remarks>
    /// <param name="protobufStar">Protobuf star</param>
    /// <returns>Entity star</returns>
    private static Domain.Entities.Star ToEntity(this Protobuf.Resources.Star protobufStar)
    {
      const string unavailableHeadshotUrl = "http://getdrawings.com/img/unknown-person-silhouette-31.png";
      var headshot = string.IsNullOrWhiteSpace(protobufStar.Headshot) ? unavailableHeadshotUrl : protobufStar.Headshot;

      var star = new Domain.Entities.Star
      {
        Id = protobufStar.Id,
        Name = protobufStar.Name,
        BirthYear = protobufStar.BirthYear,
        Headshot = headshot
      };
      return star;
    }

    /// <summary>
    /// Convert protobuf aggregate Movie object to aggregate Movie object
    /// </summary>
    /// <param name="protobufMovieAggr">Protobuf aggregate movie</param>
    /// <returns>Domain aggregate movie</returns>
    public static Domain.Aggregates.Movie ToDomainAggregate(this Protobuf.MovieAggregate protobufMovieAggr)
    {
      var movie = protobufMovieAggr.Movie.ToEntity();
      var stars = protobufMovieAggr.Stars.Select(s => s.ToEntity());
      var genres = protobufMovieAggr.Genres.Select(g => g.ToEntity());

      var movieAggr = new Domain.Aggregates.Movie
      {
        Id = movie.Id,
        Director = movie.Director,
        Year = movie.Year,
        Title = movie.Title,
        BannerUrl = movie.BannerUrl
      };

      movieAggr.Stars.AddRange(stars);
      movieAggr.Genres.AddRange(genres);

      return movieAggr;
    }

  }
}
