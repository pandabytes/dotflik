using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Dotflik.WebApp.Client.Interop
{
  /// <summary>
  /// Provide members to load Javascript module.
  /// </summary>
  public abstract class JavascriptModule
  {
    /// <summary>
    /// The JS runtime used to run Javascript code.
    /// </summary>
    protected readonly IJSRuntime m_jSRuntime;

    /// <summary>
    /// The Javascript module that contains exported variables,
    /// classes, functions, etc... If null, then the module
    /// has not been loaded yet.
    /// </summary>
    protected IJSObjectReference? m_module;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="jSRuntime">The JS runtime used to run Javascript code</param>
    public JavascriptModule(IJSRuntime jSRuntime) => m_jSRuntime = jSRuntime;

    /// <summary>
    /// Path to where the Javascript module file is located.
    /// </summary>
    protected abstract string ModuleScriptPath { get; }

    /// <summary>
    /// Load the module defined at <see cref="ModuleScriptPath"/>
    /// asynchronously.
    /// </summary>
    /// <remarks>
    /// This only needs to be called once. Calling this method
    /// more than once will do nothing.
    /// </remarks>
    /// <returns>Empty task</returns>
    public async Task LoadModuleAsync()
    {
      if (m_module is null)
      {
        m_module = await m_jSRuntime.InvokeAsync<IJSObjectReference>("import", ModuleScriptPath);
      }
    }

  }
}
