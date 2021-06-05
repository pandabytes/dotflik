using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using Grpc.Core;
using Google.Protobuf.WellKnownTypes;

using Dotflik.Protobuf.Pagination;
using Dotflik.Protobuf.Movie;
using Dotflik.Application.Repositories;
using Dotflik.WebApp.Server.Mappings;

using Bogus;
using Xunit;
using Moq;

namespace Dotflik.WebApp.Server.Services.UnitTests
{
  public class MovieServiceTests
  {
    /// <summary>
    /// Object under test
    /// </summary>
    private readonly MovieService m_movieService;

    // Mock objects
    private readonly Mock<IMovieRepository> m_movieRepositoryMock;
    private readonly ServerCallContext m_serverContext;

    /// <summary>
    /// Global max page size set by the movie service
    /// </summary>
    private readonly int m_maxPageSize;

    /// <summary>
    /// Constructor
    /// </summary>
    public MovieServiceTests()
    {
      var logger = new Mock<ILogger<MovieService>>().Object;

      m_serverContext = new Mock<ServerCallContext>().Object;

      m_movieRepositoryMock = new Mock<IMovieRepository>();
      m_movieService = new MovieService(m_movieRepositoryMock.Object, logger);

      var maxPageSizeRespond = m_movieService.GetMaxPageSize(new Empty(), m_serverContext);
      m_maxPageSize = maxPageSizeRespond.Result.MaxPageSize;
    }

    [Fact]
    public async Task GetMovieById_MovieNotFound_ThrowsRpcExceptionWithInvalidArgument()
    {
      // Arrange
      var notFoundMovieTask = Task.FromResult<Domain.Entities.Movie?>(null);
      var request = new GetMovieByIdRequest { Id = string.Empty };

      m_movieRepositoryMock.Setup(m => m.GetByIdAsync(It.IsAny<string>()))
                           .Returns(notFoundMovieTask);

      // Act and Assert
      await Assert.ThrowsAsync<ArgumentException>(
        () => m_movieService.GetMovieById(request, m_serverContext));
    }

    [Fact]
    public async Task GetMovieById_MovieFound_ReturnsProtobufMovie()
    {
      // Arrange
      var movie = GenerateFakeMovies(1)[0];
      var expectProtobufMovie = movie.ToProtobuf();
      var movieTask = Task.FromResult<Domain.Entities.Movie?>(movie);
      var request = new GetMovieByIdRequest { Id = string.Empty };

      m_movieRepositoryMock.Setup(m => m.GetByIdAsync(It.IsAny<string>()))
                           .Returns(movieTask);

      // Act
      var actualProtobufMovie = await m_movieService.GetMovieById(request, m_serverContext);

      // Assert
      Assert.Equal(expectProtobufMovie, actualProtobufMovie);
    }

    [Fact]
    public async Task GetMovieByTitle_MovieNotFound_ThrowsRpcExceptionWithInvalidArgument()
    {
      // Arrange
      var notFoundMovieTask = Task.FromResult<Domain.Entities.Movie?>(null);
      var request = new GetMovieByTitleRequest { Title = string.Empty };

      m_movieRepositoryMock.Setup(m => m.GetByTitleAsync(It.IsAny<string>()))
                           .Returns(notFoundMovieTask);

      // Act and Assert
      await Assert.ThrowsAsync<ArgumentException>(
        () => m_movieService.GetMovieByTitle(request, m_serverContext));
    }

    [Fact]
    public async Task GetMovieByTitle_MovieFound_ReturnsProtobufMovie()
    {
      // Arrange
      var movie = GenerateFakeMovies(1)[0];
      var expectProtobufMovie = movie.ToProtobuf();
      var movieTask = Task.FromResult<Domain.Entities.Movie?>(movie);
      var request = new GetMovieByTitleRequest { Title = string.Empty };

      m_movieRepositoryMock.Setup(m => m.GetByTitleAsync(It.IsAny<string>()))
                           .Returns(movieTask);

      // Act
      var actualProtobufMovie = await m_movieService.GetMovieByTitle(request, m_serverContext);

      // Assert
      Assert.Equal(expectProtobufMovie, actualProtobufMovie);
    }

    [Fact]
    public async Task ListMovies_PageSizeIsZero_ReturnsMaxPageSizeMovies()
    {
      // Arrange
      // Add 10 number of movies so that we know
      // we would only get at most m_maxPageSize movies,
      // which is a subset of all the movies
      var movies = GenerateFakeMovies(m_maxPageSize + 10);

      var request = new PaginationRequest { PageSize = 0, PageToken = string.Empty };

      var expectMovies = (IEnumerable<Domain.Entities.Movie>) movies.GetRange(0, m_maxPageSize);
      var expectMoviesTask = Task.FromResult(expectMovies);
      m_movieRepositoryMock.Setup(m => m.GetAllAsync(m_maxPageSize, It.IsAny<int>()))
                           .Returns(expectMoviesTask);

      // Act
      var actualMovies = (await m_movieService.ListMovies(request, m_serverContext)).Movies;

      // Assert
      Assert.Equal(expectMovies.Count(), actualMovies.Count);
    }

