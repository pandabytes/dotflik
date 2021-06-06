using System;
using Dotflik.Application.Repositories.Settings;

namespace Dotflik.Application.Repositories
{
  /// <summary>
  /// Base repository class containing data relating
  /// to a repository, such as database settings
  /// </summary>
  public abstract class Repository
  {
    /// <summary>
    /// Reference to a database settings
    /// </summary>
    protected readonly DatabaseSettings m_dbSettings;

    /// <summary>
    /// Name of the repository
    /// </summary>
    public abstract string RepositoryName { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="dbSettings"/> has an 
    /// empty connection string
    /// </exception>
    /// <param name="dbSettings">The database settings object</param>
    public Repository(DatabaseSettings dbSettings)
    {
      if (string.IsNullOrWhiteSpace(dbSettings.ConnectionString))
      {
        throw new ArgumentException("Connection string must not be empty");
      }

      m_dbSettings = dbSettings;
    }

  }
}
