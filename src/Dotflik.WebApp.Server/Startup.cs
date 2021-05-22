using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System;

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
      services.AddGrpc();
      services.AddGrpcReflection();

      services.AddOptions<DatabaseSettings>()
        .Bind(Configuration.GetSection(DatabaseSettings.SectionPath))
        .ValidateDataAnnotations();
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

  }
}
