using System;
using System.Collections.Generic;
using System.Text;

namespace EAuction.Seller.Product.Contracts.Models
{
    public class AuctionDetailsRequestModel
    {
        public string ProductId { get; set; }
        public Guid CorrelationId { get; set; }
    }
}
