﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dotflik.Application.Repositories;
using Dotflik.Application.Settings;
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

    async Task<Movie?> IRepository<Movie>.GetByIdAsync(string id)
    {
      throw new NotImplementedException();
    }

    async Task IRepository<Movie>.UpdateAsync(Movie entity)
    {
      throw new NotImplementedException();
    }
  }
}