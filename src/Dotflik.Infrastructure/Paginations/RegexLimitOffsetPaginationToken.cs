using System;
using System.Text.RegularExpressions;
using Dotflik.Application.Paginations;
using Dotflik.Application.Paginations.Args;
using Dotflik.Domain.Exceptions;

namespace Dotflik.Infrastructure.Paginations
{
  /// <summary>
  /// Implementation of the <see cref="LimitOffsetPaginationToken"/> 
  /// class using regex to validate the token format.
  /// </summary>
  internal class RegexLimitOffsetPaginationToken : LimitOffsetPaginationToken
  {
    /// <summary>
    /// Dynamically construct the regex so that if the <see cref="TokenFormat"/> 
    /// ever changes, we'll know to update this property as well.
    /// </summary>
    private string TokenRegex
    {
      get => "^" + string.Format(TokenFormat, "[0-9]+", "[0-9]+") + "$";
    }

    /// <inheritdoc/>
    public override string TokenFormat { get => "limit={0}&offset={1}"; }

    /// <inheritdoc/>
    public RegexLimitOffsetPaginationToken(string token) : base(token)
    {
      if (!string.IsNullOrWhiteSpace(token))
      {
        var tokens = Token.Split('&');
        Limit = int.Parse(tokens[0].Split('=')[1]);
        Offset = int.Parse(tokens[1].Split('=')[1]);
      }
      else
      {
        (Limit, Offset) = (0, 0);
      }
    }

    /// <inheritdoc/>
    public RegexLimitOffsetPaginationToken(LimitOffsetPaginationTokenArgs tokenArgs) : base(tokenArgs)
    { }

    /// <inheritdoc/>
    protected override void IsTokenValid(string token)
    {
      if (string.IsNullOrWhiteSpace(token))
      {
        return;
      }

      if (!Regex.IsMatch(token, TokenRegex))
      {
        var errorMessage = $"Token \"{token}\" is not in correct format '{TokenRegex}'.";
        throw new PageTokenFormatException(errorMessage);
      }
    }

  }
}
