using Automatonymous;
using EAuction.Buyer.Contracts.Models;
using MassTransit.Saga;
using System;

namespace EAuction.Buyer.Endpoint.Saga.AddBid
{
    public class AddBidRequestState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public DateTime RequestTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public int Version { get; set; }
        public AddAuctionRequestModel Request { get; set; }
        public string BuyerId { get; set; }
        public string ProductId { get; set; }
        public string ResponseAddress { get; internal set; }
        public Guid? RequestId { get; internal set; }
    }
}
