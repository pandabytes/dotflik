using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Dotflik.WebApp.Client.Interop
{
  /// <summary>
  /// This class exposes the functions defined in the module "Interop/modalBootstrap.ts".
  /// See https://getbootstrap.com/docs/4.0/components/modal/#usage.
  /// All methods of this class are asynchronous.
  /// </summary>
  public class ModalBootstrap : JavascriptModule
  {
    /// <inheritdoc/>
    protected override string ModuleScriptPath => "./js/modalBootstrap.js";

    /// <inheritdoc/>
    public ModalBootstrap(IJSRuntime jSRuntime) : base(jSRuntime) { }

    /// <summary>
    /// Show a modal with id <paramref name="modalId"/>.
    /// </summary>
    /// <remarks>
    /// This method will wait for the modal to be shown before it ends.
    /// </remarks>
    /// <param name="modalId">Id of the modal</param>
    /// <returns>Empty task</returns>
    public async Task ShowModalAsync(string modalId)
    {
      var normalizedModalId = NormalizeModalId(modalId);
      await Module.InvokeVoidAsync("showModal", normalizedModalId); 
    }

    /// <summary>
    /// Hide a modal with id <paramref name="modalId"/>.
    /// </summary>
    /// <remarks>
    /// This method will wait for the modal to be hidden before it ends.
    /// </remarks>
    /// <param name="modalId">Id of the modal</param>
    /// <returns>Empty task</returns>
    public async Task HideModalAsync(string modalId)
    {
      var normalizedModalId = NormalizeModalId(modalId);
      await Module.InvokeVoidAsync("hideModal", normalizedModalId);
    }

    /// <summary>
    /// Normalize the modal <paramref name="id"/> by prepending "#" to
    /// <paramref name="id"/>. If "#" is already prepended, then 
    /// <paramref name="id"/> is returned.
    /// </summary>
    /// <param name="id">The modal id</param>
    /// <returns>Return <paramref name="id"/> with "#" prepended to it</returns>
    protected static string NormalizeModalId(string id)
      => id.StartsWith("#") ? id : $"#{id}";

  }
}
