﻿using System;
using System.Collections.Generic;

namespace EAuction.Auction.Domain.Aggregate.AuctionAggregate
{
    public class AuctionItem : Entity, IAggregateRoot
    {
        public string SellerId { get; private set; }
        public string SellerName { get; private set; }
        public string ProductName { get; private set; }
        public string ShortDescription { get; private set; }
        public string DetailedDescription { get; private set; }
        public string Category { get; private set; }
        public double StartingPrice { get; private set; }
        public DateTime BidEndDate { get; private set; }
        public List<Bid> Bids { get; private set; }


        public AuctionItem(string auctionItemId, string productName, string sellerId, string sellerName, string shortDescription, string detailedDescription, string category, double startingPrice, DateTime bidEndDate)
        {
            Id = auctionItemId;
            SellerId = sellerId; 
            SellerName = sellerName;
            ProductName = productName;
            ShortDescription = shortDescription;
            DetailedDescription = detailedDescription;
            Category = category;
            StartingPrice = startingPrice;
            BidEndDate = bidEndDate;
            Bids = new List<Bid>();
        }
    }
}
