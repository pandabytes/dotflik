using System;

namespace Dotflik.Domain.Entities
{
  public class Sale
  {
    public string Id { get; init; } = string.Empty;

    public string CustomerId { get; init; } = string.Empty;

    public string MovieId { get; init; } = string.Empty;

    public DateTime SaleDate { get; init; }
  }
}


