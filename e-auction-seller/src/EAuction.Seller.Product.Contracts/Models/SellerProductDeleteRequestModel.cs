using System;
using System.Collections.Generic;
using System.Text;

namespace EAuction.Seller.Product.Contracts.Models
{
    public class SellerProductDeleteRequestModel
    {
        public Guid CorrelationId { get; set; }
        public string ProductId { get; set; }
    }
}
