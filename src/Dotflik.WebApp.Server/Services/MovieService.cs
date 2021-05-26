using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

using Microsoft.Extensions.Logging;

using Dotflik.Protobuf.Movie;
using Dotflik.Application.Repositories;
using Dotflik.Domain.Exceptions;
using Dotflik.WebApp.Server.Mappings;

namespace Dotflik.WebApp.Server.Services
{
  /// <summary>
  /// gRPC implementation of movie service
  /// </summary>
  public class MovieService : Protobuf.Movie.MovieService.MovieServiceBase
  {
    private const int MaxPageSize = 50;
    private const string PageTokenFormat = "offset={0}";

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
        var sampleToken = string.Format(PageTokenFormat, "N");
        var status = new Status(StatusCode.InvalidArgument, $"page_token must be in this format \"{sampleToken}\"");
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

      // If the returned movies count is less than the page size, then it means
      // there is no more movies to get so set the next page token to empty
      var nextPageToken = movies.Count() >= pageSize ?
        string.Format(PageTokenFormat, offset + pageSize) : string.Empty;

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

    /// <inheritdoc/>
    public override async Task<Movie> GetMovieByTitle(GetMovieByTitleRequest request, ServerCallContext context)
    {
      var title = request.Title;
      try
      {
        var movie = await m_movieRepository.GetByTitleAsync(title);
        if (movie == null)
        {
          var status = new Status(StatusCode.InvalidArgument, $"Unable to find movie with title \"{title}\"");
          throw new RpcException(status);
        }

        return movie.ToProtobuf();
      }
      catch (RepositoryException ex)
      {
        m_logger.LogError(ex.ToString());

        var status = new Status(StatusCode.Internal, $"Something has gone wrong with getting movie with id={title} from database");
        throw new RpcException(status);
      }
    }

    /// <inheritdoc/>
    public override async Task<ListMoviesByYearResponse> ListMoviesByYear(ListMoviesByYearRequest request, ServerCallContext context)
    {
      var pageSize = request.PageSize;
      var pageToken = request.PageToken;
      var fromYear = request.From;
      var toYear = request.To;

      if (fromYear > toYear)
      {
        var status = new Status(StatusCode.InvalidArgument, $"\"From year\" {fromYear} must be less than \"to year\" {toYear}");
        throw new RpcException(status);
      }

      // Get the offset from the provided page token
      var offset = ParsePageToken(pageToken);
      if (offset == null)
      {
        var sampleToken = string.Format(PageTokenFormat, "N");
        var status = new Status(StatusCode.InvalidArgument, $"page_token must be in this format \"{sampleToken}\"");
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
        var sortAscending = (request.SortYear == ListMoviesByYearRequest.Types.SortYear.Asc);
        movies = await m_movieRepository.GetMoviesByYear(pageSize, (int)offset, fromYear, toYear, sortAscending);
      }
      catch (RepositoryException ex)
      {
        m_logger.LogError(ex.ToString());

        var status = new Status(StatusCode.Internal, "Something has gone wrong with getting movies from database");
        throw new RpcException(status);
      }

      // If the returned movies count is less than the page size, then it means
      // there is no more movies to get so set the next page token to empty
      var nextPageToken = movies.Count() >= pageSize ? 
        string.Format(PageTokenFormat, offset + pageSize) : string.Empty;

      var response = new ListMoviesByYearResponse { NextPageToken = nextPageToken };
      var protobufMovies = movies.Select(m => m.ToProtobuf());
      response.Movies.AddRange(protobufMovies);

      return response;
    }

    /// <summary>
    /// Parse the page token to get the offset
    /// </summary>
    /// <remarks>
    /// If <paramref name="pageToken"/> is empty, then 0 is returned
    /// </remarks>
    /// <param name="pageToken">Page token</param>
    /// <returns>If successful, an offset value is returned. Else null</returns>
    private static int? ParsePageToken(string pageToken)
    {
      if (string.IsNullOrWhiteSpace(pageToken))
      {
        return 0;
      }

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
