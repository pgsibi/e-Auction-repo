using System;
using System.Collections.Generic;
using System.Text;

namespace EAuction.Seller.Product.Contracts.Query
{
    public class GetSellerId
    {
        public Guid CorrelationId { get; set; }

        public string EmailId { get; set; }

        public class Response
        {
            public Guid CorrelationId { get; set; }

            public string SellerId { get; set; }
        }

    }


   

   
}
