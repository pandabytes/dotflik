using System;

namespace Dotflik.Domain.Entities
{
  public class CreditCard
  {
    public string Id { get; init; } = string.Empty;

    public string FirstName { get; init; } = string.Empty;

    public DateTime ExpirationDate { get; init; }

    public string LastName { get; init; } = string.Empty;

  }
}
