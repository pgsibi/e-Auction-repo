using AutoMapper;
using EAuction.Auction.Contracts.Queries;
using EAuction.Auction.Domain.Aggregate.AuctionAggregate;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace EAuction.Auction.Endpoint.Handlers
{
    public class GetAuctionDetailsQueryHandler : IConsumer<GetProductBidDetails>
    {

        readonly ILogger<GetAuctionDetailsQueryHandler> _logger;
        private readonly IAuctionRepository _auctionRepository;
        readonly IPublishEndpoint _endpoint;
        private readonly IMapper _mapper;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="auctionRepository"></param>
        public GetAuctionDetailsQueryHandler(ILogger<GetAuctionDetailsQueryHandler> logger, IAuctionRepository auctionRepository, IPublishEndpoint endpoint, IMapper mapper)
        {
            _mapper = mapper;
            _logger = logger;
            _auctionRepository = auctionRepository;
            _endpoint = endpoint;
        }

        /// <summary>
        /// Consumer/Handler
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<GetProductBidDetails> context)
        {
            try
            {
                var auctionItem = await _auctionRepository.FindOneAsync(x => x.Id == context.Message.ProductId);
                _logger.LogInformation("Value: {Value}", context.Message);
                if (auctionItem != null)
                {
                    await context.RespondAsync(new GetProductBidDetails.Response()
                    {
                        CorrelationId = context.Message.CorrelationId,
                        AuctionItem = _mapper.Map<GetProductBidDetails.AuctionItemModel>(auctionItem)
                    });
                }

            }
            catch (Exception e)
            {
                await context.RespondAsync(new GetProductBidDetails.Response() { CorrelationId = context.Message.CorrelationId, AuctionItem = null });
            }
        }
    }
}
