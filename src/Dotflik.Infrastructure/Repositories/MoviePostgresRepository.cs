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
      var parameters = new { Limit = limit, Offset = offset };
      var sql = @"
        SELECT m.*, s.*, g.*
        FROM (SELECT *
	            FROM movies
              ORDER BY id
	            LIMIT @Limit
	            OFFSET @Offset
	           ) AS m, stars s, genres g, stars_in_movies sm, genres_in_movies gm
        WHERE s.id = sm.starId AND m.id = sm.movieId AND
	            g.id = gm.genreId AND m.id = gm.movieId";

      try
      {
        return await GetMovies(sql, parameters);
      }
      catch (Exception ex) when
        (ex is PostgresException || ex is NpgsqlException)
      {
        throw new RepositoryException($"Fail to get movies with limit={limit} and offset={offset}", ex);
      }
    }

    /// <inheritdoc/>
    async Task<Movie?> IMovieRepository.GetByIdAsync(string id)
    {
      try
      {
        var movies =  await GetByAttribute("id", id);
        if (movies.Count() == 0)
        {
          return null;
        }
        return movies.Single();
      }
      catch (Exception ex) when
        (ex is PostgresException || ex is NpgsqlException)
      {
        throw new RepositoryException($"Fail to get movie with id={id}", ex);
      }
    }

    /// <inheritdoc/>
    async Task<IEnumerable<Movie>> IMovieRepository.GetByTitleAsync(string title)
    {
      try
      {
        return await GetByAttribute("title", title);
      }
      catch (Exception ex) when
        (ex is PostgresException || ex is NpgsqlException)
      {
        throw new RepositoryException($"Fail to get movie with title={title}", ex);
      }
    }

    /// <inheritdoc/>
    async Task<IEnumerable<Movie>> IMovieRepository.GetByYear(int limit, int offset, int from, int to, bool sortAsc)
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

    /// <summary>
    /// Get movies by attribute and the attribute value
    /// </summary>
    /// <param name="attributeName">Name of the attribute in the SQL table</param>
    /// <param name="attributeValue">
    /// Value of the <paramref name="attributeName"/> that
    /// must be equal to</param>
    /// <returns>Movies</returns>
    protected async Task<IEnumerable<Movie>> GetByAttribute(string attributeName, string attributeValue)
    {
      var parameters = new { Attribute = attributeValue };
      var sql = $@"
        SELECT m.*, s.*, g.*
        FROM movies m, stars s, genres g, stars_in_movies sm, genres_in_movies gm 
        WHERE s.id = sm.starId AND m.id = sm.movieId AND
              g.id = gm.genreId AND m.id = gm.movieId AND
              m.{attributeName} = @Attribute";

      return await GetMovies(sql, parameters);
    }

    /// <summary>
    /// Get movies by executing <paramref name="sql"/> with <paramref name="parameters"/>.
    /// </summary>
    /// <remarks>
    /// Ensure the outermost select in <paramref name="sql"/> is "movies.*, stars.*, genres.*".
    /// This method relies on the outermost select to correctly map the entity objects in that order.
    /// Hence the outermost select must strictly be "movies.*, stars.*, genres.*"
    /// </remarks>
    /// <param name="sql">The SQL to execute for this query</param>
    /// <param name="parameters">The parameters to use for this query</param>
    /// <returns>A collection of movie aggregate objects where
    /// the approriate stars and genres are packed into each movie aggregate object
    /// </returns>
    protected async Task<IEnumerable<Movie>> GetMovies(string sql, object parameters)
    {
      await using var connection = new NpgsqlConnection(m_dbSettings.ConnectionString);
      await connection.OpenAsync();

      var movies = await connection.QueryAsync<Movie, Domain.Entities.Star,
                                               Domain.Entities.Genre, Movie>(
        sql: sql,
        map: (m, s, g) =>
        {
          m.Stars.Add(s);
          m.Genres.Add(g);
          return m;
        },
        param: parameters,
        splitOn: "id,id");

      var resultMovies = movies.GroupBy(m => m.Id)
                               .Select(g =>
                               {
                                 // Copy the "entities" properties
                                 // Movies are grouped by the movie id, hence
                                 // all the movies' properties are the same
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
                                });
      return resultMovies;
    }

  }
}
