using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAuction.Seller.Product.Contracts.Models
{
    /// <summary>
    /// Product Result Model
    /// </summary>
    public class ProductResultModel
    {
            public ProductModel Product { get; set; }
            public List<BidModel> Bids { get; set; }       

    }
}
