using System;

namespace Dotflik.Application.Services.Settings
{
  /// <summary>
  /// Settings of a service
  /// </summary>
  public abstract class ServiceSettings
  {
    /// <summary>
    /// The address to the service
    /// </summary>
    public abstract string Address { get; init; }

    /// <summary>
    /// The name of the service
    /// </summary>
    public abstract string Name { get; init; }
  }
}
