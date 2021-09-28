using System;

namespace EAuction.Buyer.Contracts.Models
{
    public class AuctionUpdatedResponse
    {
        public Guid CorrelationId { get; set; }
        public string BuyerId { get; set; }
        public string ProductId { get; set; }
        public Exception Exception { get; set; }
    }
}
