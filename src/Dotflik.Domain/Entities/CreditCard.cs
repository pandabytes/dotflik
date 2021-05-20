using System;

namespace Dotflik.Domain.Entities
{
  public record CreditCard(string Id, string FirstName, string LastName, DateTime ExpirationDate);
}
