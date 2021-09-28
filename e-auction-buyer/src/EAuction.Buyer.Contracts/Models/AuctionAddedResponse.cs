using System;
using System.Collections.Generic;
using System.Text;

namespace EAuction.Buyer.Contracts.Models
{
    public class AuctionAddedResponse
    {
        public Guid CorrelationId { get; set; }
        public string BuyerId { get; set; }
        public string ProductId { get; set; }
        public Exception Exception { get; set; }
    }
}
