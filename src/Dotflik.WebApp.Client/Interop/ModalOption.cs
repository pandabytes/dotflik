using System;

namespace Dotflik.WebApp.Client.Interop
{
  /// <summary>
  /// Represent an option object that bootstrap can use
  /// to display modal dialog.
  /// See https://getbootstrap.com/docs/4.0/components/modal/#methods.
  /// </summary>
  public sealed class ModalOption
  {
    private object m_backdrop;

    /// <summary>
    /// The backdrop static value.
    /// </summary>
    public static readonly string BackdropStatic = "static";

    /// <summary>
    /// Includes a modal-backdrop element. Alternatively, specify <see cref="BackdropStatic"/> 
    /// for a backdrop which doesn't close the modal on click.
    /// </summary>
    /// <remarks>
    /// The type of the value must be either <see cref="string"/> or <see cref="bool"/>.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown when value's type is not <see cref="string"/> or <see cref="bool"/>.
    /// </exception>
    public object Backdrop
    {
      get => m_backdrop;
      set
      {
        var errorMessage = $"Backdrop must be of type bool or string with value \"{BackdropStatic}\".";

        if (value.GetType() != typeof(string) && value.GetType() != typeof(bool))
        {
          throw new InvalidOperationException(errorMessage);
        }

        if (value.GetType() == typeof(string))
        {
          string backdropStr = (string)value;
          if (backdropStr != BackdropStatic)
          {
            throw new InvalidOperationException(errorMessage);
          }
        }

        m_backdrop = value;
      }
    }

    /// <summary>
    /// Set to true if closing the modal when 
    /// escape key is pressed is desired.
    /// </summary>
    public bool Keyboard { get; set; }

    /// <summary>
    /// Set to true if putting the focus on the 
    /// modal when initialized is desired.
    /// </summary>
    public bool Focus { get; set; }

    /// <summary>
    /// Set to true if showing the modal when initialized is desired.
    /// </summary>
    public bool Show { get; set; }

    public ModalOption()
    {
      m_backdrop = true;
      Keyboard = true;
      Focus = true;
      Show = true;
    }

  }
}
