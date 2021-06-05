using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

using Dotflik.Application.Repositories;
using Dotflik.Application.Repositories.Settings;
using Dotflik.Infrastructure;
using Dotflik.WebApp.Server.Interceptors;
using Dotflik.WebApp.Server.Services;
using Dotflik.WebApp.Server.Models;

namespace Dotflik.WebApp.Server
{
  public class Startup
  {
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }
    
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddGrpc(options => {
        // Order matters
        options.Interceptors.Add<GlobalExceptionInterceptor>();
        options.Interceptors.Add<PaginationRequestInterceptor>();
      });
      services.AddGrpcReflection();

      var dbSettings = ValidateDataAnnotations<PostgresDbSettings>(Configuration, PostgresDbSettings.SectionKey);
      services.AddSingleton<IDatabaseSettings>(dbSettings);

      services.AddMovieRepository(Database.PostgresSQL);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseRouting();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapGrpcService<MovieService>();

        endpoints.MapGet("/", async context =>
        {
            await context.Response.WriteAsync("Only gRPC is supported");
        });

        if (env.IsDevelopment())
        {
          endpoints.MapGrpcReflectionService();
        }

      });
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
    private static T ValidateDataAnnotations<T>(IConfiguration configuration, string sectionKey)
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