    [Fact]
    public async Task ListMovies_PageSizeIsGreaterThanMaxPageSize_ReturnsMaxPageSizeMovies()
    {
      // Arrange
      // Add 10 number of movies so that we know
      // we would only get at most m_maxPageSize movies,
      // which is a subset of all the movies
      var movies = GenerateFakeMovies(m_maxPageSize + 10);

      var request = new PaginationRequest { PageSize = m_maxPageSize + 10, PageToken = string.Empty };

      var expectMovies = (IEnumerable<Domain.Entities.Movie>)movies.GetRange(0, m_maxPageSize);
      var expectMoviesTask = Task.FromResult(expectMovies);
      m_movieRepositoryMock.Setup(m => m.GetAllAsync(m_maxPageSize, It.IsAny<int>()))
                           .Returns(expectMoviesTask);

      // Act
      var actualMovies = (await m_movieService.ListMovies(request, m_serverContext)).Movies;

      // Assert
      Assert.Equal(expectMovies.Count(), actualMovies.Count);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(30)]
    [InlineData(50)]
    public async Task ListMovies_PageSizeIsBetweenZeroAndMaxPageSize_ReturnsRequestPageSizeMovies(int pageSize)
    {
      if (pageSize <= 0 || pageSize > m_maxPageSize)
      {
        throw new ArgumentException($"{nameof(pageSize)} must be in the range [0, {m_maxPageSize}]");
      }

      // Arrange
      var movies = GenerateFakeMovies(m_maxPageSize);

      var request = new PaginationRequest { PageSize = pageSize, PageToken = string.Empty };

      var expectMovies = (IEnumerable<Domain.Entities.Movie>)movies.GetRange(0, pageSize);
      var expectMoviesTask = Task.FromResult(expectMovies);
      m_movieRepositoryMock.Setup(m => m.GetAllAsync(pageSize, It.IsAny<int>()))
                           .Returns(expectMoviesTask);

      // Act
      var actualMovies = (await m_movieService.ListMovies(request, m_serverContext)).Movies;

      // Assert
      Assert.Equal(expectMovies.Count(), actualMovies.Count);
    }

    [Theory]
    [InlineData(2, 1)]
    [InlineData(20, 3)]
    [InlineData(47, 11)]
    [InlineData(50, 3)]
    [InlineData(50, 5)]
    [InlineData(100, 1)]
    [InlineData(1000, 3)]
    [InlineData(1000, 10)]
    [InlineData(2403, 45)]
    [InlineData(9052, 20)]
    public async Task ListMovies_ConsecutiveRequestUntilPageTokenIsEmpty_TotalPagesMatch(int numMovies, int pageSize)
    {
      // Arrange
      var numPages = (int) Math.Ceiling(numMovies / (float)pageSize);
      var movies = GenerateFakeMovies(numMovies);
      var firstRequest = new PaginationRequest { PageSize = pageSize, PageToken = string.Empty };

      m_movieRepositoryMock.Setup(m => m.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                           .Returns<int, int>((limit, offset) => 
                           {
                             IEnumerable<Domain.Entities.Movie> moviesRange;
                             try
                             {
                               moviesRange = movies.GetRange(offset, limit);
                             }
                             catch (ArgumentException)
                             {
                               var remaining = numMovies - offset;
                               moviesRange = movies.GetRange(offset, remaining);
                             }
                             return Task.FromResult(moviesRange);
                           });

      // Act
      var firstResponse = await m_movieService.ListMovies(firstRequest, m_serverContext);
      var nextToken = firstResponse.PaginationResponse.NextPageToken;

      // Start from 1 because the first page is already fetched above
      var pageCounter = 1;
      while (!string.IsNullOrWhiteSpace(nextToken))
      {
        var request = new PaginationRequest { PageSize = pageSize, PageToken = nextToken };
        var response = await m_movieService.ListMovies(request, m_serverContext);
        nextToken = response.PaginationResponse.NextPageToken;

        pageCounter++;

        // If there is no movie, we substract 1 since 
        // this iteration is not counted as a "page"
        if (response.Movies.Count == 0)
        {
          pageCounter--;
        }
      }

      // Assert
      Assert.Equal(numPages, pageCounter);
    }

    /// <summary>
    /// Generate <paramref name="size"/> number of fake
    /// <see cref="Domain.Entities.Movie"/> objects
    /// </summary>
    /// <param name="size">Number of fake movie objects to generate</param>
    /// <returns><paramref name="size"/> number of movie objects</returns>
    private static List<Domain.Entities.Movie> GenerateFakeMovies(int size)
    {
      Randomizer.Seed = new Random(8675309);

      Func<int, string> createId = id => "tt" + $"{id}".PadLeft(7, '0');

      var movieId = 0;
      var fakeMovies = new Faker<Domain.Entities.Movie>()
                        .StrictMode(true)
                        .RuleFor(m => m.Id,        f => createId(movieId++))
                        .RuleFor(m => m.Title,     f => f.Random.AlphaNumeric(50))
                        .RuleFor(m => m.Year,      f => f.Random.Number(1990, 2021))
                        .RuleFor(m => m.Director,  f => f.Name.FullName())
                        .RuleFor(m => m.BannerUrl, f => f.Internet.Url());

      return fakeMovies.Generate(size);
    }

  }
}
