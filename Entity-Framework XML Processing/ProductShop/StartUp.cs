using AutoMapper;
using AutoMapper.QueryableExtensions;
using ProductShop.Data;
using ProductShop.DataTransferObjects.Export;
using ProductShop.DataTransferObjects.Import;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using ProductShop.DataTransferObjects.Export.ExportUsersProducts;
using UsersExportModel = ProductShop.DataTransferObjects.Export.UsersExportModel;
using Microsoft.EntityFrameworkCore;

namespace ProductShop
{

    public class StartUp
    {
        private static IMapper initializeMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });
            IMapper mapper = config.CreateMapper();
            return mapper;
        }
        private const string SuccsefullyImportMessage = "Successfully imported {0}";
        public static void Main(string[] args)
        {


            var productShopContext = new ProductShopContext();
            //ResetDatabase(productShopContext);
            Console.WriteLine(GetUsersWithProducts(productShopContext));



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
            var categoriesProductsXml = File.ReadAllText(@"./Datasets/categories-products.xml");


            Console.WriteLine($"Import users {ImportUsers(productShopContext, usersXml)}");
            Console.WriteLine($"Import products {ImportProducts(productShopContext, productsXml)}");
            Console.WriteLine($"Import categories {ImportCategories(productShopContext, categoriesXml)}");
            Console.WriteLine($"Import categories-products {ImportCategoryProducts(productShopContext, categoriesProductsXml)}");


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



            return string.Format(SuccsefullyImportMessage, categories.Length);
        }
        //04
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {

            var root = new XmlRootAttribute("CategoryProducts");

            var stringReader = new StringReader(inputXml);

            var sereliazer = new XmlSerializer(typeof(CategoriesProductsInputModel[]), root);

            var categoriesProductsDto = sereliazer.Deserialize(stringReader) as CategoriesProductsInputModel[];


            var validProducsId = context.Products.Select(x => x.Id).ToArray();
            var validCategories = context.Categories.Select(x => x.Id).ToArray();

            var categoryProducts = new List<CategoryProduct>();

            foreach (var item in categoriesProductsDto)
            {
                if ((validProducsId.Contains(item.ProductId) == false) || (validCategories.Contains(item.CategoryId) == false))
                {
                    continue;

                }

                categoryProducts.Add(new CategoryProduct
                {
                    ProductId = item.ProductId,
                    CategoryId = item.CategoryId,
                });
            }
            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return string.Format(SuccsefullyImportMessage, categoryProducts.Count);
        }

        //05
        public static string GetProductsInRange(ProductShopContext context)
        {

            var minRange = 500;
            var maxRange = 1000;

            var products = context.Products.Where(x => x.Price >= minRange && x.Price <= maxRange).Select(x => new ProductsExportModel
            {
                Name = x.Name,
                Price = x.Price,
                Buyer = x.Buyer.FirstName + ' ' + x.Buyer.LastName,
            })
                .OrderBy(x => x.Price)
                .Take(10)
                .ToArray();



            var rootAttribute = new XmlRootAttribute("Products");
            var emptyNameSpace = new XmlSerializerNamespaces();
            emptyNameSpace.Add(string.Empty, string.Empty);

            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);

            var serializer = new XmlSerializer(typeof(ProductsExportModel[]), rootAttribute);
            serializer.Serialize(stringWriter, products, emptyNameSpace);


            return sb.ToString();
        }

        //06
        public static string GetSoldProducts(ProductShopContext context)
        {
            var mapper = initializeMapper();
            var products = context.Users.Where(x => x.ProductsSold.Count > 0)
                .ProjectTo<UsersExportModel>(mapper.ConfigurationProvider)
                //.Select(x => new UsersExportModel
                //{
                //    FirstName = x.FirstName,
                //    LastName = x.LastName,
                //    ProductsSold = x.ProductsSold.Select(p => new ProductsSoldExportModel
                //    {
                //        Name = p.Name,
                //        Price = p.Price,
                //    }).ToArray()
                //})
                .OrderBy(user => user.LastName)
                .ThenBy(user => user.FirstName)
                .Take(5)
                .ToArray();



            var rootAttribute = new XmlRootAttribute("Users");
            var emptyNameSpace = new XmlSerializerNamespaces();
            emptyNameSpace.Add(string.Empty, string.Empty);

            var sb = new StringBuilder();
            var stringWriter = new StringWriter(sb);

            var serializer = new XmlSerializer(typeof(UsersExportModel[]), rootAttribute);
            serializer.Serialize(stringWriter, products, emptyNameSpace);


            return sb.ToString();
        }

        //07
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var mapper = initializeMapper();
            var categories = context.Categories
                .ProjectTo<CategoriesExportModel>(mapper.ConfigurationProvider)
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.TotalRevenue)
                .ToArray();


            var rootAttribute = new XmlRootAttribute("Categories");
            var emptyNameSpace = new XmlSerializerNamespaces();
            emptyNameSpace.Add(string.Empty, string.Empty);

            //var sb = new StringBuilder();
            var stringWriter = new StringWriter();

            var serializer = new XmlSerializer(typeof(CategoriesExportModel[]), rootAttribute);
            serializer.Serialize(stringWriter, categories, emptyNameSpace);
            stringWriter.Dispose();

            return stringWriter.ToString();

        }

        //08
        public static string GetUsersWithProducts(ProductShopContext context)
        {

            var users = context.Users.Where(u => u.ProductsSold.Count > 0)
                .Include(x=>x.ProductsSold)
                .ToArray() 
                
                .OrderByDescending(u=>u.ProductsSold.Count)
                .Take(10)
                .Select(x => new UsersExportModel2
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Age = x.Age,
                    ProductsSold = new SoldProducts
                    {
                        Count = x.ProductsSold.Count,
                        Products = x.ProductsSold.Select(p => new ProductSold
                        {
                            Name = p.Name,
                            Price = p.Price,
                        })
                        .OrderByDescending(pr => pr.Price)
                        .ToArray()




                    }
                })
                .ToArray();
                

            var result = new UserProductsCount
            {
                Count = context.Users.Count(u => u.ProductsSold.Count > 0),
                Users = users.ToArray()
            };



            var rootAttribute = new XmlRootAttribute("Users");
            var emptyNameSpace = new XmlSerializerNamespaces();
            emptyNameSpace.Add(string.Empty, string.Empty);

            //var sb = new StringBuilder();
            var stringWriter = new StringWriter();

            var serializer = new XmlSerializer(typeof(UserProductsCount), rootAttribute);
            serializer.Serialize(stringWriter, result, emptyNameSpace);
            stringWriter.Dispose();

            return stringWriter.ToString();

        }
    }


}