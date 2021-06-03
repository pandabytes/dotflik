using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotflik.Domain.Exceptions;

namespace Dotflik.Application.Pagination
{
  /// <summary>
  /// Represent the page token for offset pagination
  /// </summary>
  public sealed class OffsetPageToken
  {
    /// <summary>
    /// The format that the token will be in
    /// when calling <see cref="ToToken"/>
    /// </summary>
    public const string PageTokenFormat = "limit={0}&offset={1}";

    /// <summary>
    /// Dynamically construct the regex so if the <see cref="PageTokenFormat"/> 
    /// ever changes, we'll know to update this field as well
    /// </summary>
    private static readonly string PageTokenRegex = "^" + string.Format(PageTokenFormat, "[0-9]+", "[0-9]+") + "$";

    private int m_limit;

    private int m_offset;

    /// <summary>
    /// Get the limit
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the set value is less than 0
    /// </exception>
    public int Limit 
    { 
      get { return m_limit; }
      init 
      {
        if (value < 0)
        {
          throw new InvalidOperationException("Limit must be at least 0");
        }
        m_limit = value;
      }
    }

    /// <summary>
    /// Get the offset
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the set value is less than 0
    /// </exception>
    public int Offset
    {
      get { return m_offset; }
      init
      {
        if (value < 0)
        {
          throw new InvalidOperationException("Limit must be at least 0");
        }
        m_offset = value;
      }
    }

    /// <summary>
    /// Constructor. Both limit and offset must be at least 0
    /// </summary>
    /// <param name="limit">Limit</param>
    /// <param name="offset">Offset</param>
    public OffsetPageToken(int limit, int offset)
      => (Limit, Offset) = (limit, offset);

    /// <summary>
    /// Construct the offset token object from <paramref name="pageToken"/>.
    /// Both limit and offset must be at least 0
    /// </summary>
    /// <param name="pageToken">Page token in the format <see cref="PageTokenFormat"/>
    /// </param>
    public OffsetPageToken(string pageToken)
    {
      if (string.IsNullOrWhiteSpace(pageToken))
      {
        (Limit, Offset) = (0, 0);
        return;
      }

      if (!Regex.IsMatch(pageToken, PageTokenRegex))
      {
        var errorMessage = $"Token '{pageToken}' is not in correct format '{PageTokenRegex}'.";
        throw new BadPageTokenFormatException(errorMessage);
      }

      var tokens = pageToken.Split('&');
      Limit = int.Parse(tokens[0].Split('=')[1]);
      Offset = int.Parse(tokens[1].Split('=')[1]);
    }

    public string ToToken() => string.Format(PageTokenFormat, Limit, Offset);

  }
}
