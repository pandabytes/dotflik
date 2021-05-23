using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace Dotflik.WebApp.Server
{
  public class Program
  {
    public static void Main(string[] args)
    {
      CreateHostBuilder(args)
        .Build()
        .Run();
      //var m = new Domain.Entities.Movie("", "", 0, "", "");
      //dynamic c = new { m.Director, m.Year };
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
              webBuilder.UseStartup<Startup>();
            });
  }
}
