using System;
using System.Linq;
using System.Threading.Tasks;

using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

using Microsoft.Extensions.Logging;

using Dotflik.Protobuf;
using Dotflik.Protobuf.Resources;
using Dotflik.Protobuf.Pagination;

using Dotflik.Application.Pagination;
using Dotflik.Application.Repositories;
using Dotflik.WebApp.Server.Mappings;

namespace Dotflik.WebApp.Server.Services
{
  /// <summary>
  /// gRPC implementation of movie service
  /// </summary>
  public class MovieService : Protobuf.MovieService.MovieServiceBase
  {
    protected const int MaxPageSize = 50;

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
      //var m = m_xmovieRepository.GetByIdAsync("tt0496319").Result;
      return Task.FromResult(response);
    }

    /// <inheritdoc/>
    public override async Task<ListMoviesResponse> ListMovies(PaginationRequest request, ServerCallContext context)
    {
      //var (pageSize, pageToken) = (request.PageSize, request.PageToken);

      //var offsetPageToken = new OffsetPageToken(pageToken);
      //var (limit, offset) = (offsetPageToken.Limit, offsetPageToken.Offset);

      //// Constraint the page size to be within max range
      //if (pageSize > MaxPageSize || pageSize == 0)
      //{
      //  pageSize = MaxPageSize;
      //}

      //var movies = await m_movieRepository.GetAllAsync(pageSize, offset);

      //// If the returned movies count is less than the page size, then it means
      //// there is no more movies to get, so set the next page token to empty
      //var nextPageToken = string.Empty;
      //if (movies.Count() == pageSize)
      //{
      //  var nextOffsetPageToken = new OffsetPageToken(pageSize, offset + pageSize);
      //  nextPageToken = nextOffsetPageToken.ToToken();
      //}

      //var paginationResponse = new PaginationRespone { NextPageToken = nextPageToken };
      //var response = new ListMoviesResponse { PaginationResponse = paginationResponse };
      //var protobufMovies = movies.Select(m => ((Domain.Entities.Movie)m).ToProtobuf());
      //response.Movies.AddRange(protobufMovies);

      //return response;
      throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public override async Task<MovieAggregate> GetMovieById(GetMovieByIdRequest request, ServerCallContext context)
    {
      var id = request.Id;
      var movie = await m_movieRepository.GetByIdAsync(id);
      if (movie is null)
      {
        throw new ArgumentException($"Unable to find movie with id \"{id}\"");
      }

      return movie.ToProtobuf();
    }

    /// <inheritdoc/>
    public override async Task<GetMovieByTitleResponse> GetMovieByTitle(GetMovieByTitleRequest request, ServerCallContext context)
    {
      var title = request.Title;
      var movies = await m_movieRepository.GetByTitleAsync(request.Title);
      
      var response = new GetMovieByTitleResponse();
      var protobufMovieAggrs = movies.Select(m => m.ToProtobuf());
      response.Movies.AddRange(protobufMovieAggrs);

      return response;
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
