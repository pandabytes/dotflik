
namespace Dotflik.Application.Paginations.Args
{
  /// <summary>
  /// Abstract class representing arguments used to construct a token
  /// object based on its <see cref="TokenType"/>.
  /// </summary>
  public abstract class PaginationTokenArgs
  {
    /// <summary>
    /// Purpose of this class is to only construct a <see cref="PaginationTokenArgs"/>
    /// object via the implicit operator.
    /// </summary>
    private sealed class StringPaginationTokenArgs : PaginationTokenArgs {}

    /// <summary>
    /// Use this to indicate the token argument is a string. This
    /// property will always be used first to construct the token
    /// object of type <see cref="PaginationTokenType"/>. If it is null, then
    /// use other properties defined in the derived classes to 
    /// construct the token object.
    /// </summary>
    public string? Value { get; init; }

    public static implicit operator PaginationTokenArgs(string value)
      => new StringPaginationTokenArgs { Value = value };
  }
}
