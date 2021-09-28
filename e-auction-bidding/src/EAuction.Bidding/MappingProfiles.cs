using AutoMapper;
using static EAuction.Auction.Contracts.Queries.GetProductBidDetails;

namespace EAuction.Auction.Endpoint
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Domain.Aggregate.AuctionAggregate.AuctionItem, AuctionItemModel>();
            CreateMap<Domain.Aggregate.AuctionAggregate.Bid, BidModel>();
        }
    }
}
