using EAuction.Buyer.Domain.Aggregate;
using EAuction.Buyer.Domain.Aggregate.BuyerAggregate;

namespace EAuction.Buyer.Infrastucture.Repositories
{
    public class BuyerRepository : BaseRepository<Domain.Aggregate.BuyerAggregate.Buyer>, IBuyerRepository
    {
        public BuyerRepository(IMongoContext context) : base(context)
        {
        }
    }
}
