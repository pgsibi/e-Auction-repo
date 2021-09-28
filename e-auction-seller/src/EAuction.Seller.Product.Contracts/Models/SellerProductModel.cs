using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EAuction.Seller.Product.Contracts.Models
{
    /// <summary>
    /// Seller Product Model
    /// </summary>
    public class SellerProductModel
    {
        public Guid CorrelationId { get; set; }
        [Required]
        public ProductModel Product { get; set; }
        [Required]
        public SellerModel Seller { get; set; }

    }
}
