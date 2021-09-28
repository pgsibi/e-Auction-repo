using Automatonymous;
using EAuction.Seller.Product.Contracts.Models;
using MassTransit.Saga;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAuction.Seller.Product.Endpoint.Saga.AddProduct
{
    public class AddProductRequestState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public DateTime RequestTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public int Version { get; set; }
        public SellerProductModel Request { get; set; }
        public string SellerId { get; set; }
        public string ProductId { get; set; }
        public string ResponseAddress { get; internal set; }
        public Guid? RequestId { get; internal set; }
        public Guid? QueryId { get; set; }

    }
}
