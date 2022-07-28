using CarDealer.Data;
using CarDealer.Dtos.Import;
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


        //10

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var root = new XmlRootAttribute("Parts");
            var xmlSerializer = new XmlSerializer(typeof(PartXmlModel[]),root);
            var sr = new StringReader(inputXml);
            var partsDto = xmlSerializer.Deserialize(sr) as PartXmlModel[];

            var validSuppliers = context.Suppliers.Select(x=>x.Id).ToList();

            var parts = partsDto.Where(x => validSuppliers.Contains(x.SupplierId) == true)
                .Select(x => new Part
                {
                    
                    Name = x.Name,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    SupplierId = x.SupplierId,


                }).ToArray();
            context.AddRange(parts);
            context.SaveChanges();

            


            return string.Format(ImportMessage, parts.Length);
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
            var partsXml = File.ReadAllText(@".\Datasets\parts.xml");
            

            Console.WriteLine($"Import Suppliers:{ImportSuppliers(context, suppliersXml)}");
            Console.WriteLine($"Import Parts:{ImportParts(context, partsXml)}");
        }
    }
}