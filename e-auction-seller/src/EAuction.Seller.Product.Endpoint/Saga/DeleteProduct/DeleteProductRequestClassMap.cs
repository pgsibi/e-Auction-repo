﻿using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAuction.Seller.Product.Endpoint.Saga.DeleteProduct
{
    public class DeleteProductRequestClassMap : BsonClassMap<DeleteProductRequestState>
    {
        public DeleteProductRequestClassMap()
        {
            MapProperty(x => x.RequestTime)
                .SetSerializer(new DateTimeSerializer(DateTimeKind.Utc));
            MapProperty(x => x.LastUpdatedTime)
                .SetSerializer(new DateTimeSerializer(DateTimeKind.Utc));
            MapProperty(x => x.CorrelationId);
            MapProperty(x => x.ProductId);
            MapProperty(x => x.SellerId);
            MapProperty(x => x.RequestId);
            MapProperty(x => x.ResponseAddress);
            MapProperty(x => x.CurrentState);
            MapProperty(x => x.Version);
        }
    }
}
