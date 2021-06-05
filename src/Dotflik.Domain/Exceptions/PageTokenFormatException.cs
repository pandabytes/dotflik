using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dotflik.Domain.Exceptions
{
  public class PageTokenFormatException : Exception
  {
    public PageTokenFormatException() { }

    public PageTokenFormatException(string message) : base(message) { }

    public PageTokenFormatException(string message, Exception innerEx) : base(message, innerEx) { }

  }
}
