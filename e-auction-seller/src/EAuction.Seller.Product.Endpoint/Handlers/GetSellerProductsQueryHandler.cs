using AutoMapper;
using EAuction.Seller.Product.Contracts.Models;
using EAuction.Seller.Product.Domain.Aggregate.SellerAggregate;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EAuction.Seller.Product.Endpoint.Handlers
{
    public class GetSellerProductsQueryHandler : IConsumer<ProductListRequestModel>
    {
        readonly ILogger<GetSellerProductsQueryHandler> _logger;
        private readonly ISellerRepository _sellerRepository;
        readonly IPublishEndpoint _endpoint;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="sellerRepository"></param>
        public GetSellerProductsQueryHandler(ILogger<GetSellerProductsQueryHandler> logger, ISellerRepository sellerRepository, IPublishEndpoint endpoint, IMapper mapper)
        {
            _mapper = mapper;
            _logger = logger;
            _sellerRepository = sellerRepository;
            _endpoint = endpoint;
        }

        /// <summary>
        /// Consumer/Handler
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<ProductListRequestModel> context)
        {
            var seller = await _sellerRepository.FindOneAsync(x => x.Email == context.Message.EmailId);
            _logger.LogInformation("Value: {Value}", context.Message);
            await context.RespondAsync(new ProductListResponseModel() { CorrelationId = context.Message.CorrelationId, Products = _mapper.Map<List<ProductInfoModel>>(seller?.Products) });
        }
    }

}