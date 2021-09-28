using System;
using System.Collections.Generic;
using System.Text;

namespace EAuction.Seller.Product.Contracts.Commands
{
    public class AddSellerCommand
    {
        public Guid CorrelationId { get; set; }
        public Product.Domain.Aggregate.SellerAggregate.Seller Seller { get; set; }
        

        public class Completed
        {
            public Guid CorrelationId { get; set; }
        }
    }
}
