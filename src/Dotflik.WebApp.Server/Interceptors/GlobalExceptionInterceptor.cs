using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

using Dotflik.Domain.Exceptions;

namespace Dotflik.WebApp.Server.Interceptors
{
  /// <summary>
  /// Global interceptor that handles exception thrown by gRPC services
  /// and other interceptors. This interceptor class catches any thrown 
  /// exception and will throw <see cref="RpcException"/> in 
  /// correspondance with the thrown exception
  /// </summary>
  public class GlobalExceptionInterceptor : Interceptor
  {
    private readonly ILogger<GlobalExceptionInterceptor> m_logger;

    public GlobalExceptionInterceptor(ILogger<GlobalExceptionInterceptor> logger)
      => m_logger = logger;

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
    {
      try
      {
        return await continuation(request, context);
      }
      catch (Exception ex)
      {
        if (ex is PageTokenFormatException || ex is ArgumentException)
        {
          throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }

        // Log the stack trace for unexpected exception
        m_logger.LogError(ex.ToString());

        var status = new Status(StatusCode.Internal, @"Something has gone wrong on our end.
            Please wait as we resolve the problem.");
        throw new RpcException(status);
      }
    }

  }
}
