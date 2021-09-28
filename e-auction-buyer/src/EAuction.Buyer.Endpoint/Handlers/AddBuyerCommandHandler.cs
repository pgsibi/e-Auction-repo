using EAuction.Buyer.Contracts.Commands;
using EAuction.Buyer.Domain.Aggregate.BuyerAggregate;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace EAuction.Buyer.Endpoint.Handlers
{
    public class AddBuyerCommandHandler : IConsumer<AddBuyerCommand>
    {

        readonly ILogger<AddBuyerCommandHandler> _logger;
        private readonly IBuyerRepository _buyerRepository;
        readonly IPublishEndpoint _endpoint;

        /// <summary>
        /// AddBuyerCommandHandler
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="buyerRepository"></param>
        public AddBuyerCommandHandler(ILogger<AddBuyerCommandHandler> logger, IBuyerRepository buyerRepository,
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
        public async Task Consume(ConsumeContext<AddBuyerCommand> context)
        {
            await _buyerRepository.StartSessionAsync();
            _buyerRepository.Add(context.Message.Buyer);
            
            _logger.LogInformation("Value: {Value}", context.Message);
            await _endpoint.Publish(new AddBuyerCommand.Completed()
            {
                CorrelationId = context.Message.CorrelationId,
                BuyerId = context.Message.Buyer.Id
            });
            await _buyerRepository.CommitTransactionAsync();
           
        }
    }
}
