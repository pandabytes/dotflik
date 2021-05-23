using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dotflik.Application.Repositories
{
  /// <summary>
  /// Generic interface for repository. This interface provides
  /// basic CRUD (Create, Read, Update, Delete) operations on
  /// a database
  /// </summary>
  /// <typeparam name="T">The entity type stored in the database</typeparam>
  public interface IRepository<T> where T : class
  {
    /// <summary>
    /// Get entities within the limit <paramref name="limit"/> and
    /// with offset starting at <paramref name="offset"/>
    /// </summary>
    /// <param name="limit">The limit of entities</param>
    /// <param name="offset">The offset to start from</param>
    /// <returns>A collection of entities <typeparamref name="T"/></returns>
    Task<IEnumerable<T>> GetAllAsync(int limit, int offset);

    /// <summary>
    /// Get an entity <typeparamref name="T"/> by id asynchronously
    /// </summary>
    /// <param name="id">Id of the entity</param>
    /// <returns>An entity of type <typeparamref name="T"/> or null if not found</returns>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// Get an entity <typeparamref name="T"/> by id asynchronously
    /// </summary>
    /// <param name="id">Id of the entity</param>
    /// <returns>An entity of type <typeparamref name="T"/> or null if not found</returns>
    Task<T?> GetByIdAsync(string id);

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
    Task DeleteAsync(int id);

    /// <summary>
    /// Delete an entity asynchronously
    /// </summary>
    /// <param name="id">Id of entity to be deleted</param>
    /// <returns>Awaitable task</returns>
    Task DeleteAsync(string id);
  }
}
