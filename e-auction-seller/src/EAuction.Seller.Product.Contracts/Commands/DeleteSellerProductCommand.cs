using System;
using System.Collections.Generic;
using System.Text;

namespace EAuction.Seller.Product.Contracts.Commands
{
    public class DeleteSellerProductCommand
    {
        public Guid CorrelationId { get; set; }
        public string ProductId { get; set; }
        public string SellerId { get; set; }

        public class Completed
        {
            public Guid CorrelationId { get; set; }
            public string ProductId { get; set; }
        }
    }
}
