using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using Dotflik.Application.Repositories.Settings;
using Dotflik.WebApp.Server.Extensions;

namespace Dotflik.WebApp.Server
{
  public class Program
  {
    public static void Main(string[] args)
    {
      CreateHostBuilder(args)
        .Build()
        .ValidateDataAnnotations<DatabaseSettings>()
        .Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
              webBuilder.UseStartup<Startup>();
            });
  }
}
