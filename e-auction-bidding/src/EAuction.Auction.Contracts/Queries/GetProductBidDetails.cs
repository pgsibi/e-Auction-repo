using System;
using System.Collections.Generic;
using System.Text;

namespace EAuction.Auction.Contracts.Queries
{
    public class GetProductBidDetails
    {
        public Guid CorrelationId { get; set; }
        public string ProductId { get; set; }

        public class Response
        {
            public Guid CorrelationId { get; set; }
            public AuctionItemModel AuctionItem { get; set; }
        }

        public class BidModel
        {
            public double BidId { get; set; }
            public double BidAmount { get; set; }
            public string Name { get; set; }
            public string Mobile { get; set; }
            public string Email { get; set; }

        }
        public class AuctionItemModel
        {
            public string ProductId { get; set; }
            public string ProductName { get; set; }
            public string ShortDescription { get; set; }
            public string DetailedDescription { get; set; }
            public string Category { get; set; }
            public string StartingPrice { get; set; }
            public DateTime BidEndDate { get; set; }
            public List<BidModel> Bids { get; set; }
        }
    }

}
