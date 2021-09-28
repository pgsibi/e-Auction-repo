using System;
using System.Collections.Generic;
using System.Text;

namespace EAuction.Seller.Product.Contracts.Models
{
    public class SellerProductsRequestModel
    {
        public Guid CorrelationId { get; set; }
        public string SellerEmailId { get; set; }
    }
}
