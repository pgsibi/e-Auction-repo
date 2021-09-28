using EAuction.Buyer.Contracts.Queries;
using EAuction.Buyer.Domain.Aggregate.BuyerAggregate;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace EAuction.Buyer.Endpoint.Handlers
{
    public class GetBuyerIdByEmailIdQueryHandler : IConsumer<GetBuyerIdByEmailId>
    {

        readonly ILogger<GetBuyerIdByEmailIdQueryHandler> _logger;
        private readonly IBuyerRepository _buyerRepository;
        readonly IPublishEndpoint _endpoint;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="buyerRepository"></param>
        public GetBuyerIdByEmailIdQueryHandler(ILogger<GetBuyerIdByEmailIdQueryHandler> logger, IBuyerRepository buyerRepository,
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
        public async Task Consume(ConsumeContext<GetBuyerIdByEmailId> context)
        {
            var Buyer = await _buyerRepository.FindOneAsync(x => x.Email == context.Message.EmailId);
            _logger.LogInformation("Value: {Value}", context.Message);
            await context.RespondAsync(new GetBuyerIdByEmailId.Response()
            {
                CorrelationId = context.Message.CorrelationId,
                BuyerId = Buyer?.Id ?? string.Empty
            });
        }
    }
}
