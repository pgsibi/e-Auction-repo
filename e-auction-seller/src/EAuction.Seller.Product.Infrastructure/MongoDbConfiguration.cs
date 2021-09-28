using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAuction.Seller.Product.Infrastructure
{
    public class MongoDbConfiguration
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
