using System;

namespace Dotflik.Domain.Entities
{
  public record Sale(int Id, string CustomerId, string MovieId, DateTime SaleDate);
}


