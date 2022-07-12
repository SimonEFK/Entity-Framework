using System;
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
            ResetDatabase(pShopContext);

            var usersJson = File.ReadAllText(@"../../../Datasets/users.json");
            Console.WriteLine(ImportUsers(pShopContext, usersJson));
            var productJson = File.ReadAllText(@"../../../Datasets/products.json");
            Console.WriteLine(ImportProducts(pShopContext, productJson));
            var categoriesJson = File.ReadAllText(@"../../../Datasets/categories.json");

            
            Console.WriteLine("");
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