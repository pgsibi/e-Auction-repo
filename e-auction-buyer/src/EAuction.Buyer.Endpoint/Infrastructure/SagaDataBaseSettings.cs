using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAuction.Buyer.Endpoint.Infrastructure
{
    public class SagaDataBaseSettings
    {
        /// <summary>
        /// ConnectionString
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// DatabaseName
        /// </summary>
        public string DatabaseName { get; set; }
    }
}
