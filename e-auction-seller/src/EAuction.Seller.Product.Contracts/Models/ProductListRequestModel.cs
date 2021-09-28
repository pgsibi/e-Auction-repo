using System;
using System.Collections.Generic;
using System.Text;

namespace EAuction.Seller.Product.Contracts.Models
{
    public class ProductListRequestModel
    {
        public Guid CorrelationId { get; set; }
        public string EmailId { get; set; }
    }
}
