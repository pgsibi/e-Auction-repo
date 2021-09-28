using AutoMapper;
using EAuction.Seller.Product.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAuction.Seller.Product.Endpoint
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Seller.Product.Domain.Aggregate.SellerAggregate.Product, ProductInfoModel>();
        }
    }
}
