using Automatonymous;
using MassTransit.Saga;
using System;

namespace EAuction.Seller.Product.Endpoint.Saga.DeleteProduct
{
    public class DeleteProductRequestState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid? RequestId { get; internal set; }
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public DateTime RequestTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public int Version { get; set; }
        public string SellerId { get; set; }
        public string ProductId { get; set; }
        public string ResponseAddress { get; internal set; }
        
    }
}
