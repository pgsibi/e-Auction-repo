using System;

namespace EAuction.Buyer.Contracts.Commands
{
    public class AddBuyerCommand
    {
        public Guid CorrelationId { get; set; }
        public Domain.Aggregate.BuyerAggregate.Buyer Buyer { get; set; }

        public class Completed
        {
            public Guid CorrelationId { get; set; }
            public string BuyerId { get; set; }
        }
    }
}
