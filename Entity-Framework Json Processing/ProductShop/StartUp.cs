﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DataTransferObject;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {

            var pShopContext = new ProductShopContext();
           // ResetDatabase(pShopContext);



           //var usersJson = File.ReadAllText(@"../../../Datasets/users.json");
           //Console.WriteLine($"Users imported: {ImportUsers(pShopContext,usersJson)}");

           //var productJson = File.ReadAllText(@"../../../Datasets/products.json");
           //Console.WriteLine($"Products imported: {ImportProducts(pShopContext, productJson)}");

           //var categoriesJson = File.ReadAllText(@"../../../Datasets/categories.json");
           //Console.WriteLine($"Categories imported: {ImportCategories(pShopContext,categoriesJson)}");

           //var categoryProductsJson = File.ReadAllText(@"../../../Datasets/categories-products.json");
           //Console.WriteLine($"category-products imported: {ImportCategoryProducts(pShopContext,categoryProductsJson)}");

            Console.WriteLine(GetProductsInRange(pShopContext));

        }


        //05
        public static string GetProductsInRange(ProductShopContext context)
        {
            var minPrice = 500;
            var maxPrice = 1000;

            var products = context.Products.Where(x => x.Price >= minPrice && x.Price <= maxPrice)
                .Select(x => new
                {
                    name = x.Name,
                    price = x.Price,
                    seller = x.Seller.FirstName + " " + x.Seller.LastName
                })
                .OrderBy(x => x.price)
                .ToList();
            ;
            var productsJson = JsonConvert.SerializeObject(products);
            return productsJson;

        }
        //04
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var mapper = initializeMapper();
            var categoryProductsJson = JsonConvert.DeserializeObject<IEnumerable<CategoryProductsInputModel>>(inputJson);
            var categoriesProducs = mapper.Map<IEnumerable<CategoryProduct>>(categoryProductsJson);
            ;
            context.CategoryProducts.AddRange(categoriesProducs);
            context.SaveChanges();
            return $"Successfully imported {categoriesProducs.Count()}";
        }


        //03
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {



            var mapper = initializeMapper();
            var categoriesJson = JsonConvert.DeserializeObject<IEnumerable<CategoryInputModel>>(inputJson).Where(x => x.Name != null);
            var categories = mapper.Map<IEnumerable<Category>>(categoriesJson);
            context.Categories.AddRange(categories);
            context.SaveChanges();


            return $"Successfully imported {categories.Count()}";
        }

        //02
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var mapper = initializeMapper();
            var productsDeserialized = JsonConvert.DeserializeObject<IEnumerable<ProductInputModel>>(inputJson);
            var products = mapper.Map<IEnumerable<Product>>(productsDeserialized);
            context.Products.AddRange(products);
            context.SaveChanges();
            ;

            return $"Successfully imported {products.Count()}";
        }
        //01
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var mapper = initializeMapper();
            var userJsonDeserialized = JsonConvert.DeserializeObject<IEnumerable<UserInputModel>>(inputJson);
            var users = mapper.Map<IEnumerable<User>>(userJsonDeserialized);
            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count()}";
        }
        //miscellaneous
        private static IMapper initializeMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });
            IMapper mapper = config.CreateMapper();
            return mapper;
        }
        private static void ResetDatabase(ProductShopContext pShopContext)
        {
            pShopContext.Database.EnsureDeleted();
            Console.WriteLine("db Deleted");
            pShopContext.Database.EnsureCreated();
            Console.WriteLine("db Created");
        }
    }
}