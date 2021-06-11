using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dotflik.Application.Repositories;
using Dotflik.Application.Repositories.Settings;
using Dotflik.Domain.Aggregates;
using Dotflik.Domain.Exceptions;
using Dapper;
using Npgsql;

namespace Dotflik.Infrastructure.Repositories
{
  /// <summary>
  /// Implementation of the movie repository. This implementation
  /// uses Dapper as the ORM and PostgresSQL as the database
  /// </summary>
  internal class MoviePostgresRepository : Repository, IMovieRepository
  {
    /// <inheritdoc/>
    public MoviePostgresRepository(DatabaseSettings dbSettings) : base(dbSettings)
    { }

    /// <inheritdoc/> 
    async Task<IEnumerable<Movie>> IReadOnlyRepository<Movie>.GetAllAsync(int limit, int offset)
    {
      //var parameters = new { Limit = limit, Offset = offset };
      //var sql = $"SELECT * FROM movies ORDER BY id LIMIT @Limit OFFSET @Offset";

      //try
      //{
      //  await using var connection = new NpgsqlConnection(m_dbSettings.ConnectionString);
      //  await connection.OpenAsync();

      //  var movies = await connection.QueryAsync<Movie>(sql, parameters);

      //  return movies;
      //}
      //catch (Exception ex) when
      //  (ex is PostgresException || ex is NpgsqlException)
      //{
      //  throw new RepositoryException($"Fail to get movies with limit={limit} and offset={offset}", ex);
      //}
      throw new NotImplementedException();
    }

    /// <inheritdoc/>
    async Task<Movie?> IMovieRepository.GetByIdAsync(string id)
    {
      var parameters = new { Id = id };
      var sql = @"
        SELECT m.*, s.*, g.*
        FROM movies m, stars s, genres g, stars_in_movies sm, genres_in_movies gm
        WHERE s.id = sm.starId AND m.id = sm.movieId AND
              g.id = gm.genreId AND m.id = gm.movieId AND
              m.id = @Id";
      try
      {
        await using var connection = new NpgsqlConnection(m_dbSettings.ConnectionString);
        await connection.OpenAsync();
        
        var movies = await connection.QueryAsync<Movie, Domain.Entities.Star, 
                                      Domain.Entities.Genre, Movie>(sql, 
          (m, s, g) => 
          {
            m.Stars.Add(s);
            m.Genres.Add(g);
            return m;
          }, 
          param: parameters, 
          splitOn: "id,id");

        if (movies.Count() == 0)
        {
          return null;
        }

        var resultMovie = movies.GroupBy(m => m.Id).Select(g =>
        {
          // Copy the "entities" properties
          // Movies are grouped by the movie id, hence all the movie's
          // properties are the same
          var groupedMovie = g.First();
          var movie = new Movie() 
          {
            Id = groupedMovie.Id,
            Title = groupedMovie.Title,
            Director = groupedMovie.Director,
            Year = groupedMovie.Year,
            BannerUrl = groupedMovie.BannerUrl
          };
          
          // Populate the "aggregate" properties
          var genres = g.Select(m => m.Genres.Single()).ToHashSet();
          var stars = g.Select(m => m.Stars.Single()).ToHashSet();
          movie.Genres.AddRange(genres);
          movie.Stars.AddRange(stars);
          
          return movie;
        }).Single();

        return resultMovie;
      }
      catch (Exception ex) when
        (ex is PostgresException || ex is NpgsqlException)
      {
        throw new RepositoryException($"Fail to get movie with id={id}", ex);
      }
    }

    /// <inheritdoc/>
    async Task<Movie?> IMovieRepository.GetByTitleAsync(string title)
    {
      //var parameters = new { Title = title };
      //var sql = $"SELECT * FROM movies WHERE title = @Title";
      //try
      //{
      //  await using var connection = new NpgsqlConnection(m_dbSettings.ConnectionString);
      //  await connection.OpenAsync();

      //  // Use Query instead of QuerySingle to avoid InvalidOperationException
      //  // if the id is not found
      //  var movies = connection.Query<Movie>(sql, parameters).AsList();
      //  return movies?.Count == 1 ? movies[0] : null;
      //}
      //catch (Exception ex) when
      //  (ex is PostgresException || ex is NpgsqlException)
      //{
      //  throw new RepositoryException($"Fail to get movie with title={title}", ex);
      //}
      throw new NotImplementedException();
    }

    /// <inheritdoc/>
    async Task<IEnumerable<Movie>> IMovieRepository.GetMoviesByYear(int limit, int offset, int from, int to, bool sortAsc)
    {
      //var parameters = new { Limit = limit, Offset = offset, 
      //                       From = from,   To = to };
      //var sortOrder = sortAsc ? "ASC" : "DESC";
      //var sql = $@"
      //  SELECT *
      //  FROM movies
      //  WHERE year >= @From AND year <= @To
      //  ORDER BY year {sortOrder}
      //  LIMIT @Limit
      //  OFFSET @Offset
      //";

      //try
      //{
      //  await using var connection = new NpgsqlConnection(m_dbSettings.ConnectionString);
      //  await connection.OpenAsync();

      //  var movies = await connection.QueryAsync<Movie>(sql, parameters);
      //  return movies;
      //}
      //catch (Exception ex) when
      //  (ex is PostgresException || ex is NpgsqlException)
      //{
      //  var errorMessage = $@"Fail to get movies within the year range [{from}, {to}] 
      //                        and limit={limit}, offset={offset}";
      //  throw new RepositoryException(errorMessage, ex);
      //}
      throw new NotImplementedException();
    }

  }
}
