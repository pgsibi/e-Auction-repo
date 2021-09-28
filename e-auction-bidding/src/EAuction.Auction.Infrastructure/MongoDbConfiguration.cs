using System;
using System.Collections.Generic;
using System.Text;

namespace EAuction.Auction.Infrastructure
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
