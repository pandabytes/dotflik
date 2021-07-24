using System;
using Dotflik.Application.Paginations.Args;

namespace Dotflik.Application.Paginations
{
  /// <summary>
  /// Represent a pagination token using the limit and
  /// offset feature of a relational database.
  /// </summary>
  public abstract class LimitOffsetPaginationToken : PaginationToken
  {
    private int m_limit;
    private int m_offset;

    /// <inheritdoc/>
    public override PaginationTokenType TokenType { get => PaginationTokenType.LimitOffset; }

    /// <summary>
    /// Get the limit.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown when the set value is less than 0.
    /// </exception>
    public int Limit
    {
      get => m_limit;
      init
      {
        if (value < 0)
        {
          throw new ArgumentException("Limit must be at least 0");
        }
        m_limit = value;
      }
    }

    /// <summary>
    /// Get the offset.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown when the set value is less than 0.
    /// </exception>
    public int Offset
    {
      get => m_offset;
      init
      {
        if (value < 0)
        {
          throw new ArgumentException("Offset must be at least 0");
        }
        m_offset = value;
      }
    }

    /// <inheritdoc/>
    /// <remarks>
    /// <see cref="Limit"/> and <see cref="Offset"/> will be set to 0
    /// if <paramref name="token"/> is empty.
    /// </remarks>
    public LimitOffsetPaginationToken(string token) : base(token) { }

    /// <summary>
    /// Constructor. Both limit and offset must be at least 0.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="limit"/> or 
    /// <paramref name="offset"/> is less than 0.
    /// </exception>
    /// <param name="limit">Limit</param>
    /// <param name="offset">Offset</param>
    public LimitOffsetPaginationToken(LimitOffsetPaginationTokenArgs tokenArgs) : base(tokenArgs)
    {
      (Limit, Offset) = (tokenArgs.Limit, tokenArgs.Offset);
      Token = string.Format(TokenFormat, Limit, Offset);
    }

  }
}
