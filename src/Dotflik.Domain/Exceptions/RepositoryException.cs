using System;

namespace Dotflik.Domain.Exceptions
{
  /// <summary>
  /// Exception dealing with interacting a repository such
  /// connecion to database drops, invalid SQL query, bad database settings, etc...
  /// </summary>
  public class RepositoryException : Exception
  {
    public RepositoryException(string message, Exception innerEx) : base(message, innerEx) { }
  }
}
