using System;
using System.Collections.Generic;
using System.Text;

namespace EAuction.Buyer.Domain.Aggregate.BuyerAggregate
{
    public class AuctionItem : Entity
    {
        public double BidAmount { get; private set; }

        public AuctionItem(string id, double bidAmount)
        {
            Id = id;
            BidAmount = bidAmount;
        }

        public AuctionItem()
        {
            Id = Guid.NewGuid().ToString();
        }

    }
}
