using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Grpc.Core;
using Grpc.Core.Interceptors;

using Dotflik.Protobuf.Pagination;
using Dotflik.Application.Paginations;
using Dotflik.Infrastructure;
using Dotflik.Domain.Exceptions;

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

    private readonly PaginationTokenFactory m_tokenFactory;

    public PaginationRequestInterceptor(PaginationTokenFactory tokenFactory)
      => m_tokenFactory = tokenFactory;

    /// <summary>
    /// This constructor is only used for unit tests.
    /// </summary>
    protected PaginationRequestInterceptor() 
    {
      m_tokenFactory = null!;
    }

    /// <inheritdoc/>
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
          var paginationToken = GetPaginationToken(pageToken);
          switch (paginationToken.TokenType)
          {
            case PaginationTokenType.LimitOffset:
              ValidateToken((LimitOffsetPaginationToken)paginationToken, pageSize);
              break;

            default:
              throw new NotSupportedException($"Pagination token {paginationToken.TokenType} is not supported.");
          }
        }
      }

      return await continuation(request, context);
    }

    /// <summary>
    /// Validate the <see cref="LimitOffsetPaginationToken"/> token.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown when the <paramref name="limitOffsetToken"/> is not 
    /// consistent with <paramref name="pageSize"/> or offset is not
    /// a multiple of the limit.
    /// </exception>
    /// <param name="limitOffsetToken">The token to be validated</param>
    /// <param name="pageSize">Page size used to check consistency 
    /// with <paramref name="limitOffsetToken"/></param>
    protected static void ValidateToken(LimitOffsetPaginationToken limitOffsetToken, int pageSize)
    {
      var (limit, offset) = (limitOffsetToken.Limit, limitOffsetToken.Offset);

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

    /// <summary>
    /// Attempt to get the pagination token from <paramref name="pageToken"/>.
    /// This method will test for each value in <see cref="PaginationTokenType"/> until
    /// the matched token type is found.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown when the <paramref name="pageToken"/> is not in any 
    /// of the expected formats.</exception>
    /// <param name="pageToken">Token string</param>
    /// <returns>A pagination token object</returns>
    protected IPaginationToken GetPaginationToken(string pageToken)
    {
      var exceptions = new Dictionary<PaginationTokenType, Exception>();
      foreach (PaginationTokenType enumValue in Enum.GetValues(typeof(PaginationTokenType)))
      {
        try
        {
          return m_tokenFactory(enumValue, pageToken);
        }
        catch (Exception ex) when (ex is NotSupportedException || ex is PageTokenFormatException)
        {
          exceptions.Add(enumValue, ex);
        }
      }

      var message = $"Token \"{pageToken}\" is not in any of the expected formats. Please make sure to use the " +
                     "token from the service response. Otherwise set page token to empty.";
      foreach (var pair in exceptions)
      {
        message += $"{Environment.NewLine}[Token type: {pair.Key}] --> {pair.Value.Message}";
      }

      throw new ArgumentException(message);
    }

  }
}
