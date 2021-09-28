using System;
using System.Collections.Generic;
using System.Text;

namespace EAuction.Auction.Domain.Aggregate
{
    public interface IAggregateRoot
    {
        string Id { get; }
        object GetId();
    }
}
