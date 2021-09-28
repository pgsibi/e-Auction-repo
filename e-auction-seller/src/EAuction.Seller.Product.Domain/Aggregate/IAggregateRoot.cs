using System;
using System.Collections.Generic;
using System.Text;

namespace EAuction.Seller.Product.Domain.Aggregate
{
    public interface IAggregateRoot
    {
        string Id { get; }
        object GetId();
    }

}
