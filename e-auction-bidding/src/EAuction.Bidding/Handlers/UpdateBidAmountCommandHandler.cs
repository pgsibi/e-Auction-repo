using EAuction.Auction.Contracts.Commands;
using EAuction.Auction.Domain.Aggregate.AuctionAggregate;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace EAuction.Auction.Endpoint.Handlers
{
    public class UpdateBidAmountCommandHandler : IConsumer<UpdateBidAmountCommand>
    {

        private readonly IAuctionRepository _auctionRepository;
        readonly IPublishEndpoint _endpoint;
        readonly ILogger<UpdateBidAmountCommandHandler> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="auctionRepository"></param>
        /// <param name="endpoint"></param>
        /// <param name="logger"></param>
        public UpdateBidAmountCommandHandler(IAuctionRepository auctionRepository, IPublishEndpoint endpoint, ILogger<UpdateBidAmountCommandHandler> logger)
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
        public async Task Consume(ConsumeContext<UpdateBidAmountCommand> context)
        {
            try
            {
                var auctionItem = await _auctionRepository.FindOneAsync(x => x.Id == context.Message.AuctionItemId);
                _logger.LogInformation("Value: {Value}", context.Message);

                await _auctionRepository.StartSessionAsync();
                var filter = Builders<AuctionItem>.Filter.And(
                    Builders<AuctionItem>.Filter.Eq(c => c.Id, context.Message.AuctionItemId),
                    Builders<AuctionItem>.Filter.ElemMatch(i => i.Bids, u => u.Id == context.Message.BuyerId));

                var update = Builders<AuctionItem>.Update
                    .Set(l => l.Bids[-1].BidAmount, context.Message.BidAmount)
                    .Set(l => l.Bids[-1].LastUpdatedTime, DateTime.Now)
                    .Set(l => l.LastUpdatedTime, DateTime.Now);
                var result = await _auctionRepository.UpdateOneAsync(filter, update);

                // await _auctionRepository.CommitTransactionAsync();

                if (result.ModifiedCount == 1)
                {
                    await _endpoint.Publish(new UpdateBidAmountCommand.Completed()
                    {
                        CorrelationId = context.Message.CorrelationId,
                        BuyerId = context.Message.BuyerId,
                        AuctionItemId = context.Message.AuctionItemId

                    });
                }
                else
                {
                    if (auctionItem != null)
                    {
                        await _endpoint.Publish(new UpdateBidAmountCommand.Failed()
                        {
                            CorrelationId = context.Message.CorrelationId,
                            Message = "No auction found"
                        });
                    }
                }
            }
            catch (Exception e)
            {
                await _endpoint.Publish(new UpdateBidAmountCommand.Failed()
                {
                    CorrelationId = context.Message.CorrelationId,
                    Message = e.Message
                });
            }


        }

    }
}
