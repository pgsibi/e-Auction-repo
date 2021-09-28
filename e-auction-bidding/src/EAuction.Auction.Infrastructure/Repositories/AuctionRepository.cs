using EAuction.Auction.Domain.Aggregate;
using EAuction.Auction.Domain.Aggregate.AuctionAggregate;

namespace EAuction.Auction.Infrastructure.Repositories
{
    public class AuctionRepository : BaseRepository<AuctionItem>, IAuctionRepository
    {
        public AuctionRepository(IMongoContext context) : base(context)
        {
        }
    }
}
