using EAuction.Seller.Product.Contracts.Commands;
using EAuction.Seller.Product.Domain.Aggregate;
using EAuction.Seller.Product.Domain.Aggregate.SellerAggregate;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAuction.Seller.Product.Endpoint.Handlers
{
    public class DeleteProductCommandHandler : IConsumer<DeleteSellerProductCommand>
    {

        readonly ILogger<DeleteProductCommandHandler> _logger;
        private readonly ISellerRepository _sellerRepository;
        readonly IPublishEndpoint _endpoint;

        /// <summary>
        /// AddProductCommandHandler
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="sellerRepository"></param>
        /// <param name="endpoint"></param>
        public DeleteProductCommandHandler(ILogger<DeleteProductCommandHandler> logger, ISellerRepository sellerRepository,
            IPublishEndpoint endpoint)
        {
            _logger = logger;
            _sellerRepository = sellerRepository;
            _endpoint = endpoint;
        }

        /// <summary>
        /// Consumer/Handler
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<DeleteSellerProductCommand> context)
        {

            var filter = Builders<Domain.Aggregate.SellerAggregate.Seller>.Filter.And(
                Builders<Domain.Aggregate.SellerAggregate.Seller>.Filter.Where(seller => seller.Id == context.Message.SellerId),
            Builders<Domain.Aggregate.SellerAggregate.Seller>.Filter.ElemMatch(i => i.Products, u => u.Id == context.Message.ProductId));



            var update = Builders<Domain.Aggregate.SellerAggregate.Seller>.Update
                .Set(l => l.Products[-1].Status, EntityStatus.Deleted)
                .Set(l => l.Products[-1].LastUpdatedTime, DateTime.Now)
                .Set(l => l.LastUpdatedTime, DateTime.Now);
            await _sellerRepository.StartSessionAsync();
            await _sellerRepository.UpdateOneAsync(filter, update);
            _logger.LogInformation("Value: {Value}", context.Message);
            await _endpoint.Publish(new DeleteSellerProductCommand.Completed()
            {
                CorrelationId = context.Message.CorrelationId,
                ProductId = context.Message.ProductId
            });
            //await _sellerRepository.CommitTransactionAsync();


        }


    }
}
