using EAuction.Auction.Contracts.Commands;
using EAuction.Buyer.Domain.Aggregate.BuyerAggregate;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EAuction.Buyer.Endpoint.Handlers
{
    public class AddBidCommandHandler : IConsumer<AddBidCommand>
    {

        readonly ILogger<AddBidCommandHandler> _logger;        
        readonly IPublishEndpoint _endpoint;
        private readonly IBuyerRepository _buyerRepository;

        /// <summary>
        /// AddBidCommandHandler
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="buyerRepository"></param>
        /// <param name="endpoint"></param>
        public AddBidCommandHandler(ILogger<AddBidCommandHandler> logger, IBuyerRepository buyerRepository,
            IPublishEndpoint endpoint)
        {
            _logger = logger;
            _buyerRepository = buyerRepository;
            _endpoint = endpoint;
        }

        /// <summary>
        /// Consumer/Handler
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<AddBidCommand> context)
        {
            Expression<Func<Domain.Aggregate.BuyerAggregate.Buyer, IList<Domain.Aggregate.BuyerAggregate.AuctionItem>>> expression = x => x.Bids;
            var field = new ExpressionFieldDefinition<Domain.Aggregate.BuyerAggregate.Buyer>(expression);
            
            await _buyerRepository.StartSessionAsync();
            await _buyerRepository.PushItemToArray(context.Message.BuyerId, field,
                new Domain.Aggregate.BuyerAggregate.AuctionItem(context.Message.AuctionItemId, context.Message.BidAmount));            
            _logger.LogInformation("Value: {Value}", context.Message);
            await _endpoint.Publish(new AddBidCommand.Completed()
            {
                CorrelationId = context.Message.CorrelationId,
                AuctionItemId = context.Message.AuctionItemId,
                BuyerId = context.Message.BuyerId
            });
            await _buyerRepository.CommitTransactionAsync();
        }
    }
}
