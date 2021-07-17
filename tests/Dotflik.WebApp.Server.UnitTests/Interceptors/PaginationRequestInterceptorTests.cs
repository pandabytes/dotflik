using System;
using System.Threading.Tasks;

using Grpc.Core;

using Dotflik.Application.Paginations;
using Dotflik.Application.Paginations.Args;
using Dotflik.Infrastructure;
using Dotflik.Protobuf.Pagination;
using Microsoft.Extensions.DependencyInjection;

using Xunit;
using Moq;

namespace Dotflik.WebApp.Server.Interceptors.UnitTests
{
  public class PaginationRequestInterceptorTests : PaginationRequestInterceptor
  {
    /// <summary>
    /// Contain test data
    /// </summary>
    public class PaginationRequestTestData
    {
      public int PageSize { get; init; }

      public string PageToken { get; init; } = string.Empty;
    }

    public static readonly TheoryData<PaginationRequestTestData> InconsistentTokens = new()
    {
      new PaginationRequestTestData
      {
        PageSize = 1,
        PageToken = CreateTokenString(2, 1)
      },
      new PaginationRequestTestData
      {
        PageSize = 5,
        PageToken = CreateTokenString(4, 10)
      },
      new PaginationRequestTestData
      {
        PageSize = 10,
        PageToken = CreateTokenString(2, 20)
      }
    };

    public static readonly TheoryData<PaginationRequestTestData> OffsetNotMultipleOfLimitTokens = new()
    {
      new PaginationRequestTestData
      {
        PageSize = 2,
        PageToken = CreateTokenString(2, 3)
      },
      new PaginationRequestTestData
      {
        PageSize = 10,
        PageToken = CreateTokenString(10, 21)
      },
      new PaginationRequestTestData
      {
        PageSize = 5,
        PageToken = CreateTokenString(5, 9)
      }
    };

    public static readonly TheoryData<PaginationRequestTestData> ValidNonEmptyTokens = new()
    {
      new PaginationRequestTestData
      {
        PageSize = 1,
        PageToken = CreateTokenString(1, 0)
      },
      new PaginationRequestTestData
      {
        PageSize = 5,
        PageToken = CreateTokenString(5, 0)
      },
      new PaginationRequestTestData
      {
        PageSize = 5,
        PageToken = CreateTokenString(5, 25)
      },
      new PaginationRequestTestData
      {
        PageSize = 10,
        PageToken = CreateTokenString(10, 300)
      },
      new PaginationRequestTestData
      {
        PageSize = 25,
        PageToken = CreateTokenString(25, 50)
      }
    };

    /// <summary>
    /// Object under test
    /// </summary>
    private readonly PaginationRequestInterceptor m_pgRequestInterceptor;

    // Mock objects
    private readonly ServerCallContext m_serverContext;
    private readonly UnaryServerMethod<PaginationRequest, object> m_continuation;

    private readonly PaginationTokenFactory m_tokenFactory;

    /// <summary>
    /// Constructor
    /// </summary>
    public PaginationRequestInterceptorTests()
    {
      var services = new ServiceCollection();
      services.AddPaginationTokenFactory();

      var provider = services.BuildServiceProvider();
      m_tokenFactory = provider.GetRequiredService<PaginationTokenFactory>();

      m_pgRequestInterceptor = new(m_tokenFactory);
      m_serverContext = new Mock<ServerCallContext>().Object;
      m_continuation = new Mock<UnaryServerMethod<PaginationRequest, object>>().Object;
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    [InlineData(20, 15)]
    public void ValidateLimitOffsetToken_PageSizeNotEqualToToken_ThrowsArgumentException(int pageSize, int limit)
    {
      // Arrange
      var tokenArgs = new LimitOffsetPaginationTokenArgs { Limit = limit, Offset = 0 };
      var token = (LimitOffsetPaginationToken) m_tokenFactory(PaginationTokenType.LimitOffset, tokenArgs);

      // Act
      var ex = Assert.Throws<ArgumentException>(() => ValidateToken(token, pageSize));

      // Assert
      Assert.Equal(InconsistentPageTokenMessage, ex.Message);
    }

    [Theory]
    [InlineData(2, 3)]
    [InlineData(7, 30)]
    [InlineData(70, 30)]
    public void ValidateLimitOffsetToken_OffsetNotMultipleOfLimit_ThrowsArgumentException(int limit, int offset)
    {
      // Arrange
      var tokenArgs = new LimitOffsetPaginationTokenArgs { Limit = limit, Offset = offset };
      var token = (LimitOffsetPaginationToken)m_tokenFactory(PaginationTokenType.LimitOffset, tokenArgs);

      // Act
      var ex = Assert.Throws<ArgumentException>(() => ValidateToken(token, limit));

      // Assert
      Assert.Equal(OffsetNotMultipleOfLimitMessage, ex.Message);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-5)]
    [InlineData(-10)]
    [InlineData(-100)]
    public async Task UnaryServerHandler_NegativePageSize_ThrowsArgumentException(int pageSize)
    {
      // Arrange
      var request = new PaginationRequest
      {
        PageToken = string.Empty,
        PageSize = pageSize
      };

      // Act
      var ex = await Assert.ThrowsAsync<ArgumentException>(
        () => m_pgRequestInterceptor.UnaryServerHandler(request, m_serverContext, m_continuation));

      // Assert
      Assert.Equal(PageSizeLessThanZeroMessage, ex.Message);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(100)]
    public async Task UnaryServerHandler_EmptyToken_NoExceptionThrown(int pageSize)
    {
      // Arrange
      var request = new PaginationRequest
      {
        PageToken = string.Empty,
        PageSize = pageSize
      };

      // Act. No exception is thrown
      await m_pgRequestInterceptor.UnaryServerHandler(request, m_serverContext, m_continuation);
    }

    [Theory]
    [MemberData(nameof(ValidNonEmptyTokens))]
    public async Task UnaryServerHandler_ValidNonEmptyToken_NoExceptionThrown(PaginationRequestTestData testData)
    {
      // Arrange
      var request = new PaginationRequest
      {
        PageToken = testData.PageToken,
        PageSize = testData.PageSize
      };

      // Act. No exception is thrown
      await m_pgRequestInterceptor.UnaryServerHandler(request, m_serverContext, m_continuation);
    }

    [Fact]
    public void GetPaginationToken_TokenNotInAnyExpectedFormat_ThrowsArgumentException()
    {
      // Arrange
      var request = new PaginationRequest()
      {
        PageSize = 0,
        PageToken = "dummy_token_bad"
      };

      // Act & Assert
      Assert.ThrowsAsync<ArgumentException>(() => 
        m_pgRequestInterceptor.UnaryServerHandler(request, m_serverContext, m_continuation));
    }

    /// <summary>
    /// Use the token format defined in 
    /// one of the implementation classes.
    /// </summary>
    /// <param name="limit">Limit</param>
    /// <param name="offset">Offset</param>
    /// <returns>Token string value</returns>
    private static string CreateTokenString(int limit, int offset)
      => $"limit={limit}&offset={offset}";

  }
}
