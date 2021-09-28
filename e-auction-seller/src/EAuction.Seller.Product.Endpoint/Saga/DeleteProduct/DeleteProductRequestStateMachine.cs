using Automatonymous;
using EAuction.Auction.Contracts.Commands;
using EAuction.Seller.Product.Contracts.Commands;
using EAuction.Seller.Product.Contracts.Models;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace EAuction.Seller.Product.Endpoint.Saga.DeleteProduct
{
    public class DeleteProductRequestStateMachine : MassTransitStateMachine<DeleteProductRequestState>
    {

        public State RequestReceived { get; private set; }
        public State ProcessStarted { get; private set; }
        public State Processing { get; private set; }
        public State ProcessCompleted { get; private set; }
        public State ProcessFailed { get; private set; }
        public State ProductDeleted { get; private set; }

        public Event<SellerProductDeleteRequestModel> SellerProductDeleteRequest { get; private set; }
        public Event<DeleteSellerProductCommand.Completed> SellerProductDeleted { get; private set; }
        public Event<DeleteAuctionItemCommand.Completed> AuctionItemDeleted { get; private set; }
        public Event<DeleteAuctionItemCommand.Failed> AuctionItemDeletionFailed { get; private set; }
        public DeleteProductRequestStateMachine(ILogger<DeleteProductRequestStateMachine> logger)
        {
            InstanceState(x => x.CurrentState);
            Event(() => SellerProductDeleteRequest, e =>
            {
                e.CorrelateById(x => x.Message.CorrelationId);
                e.SelectId(c => NewId.NextGuid());
                e.InsertOnInitial = true;
                e.SetSagaFactory(context => InitializeState(context));
            });

            Event(() => SellerProductDeleted, e => e.CorrelateById(x => x.Message.CorrelationId));
            Event(() => AuctionItemDeleted, e => e.CorrelateById(x => x.Message.CorrelationId));
            Event(() => AuctionItemDeletionFailed, e => e.CorrelateById(x => x.Message.CorrelationId));

            Initially(
                When(SellerProductDeleteRequest)
                .Then(x => logger.LogInformation($"{x.Instance.CorrelationId}: StateMachine has started processing"))
                .ThenAsync(async x => await SendAuctionItemDeleteRequest(x))
                .TransitionTo(RequestReceived));

            During(RequestReceived,
                When(AuctionItemDeleted)
                .Then(x => UpdateSellerId(x))
                .ThenAsync(async context => await SendProductDeleteCommand(context))
                .TransitionTo(ProcessStarted));

            During(RequestReceived,
                When(AuctionItemDeletionFailed)
                .ThenAsync(async context => await SendFailureResponse(context))
                .TransitionTo(ProcessCompleted));

            During(ProcessStarted,
                When(SellerProductDeleted)
                .ThenAsync(async context => await SendDeleteProductRequestResponse(context))
                .TransitionTo(ProcessCompleted));

            DuringAny(
            When(ProcessCompleted.Enter)
              .Finalize());
        }

        private async Task SendDeleteProductRequestResponse(BehaviorContext<DeleteProductRequestState, DeleteSellerProductCommand.Completed> context)
        {
            if (context.Instance.ResponseAddress != null)
            {
                var responseEndpoint = await context.GetSendEndpoint(new Uri(context.Instance.ResponseAddress));
                await responseEndpoint.Send(new SellerProductDeletedResponse()
                {
                    CorrelationId = context.Instance.CorrelationId,
                    ProductId = context.Instance.ProductId,
                    SellerId = context.Instance.SellerId
                },
                    callback: sendContext => sendContext.RequestId = context.Instance.RequestId);
                context.Instance.LastUpdatedTime = DateTime.Now;
            }
        }

        private async Task SendFailureResponse(BehaviorContext<DeleteProductRequestState, DeleteAuctionItemCommand.Failed> context)
        {
            if (context.Instance.ResponseAddress != null)
            {
                var responseEndpoint = await context.GetSendEndpoint(new Uri(context.Instance.ResponseAddress));
                await responseEndpoint.Send(new SellerProductDeletedResponse()
                {
                    CorrelationId = context.Instance.CorrelationId,
                    Exception = new Exception(context.Data.Message)
                },
                    callback: sendContext => sendContext.RequestId = context.Instance.RequestId);
                context.Instance.LastUpdatedTime = DateTime.Now;
            }
        }

        private void UpdateSellerId(BehaviorContext<DeleteProductRequestState, DeleteAuctionItemCommand.Completed> context)
        {
            context.Instance.SellerId = context.Data.SellerId;
            context.Instance.LastUpdatedTime = DateTime.Now;
        }

        private async Task SendProductDeleteCommand(BehaviorContext<DeleteProductRequestState, DeleteAuctionItemCommand.Completed> context)
        {
            await context.Publish(new DeleteSellerProductCommand()
            {
                CorrelationId = context.Instance.CorrelationId,
                ProductId = context.Instance.ProductId,
                SellerId = context.Data.SellerId
            });
            context.Instance.LastUpdatedTime = DateTime.Now;
        }

        private async Task SendAuctionItemDeleteRequest(BehaviorContext<DeleteProductRequestState, SellerProductDeleteRequestModel> context)
        {
            await context.Publish(new DeleteAuctionItemCommand()
            {
                CorrelationId = context.Instance.CorrelationId,
                AuctionItemId = context.Instance.ProductId
            });
            context.Instance.LastUpdatedTime = DateTime.Now;
        }

        private DeleteProductRequestState InitializeState(ConsumeContext<SellerProductDeleteRequestModel> context)
        {
            return new DeleteProductRequestState()
            {
                ResponseAddress = context.ResponseAddress?.ToString(),
                RequestId = context.RequestId,
                CurrentState = Initial.Name,
                CorrelationId = context.Message.CorrelationId,
                LastUpdatedTime = DateTime.Now,
                RequestTime = DateTime.Now,
                ProductId = context.Message.ProductId,
                SellerId = ""
            };
        }


    }
}
