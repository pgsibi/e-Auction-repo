using EAuction.Buyer.Contracts.Commands;
using EAuction.Buyer.Domain.Aggregate.BuyerAggregate;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace EAuction.Buyer.Endpoint.Handlers
{
    public class UpdateBidCommandHandler : IConsumer<UpdateBidCommand>
    {

        readonly ILogger<UpdateBidCommandHandler> _logger;
        private readonly IBuyerRepository _buyerRepository;
        readonly IPublishEndpoint _endpoint;

        /// <summary>
        /// UpdateBidCommandHandler
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="buyerRepository"></param>
        /// <param name="endpoint"></param>
        public UpdateBidCommandHandler(ILogger<UpdateBidCommandHandler> logger, IBuyerRepository buyerRepository,
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
        public async Task Consume(ConsumeContext<UpdateBidCommand> context)
        {
            
            var filter = Builders<Domain.Aggregate.BuyerAggregate.Buyer>.Filter.And(
                         Builders<Domain.Aggregate.BuyerAggregate.Buyer>.Filter.Where(Buyer => Buyer.Id == context.Message.BuyerId),
                         Builders<Domain.Aggregate.BuyerAggregate.Buyer>.Filter.ElemMatch(i => i.Bids, u => u.Id == context.Message.ProductId));

            var update = Builders<Domain.Aggregate.BuyerAggregate.Buyer>.Update
                            .Set(l => l.Bids[-1].BidAmount, context.Message.BidAmount)
                            .Set(l => l.Bids[-1].LastUpdatedTime, DateTime.Now)
                            .Set(l => l.LastUpdatedTime, DateTime.Now);

            await _buyerRepository.StartSessionAsync();
            await _buyerRepository.UpdateOneAsync(filter, update);           
            _logger.LogInformation("Value: {Value}", context.Message);
            await _endpoint.Publish(new UpdateBidCommand.Completed()
            {
                CorrelationId = context.Message.CorrelationId,
                BuyerId = context.Message.BuyerId,
                ProductId = context.Message.ProductId
            });
            await _buyerRepository.CommitTransactionAsync();
        }


    }
}
