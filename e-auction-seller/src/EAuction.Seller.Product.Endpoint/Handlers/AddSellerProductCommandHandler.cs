using EAuction.Seller.Product.Contracts.Commands;
using EAuction.Seller.Product.Domain.Aggregate.SellerAggregate;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EAuction.Seller.Product.Endpoint.Handlers
{
    public class AddSellerProductCommandHandler : IConsumer<AddSellerProductCommand>
    {

        readonly ILogger<AddSellerProductCommandHandler> _logger;
        private readonly ISellerRepository _sellerRepository;
        readonly IPublishEndpoint _endpoint;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="sellerRepository"></param>
        public AddSellerProductCommandHandler(ILogger<AddSellerProductCommandHandler> logger, ISellerRepository sellerRepository,
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
        public async Task Consume(ConsumeContext<AddSellerProductCommand> context)
        {
            await _sellerRepository.StartSessionAsync();
            Expression<Func<EAuction.Seller.Product.Domain.Aggregate.SellerAggregate.Seller, IList<EAuction.Seller.Product.Domain.Aggregate.SellerAggregate.Product>>> userNameExpression = x => x.Products;
            var field = new ExpressionFieldDefinition<EAuction.Seller.Product.Domain.Aggregate.SellerAggregate.Seller>(userNameExpression);
            await _sellerRepository.PushItemToArray(context.Message.SellerId, field, context.Message.Product);
            await _endpoint.Publish(new AddSellerProductCommand.Completed() { CorrelationId = context.Message.CorrelationId });
            //await _sellerRepository.CommitTransactionAsync();
        }
    }
}
