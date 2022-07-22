using ProductShop.Data;
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


            var ProductShopContext = new ProductShopContext();
            ResetDatabase(ProductShopContext);

            
            
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



            Console.WriteLine($"Import users {ImportUsers(productShopContext,usersXml)}");
                


        }

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
            
            return string.Format(SuccsefullyImportMessage,users.Length);
        }
    }


}