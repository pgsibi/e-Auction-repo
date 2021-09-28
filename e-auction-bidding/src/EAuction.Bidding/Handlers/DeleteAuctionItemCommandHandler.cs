﻿using EAuction.Auction.Contracts.Commands;
using EAuction.Auction.Domain.Aggregate;
using EAuction.Auction.Domain.Aggregate.AuctionAggregate;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace EAuction.Auction.Endpoint.Handlers
{
    /// <summary>
    /// DeleteAuctionItemCommandHandler
    /// </summary>
    public class DeleteAuctionItemCommandHandler : IConsumer<DeleteAuctionItemCommand>
    {

        readonly ILogger<DeleteAuctionItemCommandHandler> _logger;
        private readonly IAuctionRepository _auctionRepository;
        readonly IPublishEndpoint _endpoint;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="auctionRepository"></param>
        public DeleteAuctionItemCommandHandler(ILogger<DeleteAuctionItemCommandHandler> logger, IAuctionRepository auctionRepository, IPublishEndpoint endpoint)
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
        public async Task Consume(ConsumeContext<DeleteAuctionItemCommand> context)
        {
            try
            {
                await _auctionRepository.StartSessionAsync();
                var auctionItem = await _auctionRepository.FindOneAsync(x => x.Id == context.Message.AuctionItemId);
                _logger.LogInformation("Value: {Value}", context.Message);

                var filter = Builders<AuctionItem>.Filter.And(
                    Builders<AuctionItem>.Filter.Eq(c => c.Id, context.Message.AuctionItemId),
                    Builders<AuctionItem>.Filter.Size(p => p.Bids, 0)
                );
                var update = Builders<AuctionItem>.Update
                    .Set(l => l.Status, EntityStatus.Deleted)
                    .Set(l => l.LastUpdatedTime, DateTime.Now);

                

                var result = await _auctionRepository.UpdateOneAsync(filter, update);

              //  await _auctionRepository.CommitTransactionAsync();

                if (result.ModifiedCount == 1)
                {
                    await _endpoint.Publish(new DeleteAuctionItemCommand.Completed() { CorrelationId = context.Message.CorrelationId, AuctionItemId = context.Message.AuctionItemId, SellerId = auctionItem.SellerId });
                }
                else
                {
                    if (auctionItem != null)
                    {
                        await _endpoint.Publish(new DeleteAuctionItemCommand.Failed() { CorrelationId = context.Message.CorrelationId, Message = $"Product contains {auctionItem.Bids.Count} bids" });
                    }
                }
            }
            catch (Exception e)
            {
                await _endpoint.Publish(new DeleteAuctionItemCommand.Failed() { CorrelationId = context.Message.CorrelationId, Message = e.Message });
            }


        }
    }
}
