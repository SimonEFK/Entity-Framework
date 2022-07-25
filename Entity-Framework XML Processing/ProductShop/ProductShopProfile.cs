using AutoMapper;
using ProductShop.DataTransferObjects.Export;
using ProductShop.Models;
using System;
using System.Linq;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            this.CreateMap<Product, UsersProductsSoldExportModel>();
            this.CreateMap<User, UsersExportModel>()
                .ForMember(x => x.ProductsSold, opt => opt.MapFrom(y => y.ProductsSold));
            this.CreateMap<Category, CategoriesExportModel>()
                .ForMember(c => c.Count, map => map.MapFrom(x => x.CategoryProducts.Count))
                .ForMember(c=> c.AvergePrice,map=>map.MapFrom(x=>x.CategoryProducts.Average(p=>p.Product.Price)))
                .ForMember(c=> c.TotalRevenue,map=>map.MapFrom(x=>x.CategoryProducts.Sum(p=>p.Product.Price)));
            
        }
    }
}
