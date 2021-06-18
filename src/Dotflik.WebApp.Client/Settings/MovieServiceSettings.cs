using System;
using System.ComponentModel.DataAnnotations;

using Dotflik.Application.Services.Settings;

namespace Dotflik.WebApp.Client.Settings
{
  /// <summary>
  /// gRPC movie service settings
  /// </summary>
  public class MovieServiceSettings : ServiceSettings
  {
    /// <inheritdoc/>
    [Required]
    public override string Address { get; init; } = string.Empty;

    /// <inheritdoc/>
    [Required]
    public override string Name { get; init; } = string.Empty;
  }

}
