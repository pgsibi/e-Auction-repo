using System;
using System.Collections.Generic;
using System.Text;

namespace EAuction.Auction.Contracts.Commands
{
    public class AddAuctionItemCommand
    {
        public Guid CorrelationId { get; set; }
        public string AuctionItemId { get; set; }
        public string SellerId { get; set; }
        public string SellerName { get; set; }
        public string ProductName { get; set; }
        public string ShortDescription { get; set; }
        public string DetailedDescription { get; set; }
        public string Category { get; set; }
        public double StartingPrice { get; set; }
        public DateTime BidEndDate { get; set; }        
        
        public class Completed
        {
            public Guid CorrelationId { get; set; }
            public string ProductId { get; set; }
        }

        public class Failed
        {
            public Guid CorrelationId { get; set; }
            public string Message { get; set; }
        }
    }
}
