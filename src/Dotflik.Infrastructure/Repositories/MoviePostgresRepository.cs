using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dotflik.Application.Repositories;
using Dotflik.Application.Repositories.Settings;
using Dotflik.Domain.Entities;
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
    public override string RepositoryName => "movies";

    /// <inheritdoc/>
    public MoviePostgresRepository(IDatabaseSettings dbSettings) : base(dbSettings)
    { }

    async Task IRepository<Movie>.CreateAsync(Movie entity)
    {
      throw new NotImplementedException();
    }

    async Task IRepository<Movie>.DeleteAsync(int id)
    {
      throw new NotImplementedException();
    }

    async Task IRepository<Movie>.DeleteAsync(string id)
    {
      throw new NotImplementedException();
    }

    /// <inheritdoc/> 
    async Task<IEnumerable<Movie>> IRepository<Movie>.GetAllAsync(int limit, int offset)
    {
      var parameters = new { Limit = limit, Offset = offset };
      var sql = $"SELECT * FROM {RepositoryName} ORDER BY id LIMIT @Limit OFFSET @Offset";

      try
      {
        await using var connection = new NpgsqlConnection(m_dbSettings.ConnectionString);
        await connection.OpenAsync();

        var movies = await connection.QueryAsync<Movie>(sql, parameters);
        return movies;
      }
      catch (Exception ex) when
        (ex is PostgresException || ex is NpgsqlException)
      {
        throw new RepositoryException($"Fail to get movies with limit={limit} and offset={offset}", ex);
      }
    }

    async Task<Movie?> IRepository<Movie>.GetByIdAsync(int id)
    {
      throw new NotImplementedException();
    }

    /// <inheritdoc/>
    async Task<Movie?> IRepository<Movie>.GetByIdAsync(string id)
    {
      var parameters = new { Id = id };
      var sql = $"SELECT * FROM {RepositoryName} WHERE id = @Id";
      try
      {
        await using var connection = new NpgsqlConnection(m_dbSettings.ConnectionString);
        await connection.OpenAsync();

        // Use Query instead of QuerySingle to avoid InvalidOperationException
        // if the id is not found
        var movies = connection.Query<Movie>(sql, parameters).AsList();
        return movies?.Count == 1 ? movies[0] : null;
      }
      catch (Exception ex) when
        (ex is PostgresException || ex is NpgsqlException)
      {
        throw new RepositoryException($"Fail to get movie with id={id}", ex);
      }
    }

    async Task IRepository<Movie>.UpdateAsync(Movie entity)
    {
      throw new NotImplementedException();
    }

    /// <inheritdoc/>
    async Task<Movie?> IMovieRepository.GetByTitleAsync(string title)
    {
      var parameters = new { Title = title };
      var sql = $"SELECT * FROM {RepositoryName} WHERE title = @Title";
      try
      {
        await using var connection = new NpgsqlConnection(m_dbSettings.ConnectionString);
        await connection.OpenAsync();

        // Use Query instead of QuerySingle to avoid InvalidOperationException
        // if the id is not found
        var movies = connection.Query<Movie>(sql, parameters).AsList();
        return movies?.Count == 1 ? movies[0] : null;
      }
      catch (Exception ex) when
        (ex is PostgresException || ex is NpgsqlException)
      {
        throw new RepositoryException($"Fail to get movie with title={title}", ex);
      }
    }

    /// <inheritdoc/>
    async Task<IEnumerable<Movie>> IMovieRepository.GetMoviesByYear(int limit, int offset, int from, int to, bool sortAsc)
    {
      var parameters = new { Limit = limit, Offset = offset, 
                             From = from,   To = to };
      var sortOrder = sortAsc ? "ASC" : "DESC";
      var sql = $@"
        SELECT *
        FROM {RepositoryName}
        WHERE year >= @From AND year <= @To
        ORDER BY year {sortOrder}
        LIMIT @Limit
        OFFSET @Offset
      ";

      try
      {
        await using var connection = new NpgsqlConnection(m_dbSettings.ConnectionString);
        await connection.OpenAsync();

        var movies = await connection.QueryAsync<Movie>(sql, parameters);
        return movies;
      }
      catch (Exception ex) when
        (ex is PostgresException || ex is NpgsqlException)
      {
        var errorMessage = $@"Fail to get movies within the year range [{from}, {to}] 
                              and limit={limit}, offset={offset}";
        throw new RepositoryException(errorMessage, ex);
      }
    }


  }
}
