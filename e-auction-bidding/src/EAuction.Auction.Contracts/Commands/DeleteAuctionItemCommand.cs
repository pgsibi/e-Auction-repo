using System;
using System.Collections.Generic;
using System.Text;

namespace EAuction.Auction.Contracts.Commands
{
    public class DeleteAuctionItemCommand
    {
        public Guid CorrelationId { get; set; }

        public string AuctionItemId { get; set; }

        public class Completed
        {
            public Guid CorrelationId { get; set; }
            public string SellerId { get; set; }
            public string AuctionItemId { get; set; }
        }

        public class Failed
        {
            public Guid CorrelationId { get; set; }
            public string Message { get; set; }            
        }
   
    }
}
