using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Dotflik.Application.Repositories;
using Dotflik.Application.Repositories.Settings;
using Dotflik.Domain.Entities;
using Dotflik.Domain.Exceptions;
using Npgsql;

namespace Dotflik.Infrastructure.Repositories
{
  internal class GenrePostgresRepository : Repository, IGenreRepository
  {
    public GenrePostgresRepository(DatabaseSettings dbSettings) : base(dbSettings) { }

    Task<IEnumerable<Genre>> IReadOnlyRepository<Genre>.GetAllAsync(int limit, int offset)
    {
      throw new NotImplementedException();
    }

    /// <inheritdoc/>
    async Task<Genre?> IGenreRepository.GetByIdAsync(int id)
    {
      var parameters = new { Id = id };
      var sql = $"SELECT * FROM genres WHERE id = @Id";

      try
      {
        await using var connection = new NpgsqlConnection(m_dbSettings.ConnectionString);
        await connection.OpenAsync();

        // Use Query instead of QuerySingle to avoid InvalidOperationException
        // if the id is not found
        var genres = await connection.QueryAsync<Genre>(sql, parameters);
        return genres.FirstOrDefault();
      }
      catch (Exception ex) when
        (ex is PostgresException || ex is NpgsqlException)
      {
        throw new RepositoryException($"Fail to get genre with id={id}", ex);
      }
    }

    /// <inheritdoc/>
    async Task<Genre?> IGenreRepository.GetByNameAsync(string name)
    {
      var parameters = new { Name = name };
      var sql = $"SELECT * FROM genres WHERE Name = @Id";

      try
      {
        await using var connection = new NpgsqlConnection(m_dbSettings.ConnectionString);
        await connection.OpenAsync();

        // Use Query instead of QuerySingle to avoid InvalidOperationException
        // if the id is not found
        var genres = await connection.QueryAsync<Genre>(sql, parameters);
        return genres.FirstOrDefault();
      }
      catch (Exception ex) when
        (ex is PostgresException || ex is NpgsqlException)
      {
        throw new RepositoryException($"Fail to get genre with name \"{name}\"", ex);
      }
    }

    /// <inheritdoc/>
    async Task<IEnumerable<string>> IGenreRepository.GetGenreNamesAsync()
    {
      const string sql = "SELECT name FROM genres";
      try
      {
        await using var connection = new NpgsqlConnection(m_dbSettings.ConnectionString);
        await connection.OpenAsync();

        var genres = await connection.QueryAsync<Genre>(sql);
        return genres.Select(g => g.Name);
      }
      catch (Exception ex) when
        (ex is PostgresException || ex is NpgsqlException)
      {
        throw new RepositoryException("Fail to get only genre names", ex);
      }
    }

  }
}
