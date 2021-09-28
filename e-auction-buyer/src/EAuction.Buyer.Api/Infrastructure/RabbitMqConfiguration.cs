using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAuction.Buyer.Api.Infrastructure
{
    public class RabbitMqConfiguration
    {
        /// <summary>
        /// RabbitMqRootUri
        /// </summary>
        public string RabbitMqRootUri { get; set; }
        /// <summary>
        /// UserName
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
    }
}
