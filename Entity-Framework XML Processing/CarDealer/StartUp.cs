using CarDealer.Data;
using CarDealer.DataTransferObject.Import;
using CarDealer.Models;
using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        private const string ImportMessage = "Successfully imported {0}";
        public static void Main(string[] args)
        {
            var db = new CarDealerContext();
            
            ResetDatabase(db);

        }

        //09
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            var root = new XmlRootAttribute("Suppliers");
            var xmlSereliazer = new XmlSerializer(typeof(SupplierXmlModel[]),root);
            var stringReader = new StringReader(inputXml);

            var suppliersDto = xmlSereliazer.Deserialize(stringReader) as SupplierXmlModel[];
            var suppliers = suppliersDto.Select(x => new Supplier
            {
                Name = x.Name,
                IsImporter = x.IsImporter,
            }).ToArray();
            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return string.Format(ImportMessage,suppliers.Length);
        }



        private static void ResetDatabase(CarDealerContext db)
        {            
            db.Database.EnsureDeleted();
            Console.WriteLine("Database deleted");
            db.Database.EnsureCreated();
            Console.WriteLine("Database created");

            Seed(db);
        }

        private static void Seed(CarDealerContext context)
        {
            var suppliersXml = File.ReadAllText(@".\Datasets\suppliers.xml");


            Console.WriteLine($"Import Suppliers{ImportSuppliers(context, suppliersXml)}");
        }
    }
}