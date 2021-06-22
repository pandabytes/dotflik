using System;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using Fluxor;

using Grpc.Net.Client;
using Grpc.Net.Client.Web;

using Dotflik.Protobuf;
using Dotflik.Infrastructure;
using Dotflik.Application.Validation;
using Dotflik.WebApp.Client.Settings;

namespace Dotflik.WebApp.Client
{
  public class Program
  {
    /// <summary>
    /// Main entry point
    /// </summary>
    /// <param name="args"></param>
    public static async Task Main(string[] args)
    {
      var builder = WebAssemblyHostBuilder.CreateDefault(args);
      builder.RootComponents.Add<App>("#app");

      builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

      builder.Services.AddDataAnnotationValidator();

      builder.Services.AddFluxor(o => o.ScanAssemblies(typeof(Program).Assembly));

      builder.Services.AddSingleton(sp =>
      {
        var config = sp.GetRequiredService<IConfiguration>();
        var grpcSettings = config.GetSection(GrpcSettings.SectionKey).Get<GrpcSettings>();

        var validator = sp.GetRequiredService<IDataAnnotationValidator>();
        validator.ValidateRecursively(grpcSettings);

        return grpcSettings;
      });

      // Initial call to validate the settings right at start up
      _ = builder.Build()
                 .Services
                 .GetRequiredService<GrpcSettings>();

      // Add grpc clients
      builder.Services.AddSingleton(sp =>
      {
        var movieServiceSettings = sp.GetRequiredService<GrpcSettings>().MovieServiceSettings;
        var channel = GrpcChannel.ForAddress(movieServiceSettings.Address, new GrpcChannelOptions
        {
          HttpHandler = new GrpcWebHandler(new HttpClientHandler())
        });
        return new MovieService.MovieServiceClient(channel);
      });

      builder.Services.AddSingleton(sp =>
      {
        var genreServiceSettings = sp.GetRequiredService<GrpcSettings>().GenreServiceSettings;
        var channel = GrpcChannel.ForAddress(genreServiceSettings.Address, new GrpcChannelOptions
        {
          HttpHandler = new GrpcWebHandler(new HttpClientHandler())
        });
        return new GenreService.GenreServiceClient(channel);
      });

      await builder.Build().RunAsync();
    }
  }

  

}
