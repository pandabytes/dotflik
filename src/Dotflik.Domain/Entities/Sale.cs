using System;

namespace Fabflix.Domain.Entities
{
  public record Sale(int Id, string CustomerId, string MovieId, DateTime SaleDate);
}


