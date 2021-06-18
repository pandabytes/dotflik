using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grpc.Core;
using Google.Protobuf.WellKnownTypes;

using Dotflik.Application.Repositories;
using Dotflik.WebApp.Server.Services;

using Moq;
using Xunit;

namespace Dotflik.WebApp.Server.Services.UnitTests
{
  public class GenreServiceTests
  {
    /// <summary>
    /// Object under test
    /// </summary>
    private readonly GenreService m_genreService;

    // Mock objects
    private readonly ServerCallContext m_serverContext;
    private readonly Mock<IGenreRepository> m_genreRepositoryMock;

    private static readonly IEnumerable<string> GenreNames = new string[] 
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

    public GenreServiceTests()
    {
      m_serverContext = new Mock<ServerCallContext>().Object;
      m_genreRepositoryMock = new();

      var genreNamesTask = Task.FromResult(GenreNames);
      m_genreRepositoryMock.Setup(gr => gr.GetGenreNamesAsync())
                           .Returns(genreNamesTask);

      m_genreService = new(m_genreRepositoryMock.Object);
    }

    [Fact]
    public async Task GetGenreNamesOnly_SimpleCall_ReturnsListOfGenreNames()
    {
      // Act
      var response = await m_genreService.GetGenreNamesOnly(new Empty(), m_serverContext);
      var actualGenreNames = response.GenreNames;

      // Assert
      Assert.Equal(GenreNames, actualGenreNames);
    }

  }

}
