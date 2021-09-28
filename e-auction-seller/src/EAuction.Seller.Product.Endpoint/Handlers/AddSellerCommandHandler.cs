using EAuction.Seller.Product.Contracts.Commands;
using EAuction.Seller.Product.Domain.Aggregate.SellerAggregate;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAuction.Seller.Product.Endpoint.Handlers
{
    public class AddSellerCommandHandler : IConsumer<AddSellerCommand>
    {

        readonly ILogger<AddSellerCommandHandler> _logger;
        private readonly ISellerRepository _sellerRepository;
        readonly IPublishEndpoint _endpoint;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="sellerRepository"></param>
        public AddSellerCommandHandler(ILogger<AddSellerCommandHandler> logger, ISellerRepository sellerRepository,
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
        public async Task Consume(ConsumeContext<AddSellerCommand> context)
        {
            await _sellerRepository.StartSessionAsync();
            _sellerRepository.Add(context.Message.Seller);            
            await _endpoint.Publish(new AddSellerCommand.Completed() { CorrelationId = context.Message.CorrelationId });
            //await _sellerRepository.CommitTransactionAsync();
        }
    }
}
