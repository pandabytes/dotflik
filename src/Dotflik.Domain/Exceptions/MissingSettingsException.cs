using System;

namespace Dotflik.Domain.Exceptions
{
  /// <summary>
  /// Exception that indicates settings are missing
  /// </summary>
  public class MissingSettingsException : Exception
  {
    public MissingSettingsException(string message) : base(message) { }

    public MissingSettingsException(string message, Exception innerEx) : base(message, innerEx) { }
  }
}
