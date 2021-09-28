using EAuction.Auction.Contracts.Commands;
using EAuction.Auction.Domain.Aggregate.AuctionAggregate;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EAuction.Auction.Endpoint.Handlers
{
    public class AddBidCommandHandler : IConsumer<AddBidCommand>
    {

        readonly ILogger<AddBidCommandHandler> _logger;
        private readonly IAuctionRepository _auctionRepository;
        readonly IPublishEndpoint _endpoint;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="auctionRepository"></param>
        public AddBidCommandHandler(ILogger<AddBidCommandHandler> logger, IAuctionRepository auctionRepository, IPublishEndpoint endpoint)
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
        public async Task Consume(ConsumeContext<AddBidCommand> context)
        {
            try
            {
                Expression<Func<AuctionItem, IList<Bid>>> expression = x => x.Bids;
                var field = new ExpressionFieldDefinition<AuctionItem>(expression);
                await _auctionRepository.StartSessionAsync();

                await _auctionRepository.PushItemToArray(context.Message.AuctionItemId, field, new Bid(context.Message.BuyerId,
                    context.Message.BuyerName, context.Message.Phone, context.Message.Email, context.Message.BidAmount));

                await _endpoint.Publish(new AddBidCommand.Completed()
                {
                    CorrelationId = context.Message.CorrelationId,
                    BuyerId = context.Message.BuyerId,
                    AuctionItemId = context.Message.AuctionItemId
                });
               // await _auctionRepository.CommitTransactionAsync();
            }
            catch (Exception e)
            {
                await _endpoint.Publish(new AddBidCommand.Failed()
                {
                    CorrelationId = context.Message.CorrelationId,
                    Message = e.Message
                });
            }


        }
    }
}
