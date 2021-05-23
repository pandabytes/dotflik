using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

using Microsoft.Extensions.Logging;

using Dotflik.Protobuf.Movie;
using Dotflik.WebApp.Server.Mappings;
using Dotflik.Application.Repositories;
using Dotflik.Domain.Exceptions;

namespace Dotflik.WebApp.Server.Services
{
  /// <summary>
  /// gRPC implementation of movie service
  /// </summary>
  public class MovieService : Protobuf.Movie.MovieService.MovieServiceBase
  {
    private const int MaxPageSize = 50;

    private readonly ILogger<MovieService> m_logger;
    private readonly IMovieRepository m_movieRepository;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="movieRepository">Movie repository object</param>
    public MovieService(IMovieRepository movieRepository, ILogger<MovieService> logger)
      => (m_movieRepository, m_logger) = (movieRepository, logger);

    /// <inheritdoc/>
    public override async Task<ListMoviesResponse> ListMovies(ListMoviesRequest request, ServerCallContext context)
    {
      var pageSize = request.PageSize;
      var pageToken = request.PageToken;

      // Get the offset from the provided page token
      var offset = ParsePageToken(pageToken);
      if (offset == null)
      {
        var status = new Status(StatusCode.InvalidArgument, "page_token must be in this format \"offset=<int>\"");
        throw new RpcException(status);
      }

      // Constraint the page size to be within max range
      if (pageSize > MaxPageSize || pageSize <= 0)
      {
        pageSize = MaxPageSize;
      }

      IEnumerable<Domain.Entities.Movie> movies;
      try
      {
        movies = await m_movieRepository.GetAllAsync(pageSize, (int)offset);
      }
      catch (RepositoryException ex)
      {
        m_logger.LogError(ex.ToString());

        var status = new Status(StatusCode.Internal, "Something has gone wrong with getting movies from database");
        throw new RpcException(status);
      }

      var nextPageToken = $"offset={offset + pageSize}";
      var response = new ListMoviesResponse { NextPageToken = nextPageToken };
      var protobufMovies = movies.Select(m => m.ToProtobuf());
      response.Movies.AddRange(protobufMovies);

      return response;
    }

    /// <inheritdoc/>
    public override async Task<Movie> GetMovieById(GetMovieByIdRequest request, ServerCallContext context)
    {
      var id = request.Id;
      try
      {
        var movie = await m_movieRepository.GetByIdAsync(id);
        if (movie == null)
        {
          var status = new Status(StatusCode.InvalidArgument, $"Unable to find movie with id {id}");
          throw new RpcException(status);
        }

        return movie.ToProtobuf();
      }
      catch (RepositoryException ex)
      {
        m_logger.LogError(ex.ToString());

        var status = new Status(StatusCode.Internal, $"Something has gone wrong with getting movie with id={id} from database");
        throw new RpcException(status);
      }
    }

    /// <summary>
    /// Parse the page token to get the offset
    /// </summary>
    /// <param name="pageToken">Page token</param>
    /// <returns>If successful, an offset value is returned. Else null</returns>
    private static int? ParsePageToken(string pageToken)
    {
      var tokens = pageToken.Split('=');

      if (!pageToken.Contains("offset") || tokens.Length != 2)
      {
        return null;
      }

      var isInt = int.TryParse(tokens[1], out int offset);
      return isInt ? offset : null;
    }

  }
}
