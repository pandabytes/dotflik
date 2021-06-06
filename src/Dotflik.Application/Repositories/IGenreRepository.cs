using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dotflik.Domain.Entities;

namespace Dotflik.Application.Repositories
{
  /// <summary>
  /// Interface for interacting with the <see cref="Genre"/> repository
  /// </summary>
  public interface IGenreRepository : IReadOnlyRepository<Genre>
  {
    /// <summary>
    /// Get an entity <typeparamref name="T"/> by id asynchronously
    /// </summary>
    /// <param name="id">Id of the genre</param>
    /// <returns>Genre object or null if not found</returns>
    Task<Genre?> GetByIdAsync(int id);

    /// <summary>
    /// Get a genre by its name asynchronously
    /// </summary>
    /// <param name="name">Name of the genre</param>
    /// <returns>Genre object or null if not found</returns>
    Task<Genre?> GetByNameAsync(string name);
  }
}
