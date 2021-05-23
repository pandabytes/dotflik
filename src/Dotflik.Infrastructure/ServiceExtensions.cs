using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using Dotflik.Application.Settings;
using Dotflik.Application.Repositories;
using Dotflik.Infrastructure.Repositories;
using Dotflik.Infrastructure.Settings;

namespace Dotflik.Infrastructure
{
  /// <summary>
  /// Provide extensions to the class <see cref="IServiceCollection"/>,
  /// mainly to provide methods to register dependencies.
  /// </summary>
  public static class ServiceExtensions
  {
    /// <summary>
    /// Register an implementation of <see cref="IDatabaseSettings"/> as a singleton
    /// </summary>
    /// <exception cref="ValidationException">
    /// Thrown when database settings aren't available or fail the data annotation validation
    /// </exception>
    /// <param name="services">Services collection object</param>
    /// <param name="configuration">Configuration that contains database settings</param>
    /// <returns>Services collection object</returns>
    public static IServiceCollection AddDbSettings(this IServiceCollection services, IConfiguration configuration)
    {
      var dbSettings = ValidateSettingsDataAnnotations<PostgresDbSettings>(configuration, PostgresDbSettings.SectionKey);
      services.AddSingleton<IDatabaseSettings>(dbSettings);
      return services;
    }

    /// <summary>
    /// Register repositories as scoped services
    /// </summary>
    /// <param name="services">Services collection object</param>
    /// <returns>Services collection object</returns>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {     
      services.AddScoped<IMovieRepository, MoviePostgresRepository>();
      return services;
    }

    /// <summary>
    /// Validate the data annotations on the settings object
    /// in <paramref name="configuration"/> with key <paramref name="sectionKey"/>.
    /// </summary>
    /// <remarks>
    /// If no data annotation is specified in type <typeparamref name="T"/>, then
    /// validation will always pass
    /// </remarks>
    /// 
    /// <exception cref="ValidationException">
    /// Thrown when settings aren't available or fail the data annotation validation
    /// </exception>
    /// <typeparam name="T">
    /// A class that has <see cref="ValidationAttribute"/> marked on its properties
    /// </typeparam>
    /// 
    /// <param name="configuration">Configuration object</param>
    /// <param name="sectionKey">Key in the <paramref name="configuration"/></param>
    /// 
    /// <returns>The settings object found at 
    /// <paramref name="sectionKey"/> in the <paramref name="configuration"/>
    /// </returns>
    private static T ValidateSettingsDataAnnotations<T>(IConfiguration configuration, string sectionKey)
      where T : class
    {
      var settings = configuration.GetSection(sectionKey).Get<T>();
      if (settings == null)
      {
        throw new ValidationException($"Settings \"{sectionKey}\" is not provided in the configuration");
      }

      var validationResults = new List<ValidationResult>();
      var validationContext = new ValidationContext(settings, null, null);

      if (!Validator.TryValidateObject(settings, validationContext, validationResults, true))
      {
        var errorMessages = validationResults.Select(vr => vr.ErrorMessage);
        var exMessage = $"Validation failed for section \"{sectionKey}\";" + string.Join(";", errorMessages);
        throw new ValidationException(exMessage);
      }

      return settings;
    }
  }
}
