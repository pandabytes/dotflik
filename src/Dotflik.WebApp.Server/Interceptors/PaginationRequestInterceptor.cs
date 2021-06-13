using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Grpc.Core;
using Grpc.Core.Interceptors;

using Dotflik.Protobuf.Pagination;
using Dotflik.Application.Pagination;

namespace Dotflik.WebApp.Server.Interceptors
{
  /// <summary>
  /// Interceptor that handles verifying <see cref="PaginationRequest"/>
  /// has valid payload and if not, <see cref="ArgumentException"/> 
  /// will be thrown
  /// </summary>
  public class PaginationRequestInterceptor : Interceptor
  {
    protected const string InconsistentPageTokenMessage = 
      @"Page token is not consistent with page size.
        Please make sure to use the token from the service response. 
        Otherwise set page token to empty.";

    protected const string OffsetNotMultipleOfLimitMessage = 
      @"Offset must be a multiple of limit. Please make sure to use the 
        token from the service response. Otherwise set page token to empty.";

    protected const string PageSizeLessThanZeroMessage = "Page size must be at least 0";

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
    {
      var paginationRequest = request as PaginationRequest;
      if (paginationRequest != null)
      {
        var (pageSize, pageToken) = (paginationRequest.PageSize, paginationRequest.PageToken);

        if (pageSize < 0)
        {
          throw new ArgumentException(PageSizeLessThanZeroMessage);
        }

        if (!string.IsNullOrWhiteSpace(pageToken))
        {
          var offsetPageToken = new OffsetPageToken(pageToken);
          var (limit, offset) = (offsetPageToken.Limit, offsetPageToken.Offset);

          // Ensure the page size and limit in the token are consistent
          // If offset is 0, it means we're getting results for 1st page and
          // limit can be safely ignored since we'll use pageSize instead
          if (pageSize != limit)
          {
            throw new ArgumentException(InconsistentPageTokenMessage);
          }

          // This check verifies whether the offset in the token is consistent
          // with the limit. The logic here is the limit should always be divisible
          // by the offset and if it's false, then the client is not being
          // consistent with the request
          if (offset % limit != 0)
          {
            throw new ArgumentException(OffsetNotMultipleOfLimitMessage);
          }
        }
      }

      return await continuation(request, context);
    }

  }
}
