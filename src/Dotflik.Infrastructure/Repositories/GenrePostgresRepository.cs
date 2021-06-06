using System;
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
  public class GenrePostgresRepository : Repository, IGenreRepository
  {
    public override string RepositoryName => "genres";

    public GenrePostgresRepository(IDatabaseSettings dbSettings) : base(dbSettings) { }

    Task<IEnumerable<Genre>> IReadOnlyRepository<Genre>.GetAllAsync(int limit, int offset)
    {
      throw new NotImplementedException();
    }

    /// <inheritdoc/>
    async Task<Genre?> IGenreRepository.GetByIdAsync(int id)
    {
      var parameters = new { Id = id };
      var sql = $"SELECT * FROM {RepositoryName} WHERE id = @Id";

      try
      {
        await using var connection = new NpgsqlConnection(m_dbSettings.ConnectionString);
        await connection.OpenAsync();

        // Use Query instead of QuerySingle to avoid InvalidOperationException
        // if the id is not found
        var genres = connection.Query<Genre>(sql, parameters).AsList();
        return genres?.Count == 1 ? genres[0] : null;
      }
      catch (Exception ex) when
        (ex is PostgresException || ex is NpgsqlException)
      {
        throw new RepositoryException($"Fail to get genre with id={id}", ex);
      }
    }

    async Task<Genre?> IGenreRepository.GetByNameAsync(string name)
    {
      var parameters = new { Name = name };
      var sql = $"SELECT * FROM {RepositoryName} WHERE Name = @Id";

      try
      {
        await using var connection = new NpgsqlConnection(m_dbSettings.ConnectionString);
        await connection.OpenAsync();

        // Use Query instead of QuerySingle to avoid InvalidOperationException
        // if the id is not found
        var genres = connection.Query<Genre>(sql, parameters).AsList();
        return genres?.Count == 1 ? genres[0] : null;
      }
      catch (Exception ex) when
        (ex is PostgresException || ex is NpgsqlException)
      {
        throw new RepositoryException($"Fail to get genre with name \"{name}\"", ex);
      }
    }

  }
}
