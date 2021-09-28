using System;
using System.Collections.Generic;
using System.Text;

namespace EAuction.Seller.Product.Contracts.Models
{
    public class SellerProductAddedResponse
    {
        public Guid CorrelationId { get; set; }

        public string SellerId { get; set; }

        public string ProductId { get; set; }
    }
}
