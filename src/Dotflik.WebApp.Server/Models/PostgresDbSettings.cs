using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using Dotflik.Application.Repositories.Settings;

namespace Dotflik.WebApp.Server.Models
{
  /// <summary>
  /// Containing database settings for PostgresSQL
  /// </summary>
  public class PostgresDbSettings : DatabaseSettings
  {
    /// <summary>
    /// Define where this settings can be found in <see cref="IConfiguration"/>
    /// </summary>
    public static readonly string SectionKey = "DatabaseSettings";

    /// <inheritdoc/>
    public override string ConnectionString
    { 
      get => $"Host={Host};Port={Port};Username={Username};Password={Password};Database={Database}"; 
    }

    [Required]
    public string Database { get; init; } = string.Empty;

    [Required]
    public string Host { get; init; } = string.Empty;

    [Range(1, 65535)]
    public int Port { get; init; }

    [Required]
    public string Username { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;

  }
}
