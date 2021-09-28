using Automatonymous;
using EAuction.Buyer.Contracts.Commands;
using EAuction.Buyer.Contracts.Models;
using EAuction.Buyer.Contracts.Queries;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace EAuction.Buyer.Endpoint.Saga.AddBid
{
    public class AddBidRequestStateMachine : MassTransitStateMachine<AddBidRequestState>
    {
        public State RequestReceived { get; private set; }
        public State ProcessStarted { get; private set; }
        public State Processing { get; private set; }
        public State ProcessCompleted { get; private set; }
        public State ProcessFailed { get; private set; }
        public State BidAdded { get; private set; }

        public Event<AddAuctionRequestModel> AddAuctionRequest { get; private set; }
        public Event<GetBuyerIdByEmailId.Response> BuyerIdResponse { get; private set; }
        public Event<Buyer.Contracts.Commands.AddBidCommand.Completed> BuyerBidAdded { get; private set; }
        public Event<AddBuyerCommand.Completed> BuyerAdded { get; private set; }
        public Event<Auction.Contracts.Commands.AddBidCommand.Completed> AuctionBidAdded { get; private set; }
        public Event<Auction.Contracts.Commands.AddBidCommand.Failed> AuctionBidFailed { get; private set; }

        public AddBidRequestStateMachine(ILogger<AddBidRequestStateMachine> logger, IEndpointNameFormatter formatter)
        {
            InstanceState(x => x.CurrentState);
            Event(() => AddAuctionRequest, e =>
            {
                e.CorrelateById(x => x.Message.CorrelationId);
                e.SelectId(c => NewId.NextGuid());
                e.InsertOnInitial = true;
                e.SetSagaFactory(context => InitializeState(context));
            });

            Event(() => BuyerIdResponse, e => e.CorrelateById(x => x.Message.CorrelationId));
            Event(() => BuyerBidAdded, e => e.CorrelateById(x => x.Message.CorrelationId));
            Event(() => BuyerAdded, e => e.CorrelateById(x => x.Message.CorrelationId));
            Event(() => AuctionBidAdded, e => e.CorrelateById(x => x.Message.CorrelationId));
            Event(() => AuctionBidFailed, e => e.CorrelateById(x => x.Message.CorrelationId));

            Initially(
                When(AddAuctionRequest)
                .Then(context => logger.LogInformation($"{context.Instance.CorrelationId}: StateMachine has started processing"))
                .ThenAsync(async context => await SendGetBuyerIdByEmailRequest(context))
                .TransitionTo(RequestReceived)
            );

            During(RequestReceived,
                When(BuyerIdResponse)
                .Then(context => UpdateBuyerId(context))
                .ThenAsync(async context => await AddBuyerBid(context))
                .TransitionTo(ProcessStarted));

            During(ProcessStarted,
                When(BuyerBidAdded)
                .ThenAsync(async context => await AddAuctionBid(context))
                .TransitionTo(Processing));

            During(ProcessStarted,
                When(BuyerAdded)
                .ThenAsync(async context => await AddAuctionBid(context))
                .TransitionTo(Processing));

            During(Processing,
                When(AuctionBidAdded)
                .ThenAsync(async context => await SendAuctionAddedResponse(context))
                .TransitionTo(ProcessCompleted));

            During(Processing,
                When(AuctionBidFailed)
                .ThenAsync(async context => await SendFailureResponse(context))
                .TransitionTo(ProcessFailed));

            DuringAny(
            When(ProcessCompleted.Enter)
              .Finalize());

        }

        private AddBidRequestState InitializeState(ConsumeContext<AddAuctionRequestModel> context)
        {
            return new AddBidRequestState()
            {
                ResponseAddress = context.ResponseAddress?.ToString(),
                RequestId = context.RequestId,
                CurrentState = Initial.Name,
                CorrelationId = context.Message.CorrelationId,
                LastUpdatedTime = DateTime.Now,
                RequestTime = DateTime.Now,
                ProductId = "",
                BuyerId = "",
                Request = context.Message
            };
        } 
       
        private async Task AddBuyerBid(BehaviorContext<AddBidRequestState, GetBuyerIdByEmailId.Response> context)
        {
            var Buyer = new Domain.Aggregate.BuyerAggregate.Buyer(string.IsNullOrEmpty(context.Data.BuyerId) ? Guid.NewGuid().ToString() : context.Data.BuyerId,
                                                    context.Instance.Request.FirstName, context.Instance.Request.LastName, context.Instance.Request.Address,
                                                    context.Instance.Request.City, context.Instance.Request.State,
                                                    context.Instance.Request.Pin, context.Instance.Request.Phone, context.Instance.Request.Email);

            Buyer.Bids.Add(new Domain.Aggregate.BuyerAggregate.AuctionItem(context.Instance.ProductId, context.Instance.Request.BidAmount));
            if (context.Data.BuyerId == string.Empty)
            {
                await context.Publish(new EAuction.Buyer.Contracts.Commands.AddBuyerCommand()
                {
                    CorrelationId = context.Data.CorrelationId,
                    Buyer = Buyer
                });
            }
            else
            {
                await context.Publish(new Buyer.Contracts.Commands.AddBidCommand()
                {
                    CorrelationId = context.Data.CorrelationId,
                    BuyerId = Buyer.Id,
                    ProductId = context.Instance.Request.ProductId,
                    BidAmount = context.Instance.Request.BidAmount
                });
            }
            context.Instance.BuyerId = Buyer.Id;
            context.Instance.ProductId = context.Instance.Request.ProductId;
            context.Instance.LastUpdatedTime = DateTime.Now;
        }

        private void UpdateBuyerId(BehaviorContext<AddBidRequestState, GetBuyerIdByEmailId.Response> context)
        {
            context.Instance.BuyerId = context.Data.BuyerId;
        }

        private async Task SendGetBuyerIdByEmailRequest(BehaviorContext<AddBidRequestState, AddAuctionRequestModel> context)
        {
            await context.Publish(new GetBuyerIdByEmailId()
            {
                CorrelationId = context.Instance.CorrelationId,
                EmailId = context.Instance.Request.Email
            });
            context.Instance.LastUpdatedTime = DateTime.Now;
        }       

        private async Task SendAuctionAddedResponse<T>(BehaviorContext<AddBidRequestState, T> context)
        {
            //Send response back to orignial requestor once we are done with this step
            if (context.Instance.ResponseAddress != null)
            {
                var responseEndpoint = await context.GetSendEndpoint(new Uri(context.Instance.ResponseAddress));
                var BuyerId = context.Instance.BuyerId;
                var productId = context.Instance.ProductId;
                await responseEndpoint.Send(new AuctionAddedResponse()
                {
                    CorrelationId = context.Instance.CorrelationId,
                    BuyerId = BuyerId.ToString(),
                    ProductId = productId.ToString()
                },
                    callback: sendContext => sendContext.RequestId = context.Instance.RequestId);
                context.Instance.LastUpdatedTime = DateTime.Now;
            }
        }

        private async Task AddAuctionBid<T>(BehaviorContext<AddBidRequestState, T> context)
        {
            await context.Publish(new Auction.Contracts.Commands.AddBidCommand()
            {
                CorrelationId = context.Instance.CorrelationId,
                BuyerId = context.Instance.BuyerId,
                BuyerName = context.Instance.Request.FirstName,
                Phone = context.Instance.Request.Phone,
                Email = context.Instance.Request.Email,
                AuctionItemId = context.Instance.ProductId,
                BidAmount = context.Instance.Request.BidAmount
            });
            context.Instance.LastUpdatedTime = DateTime.Now;
        }

        private async Task SendFailureResponse(BehaviorContext<AddBidRequestState, Auction.Contracts.Commands.AddBidCommand.Failed> context)
        {
            
            if (context.Instance.ResponseAddress != null)
            {
                var responseEndpoint = await context.GetSendEndpoint(new Uri(context.Instance.ResponseAddress));
                var BuyerId = context.Instance.BuyerId;
                var productId = context.Instance.ProductId;
                await responseEndpoint.Send(new AuctionAddedResponse()
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

    }
}
