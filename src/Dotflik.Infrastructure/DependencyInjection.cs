using System;

using Microsoft.Extensions.DependencyInjection;

using Dotflik.Domain.Entities;
using Dotflik.Application.Repositories.Settings;
using Dotflik.Application.Repositories;
using Dotflik.Infrastructure.Repositories;

namespace Dotflik.Infrastructure
{
  /// <summary>
  /// Provide methods to add <see cref="IWriteOnlyRepository{T}"/> service
  /// </summary>
  public static class DependencyInjection
  {
    /// <summary>
    /// Add <see cref="IMovieRepository"/> as a scoped service. Ensure <see cref="DatabaseSettings"/>
    /// is also registered, otherwise an exception will be thrown when trying to
    /// create the movie repository service
    /// </summary>
    /// <exception cref="NotSupportedException">
    /// Thrown when <paramref name="database"/> is not supported yet
    /// </exception>
    /// <param name="services"></param>
    /// <param name="database">The database that is used to store <see cref="Movie"/></param>
    /// <returns>Services</returns>
    public static IServiceCollection AddMovieRepository(this IServiceCollection services, Database database)
    {
      switch (database)
      {
        case Database.PostgresSQL:
          services.AddScoped<IMovieRepository, MoviePostgresRepository>();
          break;
        default:
          throw new NotSupportedException($"Database {database} is not supported yet");
      }

      return services;
    }

    /// <summary>
    /// Add <see cref="IGenreRepository"/> as a scoped service. Ensure <see cref="DatabaseSettings"/>
    /// is also registered, otherwise an exception will be thrown when trying to
    /// create the movie repository service
    /// </summary>
    /// <exception cref="NotSupportedException">
    /// Thrown when <paramref name="database"/> is not supported yet
    /// </exception>
    /// <param name="services"></param>
    /// <param name="database">The database that is used to store <see cref="Genre"/></param>
    /// <returns>Services</returns>
    public static IServiceCollection AddGenreRepository(this IServiceCollection services, Database database)
    {
      switch (database)
      {
        case Database.PostgresSQL:
          services.AddScoped<IGenreRepository, GenrePostgresRepository>();
          break;
        default:
          throw new NotSupportedException($"Database {database} is not supported yet");
      }

      return services;
    }

  }
}
