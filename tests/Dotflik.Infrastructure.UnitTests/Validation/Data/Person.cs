using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dotflik.Infrastructure.Validation.Data.UnitTests
{
  public record Person([property: Required] string Name,
                             string NickName,
                             [property: Range(0, 100)] int Age,
                             [property: Required] Job Job,
                             [property: MinLength(1)] IList<Address> Addresses,
                             [property: Required] IList<Person> Kids);

  public record Address([property: Required] string Number,
                        [property: Required] string Street,
                        [property: Required] string State,
                        [property: Required] string City,
                        [property: Required] string ZipCode);

  public record Job([property: Required] string Name);
}
