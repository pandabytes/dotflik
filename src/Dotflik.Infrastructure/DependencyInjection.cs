using System;
using Microsoft.Extensions.DependencyInjection;

using Dotflik.Domain.Entities;
using Dotflik.Application.Paginations;
using Dotflik.Application.Paginations.Args;
using Dotflik.Application.Repositories.Settings;
using Dotflik.Application.Repositories;
using Dotflik.Application.Validation;
using Dotflik.Infrastructure.Paginations;
using Dotflik.Infrastructure.Repositories;
using Dotflik.Infrastructure.Validation;

namespace Dotflik.Infrastructure
{
  /// <summary>
  /// Factory method for creating a pagination token object.
  /// </summary>
  /// <exception cref="NotSupportedException">
  /// Thrown when a token type in <see cref="PaginationTokenType"/> is not supported.
  /// </exception>
  /// <exception cref="ArgumentException">
  /// Thrown when <param name="tokenType"/> and 
  /// <param name="tokenArgs"/> don't match.
  /// </exception>
  /// <param name="tokenType">Type of the token</param>
  /// <param name="tokenArgs">Arguments to construct the token</param>
  /// <returns>Token object</returns>
  public delegate IPaginationToken PaginationTokenFactory(PaginationTokenType tokenType, PaginationTokenArgs tokenArgs);

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

    /// <summary>
    /// Add <see cref="IDataAnnotationValidator"/> as a transient service
    /// </summary>
    /// <param name="services"></param>
    /// <returns>Services</returns>
    public static IServiceCollection AddDataAnnotationValidator(this IServiceCollection services)
    {
      services.AddTransient<IDataAnnotationValidator, DataAnnotationValidator>();
      return services;
    }

    /// <summary>
    /// Add a <see cref="PaginationTokenFactory"/> as a singleton
    /// to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <returns>Services</returns>
    public static IServiceCollection AddPaginationTokenFactory(this IServiceCollection services)
    {
      services.AddTransient<PaginationTokenFactory>(provider => (tokenType, tokenArgs) =>
      {
        // Construct token using string
        if (tokenArgs.Value is not null)
        {
          switch (tokenType)
          {
            case PaginationTokenType.LimitOffset:
              return new RegexLimitOffsetPaginationToken(tokenArgs.Value);

            default:
              throw new NotSupportedException($"Pagination token type \"{tokenType}\" is not supported.");
          }
        }

        var mismatchArgsMessage = $"Type of {nameof(tokenArgs)} is {tokenArgs.GetType().FullName} in which " + 
                                  $"doesn't match with token type {tokenType}";

        // Construct token using arguments specific to each token type
        switch (tokenType)
        {
          case PaginationTokenType.LimitOffset:
            var limitOffsetArgs = tokenArgs as LimitOffsetPaginationTokenArgs;
            if (limitOffsetArgs is null)
            {
              throw new ArgumentException(mismatchArgsMessage);
            }

            return new RegexLimitOffsetPaginationToken(limitOffsetArgs);

          default:
            throw new NotSupportedException($"Pagination token type \"{tokenType}\" is not supported.");
        }
      });

      return services;
    }

  }
}
