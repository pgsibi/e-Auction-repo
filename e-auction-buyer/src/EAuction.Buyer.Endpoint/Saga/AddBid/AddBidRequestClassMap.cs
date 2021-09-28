using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;

namespace EAuction.Buyer.Endpoint.Saga.AddBid
{
    public class AddBidRequestClassMap :
        BsonClassMap<AddBidRequestState>
    {
        public AddBidRequestClassMap()
        {
            MapProperty(x => x.RequestTime)
                .SetSerializer(new DateTimeSerializer(DateTimeKind.Utc));
            MapProperty(x => x.LastUpdatedTime)
                .SetSerializer(new DateTimeSerializer(DateTimeKind.Utc));
            MapProperty(x => x.CorrelationId);
            MapProperty(x => x.ProductId);
            MapProperty(x => x.BuyerId);
            MapProperty(x => x.Request);
            MapProperty(x => x.RequestId);
            MapProperty(x => x.ResponseAddress);
            MapProperty(x => x.CurrentState);
            MapProperty(x => x.Version);
        }
    }
}
