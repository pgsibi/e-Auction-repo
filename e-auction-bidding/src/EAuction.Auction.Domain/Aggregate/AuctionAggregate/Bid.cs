using System;
using System.Collections.Generic;
using System.Text;

namespace EAuction.Auction.Domain.Aggregate.AuctionAggregate
{
    public class Bid : Entity
    {

        public string BuyerName { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }
        public double BidAmount { get; private set; }

        public Bid(string id, string buyerName, string email, string phone,  double bidAmount)
        {
            Id = id;
            BuyerName = buyerName;            
            Email = email;
            Phone = phone;
            BidAmount = bidAmount;
        }
    }
}
