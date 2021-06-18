using System;
using System.ComponentModel.DataAnnotations;
using Dotflik.Application.Services.Settings;

namespace Dotflik.WebApp.Client.Settings
{
  /// <summary>
  /// gRPC genre service settings
  /// </summary>
  public class GenreServiceSettings : ServiceSettings
  {
    /// <inheritdoc/>
    [Required]
    public override string Address { get; init; } = string.Empty;

    /// <inheritdoc/>
    [Required]
    public override string Name { get; init; } = string.Empty;
  }


}
