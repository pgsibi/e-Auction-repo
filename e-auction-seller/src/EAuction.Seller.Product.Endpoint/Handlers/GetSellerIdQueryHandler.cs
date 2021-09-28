using EAuction.Seller.Product.Contracts.Query;
using EAuction.Seller.Product.Domain.Aggregate.SellerAggregate;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAuction.Seller.Product.Endpoint.Handlers
{
    public class GetSellerIdQueryHandler : IConsumer<GetSellerId>
    {

        readonly ILogger<GetSellerIdQueryHandler> _logger;
        private readonly ISellerRepository _sellerRepository;
        readonly IPublishEndpoint _endpoint;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="sellerRepository"></param>
        public GetSellerIdQueryHandler(ILogger<GetSellerIdQueryHandler> logger, ISellerRepository sellerRepository,
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
        public async Task Consume(ConsumeContext<GetSellerId> context)
        {
            await _sellerRepository.StartSessionAsync();
            var seller = await _sellerRepository.FindOneAsync(x => x.Email == context.Message.EmailId);
            _logger.LogInformation("Value: {Value}", context.Message);
            await context.RespondAsync(new GetSellerId.Response() { CorrelationId = context.Message.CorrelationId, SellerId = seller?.Id ?? String.Empty });
            await _sellerRepository.AbortTransactionAsync();
        }
    }
}
