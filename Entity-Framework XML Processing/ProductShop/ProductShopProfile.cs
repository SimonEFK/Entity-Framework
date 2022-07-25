using AutoMapper;
using ProductShop.DataTransferObjects.Export;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            this.CreateMap<Product,UsersProductsSoldExportModel>();
            this.CreateMap<User, UsersExportModel>()
                .ForMember(x => x.ProductsSold, opt => opt.MapFrom(y => y.ProductsSold));
        }
    }
}
