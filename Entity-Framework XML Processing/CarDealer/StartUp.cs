﻿using CarDealer.Data;
using CarDealer.Dtos.Import;
using CarDealer.Models;
using System;
using System.Globalization;
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




            Console.WriteLine($"Import Suppliers:{ImportSuppliers(context, suppliersXml)}");
            Console.WriteLine($"Import Parts:{ImportParts(context, partsXml)}");
            Console.WriteLine($"Import Cars:{ImportCars(context, carsXml)}");
            Console.WriteLine($"Import Customers:{ImportCustomers(context, customersXml)}");
        }
    }
}