using System;

namespace Dotflik.Application.Paginations
{
  /// <summary>
  /// Interface that provides a pagination token format and its value.
  /// </summary>
  public interface IPaginationToken
  {
    /// <summary>
    /// The type of the token.
    /// </summary>
    PaginationTokenType TokenType { get; }

    /// <summary>
    /// The format of the token.
    /// </summary>
    string TokenFormat { get; }

    /// <summary>
    /// The token value. Empty string is acceptable.
    /// </summary>
    string Token { get; }
  }
}
