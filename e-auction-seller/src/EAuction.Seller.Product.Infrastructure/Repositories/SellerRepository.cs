using EAuction.Seller.Product.Domain.Aggregate;
using EAuction.Seller.Product.Domain.Aggregate.SellerAggregate;

namespace EAuction.Seller.Product.Infrastructure.Repositories
{
    public class SellerRepository : BaseRepository<Domain.Aggregate.SellerAggregate.Seller>, ISellerRepository
    {
        public SellerRepository(IMongoContext context) : base(context)
        {
        }
    }


}
