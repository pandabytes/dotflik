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
        PageSize = 10,
        PageToken = new OffsetPageToken(2, 2)
      },
      new PaginationRequestTestData
      {
        PageSize = 5,
        PageToken = new OffsetPageToken(4, 1)
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

    /// <summary>
    /// Object under test
    /// </summary>
    private readonly PaginationRequestInterceptor m_pgRequestInterceptor;

    /// <summary>
    /// Constructor
    /// </summary>
    public PaginationRequestInterceptorTests()
    {
      m_pgRequestInterceptor = new();
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

      var serverContext = new Mock<ServerCallContext>().Object;

      var continuation = new Mock<UnaryServerMethod<PaginationRequest, object>>().Object;

      // Act
      var ex = await Assert.ThrowsAsync<ArgumentException>(
        () => m_pgRequestInterceptor.UnaryServerHandler(request, serverContext, continuation));

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

      var serverContext = new Mock<ServerCallContext>().Object;

      var continuation = new Mock<UnaryServerMethod<PaginationRequest, object>>().Object;

      // Act
      var ex = await Assert.ThrowsAsync<ArgumentException>(
        () => m_pgRequestInterceptor.UnaryServerHandler(request, serverContext, continuation));

      // Assert
      Assert.Equal(OffsetNotMultipleOfLimitMessage, ex.Message);
    }

  }
}
