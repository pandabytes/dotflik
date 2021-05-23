using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

using Dotflik.Protobuf.Movie;
using Dotflik.WebApp.Server.Mappings;
using Dotflik.Application.Repositories;

namespace Dotflik.WebApp.Server.Services
{
  /// <summary>
  /// gRPC implementation of movie service
  /// </summary>
  public class MovieService : Protobuf.Movie.MovieService.MovieServiceBase
  {
    private const int MaxPageSize = 50;

    private readonly IMovieRepository m_movieRepository;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="movieRepository">Movie repository object</param>
    public MovieService(IMovieRepository movieRepository)
      => m_movieRepository = movieRepository;

    /// <inheritdoc/>
    public override async Task<GetMoviesResponse> ListMovies(GetMoviesRequest request, ServerCallContext context)
    {
      var pageSize = request.PageSize;
      var pageToken = request.PageToken;
      var offset = 0;

      // Get the offset from the provided page token
      if (!string.IsNullOrWhiteSpace(pageToken))
      {
        var tokens = pageToken.Split('&');
        offset = int.Parse(tokens[1].Split('=')[1]);
      }

      // Constraint the page size to be within max range
      if (pageSize > MaxPageSize || pageSize <= 0)
      {
        pageSize = MaxPageSize;
      }

      var movies = await m_movieRepository.GetAllAsync(pageSize, offset);
      
      var response = new GetMoviesResponse { NextPageToken = "next_token" };
      var protobufMovies = movies.Select(m => m.ToProtobuf());
      response.Movies.AddRange(protobufMovies);

      var nextPageToken = $"limit={pageSize}&offset={offset + pageSize}"; ;
      response.NextPageToken = nextPageToken;

      return response;
    }


    //public override async Task GetMovies(Empty request, IServerStreamWriter<Protobuf.Movie.Movie> responseStream, ServerCallContext context)
    //{
    //  //await GetMovies();

    //  for (int i = 0; i < 5; i++)
    //  {
    //    await using (var connection = new NpgsqlConnection(m_connectionString))
    //    {
    //      await connection.OpenAsync();
    //      await using (var cmd = new NpgsqlCommand("SELECT * FROM movies limit 1", connection))
    //      await using (var reader = await cmd.ExecuteReaderAsync())
    //      {
    //        while (await reader.ReadAsync())
    //        {
    //          await Task.Delay(1000);

    //          //Console.WriteLine(reader.GetString(0));
    //          var id = reader.GetString(0);
    //          var title = reader.GetString(1);
    //          var year = reader.GetInt32(2);
    //          var director = reader.GetString(3);
    //          string bannerUrl = string.Empty;

    //          if (!reader.IsDBNull(4))
    //          {
    //            bannerUrl = reader.GetString(4);
    //          }

    //          var response = new Protobuf.Movie.Movie { Id = id, Title = title, Year = year, Director = director, BannerUrl = bannerUrl };
    //          await responseStream.WriteAsync(response);
    //        }
    //      }

    //      var movies = connection.Query<Protobuf.Movie.Movie>("SELECT * FROM movies LIMIT 0");

    //    }

    //    //var response = new Movie { Id = (i+1).ToString(), Year = 2021 + i + 1};
    //    //await responseStream.WriteAsync(response);
    //    break;
    //  }
    //}

    //private async Task GetMovies()
    //{
    //  await using (var connection = new NpgsqlConnection(m_connectionString))
    //  {
    //    await connection.OpenAsync();
    //    //connection.Execute("SELECT * FROM movies LIMIT 5");
    //    //var movies = connection.Query<Protobuf.Movie.Movie>("SELECT * FROM movies LIMIT 5");

    //    var builder = new SqlBuilder();
    //    //var template = builder.AddTemplate("SELECT * FROM movies /**where**/");

    //    //var movie = new Domain.Entities.Movie("", "", 0, "Mel Gibson", "https");
    //    ////builder.OrWhere("director = @Director", new { movie.Director });
    //    //builder.Where("id = @Id", new { movie.Director });
    //    //builder.Where("year > @Year", new { movie.Director });
    //    //builder.OrWhere("director = @Director", new { movie.Director });
    //    //Console.WriteLine(movie.ToString());
    //    //Console.WriteLine(template.RawSql);

    //    //var movies = connection.Query<Protobuf.Movie.Movie>(template.RawSql, template.Parameters);
    //  }
    //}

  }
}
