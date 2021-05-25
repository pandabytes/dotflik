using System;

namespace Dotflik.Application.Repositories.Settings
{
  /// <summary>
  /// Interface for database settings
  /// </summary>
  public interface IDatabaseSettings
  {
    /// <summary>
    /// Connection string to a database. This string is 
    /// dependent on whichever database is used
    /// </summary>
    string ConnectionString { get; }
  }
}
