using System;
using System.Collections.Generic;
using System.Text;

namespace EAuction.Buyer.Contracts.Queries
{
    public class GetBuyerIdByEmailId
    {
        public Guid CorrelationId { get; set; }
        public string EmailId { get; set; }

        public class Response
        {
            public Guid CorrelationId { get; set; }
            public string BuyerId { get; set; }

        }
    }
}
