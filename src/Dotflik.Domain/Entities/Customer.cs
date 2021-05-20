using System;

namespace Dotflik.Domain.Entities
{
  public record Customer(int Id, string FirstName, string LastName, string CreditCardId, string Address, string Email, string Password);
}
