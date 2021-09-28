using EAuction.Auction.Contracts.Commands;
using EAuction.Auction.Domain.Aggregate.AuctionAggregate;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace EAuction.Auction.Endpoint.Handlers
{
    public class AddAuctionItemCommandHandler : IConsumer<AddAuctionItemCommand>
    {

        readonly ILogger<AddAuctionItemCommandHandler> _logger;
        private readonly IAuctionRepository _auctionRepository;
        readonly IPublishEndpoint _endpoint;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="auctionRepository"></param>
        public AddAuctionItemCommandHandler(ILogger<AddAuctionItemCommandHandler> logger, IAuctionRepository auctionRepository, IPublishEndpoint endpoint)
        {
            _logger = logger;
            _auctionRepository = auctionRepository;
            _endpoint = endpoint;
        }

        /// <summary>
        /// Consumer/Handler
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<AddAuctionItemCommand> context)
        {
            try
            {
                await _auctionRepository.StartSessionAsync();
                var auctionItem = new AuctionItem(context.Message.AuctionItemId, context.Message.ProductName,
                                                    context.Message.SellerId,
                                                    context.Message.SellerName,
                                                    context.Message.ShortDescription,
                                                    context.Message.DetailedDescription,
                                                    context.Message.Category, context.Message.StartingPrice,
                                                    context.Message.BidEndDate);
                _auctionRepository.Add(auctionItem);
                await _endpoint.Publish(new AddAuctionItemCommand.Completed() { CorrelationId = context.Message.CorrelationId });
                //await _auctionRepository.CommitTransactionAsync();
            }
            catch (Exception e)
            {
                await _endpoint.Publish(new AddAuctionItemCommand.Failed() { CorrelationId = context.Message.CorrelationId, Message = e.Message });
            }
        }
    }
}
