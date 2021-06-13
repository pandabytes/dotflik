using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grpc.Core;

using Dotflik.Application.Pagination;
using Dotflik.Protobuf.Pagination;

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

      public OffsetPageToken PageToken { get; init; } = null!;
    }

    public static readonly TheoryData<PaginationRequestTestData> InconsistentTokens = new()
    {
      new PaginationRequestTestData
      {
        PageSize = 1,
        PageToken = new OffsetPageToken(2, 1)
      },
      new PaginationRequestTestData
      {
        PageSize = 5,
        PageToken = new OffsetPageToken(4, 10)
      },
      new PaginationRequestTestData
      {
        PageSize = 10,
        PageToken = new OffsetPageToken(2, 20)
      }
    };

    public static readonly TheoryData<PaginationRequestTestData> OffsetNotMultipleOfLimitTokens = new()
    {
      new PaginationRequestTestData
      {
        PageSize = 2,
        PageToken = new OffsetPageToken(2, 3)
      },
      new PaginationRequestTestData
      {
        PageSize = 10,
        PageToken = new OffsetPageToken(10, 21)
      },
      new PaginationRequestTestData
      {
        PageSize = 5,
        PageToken = new OffsetPageToken(5, 9)
      }
    };

    public static readonly TheoryData<PaginationRequestTestData> ValidNonEmptyTokens = new()
    {
      new PaginationRequestTestData
      {
        PageSize = 1,
        PageToken = new OffsetPageToken(1, 0)
      },
      new PaginationRequestTestData
      {
        PageSize = 5,
        PageToken = new OffsetPageToken(5, 0)
      },
      new PaginationRequestTestData
      {
        PageSize = 5,
        PageToken = new OffsetPageToken(5, 25)
      },
      new PaginationRequestTestData
      {
        PageSize = 10,
        PageToken = new OffsetPageToken(10, 300)
      },
      new PaginationRequestTestData
      {
        PageSize = 25,
        PageToken = new OffsetPageToken(25, 50)
      }
    };

    /// <summary>
    /// Object under test
    /// </summary>
    private readonly PaginationRequestInterceptor m_pgRequestInterceptor;

    // Mock objects
    private readonly ServerCallContext m_serverContext;
    private readonly UnaryServerMethod<PaginationRequest, object> m_continuation;

    /// <summary>
    /// Constructor
    /// </summary>
    public PaginationRequestInterceptorTests()
    {
      m_pgRequestInterceptor = new();
      m_serverContext = new Mock<ServerCallContext>().Object;
      m_continuation = new Mock<UnaryServerMethod<PaginationRequest, object>>().Object;
    }

    [Theory]
    [MemberData(nameof(InconsistentTokens))]
    public async Task UnaryServerHandler_PageSizeNotEqualToToken_ThrowsArgumentException(PaginationRequestTestData testData)
    {
      // Arrange
      var request = new PaginationRequest
      {
        PageToken = testData.PageToken.ToToken(),
        PageSize = testData.PageSize
      };

      // Act
      var ex = await Assert.ThrowsAsync<ArgumentException>(
        () => m_pgRequestInterceptor.UnaryServerHandler(request, m_serverContext, m_continuation));

      // Assert
      Assert.Equal(InconsistentPageTokenMessage, ex.Message);
    }

    [Theory]
    [MemberData(nameof(OffsetNotMultipleOfLimitTokens))]
    public async Task UnaryServerHandler_OffsetNotMultipleOfLimit_ThrowsArgumentException(PaginationRequestTestData testData)
    {
      // Arrange
      var request = new PaginationRequest
      {
        PageToken = testData.PageToken.ToToken(),
        PageSize = testData.PageSize
      };

      // Act
      var ex = await Assert.ThrowsAsync<ArgumentException>(
        () => m_pgRequestInterceptor.UnaryServerHandler(request, m_serverContext, m_continuation));

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
        PageToken = testData.PageToken.ToToken(),
        PageSize = testData.PageSize
      };

      // Act. No exception is thrown
      await m_pgRequestInterceptor.UnaryServerHandler(request, m_serverContext, m_continuation);
    }
  }
}
