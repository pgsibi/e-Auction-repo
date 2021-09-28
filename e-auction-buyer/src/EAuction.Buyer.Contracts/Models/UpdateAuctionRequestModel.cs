using Microsoft.AspNetCore.Mvc;
using System;

namespace EAuction.Buyer.Contracts.Models
{
    public class UpdateAuctionRequestModel
    {
        public Guid CorrelationId { get; set; }
        [FromRoute(Name = "buyerEmailld")] public string BuyerEmailId { get; set; }
        [FromRoute(Name = "productId")] public string ProductId { get; set; }
        [FromRoute(Name = "newBidAmount")] public double BidAmount { get; set; }
    }
}
