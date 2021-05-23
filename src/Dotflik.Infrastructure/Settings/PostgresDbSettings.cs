using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using Dotflik.Application.Settings;

namespace Dotflik.Infrastructure.Settings
{
  /// <summary>
  /// Containing database settings for PostgresSQL
  /// </summary>
  internal class PostgresDbSettings : IDatabaseSettings
  {
    /// <summary>
    /// Define where this settings can be found in <see cref="IConfiguration"/>
    /// </summary>
    public static readonly string SectionKey = "DatabaseSettings";

    /// <inheritdoc/>
    string IDatabaseSettings.ConnectionString 
      => $"Host={Host};Port={Port};Username={Username};Password={Password};Database={Database}";

    [Required]
    public string Database { get; init; } = null!;

    [Required]
    public string Host { get; init; } = null!;

    [Range(1, 65535)]
    public int Port { get; init; }

    [Required]
    public string Username { get; init; } = null!;

    [Required]
    public string Password { get; init; } = null!;

  }
}
