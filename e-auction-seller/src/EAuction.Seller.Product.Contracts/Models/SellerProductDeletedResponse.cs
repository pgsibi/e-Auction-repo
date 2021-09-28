using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace EAuction.Seller.Product.Contracts.Models
{
    public class SellerProductDeletedResponse
    {
        public Guid CorrelationId { get; set; }
        public string SellerId { get; set; }
        public string ProductId { get; set; }       
        public Exception Exception { get; set; }
    }
}
