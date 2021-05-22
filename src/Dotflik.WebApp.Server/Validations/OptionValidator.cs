using System;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Dotflik.WebApp.Server.Validations
{
  public static class OptionValidator
  {
    /// <summary>
    /// Begin the validation on <see cref="IOptions{T}"/> with generic type <typeparamref name="T"/>.
    /// This method should be called after <see cref="IHostBuilder.Build"/>
    /// </summary>
    /// <remarks>
    /// This method simply request the service with type IOptions&lt;<typeparamref name="T"/>&gt;
    /// and the validation logic will be executed by the DI container. Hence it is important
    /// to set up the validation logic for <typeparamref name="T"/> using <see cref="IOptions{TOptions}"/>
    /// in <see cref="Startup.ConfigureServices(IServiceCollection)"/>
    /// </remarks>
    /// <typeparam name="T">The settings</typeparam>
    /// <param name="host"></param>
    /// <returns>The <paramref name="host"/> object</returns>
    public static IHost BeginOptionsValidation<T>(this IHost host) where T : class
    {
      _ = host.Services.GetRequiredService<IOptions<T>>().Value;
      return host;
    }

  }
}
