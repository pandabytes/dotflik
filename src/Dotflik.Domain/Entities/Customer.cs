using System;

namespace Dotflik.Domain.Entities
{
  //public record Customer(int Id, string FirstName, string LastName, string CreditCardId, string Address, string Email, string Password);

  public class Customer
  {
    public string Id { get; init; } = string.Empty;

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string Address { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Password should never be in plain-text
    /// </summary>
    public string Password { get; init; } = string.Empty;

    /// <summary>
    /// Credit card id
    /// </summary>
    public string CcId { get; init; } = string.Empty;
  }
}
