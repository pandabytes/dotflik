using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

using Dotflik.Application.Repositories;
using Dotflik.Application.Repositories.Settings;
using Dotflik.Domain.Exceptions;
using Dotflik.Infrastructure;
using Dotflik.WebApp.Server.Interceptors;
using Dotflik.WebApp.Server.Services;
using Dotflik.WebApp.Server.Settings;

namespace Dotflik.WebApp.Server
{
  public class Startup
  {
    private const string AllowAllCorsPolicy = "AllowAll";

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

      services.AddCors(o => o.AddPolicy(AllowAllCorsPolicy, builder =>
      {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader()
               .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
      }));

      var dbSettings = Configuration.GetSection(PostgresDbSettings.SectionKey)
                                    .Get<PostgresDbSettings>();
      if (dbSettings is null)
      {
        throw new MissingSettingsException($"Settings \"{PostgresDbSettings.SectionKey}\" is not provided in the configuration");
      }
      services.AddSingleton<DatabaseSettings>(dbSettings);

      services.AddMovieRepository(Database.PostgresSQL)
              .AddGenreRepository(Database.PostgresSQL)
              .AddDataAnnotationValidator()
              .AddPaginationTokenFactory();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseRouting();
      app.UseGrpcWeb();
      app.UseCors();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapGrpcService<MovieService>()
                 .EnableGrpcWeb()
                 .RequireCors(AllowAllCorsPolicy);

        endpoints.MapGrpcService<GenreService>()
                 .EnableGrpcWeb()
                 .RequireCors(AllowAllCorsPolicy);

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

  }
}
