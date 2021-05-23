using System;
using System.Threading.Tasks;
using Dotflik.Domain.Entities;

namespace Dotflik.Application.Repositories
{
  /// <summary>
  /// Interface for interacting with the <see cref="Movie"/> repository
  /// </summary>
  public interface IMovieRepository : IRepository<Movie>
  {
    /// <summary>
    /// Get a movie by its title asynchronously
    /// </summary>
    /// <param name="title">Title of the movie</param>
    /// <returns>Movie object or null if not found</returns>
    Task<Movie?> GetByTitleAsync(string title);

    //Task<IEnumerable<Movie>> GetMovieByYear(int from, int to);

    //Task<IEnumerable<Movie>> GetMoviesByRating(float minRating, float maxRating, int? limit);
  }
}
