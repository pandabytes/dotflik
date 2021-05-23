using System;
using Dotflik.Application.Settings;

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
    protected readonly IDatabaseSettings m_dbSettings;

    /// <summary>
    /// Name of the repository
    /// </summary>
    public abstract string RepositoryName { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dbSettings">The database settings object</param>
    public Repository(IDatabaseSettings dbSettings)
      => m_dbSettings = dbSettings;

  }
}
