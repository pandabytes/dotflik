using System;
using System.Threading.Tasks;
using Dotflik.WebApp.Client.Interop;
using Microsoft.AspNetCore.Components;

namespace Dotflik.WebApp.Client.Components.Modals
{
  /// <summary>
  /// Serve as a template to create modal dialog. Consumers who 
  /// want to use this template can set the header, body, and footer
  /// of the modal to whatever they want.
  /// </summary>
  /// <remarks>
  /// If a child class wants to expose some of the protected fields,
  /// then simply create a public property that wraps around 
  /// the protected field and add <see cref="ParameterAttribute"/>
  /// to that public property.
  /// </remarks>
  public partial class ModalTemplate : ComponentBase
  {
    /// <summary>
    /// Store the modal id. To access this id,
    /// use <see cref="ModalId"/> instead.
    /// </summary>
    private string m_modalId = string.Empty;

    /// <summary>
    /// The UI fragment of the modal header. To render the
    /// header correctly, wrap it in 
    /// &lt;div class="modal-header"&gt;&lt;/div&gt;
    /// </summary>
    protected RenderFragment? m_modalHeader;

    /// <summary>
    /// The UI fragment of the modal body. To render the
    /// body correctly, wrap it in 
    /// &lt;div class="modal-body"&gt;&lt;/div&gt;
    /// </summary>
    protected RenderFragment? m_modalBody;

    /// <summary>
    /// The UI fragment of the modal footer. To render the
    /// header correctly, wrap it in 
    /// &lt;div class="modal-footer"&gt;&lt;/div&gt;
    /// </summary>
    protected RenderFragment? m_modalFooter;

    /// <summary>
    /// Modal bootstrap module that provides methods
    /// used to interact with bootstrap modal.
    /// </summary>
    [Inject]
    protected ModalBootstrap ModalBootstrap { get; init; } = null!;

    /// <summary>
    /// The id of the modal.
    /// </summary>
    /// <remarks>
    /// This id should be unique across all modals to reduce the chance
    /// modals to have the same id in which can cause unexpected behavior.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown when id is null, empty, or whitespace.
    /// </exception>
    [Parameter]
    public string ModalId
    {
      get
      {
        if (string.IsNullOrWhiteSpace(m_modalId))
        {
          throw new InvalidOperationException($"{nameof(ModalId)} is not set. Id must not be empty or whitespace");
        }
        return m_modalId;
      }
      set => m_modalId = value;
    }

    /// <summary>
    /// True if the modal should be open 
    /// right after it is rendered for the first time.
    /// </summary>
    [Parameter]
    public bool OpenAtFirstRender { get; set; } = false;

    /// <summary>
    /// True if modal dialog should have the fade
    /// effect when it opens or hides.
    /// </summary>
    [Parameter]
    public bool Fade { get; set; } = true;

    /// <summary>
    /// Call <see cref="SetHeader"/>, <see cref="SetBody"/>,
    /// and <see cref="SetFooter"/>.
    /// </summary>
    protected override void OnInitialized()
    {
      SetHeader();
      SetBody();
      SetFooter();
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
      if (firstRender && OpenAtFirstRender)
      {
        await OpenAsync();
      }
    }

    /// <summary>
    /// Open the modal dialog asynchronously.
    /// </summary>
    /// <returns>Empty task</returns>
    public async Task OpenAsync()
      => await ModalBootstrap.ShowModalAsync(ModalId);

    /// <summary>
    /// Hide the modal dialog asynchronously.
    /// </summary>
    /// <returns>Empty task</returns>
    public async Task CloseAsync()
      => await ModalBootstrap.HideModalAsync(ModalId);

    /// <summary>
    /// Set the <see cref="m_modalHeader"/> to the desired UI fragment.
    /// If the header is not required, then do not override this method.
    /// </summary>
    protected virtual void SetHeader() { }

    /// <summary>
    /// Set the <see cref="m_modalBody"/> to the UI fragment.
    /// If the body is not required, then do not override this method.
    /// </summary>
    protected virtual void SetBody() { }

    /// <summary>
    /// Set the <see cref="m_modalFooter"/> to the desired UI fragment.
    /// If the footer is not required, then do not override this method.
    /// </summary>
    protected virtual void SetFooter() { }
  }
}
