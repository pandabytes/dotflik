using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text.RegularExpressions;

namespace Dotflik.WebApp.Server
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var x = "limit=23&offset=121232312";
      MatchCollection mc = Regex.Matches(x, "^limit=[0-9]+&offset=[0-9]+$");
      //var tokens = x.Split('&');
      //var limit = tokens[0].Split('=')[1];
      //var offset = tokens[1].Split('=')[1];
      //Console.WriteLine(limit);
      //Console.WriteLine(offset);
      foreach (var m in mc)
      {
        Console.WriteLine(m);
      }

      Console.WriteLine(Regex.IsMatch(x, "^limit=[0-9]+&offset=[0-9]+$"));

      var y = Regex.Replace(x, "^limit=[0-9]+", "fo");
      Console.WriteLine(y);

      CreateHostBuilder(args)
        .Build()
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
