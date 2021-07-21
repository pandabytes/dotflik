using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using Fluxor;

using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;

using Dotflik.Protobuf;
using Dotflik.Infrastructure;
using Dotflik.Application.Validation;
using Dotflik.WebApp.Client.Settings;
using Dotflik.WebApp.Client.Interop;
using Dotflik.WebApp.Client.Store.Genres;

namespace Dotflik.WebApp.Client
{
  /// <summary>
  /// Delegate used to get the genre color. The color depends
  /// on whatever this delgate returns. If <paramref name="genreName"/>
  /// is invalid then a default color should be returned.
  /// </summary>
  /// <remarks>
  /// Color can be a literal color string such as "red", "blue", etc. Or
  /// it can be a css class such as bootstrap badge class "badge-primary".
  /// </remarks>
  /// <param name="genreName">Name of the genre.</param>
  /// <returns>Color string.</returns>
  public delegate string GetGenreColor(string genreName);

  public static class Program
  {
    /// <summary>
    /// Main entry point.
    /// </summary>
    /// <param name="args"></param>
    public static async Task Main(string[] args)
    {
      var builder = WebAssemblyHostBuilder.CreateDefault(args);
      builder.RootComponents.Add<App>("#app");

      builder.AddDependencies()
             .AddGetGenreColor();

      var webHost = builder.Build();
      var services = webHost.Services;

      // Get service to validate the settings right at start up
      _ = services.GetRequiredService<GrpcSettings>();

      await services.GetRequiredService<ModalBootstrap>()
                    .LoadModuleAsync();

      // Set the genre color mappings in a fire-and-forget style. We don't care
      // if it fails or not, so that it doesn't block the main thread
      var dispatcher = services.GetRequiredService<IDispatcher>();
      var genreService = services.GetRequiredService<GenreService.GenreServiceClient>();
      _ = SetGenreColorMappingsAsync(dispatcher, genreService, 3).ConfigureAwait(false);

      await webHost.RunAsync();
    }

    /// <summary>
    /// Add all the dependencies needed for the Dotflik application.
    /// </summary>
    /// <param name="builder">The builde.r</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    private static WebAssemblyHostBuilder AddDependencies(this WebAssemblyHostBuilder builder)
    {
      builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

      builder.Services.AddDataAnnotationValidator();

      builder.Services.AddSingleton<ModalBootstrap>();

      builder.Services.AddFluxor(o => o.ScanAssemblies(typeof(Program).Assembly));

      builder.Services.AddSingleton(sp =>
      {
        var config = sp.GetRequiredService<IConfiguration>();
        var grpcSettings = config.GetSection(GrpcSettings.SectionKey).Get<GrpcSettings>();

        var validator = sp.GetRequiredService<IDataAnnotationValidator>();
        validator.ValidateRecursively(grpcSettings);

        return grpcSettings;
      });

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

      return builder;
    }

    /// <summary>
    /// Add the <see cref="GetGenreColor"/> delegate as a scoped service.
    /// </summary>
    /// <remarks>
    /// Add as a scoped service because the <see cref="GetGenreColor"/> delgate
    /// queries for <see cref="IState{TState}"/> in which is added as a scoped service.
    /// </remarks>
    /// <param name="builder">The builder.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    private static WebAssemblyHostBuilder AddGetGenreColor(this WebAssemblyHostBuilder builder)
    {
      builder.Services.AddScoped<GetGenreColor>(sp => genreName =>
      {
        var genresState = sp.GetRequiredService<IState<GenresState>>();
        var genreColorMappings = genresState.Value.GenreColorMappings;
        return genreColorMappings.GetValueOrDefault(genreName, "badge-pill");
      });

      return builder;
    }

    /// <summary>
    /// Set the genre color mappings to the store asynchronously. If this method encounters an
    /// <see cref="RpcException"/>, then nothing is done and the application can
    /// still continue.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to send action to update the store.</param>
    /// <param name="genreService">The genre service to query all the genre names.</param>
    /// <param name="genreServiceTimeout">
    /// Time to wait in seconds for the <paramref name="genreService"/> to return the list of genre names.
    /// </param>
    /// <returns>Empty task.</returns>
    private static async Task SetGenreColorMappingsAsync(IDispatcher dispatcher, 
                                                         GenreService.GenreServiceClient genreService, 
                                                         int genreServiceTimeout)
    {
      try
      {
        var grpcCallOptions = new CallOptions(deadline: DateTime.UtcNow.AddSeconds(genreServiceTimeout));
        var response = await genreService.GetGenreNamesOnlyAsync(new Empty(), grpcCallOptions);
        var genreNames = response.GenreNames;

        string[] badgeColors = new string[]
        {
          "badge-primary",
          "badge-secondary",
          "badge-success",
          "badge-danger",
          "badge-warning",
          "badge-info",
          "badge-light",
          "badge-dark",
        };

        var genreColorMappings = new Dictionary<string, string>();
        var badgeIndex = 0;
        foreach (var genreName in genreNames)
        {
          // Cycle through the color list since the colors don't cover all the genre names
          badgeIndex = badgeIndex >= badgeColors.Length ? 0 : badgeIndex;
          genreColorMappings.Add(genreName, badgeColors[badgeIndex++]);
        }

        dispatcher.Dispatch(new GenresSetGenreColorMappingsAction(genreColorMappings));
      }
      catch (RpcException)
      {
        // Ignore rpc exception to let the app continues
      }
    }

  }

}
