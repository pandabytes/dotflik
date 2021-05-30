using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

using Microsoft.Extensions.Logging;

using Dotflik.Protobuf.Movie;
using Dotflik.Protobuf.Pagination;

using Dotflik.Application.Pagination;
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
    protected const int MaxPageSize = 50;
    protected const string PageTokenFormat = "limit={0}&offset={1}";

    private readonly ILogger<MovieService> m_logger;
    private readonly IMovieRepository m_movieRepository;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="movieRepository">Movie repository object</param>
    public MovieService(IMovieRepository movieRepository, ILogger<MovieService> logger)
      => (m_movieRepository, m_logger) = (movieRepository, logger);

    /// <inheritdoc/>
    public override Task<GetMaxPageSizeResponse> GetMaxPageSize(Empty request, ServerCallContext context)
    {
      var response = new GetMaxPageSizeResponse { MaxPageSize = MaxPageSize };
      return Task.FromResult(response);
    }

    /// <inheritdoc/>
    public override async Task<ListMoviesResponse> ListMovies(PaginationRequest request, ServerCallContext context)
    {
      var (pageSize, pageToken) = (request.PageSize, request.PageToken);

      IEnumerable<Domain.Entities.Movie> movies;
      int limit, offset;
      try
      {
        var offsetPageToken = new OffsetPageToken(pageToken);
        (limit, offset) = (offsetPageToken.Limit, offsetPageToken.Offset);

        // Ensure the page size and limit in the token are consistent
        // If offset is 0, it means we're getting results for 1st page and
        // limit can be safely ignored since we'll use pageSize instead
        if (pageSize != limit && offset != 0)
        {
          throw new ArgumentException("page token is not consistent with page size.");
        }

        // This check verifies whether the offset in the token is consistent
        // with the limit. The logic here is the limit should always be divisible
        // by the offset and if it's false, then the client is not being
        // consistent with the request
        if (offset > 0 && offset % limit != 0)
        {
          throw new ArgumentException(@"Bad page token value. Please make sure to use the 
            token from the service response. Otherwise set page token to empty.");
        }

        // Constraint the page size to be within max range
        if (pageSize > MaxPageSize || pageSize == 0)
        {
          pageSize = MaxPageSize;
        }

        movies = await m_movieRepository.GetAllAsync(pageSize, offset);
      }
      catch (Exception ex)
      {
        if (ex is BadPageTokenFormatException || ex is ArgumentException)
        {
          var status = new Status(StatusCode.InvalidArgument, ex.Message);
          throw new RpcException(status);
        }
        else if (ex is RepositoryException)
        {
          var status = new Status(StatusCode.Internal, "Something has gone wrong with getting movies from database.");
          throw new RpcException(status);
        }

        // If it reaches here, it means something unexpected happened
        m_logger.LogError(ex.ToString());
        throw;
      }

      // If the returned movies count is less than the page size, then it means
      // there is no more movies to get so set the next page token to empty
      var nextPageToken = string.Empty;
      if (movies.Count() >= pageSize)
      {
        var nextOffsetPageToken = new OffsetPageToken { Limit = pageSize, Offset = offset + pageSize };
        nextPageToken = nextOffsetPageToken.ToToken();
      }

      var paginationResponse = new PaginationRespone { NextPageToken = nextPageToken };
      var response = new ListMoviesResponse { PaginationResponse = paginationResponse };
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
      //var pageSize = request.PageSize;
      //var pageToken = request.PageToken;
      //var fromYear = request.From;
      //var toYear = request.To;

      //if (fromYear > toYear)
      //{
      //  var status = new Status(StatusCode.InvalidArgument, $"\"From year\" {fromYear} must be less than \"to year\" {toYear}");
      //  throw new RpcException(status);
      //}

      //// Get the offset from the provided page token
      //var offset = ParsePageToken(pageToken);
      //if (offset == null)
      //{
      //  var sampleToken = string.Format(PageTokenFormat, "N");
      //  var status = new Status(StatusCode.InvalidArgument, $"page_token must be in this format \"{sampleToken}\"");
      //  throw new RpcException(status);
      //}

      //// Constraint the page size to be within max range
      //if (pageSize > MaxPageSize || pageSize <= 0)
      //{
      //  pageSize = MaxPageSize;
      //}

      //IEnumerable<Domain.Entities.Movie> movies;
      //try
      //{
      //  var sortAscending = (request.SortYear == ListMoviesByYearRequest.Types.SortYear.Asc);
      //  movies = await m_movieRepository.GetMoviesByYear(pageSize, (int)offset, fromYear, toYear, sortAscending);
      //}
      //catch (RepositoryException ex)
      //{
      //  m_logger.LogError(ex.ToString());

      //  var status = new Status(StatusCode.Internal, "Something has gone wrong with getting movies from database");
      //  throw new RpcException(status);
      //}

      //// If the returned movies count is less than the page size, then it means
      //// there is no more movies to get so set the next page token to empty
      //var nextPageToken = movies.Count() >= pageSize ? 
      //  string.Format(PageTokenFormat, offset + pageSize) : string.Empty;

      //var response = new ListMoviesByYearResponse { NextPageToken = nextPageToken };
      //var protobufMovies = movies.Select(m => m.ToProtobuf());
      //response.Movies.AddRange(protobufMovies);

      //return response;
      throw new NotImplementedException();
    }

  }
}
