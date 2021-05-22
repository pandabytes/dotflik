using System;
using System.ComponentModel.DataAnnotations;

namespace Dotflik.WebApp.Server.Models
{
  public class DatabaseSettings
  {
    public static readonly string SectionPath = "DatabaseSettings";

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
