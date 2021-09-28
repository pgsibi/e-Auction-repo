using System;

namespace EAuction.Auction.Contracts.Commands
{
    public class UpdateBidAmountCommand
    {
        public Guid CorrelationId { get; set; }
        public string AuctionItemId { get; set; }
        public string BuyerId { get; set; }
        public string BuyerName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public double BidAmount { get; set; }

        public class Completed
        {
            public Guid CorrelationId { get; set; }
            public string AuctionItemId { get; set; }
            public string BuyerId { get; set; }
        }

        public class Failed
        {
            public Guid CorrelationId { get; set; }
            public string Message { get; set; }
        }
    }
}
