using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dotflik.Application.Repositories
{
  /// <summary>
  /// Generic interface for repository. This interface provides
  /// read-only operations (Read) on a database
  /// </summary>
  /// <typeparam name="T">The entity type stored in the database</typeparam>
  public interface IReadOnlyRepository<T> where T : class
  {
    /// <summary>
    /// Get entities within the limit <paramref name="limit"/> and
    /// with offset starting at <paramref name="offset"/>
    /// </summary>
    /// <param name="limit">The limit of entities</param>
    /// <param name="offset">The offset to start from</param>
    /// <returns>A collection of entities <typeparamref name="T"/></returns>
    Task<IEnumerable<T>> GetAllAsync(int limit, int offset);
  }
}
