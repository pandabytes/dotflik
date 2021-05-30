using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dotflik.Domain.Exceptions
{
  public class BadPageTokenFormatException : Exception
  {
    public BadPageTokenFormatException() { }

    public BadPageTokenFormatException(string message) : base(message) { }

    public BadPageTokenFormatException(string message, Exception innerEx) : base(message, innerEx) { }

  }
}
