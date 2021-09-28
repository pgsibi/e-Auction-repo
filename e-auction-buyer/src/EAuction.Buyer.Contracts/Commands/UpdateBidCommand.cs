using System;

namespace EAuction.Buyer.Contracts.Commands
{
    public class UpdateBidCommand
    {
        public Guid CorrelationId { get; set; }
        public string BuyerId { get; set; }
        public string ProductId { get; set; }
        public double BidAmount { get; set; }

        public class Completed
        {
            public Guid CorrelationId { get; set; }
            public string BuyerId { get; set; }
            public string ProductId { get; set; }
        }
      
    }
}
