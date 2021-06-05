using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using Dotflik.Domain.Exceptions;
using Dotflik.Protobuf.Pagination;

using Xunit;
using Moq;

namespace Dotflik.WebApp.Server.Interceptors.UnitTests
{
  public class GlobalExceptionInterceptorTests
  {
    /// <summary>
    /// Object under test
    /// </summary>
    private readonly GlobalExceptionInterceptor m_globalExcepInterceptor;

    // Mock objects
    private readonly ServerCallContext m_serverContext;
    private readonly Mock<UnaryServerMethod<object, object>> m_mockContinuation;

    public GlobalExceptionInterceptorTests()
    {
      var logger = new Mock<ILogger<GlobalExceptionInterceptor>>().Object;
      m_globalExcepInterceptor = new GlobalExceptionInterceptor(logger);
      
      m_serverContext = new Mock<ServerCallContext>().Object;
      m_mockContinuation = new Mock<UnaryServerMethod<object, object>>();
    }

    [Fact]
    public async Task UnaryServerHandler_ThrowsPageTokenFormatException_ThrowsRpcExceptionInvalidArgument()
    {
      // Arrange
      var request = new object();

      const string badTokenFormatMessage = "bad token format";
      m_mockContinuation.Setup(p => p(It.IsAny<object>(), It.IsAny<ServerCallContext>()))
                        .Throws(new PageTokenFormatException(badTokenFormatMessage));

      // Act
      var ex = await Assert.ThrowsAsync<RpcException>(
        () => m_globalExcepInterceptor.UnaryServerHandler(request, m_serverContext, m_mockContinuation.Object));

      // Assert
      Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
      Assert.Equal(badTokenFormatMessage, ex.Status.Detail);
    }

    [Fact]
    public async Task UnaryServerHandler_ThrowsArgumentException_ThrowsRpcExceptionInvalidArgument()
    {
      // Arrange
      var request = new object();

      const string badArgumentMessage = "bad argument";
      m_mockContinuation.Setup(p => p(It.IsAny<object>(), It.IsAny<ServerCallContext>()))
                        .Throws(new ArgumentException(badArgumentMessage));

      // Act
      var ex = await Assert.ThrowsAsync<RpcException>(
        () => m_globalExcepInterceptor.UnaryServerHandler(request, m_serverContext, m_mockContinuation.Object));

      // Assert
      Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
      Assert.Equal(badArgumentMessage, ex.Status.Detail);
    }

    [Fact]
    public async Task UnaryServerHandler_ThrowsUnexpectedException_ThrowsRpcExceptionInternalError()
    {
      // Arrange
      var request = new object();

      const string unexpectedMessage = "unexpected message";
      m_mockContinuation.Setup(p => p(It.IsAny<object>(), It.IsAny<ServerCallContext>()))
                        .Throws(new Exception(unexpectedMessage));

      // Act
      var ex = await Assert.ThrowsAsync<RpcException>(
        () => m_globalExcepInterceptor.UnaryServerHandler(request, m_serverContext, m_mockContinuation.Object));

      // Assert
      Assert.Equal(StatusCode.Internal, ex.StatusCode);
    }

  }
}
