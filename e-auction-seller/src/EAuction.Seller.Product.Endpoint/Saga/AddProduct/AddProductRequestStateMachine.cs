using Automatonymous;
using EAuction.Auction.Contracts.Commands;
using EAuction.Seller.Product.Contracts.Commands;
using EAuction.Seller.Product.Contracts.Models;
using EAuction.Seller.Product.Contracts.Query;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace EAuction.Seller.Product.Endpoint.Saga.AddProduct
{
    public class AddProductRequestStateMachine :
       MassTransitStateMachine<AddProductRequestState>
    {
        
        public Event<SellerProductModel> SellerProductAddRequest { get; private set; }
        public Event<GetSellerId.Response> SellerIdResponse { get; private set; }
        public Event<AddSellerProductCommand.Completed> SellerProductAdded { get; private set; }
        public Event<AddSellerCommand.Completed> SellerAdded { get; private set; }
        public Event<AddAuctionItemCommand.Completed> AuctionItemAdded { get; private set; }


        public State RequestReceived { get; private set; }
        public State ProcessStarted { get; private set; }
        public State Processing { get; private set; }
        public State ProcessCompleted { get; private set; }
        public State ProductCreated { get; private set; }

        public AddProductRequestStateMachine(ILogger<AddProductRequestStateMachine> logger)
        {

            InstanceState(x => x.CurrentState);
            Event(() => SellerProductAddRequest, e =>
            {
                e.CorrelateById(x => x.Message.CorrelationId);
                e.SelectId(c => NewId.NextGuid());
                e.InsertOnInitial = true;
                e.SetSagaFactory(context => InitializeState(context));
            });

            Event(() => SellerIdResponse, e => e.CorrelateById(x => x.Message.CorrelationId));
            Event(() => SellerProductAdded, e => e.CorrelateById(x => x.Message.CorrelationId));
            Event(() => SellerAdded, e => e.CorrelateById(x => x.Message.CorrelationId));

            Initially(
                When(SellerProductAddRequest)
                .Then(x => logger.LogInformation($"{x.Instance.CorrelationId}: StateMachine has started processing"))
                .ThenAsync(async x => await SendGetSellerIdByEmailRequest(x))
                .TransitionTo(ProcessStarted)
            );

            During(ProcessStarted,
                When(SellerIdResponse)
                .Then(x => UpdateSellerId(x))
                .ThenAsync(async context => await StartAddingProduct(context))
                .TransitionTo(Processing));

            During(Processing,
                When(SellerProductAdded)
                .ThenAsync(async x => await StartAddingAuctionItem(x))
                .TransitionTo(ProcessCompleted));

            During(Processing,
                When(SellerAdded)
                .ThenAsync(async x => await StartAddingAuctionItem(x))
                .TransitionTo(ProcessCompleted));

            During(Processing,
               When(AuctionItemAdded)
               .ThenAsync(async x => await SendAddProductRequestResponse(x))
               .TransitionTo(ProductCreated));

            DuringAny(
            When(ProductCreated.Enter)
              .Finalize());


        }

        
        private async Task StartAddingAuctionItem<T>(BehaviorContext<AddProductRequestState, T> context)
        {
            await context.Publish(new AddAuctionItemCommand()
            {
                CorrelationId = context.Instance.CorrelationId,
                AuctionItemId = context.Instance.ProductId,
                SellerId = context.Instance.SellerId,
                SellerName = context.Instance.Request.Seller.FirstName,
                ProductName = context.Instance.Request.Product.ProductName,
                ShortDescription = context.Instance.Request.Product.ShortDescription,
                DetailedDescription = context.Instance.Request.Product.DetailedDescription,
                Category = context.Instance.Request.Product.Category,
                StartingPrice = Double.Parse(context.Instance.Request.Product.StartingPrice),
                BidEndDate = context.Instance.Request.Product.BidEndDate
            });
            context.Instance.LastUpdatedTime = DateTime.Now;
        }

        private void UpdateSellerId(BehaviorContext<AddProductRequestState, GetSellerId.Response> x)
        {
            x.Instance.SellerId = x.Data.SellerId;
        }

        
        private async Task StartAddingProduct(BehaviorContext<AddProductRequestState, GetSellerId.Response> context)
        {
            var sellerInfo = context.Instance.Request.Seller;
            var productInfo = context.Instance.Request.Product;
            var seller = new EAuction.Seller.Product.Domain.Aggregate.SellerAggregate.Seller(string.IsNullOrEmpty(context.Data.SellerId) ? Guid.NewGuid().ToString() : context.Data.SellerId,
                                                    sellerInfo.FirstName, sellerInfo.LastName, sellerInfo.Address,
                                                    sellerInfo.City, sellerInfo.State, sellerInfo.Pin, sellerInfo.Phone, sellerInfo.Email);

            seller.Products.Add( new EAuction.Seller.Product.Domain.Aggregate.SellerAggregate.Product(productInfo.ProductName, productInfo.ShortDescription,
                productInfo.DetailedDescription, productInfo.Category, double.Parse(productInfo.StartingPrice),
                productInfo.BidEndDate));
            if (context.Data.SellerId == string.Empty)
            {
                await context.Publish(new AddSellerCommand() { CorrelationId = context.Data.CorrelationId, Seller = seller });
            }
            else
            {
                await context.Publish(new AddSellerProductCommand() { CorrelationId = context.Data.CorrelationId, Product = seller.Products[0], SellerId = seller.Id });
            }
            context.Instance.SellerId = seller.Id;
            context.Instance.ProductId = seller.Products[0].Id;
            context.Instance.LastUpdatedTime = DateTime.Now;
        }

        private async Task SendGetSellerIdByEmailRequest(BehaviorContext<AddProductRequestState> context)
        {
            await context.Publish(new GetSellerId() { CorrelationId = context.Instance.CorrelationId, EmailId = context.Instance.Request.Seller.Email });
            context.Instance.LastUpdatedTime = DateTime.Now;
        }

        private async Task SendAddProductRequestResponse<T>(BehaviorContext<AddProductRequestState, T> context)
        {
            //Send response back to orignial requestor once we are done with this step
            if (context.Instance.ResponseAddress != null)
            {
                var responseEndpoint = await context.GetSendEndpoint(new Uri(context.Instance.ResponseAddress));
                var sellerId = context.Instance.SellerId;
                var productId = context.Instance.ProductId;
                await responseEndpoint.Send(new SellerProductAddedResponse() { CorrelationId = context.Instance.CorrelationId, SellerId = sellerId.ToString(), ProductId = productId.ToString() },
                    callback: sendContext => sendContext.RequestId = context.Instance.RequestId);
                context.Instance.LastUpdatedTime = DateTime.Now;
            }
        }


        private AddProductRequestState InitializeState(ConsumeContext<SellerProductModel> context)
        {
            return new AddProductRequestState()
            {
                ResponseAddress = context.ResponseAddress?.ToString(),
                RequestId = context.RequestId,
                CurrentState = Initial.Name,
                CorrelationId = context.Message.CorrelationId,
                LastUpdatedTime = DateTime.Now,
                RequestTime = DateTime.Now,
                ProductId = "",
                SellerId = "",
                Request = context.Message
            };
        }
    }
}
