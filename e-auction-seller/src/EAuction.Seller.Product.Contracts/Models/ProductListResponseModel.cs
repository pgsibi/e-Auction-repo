using System;
using System.Collections.Generic;
using System.Text;

namespace EAuction.Seller.Product.Contracts.Models
{
    public class ProductListResponseModel
    {
        public Guid CorrelationId { get; set; }
        public List<ProductInfoModel> Products { get; set; }
    }
}
