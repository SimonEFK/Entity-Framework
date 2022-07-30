using CarDealer.Data;
using CarDealer.Dtos.Export;
using CarDealer.Dtos.Import;
using CarDealer.Models;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        private const string ImportMessage = "Successfully imported {0}";
        public static void Main(string[] args)
        {
            var db = new CarDealerContext();

            //ResetDatabase(db);
            Console.WriteLine(GetTotalSalesByCustomer(db));




        }

        //18
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {

            var customers = context.Customers.Where(x => x.Sales.Count > 0).Select(x => new TotalSalesByCustomerXmlExport
            {
                FullName = x.Name,
                BoughtCars = x.Sales.Count,
                MoneySpent = x.Sales.Select(c => c.Car).SelectMany(cp => cp.PartCars).Sum(x => x.Part.Price)
            })
                .OrderByDescending(x=>x.MoneySpent)
                .ToArray();

            var root = new XmlRootAttribute("customers");
            var emptyNameSpace = new XmlSerializerNamespaces();
            emptyNameSpace.Add(string.Empty, string.Empty);
            var xmlSerializer = new XmlSerializer(typeof(TotalSalesByCustomerXmlExport[]), root);
            using var sw = new StringWriter();
            xmlSerializer.Serialize(sw, customers, emptyNameSpace);
            return sw.ToString();



            
        }
        //17
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {

            var cars = context.Cars
                .Select(x => new CarAndCarPartsXmlExport
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance,
                    Parts = x.PartCars
                    .Select(p => new CarPartsXmlExport
                    {
                        Name = p.Part.Name,
                        Price = p.Part.Price


                    })
                    .OrderByDescending(p => p.Price)
                    .ToArray()
                })
                .OrderByDescending(c => c.TravelledDistance)
                .ThenBy(x => x.Model)
                .Take(5)
                .ToArray();

            var root = new XmlRootAttribute("cars");
            var emptyNameSpace = new XmlSerializerNamespaces();
            emptyNameSpace.Add(string.Empty, string.Empty);
            var xmlSerializer = new XmlSerializer(typeof(CarAndCarPartsXmlExport[]), root);
            using var sw = new StringWriter();
            xmlSerializer.Serialize(sw, cars, emptyNameSpace);
            return sw.ToString();



        }
        //16
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers.Where(x => x.IsImporter == false)
                .Select(x => new SuppliersXmlExport
                {
                    Id = x.Id,
                    Name = x.Name,
                    PartsCount = x.Parts.Count
                })
                .ToArray();

            var root = new XmlRootAttribute("suppliers");
            var emptyNameSpace = new XmlSerializerNamespaces();
            emptyNameSpace.Add(string.Empty, string.Empty);
            var xmlSerializer = new XmlSerializer(typeof(SuppliersXmlExport[]), root);
            using var sw = new StringWriter();
            xmlSerializer.Serialize(sw, suppliers, emptyNameSpace);
            return sw.ToString();





        }
        //15
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(x => x.Make == "BMW")
                .Select(x => new CarSpecificMakeXmlExport
                {
                    Id = x.Id,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance,


                })
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .ToArray();
            var xmlSerializer = new XmlSerializer(typeof(CarSpecificMakeXmlExport[]), new XmlRootAttribute("cars"));
            var emptyNameSpace = new XmlSerializerNamespaces();
            emptyNameSpace.Add(string.Empty, string.Empty);

            using var sw = new StringWriter();
            xmlSerializer.Serialize(sw, cars, emptyNameSpace);

            return sw.ToString();
        }
        //14
        public static string GetCarsWithDistance(CarDealerContext context)
        {

            var maxDistance = 2_000_000;

            var cars = context.Cars
                .Where(c => c.TravelledDistance > maxDistance)
                .Select(x => new CarXmlExportModel
                {
                    Make = x.Make,
                    Model = x.Model,
                    TraveledDistance = x.TravelledDistance

                })
                .OrderBy(x => x.Make)
                .ThenBy(x => x.Model)
                .Take(10)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(CarXmlExportModel[]), new XmlRootAttribute("cars"));

            var emptyNameSpace = new XmlSerializerNamespaces();
            emptyNameSpace.Add(string.Empty, string.Empty);
            using var sw = new StringWriter();

            xmlSerializer.Serialize(sw, cars, emptyNameSpace);



            return sw.ToString();
        }
        //13
        public static string ImportSales(CarDealerContext context, string inputXml)
        {

            var xmlSerializer = new XmlSerializer(typeof(SaleXmlModel[]), new XmlRootAttribute("Sales"));

            var salesDto = xmlSerializer.Deserialize(new StringReader(inputXml)) as SaleXmlModel[];
            var validCarIds = context.Cars.Select(x => x.Id).ToList();

            var sales = salesDto.Where(x => validCarIds.Contains(x.CarId)).Select(x => new Sale
            {
                CarId = x.CarId,
                CustomerId = x.CustomerId,
                Discount = x.Discount,
            }).ToArray();

            context.Sales.AddRange(sales);
            context.SaveChanges();
            return string.Format(ImportMessage, sales.Length);

        }
        //12
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            var root = new XmlRootAttribute("Customers");
            var xmlSereliazer = new XmlSerializer(typeof(CustomerXmlModel[]), root);
            var sr = new StringReader(inputXml);
            var customersDto = xmlSereliazer.Deserialize(sr) as CustomerXmlModel[];

            var customers = customersDto.Select(x => new Customer
            {
                Name = x.Name,
                IsYoungDriver = x.IsYoungDriver,
                BirthDate = DateTime.Parse(x.BirthDate, CultureInfo.InvariantCulture),


            }).ToArray();
            context.Customers.AddRange(customers);
            context.SaveChanges();




            return string.Format(ImportMessage, customers.Length);
        }
        //11
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var root = new XmlRootAttribute("Cars");
            var xmlSereliazer = new XmlSerializer(typeof(CarXmlModel[]), root);
            var sr = new StringReader(inputXml);
            var carsDto = xmlSereliazer.Deserialize(sr) as CarXmlModel[];
            var validParts = context.Parts.Select(x => x.Id).ToList();

            foreach (var currentCar in carsDto)
            {
                var car = new Car
                {
                    Make = currentCar.Make,
                    Model = currentCar.Model,
                    TravelledDistance = currentCar.TraveledDistance

                };

                foreach (var part in currentCar.CarPartXmlModel.Select(x => x.Id).Distinct())
                {
                    if (validParts.Contains(part))
                    {
                        var partCar = new PartCar
                        {
                            PartId = part
                        };
                        car.PartCars.Add(partCar);
                    }
                }
                context.Cars.Add(car);
                context.SaveChanges();
            }


            return string.Format(ImportMessage, carsDto.Length);
        }
        //10
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var root = new XmlRootAttribute("Parts");
            var xmlSerializer = new XmlSerializer(typeof(PartXmlModel[]), root);
            var sr = new StringReader(inputXml);
            var partsDto = xmlSerializer.Deserialize(sr) as PartXmlModel[];

            var validSuppliers = context.Suppliers.Select(x => x.Id).ToList();

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
            var xmlSereliazer = new XmlSerializer(typeof(SupplierXmlModel[]), root);
            var stringReader = new StringReader(inputXml);

            var suppliersDto = xmlSereliazer.Deserialize(stringReader) as SupplierXmlModel[];
            var suppliers = suppliersDto.Select(x => new Supplier
            {
                Name = x.Name,
                IsImporter = x.IsImporter,
            }).ToArray();
            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return string.Format(ImportMessage, suppliers.Length);
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
            var carsXml = File.ReadAllText(@".\Datasets\cars.xml");
            var customersXml = File.ReadAllText(@".\Datasets\customers.xml");
            var salesXml = File.ReadAllText(@".\Datasets\sales.xml");


            Console.WriteLine($"Import Suppliers:{ImportSuppliers(context, suppliersXml)}");
            Console.WriteLine($"Import Parts:{ImportParts(context, partsXml)}");
            Console.WriteLine($"Import Cars:{ImportCars(context, carsXml)}");
            Console.WriteLine($"Import Customers:{ImportCustomers(context, customersXml)}");
            Console.WriteLine($"Import Sales:{ImportSales(context, salesXml)}");
        }
    }
}