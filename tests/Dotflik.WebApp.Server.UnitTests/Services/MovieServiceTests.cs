using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using Grpc.Core;
using Google.Protobuf.WellKnownTypes;

using Dotflik.Protobuf;
using Dotflik.Protobuf.Pagination;
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
    /// Random seed to generate fake data
    /// </summary>
    private const int RandomSeed = 8675309;

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
      var notFoundMovieTask = Task.FromResult<Domain.Aggregates.Movie?>(null);
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
      var movie = GenerateFakeMovieAggrs(1)[0];
      var expectProtobufMovie = movie.ToProtobuf();
      var movieTask = Task.FromResult<Domain.Aggregates.Movie?>(movie);
      var request = new GetMovieByIdRequest { Id = string.Empty };

      m_movieRepositoryMock.Setup(m => m.GetByIdAsync(It.IsAny<string>()))
                           .Returns(movieTask);

      // Act
      var actualProtobufMovie = await m_movieService.GetMovieById(request, m_serverContext);

      // Assert
      Assert.Equal(expectProtobufMovie, actualProtobufMovie);
    }

    [Fact]
    public async Task GetMovieByTitle_MovieFound_ReturnsProtobufMovie()
    {
      // Arrange
      var movies = GenerateFakeMovieAggrs(5);
      var expectProtobufMovies = movies.Select(m => m.ToProtobuf());
      var movieTask = Task.FromResult<IEnumerable<Domain.Aggregates.Movie>>(movies);
      var request = new GetMovieByTitleRequest { Title = string.Empty };

      m_movieRepositoryMock.Setup(m => m.GetByTitleAsync(It.IsAny<string>()))
                           .Returns(movieTask);

      // Act
      var response = await m_movieService.GetMovieByTitle(request, m_serverContext);
      var actualProtobufMovie = response.Movies;

      // Assert
      Assert.Equal(expectProtobufMovies, actualProtobufMovie);
    }

    [Fact]
    public async Task ListMovies_PageSizeIsZero_ReturnsMaxPageSizeMovies()
    {
      // Arrange
      // Add 10 number of movies so that we know
      // we would only get at most m_maxPageSize movies,
      // which is a subset of all the movies
      var movies = GenerateFakeMovieAggrs(m_maxPageSize + 10);

      var request = new PaginationRequest { PageSize = 0, PageToken = string.Empty };

      var expectMovies = (IEnumerable<Domain.Aggregates.Movie>) movies.GetRange(0, m_maxPageSize);
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
      var movies = GenerateFakeMovieAggrs(m_maxPageSize + 10);

      var request = new PaginationRequest { PageSize = m_maxPageSize + 10, PageToken = string.Empty };

      var expectMovies = (IEnumerable<Domain.Aggregates.Movie>)movies.GetRange(0, m_maxPageSize);
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
    public async Task ListMovies_PageSizeIsBetweenOneAndMaxPageSize_ReturnsRequestPageSizeMovies(int pageSize)
    {
      if (pageSize < 1 || pageSize > m_maxPageSize)
      {
        throw new ArgumentException($"{nameof(pageSize)} must be in the range [1, {m_maxPageSize}]");
      }

      // Arrange
      var movies = GenerateFakeMovieAggrs(m_maxPageSize);

      var request = new PaginationRequest { PageSize = pageSize, PageToken = string.Empty };

      var expectMovies = (IEnumerable<Domain.Aggregates.Movie>)movies.GetRange(0, pageSize);
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
    [InlineData(5000, 20)]
    public async Task ListMovies_ConsecutiveRequestUntilPageTokenIsEmpty_TotalPagesMatch(int numMovies, int pageSize)
    {
      // Arrange
      var numPages = (int) Math.Ceiling(numMovies / (float)pageSize);
      var movies = GenerateFakeMovieAggrs(numMovies);
      var firstRequest = new PaginationRequest { PageSize = pageSize, PageToken = string.Empty };

      m_movieRepositoryMock.Setup(m => m.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                           .Returns<int, int>((limit, offset) => 
                           {
                             IEnumerable<Domain.Aggregates.Movie> moviesRange;
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
    /// <see cref="Domain.Entities.Star"/> objects
    /// </summary>
    /// <param name="size">Number of fake star objects to generate</param>
    /// <param name="seed">Random seed</param>
    /// <returns><paramref name="size"/> number of star objects</returns>
    private static List<Domain.Entities.Star> GenerateFakeStars(int size, int seed)
    {
      Func<int, string> createId = id => "nm" + $"{id}".PadLeft(7, '0');

      var starId = 0;
      var fakeStars = new Faker<Domain.Entities.Star>()
                        .StrictMode(true)
                        .RuleFor(m => m.Id, f => createId(starId++))
                        .RuleFor(m => m.Name, f => f.Random.AlphaNumeric(50))
                        .RuleFor(m => m.BirthYear, f => f.Random.Number(1990, 2021))
                        .RuleFor(m => m.Headshot, f => f.Internet.Url());
      
      return fakeStars.UseSeed(seed).Generate(size);
    }

    /// <summary>
    /// Generate <paramref name="size"/> number of fake
    /// <see cref="Domain.Entities.Genre"/> objects
    /// </summary>
    /// <param name="size">Number of fake genre objects to generate</param>
    /// <param name="seed">Random seed</param>
    /// <returns><paramref name="size"/> number of genre objects</returns>
    private static List<Domain.Entities.Genre> GenerateFakeGenres(int size, int seed)
    {
      string[] genreNames = new string[] 
      {
        "Action",
        "Adult",
        "Adventure",
        "Animation",
        "Biography",
        "Comedy",
        "Crime",
        "Documentary",
        "Drama",
        "Family",
        "Fantasy",
        "History",
        "Horror",
        "Music",
        "Musical",
        "Mystery",
        "Reality-TV",
        "Romance",
        "Sci-Fi",
        "Sport",
        "Thriller",
        "War",
        "Western"
      };

      var genreId = 0;
      var fakeGenres = new Faker<Domain.Entities.Genre>()
                        .StrictMode(true)
                        .RuleFor(m => m.Id, f => genreId++)
                        .RuleFor(m => m.Name, f => f.PickRandom(genreNames));

      return fakeGenres.UseSeed(seed).Generate(size);
    }

    /// <summary>
    /// Generate <paramref name="size"/> number of fake
    /// <see cref="Domain.Aggregates.Movie"/> objects
    /// </summary>
    /// <param name="size">Number of fake movie objects to generate</param>
    /// <param name="seed">Random seed</param>
    /// <returns><paramref name="size"/> number of movie objects</returns>
    private static List<Domain.Aggregates.Movie> GenerateFakeMovieAggrs(int size, int seed = RandomSeed)
    {
      Func<int, string> createId = id => "tt" + $"{id}".PadLeft(7, '0');

      var movieId = 0;
      var fakerMovieAggrs = new Faker<Domain.Aggregates.Movie>()
                            .StrictMode(true)
                            .RuleFor(m => m.Id, f => createId(movieId++))
                            .RuleFor(m => m.Title, f => f.Random.AlphaNumeric(50))
                            .RuleFor(m => m.Year, f => f.Random.Number(1990, 2021))
                            .RuleFor(m => m.Director, f => f.Name.FullName())
                            .RuleFor(m => m.BannerUrl, f => f.Internet.Url())
                            .RuleFor(m => m.Stars, f => GenerateFakeStars(f.Random.Number(1, 5), seed))
                            .RuleFor(m => m.Genres, f => GenerateFakeGenres(f.Random.Number(1, 5), seed));

      return fakerMovieAggrs.UseSeed(seed).Generate(size);
    }

  }
}
