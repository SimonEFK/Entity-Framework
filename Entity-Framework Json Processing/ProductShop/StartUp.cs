﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DataTransferObject;
using ProductShop.Models;
using Microsoft.EntityFrameworkCore;

namespace ProductShop
{
    public class StartUp
    {
        private const string ImportSuccsesfulyMessage = "Successfully imported {0}.";
        public static void Main(string[] args)
        {

            var pShopContext = new ProductShopContext();
            //ResetDatabase(pShopContext);

        }
       

        //08
        public static string GetUsersWithProducts(ProductShopContext context)
        {

            var users = context.Users
                .Include(x=>x.ProductsSold)
                .Where(x => x.ProductsSold.Any(y => y.BuyerId != null))
                .ToList()                
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    age = x.Age,
                    soldProducts = new
                    {
                        count = x.ProductsSold.Where(y=>y.BuyerId!=null).Count(),
                        products = x.ProductsSold
                        .Where(y => y.BuyerId != null)
                        .Select(y => new
                        {
                            name = y.Name,
                            price = y.Price

                        }).ToList()

                    }
                })
                .OrderByDescending(x => x.soldProducts.count);

            var result = new
            {
                usersCount = users.Count(),
                users = users


            };
            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            };
            var usersJson = JsonConvert.SerializeObject(result, jsonSettings);
            return usersJson;



        }
        //07
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Select(c => new
                {
                    category = c.Name,
                    productsCount = c.CategoryProducts.Count(),
                    averagePrice = c.CategoryProducts.Average(x => x.Product.Price).ToString("F2"),
                    totalRevenue = c.CategoryProducts.Sum(x => x.Product.Price).ToString("F2")



                })
                .OrderByDescending(x => x.productsCount)
                .ToList();

            var jsonOptions = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            };
            var categoriesJson = JsonConvert.SerializeObject(categories, Formatting.Indented, jsonOptions);
            return categoriesJson;


        }
        //06
        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users

                .Where(u => u.ProductsSold.Any(x => x.BuyerId != null))
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    soldProducts = x.ProductsSold.Where(y => y.BuyerId != null).Select(y => new
                    {
                        name = y.Name,
                        price = y.Price,
                        buyerFirstName = y.Buyer.FirstName,
                        buyerLastName = y.Buyer.LastName
                    })
                })
                .OrderBy(x => x.lastName)
                .ThenBy(x => x.firstName)
                .ToList();
            var usersJson = JsonConvert.SerializeObject(users);
            return usersJson;
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

            return string.Format(ImportSuccsesfulyMessage, categoriesProducs.Count());
         
        }
        //03
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {



            var mapper = initializeMapper();
            var categoriesJson = JsonConvert.DeserializeObject<IEnumerable<CategoryInputModel>>(inputJson).Where(x => x.Name != null);
            var categories = mapper.Map<IEnumerable<Category>>(categoriesJson);
            context.Categories.AddRange(categories);
            context.SaveChanges();

            return string.Format(ImportSuccsesfulyMessage, categories.Count());
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

            return string.Format(ImportSuccsesfulyMessage,products.Count());
        }
        //01
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var mapper = initializeMapper();
            var userJsonDeserialized = JsonConvert.DeserializeObject<IEnumerable<UserInputModel>>(inputJson);
            var users = mapper.Map<IEnumerable<User>>(userJsonDeserialized);
            context.Users.AddRange(users);
            context.SaveChanges();

            return string.Format(ImportSuccsesfulyMessage, users.Count());
            
        }


        
        private static IMapper initializeMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });
            IMapper mapper = config.CreateMapper();
            return mapper;
        }
        private static void Seed(ProductShopContext productShopContext)
        {
            var usersJson = File.ReadAllText(@"./Datasets/users.json");
            var productJson = File.ReadAllText(@"./Datasets/products.json");
            var categoriesJson = File.ReadAllText(@"./Datasets/categories.json");
            var categoryProductsJson = File.ReadAllText(@"./Datasets/categories-products.json");

            Console.WriteLine($"Users imported: {ImportUsers(productShopContext, usersJson)}");
            Console.WriteLine($"Products imported: {ImportProducts(productShopContext, productJson)}");
            Console.WriteLine($"Categories imported: {ImportCategories(productShopContext, categoriesJson)}");
            Console.WriteLine($"Category-Products imported: {ImportCategoryProducts(productShopContext, categoryProductsJson)}");
        }
        private static void ResetDatabase(ProductShopContext pShopContext)
        {
            pShopContext.Database.EnsureDeleted();
            Console.WriteLine("db Deleted");
            pShopContext.Database.EnsureCreated();
            Console.WriteLine("db Created");

            Seed(pShopContext);
        }
    }
}