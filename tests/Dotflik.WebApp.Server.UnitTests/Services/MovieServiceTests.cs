using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using Dotflik.Protobuf.Pagination;
using Dotflik.Protobuf.Movie;
using Grpc.Core;

using Dotflik.Application.Repositories;
using Dotflik.Domain.Exceptions;
using Dotflik.WebApp.Server.Mappings;

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

    private readonly Mock<IMovieRepository> m_movieRepositoryMock;
    private readonly ServerCallContext m_serverContext;

    public MovieServiceTests()
    {
      var logger = new Mock<ILogger<MovieService>>().Object;

      m_serverContext = new Mock<ServerCallContext>().Object;

      m_movieRepositoryMock = new Mock<IMovieRepository>();
      m_movieService = new MovieService(m_movieRepositoryMock.Object, logger);
    }

    [Fact]
    public async Task GetMovieById_MovieNotFound_ThrowsRpcExceptionWithInvalidArgument()
    {
      // Arrange
      var notFoundMovieTask = Task.FromResult<Domain.Entities.Movie?>(null);
      m_movieRepositoryMock.Setup(m => m.GetByIdAsync(It.IsAny<string>()))
                           .Returns(notFoundMovieTask);

      var request = new GetMovieByIdRequest { Id = string.Empty };

      // Act
      var ex = await Assert.ThrowsAsync<RpcException>(
        () => m_movieService.GetMovieById(request, m_serverContext));

      // Assert
      Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
    }

    [Fact]
    public async Task GetMovieById_MovieFound_ReturnsProtobufMovie()
    {
      // Arrange
      var movie = new Domain.Entities.Movie() { 
        Id = "00AAB", Director="Hans Zimmerman",
        Year = 1990, Title = "Cotton Candy", BannerUrl = "https://movie.com"
      };
      var expectProtobufMovie = movie.ToProtobuf();
      var movieTask = Task.FromResult<Domain.Entities.Movie?>(movie);

      m_movieRepositoryMock.Setup(m => m.GetByIdAsync(It.IsAny<string>()))
                           .Returns(movieTask);

      var request = new GetMovieByIdRequest { Id = string.Empty };

      // Act
      var actualProtobufMovie = await m_movieService.GetMovieById(request, m_serverContext);

      // Assert
      Assert.Equal(expectProtobufMovie, actualProtobufMovie);
    }

    [Fact]
    public async Task GetMovieById_RepositoryProblem_ThrowsRepositoryExceptionWithInternalError()
    {
      // Arrange
      var movieTask = Task.FromResult<Domain.Entities.Movie?>(null);

      m_movieRepositoryMock.Setup(m => m.GetByIdAsync(It.IsAny<string>()))
                           .Throws(new RepositoryException("dummy exception"));

      var request = new GetMovieByIdRequest { Id = string.Empty };

      // Act
      var ex = await Assert.ThrowsAsync<RpcException>(
        () => m_movieService.GetMovieById(request, m_serverContext));

      // Assert
      Assert.Equal(StatusCode.Internal, ex.StatusCode);
    }

    [Fact]
    public async Task GetMovieByTitle_MovieNotFound_ThrowsRpcExceptionWithInvalidArgument()
    {
      // Arrange
      var notFoundMovieTask = Task.FromResult<Domain.Entities.Movie?>(null);
      m_movieRepositoryMock.Setup(m => m.GetByTitleAsync(It.IsAny<string>()))
                           .Returns(notFoundMovieTask);

      var request = new GetMovieByTitleRequest { Title = string.Empty };

      // Act
      var ex = await Assert.ThrowsAsync<RpcException>(
        () => m_movieService.GetMovieByTitle(request, m_serverContext));

      // Assert
      Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
    }

    [Fact]
    public async Task GetMovieByTitle_MovieFound_ReturnsProtobufMovie()
    {
      // Arrange
      var movie = new Domain.Entities.Movie()
      {
        Id = "00AAB",
        Director = "Hans Zimmerman",
        Year = 1990,
        Title = "Cotton Candy",
        BannerUrl = "https://movie.com"
      };
      var expectProtobufMovie = movie.ToProtobuf();
      var movieTask = Task.FromResult<Domain.Entities.Movie?>(movie);

      m_movieRepositoryMock.Setup(m => m.GetByTitleAsync(It.IsAny<string>()))
                           .Returns(movieTask);

      var request = new GetMovieByTitleRequest { Title = string.Empty };

      // Act
      var actualProtobufMovie = await m_movieService.GetMovieByTitle(request, m_serverContext);

      // Assert
      Assert.Equal(expectProtobufMovie, actualProtobufMovie);
    }

    [Fact]
    public async Task GetMovieByTitle_RepositoryProblem_ThrowsRepositoryExceptionWithInternalError()
    {
      // Arrange
      var movieTask = Task.FromResult<Domain.Entities.Movie?>(null);

      m_movieRepositoryMock.Setup(m => m.GetByTitleAsync(It.IsAny<string>()))
                           .Throws(new RepositoryException("dummy exception"));

      var request = new GetMovieByTitleRequest { Title = string.Empty };

      // Act
      var ex = await Assert.ThrowsAsync<RpcException>(
        () => m_movieService.GetMovieByTitle(request, m_serverContext));

      // Assert
      Assert.Equal(StatusCode.Internal, ex.StatusCode);
    }

    [Theory]
    [InlineData("012546")]
    [InlineData("bad token")]
    [InlineData("invalid_token")]
    public async Task ListMovies_InconsistentPageToken_ThrowsRpcExceptionWithInvalidArgument(string token)
    {
      // Arrange
      var request = new PaginationRequest { PageToken = token };

      // Act
      var ex = await Assert.ThrowsAsync<RpcException>(
        () => m_movieService.ListMovies(request, m_serverContext));

      // Assert
      Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
    }

    [Fact]
    public async Task ListMovies_RepositoryProblem_ThrowsRepositoryExceptionWithInternalError()
    {
      // Arrange
      var movieTask = Task.FromResult<Domain.Entities.Movie?>(null);

      m_movieRepositoryMock.Setup(m => m.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                           .Throws(new RepositoryException("dummy exception"));

      var request = new ListMoviesRequest();

      // Act
      var ex = await Assert.ThrowsAsync<RpcException>(
        () => m_movieService.ListMovies(request, m_serverContext));

      // Assert
      Assert.Equal(StatusCode.Internal, ex.StatusCode);
    }

    [Fact]
    public async Task ListMovies_()
    {
      // Arrange

    }

  }
}
