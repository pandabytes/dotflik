using System;
using Dotflik.Domain.Entities;

namespace Dotflik.Application.Repositories
{
  /// <summary>
  /// Interface for interacting with the <see cref="Movie"/> repository
  /// </summary>
  public interface IMovieRepository : IRepository<Movie>
  {
    //Task<IEnumerable<Movie>> GetMovieByTitle(string title);

    //Task<IEnumerable<Movie>> GetMovieByYear(int from, int to);

    //Task<IEnumerable<Movie>> GetMoviesByRating(float minRating, float maxRating, int? limit);
  }
}
