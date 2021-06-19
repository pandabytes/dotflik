using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Dotflik.Application.Validation;

namespace Dotflik.WebApp.Server.Extensions
{
  /// <summary>
  /// Host extension methods. Mainly used at startup in Main entry point
  /// </summary>
  internal static class IHostExtensions
  {
    /// <summary>
    /// Validate the <typeparamref name="T"/> dependency in
    /// the service collection. This must be called 
    /// after <see cref="IHostBuilder.Build"/>
    /// </summary>
    /// <typeparam name="T">The dependency in the service collection</typeparam>
    /// <param name="host">Host</param>
    /// <returns>Host</returns>
    public static IHost ValidateDataAnnotations<T>(this IHost host)
      where T : class
    {
      var validator = host.Services.GetRequiredService<IDataAnnotationValidator>();
      var service = host.Services.GetRequiredService<T>();

      validator.Validate(service);

      return host;
    }

  }
}
