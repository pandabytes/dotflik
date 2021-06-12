using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dotflik.Domain.Aggregates;

namespace Dotflik.Application.Repositories
{
  /// <summary>
  /// Interface for interacting with the <see cref="Movie"/> repository
  /// </summary>
  public interface IMovieRepository : IReadOnlyRepository<Movie>
  {
    /// <summary>
    /// Get a <see cref="Movie"/> by its id asynchronously
    /// </summary>
    /// <param name="id">Id of the genre</param>
    /// <returns>Movie object or null if not found</returns>
    Task<Movie?> GetByIdAsync(string id);

    /// <summary>
    /// Get a collection of <see cref="Movie"/> that have
    /// matching title <paramref name="title"/> asynchronously
    /// </summary>
    /// <param name="title">Title of the movie</param>
    /// <returns>Collection of movies</returns>
    Task<IEnumerable<Movie>> GetByTitleAsync(string title);

    /// <summary>
    /// Get movies within the year range [<paramref name="from"/>, <paramref name="to"/>]
    /// asynchronously. Use <paramref name="limit"/> and <paramref name="offset"/> to limit
    /// the number of movies to return
    /// </summary>
    /// <param name="limit">Limit of the number of movies</param>
    /// <param name="offset">The offset to start from</param>
    /// <param name="from">From year</param>
    /// <param name="to">To year</param>
    /// <param name="sortAsc">If true, sort the results in acending order. Else descending order</param>
    /// <returns>Collection of movies</returns>
    Task<IEnumerable<Movie>> GetByYear(int limit, int offset, int from, int to, bool sortAsc);

    //Task<IEnumerable<Movie>> GetMoviesByRating(float minRating, float maxRating, int? limit);
  }
}
