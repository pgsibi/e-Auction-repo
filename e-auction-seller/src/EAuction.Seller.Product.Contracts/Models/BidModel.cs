using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAuction.Seller.Product.Contracts.Models
{
    public class BidModel
    {
        public double BidAmount { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }
    }

}
