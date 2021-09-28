using System;
using System.Collections.Generic;
using System.Text;
using EAuction.Seller.Product.Domain.Aggregate.SellerAggregate;


namespace EAuction.Seller.Product.Contracts.Commands
{
    public class AddSellerProductCommand
    {
        public Guid CorrelationId { get; set; }
        public Product.Domain.Aggregate.SellerAggregate.Product Product {get;set;}
        public string  SellerId { get; set; }

        public class Completed
        {
            public Guid CorrelationId { get; set; }
        }
    }
}
