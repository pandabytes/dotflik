using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dotflik.Application.Repositories
{
  /// <summary>
  /// Generic interface for repository. This interface provides
  /// write-only operations (Create, Update, Delete) on a database
  /// </summary>
  /// <typeparam name="T">The entity type stored in the database</typeparam>
  public interface IWriteOnlyRepository<T> where T : class
  {
    /// <summary>
    /// Create an entity asynchronously
    /// </summary>
    /// <param name="entity">Entity to be added</param>
    /// <returns>Awaitable task</returns>
    Task CreateAsync(T entity);

    /// <summary>
    /// Update an entity asynchronously
    /// </summary>
    /// <param name="entity">Entity to be updated</param>
    /// <returns>Awaitable task</returns>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Delete an entity asynchronously
    /// </summary>
    /// <param name="id">Id of entity to be deleted</param>
    /// <returns>Awaitable task</returns>
    Task DeleteAsync(string id);
  }
}
