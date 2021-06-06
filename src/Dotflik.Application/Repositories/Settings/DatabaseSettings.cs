using System;

namespace Dotflik.Application.Repositories.Settings
{
  /// <summary>
  /// Base class for database settings
  /// </summary>
  public abstract class DatabaseSettings
  {
    /// <summary>
    /// Connection string to a database. This string is 
    /// dependent on whichever database is used
    /// </summary>
    public abstract string ConnectionString { get; }
  }
}
