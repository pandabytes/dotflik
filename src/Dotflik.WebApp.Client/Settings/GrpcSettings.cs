using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

namespace Dotflik.WebApp.Client.Settings
{
  /// <summary>
  /// Settings of gRPC services
  /// </summary>
  public class GrpcSettings
  {
    /// <summary>
    /// Define where this settings can be found in <see cref="IConfiguration"/>
    /// </summary>
    public static readonly string SectionKey = nameof(GrpcSettings);

    [Required]
    public MovieServiceSettings MovieServiceSettings { get; init; } = null!;

    [Required]
    public GenreServiceSettings GenreServiceSettings { get; init; } = null!;
  }

}
