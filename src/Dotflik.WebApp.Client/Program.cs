using System;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

using Grpc.Net.Client;
using Grpc.Net.Client.Web;

using Dotflik.Protobuf.Movie;

namespace Dotflik.WebApp.Client
{
  public class Program
  {
    public static async Task Main(string[] args)
    {
      var builder = WebAssemblyHostBuilder.CreateDefault(args);
      builder.RootComponents.Add<App>("#app");

      builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

      builder.Services.AddSingleton(sp =>
      {
        var channel = GrpcChannel.ForAddress("https://localhost:5001", new GrpcChannelOptions
        {
          HttpHandler = new GrpcWebHandler(new HttpClientHandler())
        });
        
        return new MovieService.MovieServiceClient(channel);
      });

      await builder.Build().RunAsync();
    }
  }
}
