
namespace Dotflik.Application.Paginations.Args
{
  /// <summary>
  /// The limit offset pagination token arguments.
  /// </summary>
  public class LimitOffsetPaginationTokenArgs : PaginationTokenArgs
  {
    public int Limit { get; init; }

    public int Offset { get; init; }
  }
}
