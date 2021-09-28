using Automatonymous;
using EAuction.Buyer.Contracts.Commands;
using EAuction.Buyer.Contracts.Models;
using EAuction.Buyer.Contracts.Queries;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace EAuction.Buyer.Endpoint.Saga.UpdateBid
{
    public class UpdateBidRequestStateMachine : MassTransitStateMachine<UpdateBidRequestState>
    {
        public State RequestReceived { get; private set; }
        public State ProcessStarted { get; private set; }
        public State Processing { get; private set; }
        public State ProcessCompleted { get; private set; }
        public State ProcessFailed { get; private set; }
        public State BidUpdated { get; private set; }

        public Event<UpdateAuctionRequestModel> UpdateAuctionRequest { get; private set; }
        public Event<GetBuyerIdByEmailId.Response> BuyerIdResponse { get; private set; }
        public Event<Buyer.Contracts.Commands.UpdateBidCommand.Completed> BuyerAmountUpdated { get; private set; }
        public Event<Auction.Contracts.Commands.UpdateBidAmountCommand.Completed> AuctionBidAmountUpdated { get; private set; }
        public Event<Auction.Contracts.Commands.UpdateBidAmountCommand.Failed> UpdateBidAmountFailed { get; private set; }


        public UpdateBidRequestStateMachine(ILogger<UpdateBidRequestStateMachine> logger, IEndpointNameFormatter formatter)
        {
            InstanceState(x => x.CurrentState);
            Event(() => UpdateAuctionRequest, e =>
            {
                e.CorrelateById(x => x.Message.CorrelationId);
                e.SelectId(c => NewId.NextGuid());
                e.InsertOnInitial = true;
                e.SetSagaFactory(context => InitializeState(context));
            });

            Event(() => BuyerIdResponse, e => e.CorrelateById(x => x.Message.CorrelationId));
            Event(() => UpdateAuctionRequest, e => e.CorrelateById(x => x.Message.CorrelationId));
            Event(() => BuyerAmountUpdated, e => e.CorrelateById(x => x.Message.CorrelationId));
            Event(() => AuctionBidAmountUpdated, e => e.CorrelateById(x => x.Message.CorrelationId));
            Event(() => UpdateBidAmountFailed, e => e.CorrelateById(x => x.Message.CorrelationId));

            Initially(
                    When(UpdateAuctionRequest)
                    .Then(async context => await SendGetBuyerIdByEmailRequest(context))
                    .TransitionTo(RequestReceived));

            During(RequestReceived,
                When(BuyerIdResponse)
                .Then(context => UpdateBuyerId(context))
                .ThenAsync(async context => await UpdateBuyerBid(context))
                .TransitionTo(ProcessStarted));

            During(ProcessStarted,
                When(BuyerAmountUpdated)
                .ThenAsync(async context => await UpdateAuctionBidAmount(context))
                .TransitionTo(Processing));

            During(Processing,
                When(AuctionBidAmountUpdated)
                .ThenAsync(async context => await SendAuctionUpdatedResponse(context))
                .TransitionTo(ProcessCompleted));

            During(Processing,
                When(UpdateBidAmountFailed)
                .ThenAsync(async context => await SendFailureResponse(context))
                .TransitionTo(ProcessFailed));

            DuringAny(
            When(ProcessCompleted.Enter)
              .Finalize());

        }

        private UpdateBidRequestState InitializeState(ConsumeContext<UpdateAuctionRequestModel> context)
        {
            return new UpdateBidRequestState()
            {
                ResponseAddress = context.ResponseAddress?.ToString(),
                RequestId = context.RequestId,
                CurrentState = Initial.Name,
                CorrelationId = context.Message.CorrelationId,
                LastUpdatedTime = DateTime.Now,
                RequestTime = DateTime.Now,
                ProductId = context.Message.ProductId,
                BuyerId = "",
                Request = context.Message
            };
        }
        private void UpdateBuyerId(BehaviorContext<UpdateBidRequestState, GetBuyerIdByEmailId.Response> context)
        {
            context.Instance.BuyerId = context.Data.BuyerId;
        }

        private async Task SendGetBuyerIdByEmailRequest(BehaviorContext<UpdateBidRequestState, UpdateAuctionRequestModel> context)
        {
            await context.Publish(new GetBuyerIdByEmailId()
            {
                CorrelationId = context.Instance.CorrelationId,
                EmailId = context.Instance.Request.BuyerEmailId
            });
            context.Instance.LastUpdatedTime = DateTime.Now;
        }

        private async Task SendFailureResponse(BehaviorContext<UpdateBidRequestState, Auction.Contracts.Commands.UpdateBidAmountCommand.Failed> context)
        {
            //Send response back to orignial requestor once we are done with this step
            if (context.Instance.ResponseAddress != null)
            {
                var responseEndpoint = await context.GetSendEndpoint(new Uri(context.Instance.ResponseAddress));
                var BuyerId = context.Instance.BuyerId;
                var productId = context.Instance.ProductId;
                await responseEndpoint.Send(new AuctionUpdatedResponse()
                {
                    CorrelationId = context.Instance.CorrelationId,
                    BuyerId = BuyerId?.ToString(),
                    ProductId = productId?.ToString(),
                    Exception = new Exception(context.Data.Message)
                },
                    callback: sendContext => sendContext.RequestId = context.Instance.RequestId);
                context.Instance.LastUpdatedTime = DateTime.Now;
            }
        }

        private async Task SendAuctionUpdatedResponse(BehaviorContext<UpdateBidRequestState, Auction.Contracts.Commands.UpdateBidAmountCommand.Completed> context)
        {
            if (context.Instance.ResponseAddress != null)
            {
                var responseEndpoint = await context.GetSendEndpoint(new Uri(context.Instance.ResponseAddress));
                var BuyerId = context.Instance.BuyerId;
                var productId = context.Instance.ProductId;
                await responseEndpoint.Send(new AuctionUpdatedResponse()
                {
                    CorrelationId = context.Instance.CorrelationId,
                    BuyerId = BuyerId.ToString(),
                    ProductId = productId.ToString()
                },
                    callback: sendContext => sendContext.RequestId = context.Instance.RequestId);
                context.Instance.LastUpdatedTime = DateTime.Now;
            }
        }

        private async Task UpdateAuctionBidAmount(BehaviorContext<UpdateBidRequestState, UpdateBidCommand.Completed> context)
        {
            await context.Publish(new Auction.Contracts.Commands.UpdateBidAmountCommand()
            {
                CorrelationId = context.Instance.CorrelationId,
                BuyerId = context.Instance.BuyerId,
                AuctionItemId = context.Instance.ProductId,
                BidAmount = context.Instance.Request.BidAmount
            });
            context.Instance.LastUpdatedTime = DateTime.Now;
        }

        private async Task UpdateBuyerBid<T>(BehaviorContext<UpdateBidRequestState, T> context)
        {
            await context.Publish(new UpdateBidCommand()
            {
                CorrelationId = context.Instance.CorrelationId,
                BuyerId = context.Instance.BuyerId,
                ProductId = context.Instance.ProductId,
                BidAmount = context.Instance.Request.BidAmount
            });
            context.Instance.LastUpdatedTime = DateTime.Now;
        }

    }
}
