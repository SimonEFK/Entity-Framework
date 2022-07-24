﻿using ProductShop.Data;
using ProductShop.DataTransferObjects.Import;
using ProductShop.Models;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ProductShop
{

    public class StartUp
    {
        private const string SuccsefullyImportMessage = "Successfully imported {0}";
        public static void Main(string[] args)
        {


            var productShopContext = new ProductShopContext();
            //ResetDatabase(ProductShopContext);


            var categoriesXml = File.ReadAllText(@"./Datasets/categories.xml");
            Console.WriteLine(ImportCategories(productShopContext, categoriesXml));



        }

        private static void ResetDatabase(ProductShopContext ProductShopContext)
        {
            ProductShopContext.Database.EnsureDeleted();
            Console.WriteLine("Database Deleted");
            ProductShopContext.Database.EnsureCreated();
            Console.WriteLine("Database Created");

            Seed(ProductShopContext);
        }


        private static void Seed(ProductShopContext productShopContext)
        {
            var usersXml = File.ReadAllText(@"./Datasets/users.xml");
            var productsXml = File.ReadAllText(@"./Datasets/products.xml");
            var categoriesXml = File.ReadAllText(@"./Datasets/categories.xml");
            



            Console.WriteLine($"Import users {ImportUsers(productShopContext, usersXml)}");
            Console.WriteLine($"Import products {ImportProducts(productShopContext, productsXml)}");
            Console.WriteLine($"Import categories {ImportCategories(productShopContext, categoriesXml)}");


        }

        //01
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {

            var rootAttribute = new XmlRootAttribute("Users");


            var xmlSereliazer = new XmlSerializer(typeof(UserInputModel[]), rootAttribute);

            var stringReader = new StringReader(inputXml);
            var userDto = xmlSereliazer.Deserialize(stringReader) as UserInputModel[];

            var users = userDto.Select(x => new User
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                Age = x.Age
            }).ToArray();

            context.Users.AddRange(users);
            context.SaveChanges();

            return string.Format(SuccsefullyImportMessage, users.Length);
        }
        //02
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var root = new XmlRootAttribute("Products");

            var stringReader = new StringReader(inputXml);

            var sereliazer = new XmlSerializer(typeof(ProductsInputModel[]), root);

            var productsDto = sereliazer.Deserialize(stringReader) as ProductsInputModel[];

            var validUsersId = context.Users.Select(x => x.Id).ToArray();

            var products = productsDto.Where(x => validUsersId.Contains(x.SellerId)).Select(x => new Product
            {
                Name = x.Name,
                Price = x.Price,
                SellerId = x.SellerId,
                BuyerId = x.BuyerId,

            }).ToArray();
            context.Products.AddRange(products);
            context.SaveChanges();



            return string.Format(SuccsefullyImportMessage, products.Length);

        }
        //03
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var root = new XmlRootAttribute("Categories");

            var stringReader = new StringReader(inputXml);

            var sereliazer = new XmlSerializer(typeof(CategoriesInputModel[]), root);

            var categoriesDto = sereliazer.Deserialize(stringReader) as CategoriesInputModel[];

            var categories = categoriesDto.Select(x => new Category
            {
                Name = x.Name,
            }).ToArray();

            context.Categories.AddRange(categories);
            context.SaveChanges();



            return string.Format(SuccsefullyImportMessage,categories.Length);
        }
    }


}