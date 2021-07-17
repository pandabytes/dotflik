using Dotflik.Domain.Exceptions;
using Dotflik.Application.Paginations.Args;

namespace Dotflik.Application.Paginations
{
  /// <summary>
  /// Base class of different types of pagination token classes.
  /// </summary>
  public abstract class PaginationToken : IPaginationToken
  {
    /// <inheritdoc/>
    public abstract PaginationTokenType TokenType { get; }

    /// <inheritdoc/>
    public abstract string TokenFormat { get; }

    /// <inheritdoc/>
    public string Token { get; protected set; } = string.Empty;

    /// <summary>
    /// Construct the token object from <paramref name="token"/>. Empty string
    /// is acceptable.
    /// </summary>
    /// <exception cref="PageTokenFormatException">
    /// Thrown when <paramref name="token"/> is not in the correct format.
    /// </exception>
    /// <param name="token">Token string</param>
    protected PaginationToken(string token)
    {
      IsTokenValid(token);
      Token = token;
    }

    /// <summary>
    /// Construct object with token arguments.
    /// </summary>
    /// <param name="tokenArgs">Arguments to construct the token object</param>
    protected PaginationToken(PaginationTokenArgs tokenArgs) { }

    /// <summary>
    /// Check if the <paramref name="token"/> is in the format
    /// defined in <see cref="TokenFormat"/>.
    /// </summary>
    /// <exception cref="PageTokenFormatException">
    /// Thrown when <paramref name="token"/> is not in the correct format.
    /// </exception>
    /// <param name="token">Token string to be checked if it's valid</param>
    protected abstract void IsTokenValid(string token);
  }
}
