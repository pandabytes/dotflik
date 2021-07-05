using System;
using Dotflik.WebApp.Client.Interop;
using Xunit;

namespace Dotflik.WebApp.Client.UnitTests
{
  public class ModalOptionTests
  {
    [Theory]
    [InlineData(1)]
    [InlineData(10.0)]
    [InlineData(100.0f)]
    public void Backdrop_TypeNotStringOrBool_ThrowsInvalidOperationException(object value)
      => Assert.Throws<InvalidOperationException>(() => new ModalOption { Backdrop = value });

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public void Backdrop_TypeIsBool_NoExceptionThrown(bool value, bool expectedValue)
    {
      var modalOption = new ModalOption { Backdrop = value };
      Assert.Equal(expectedValue, modalOption.Backdrop);
    }

    [Theory]
    [InlineData("")]
    [InlineData("x")]
    [InlineData("xxx")]
    [InlineData("static.xxx")]
    [InlineData("statix")]
    public void Backdrop_TypeIsStringAndValueIsNotStatic_ThrowsInvalidOperationException(string value)
      => Assert.Throws<InvalidOperationException>(() => new ModalOption { Backdrop = value });

    [Fact]
    public void Backdrop_TypeIsStringAndValueIsStatic_NoExceptionThrown()
    {
      var modalOption = new ModalOption { Backdrop = ModalOption.BackdropStatic };
      Assert.Equal(ModalOption.BackdropStatic, modalOption.Backdrop);
    }

  }
}
