using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CarDealer
{

    public class StartUp
    {
        private const string ImportSuccsesfulyMessage = "Successfully imported {0}.";
        public static void Main(string[] args)
        {
            var carDealerContext = new CarDealerContext();
            ResetDatabase(carDealerContext);




        }


        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var validCars = context.Cars.Select(x => x.Id).ToList();
            var validCustomers = context.Customers.Select(x => x.Id).ToList();


            var salesDto = JsonConvert.DeserializeObject<IEnumerable<Sale>>(inputJson).ToList();



            var salesCount = salesDto.Count;
            context.AddRange(salesDto);
            context.SaveChanges();


            return string.Format(ImportSuccsesfulyMessage, salesCount);
        }



        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<IEnumerable<Customer>>(inputJson);


            context.AddRange(customers);
            context.SaveChanges();


            return string.Format(ImportSuccsesfulyMessage, customers.Count());
        }


        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var cars = JsonConvert.DeserializeObject<IEnumerable<CarImportModel>>(inputJson);


            var dbCars = new List<Car>();
            var validParts = context.Parts.Select(x => x.Id).ToList();

            foreach (var currentCar in cars)
            {
                var car = new Car
                {
                    Make = currentCar.Make,
                    Model = currentCar.Model,
                    TravelledDistance = currentCar.TravelledDistance,
                    PartCars = new List<PartCar>()
                };

                foreach (var part in currentCar.PartsId.Distinct())
                {
                    if (validParts.Contains(part))
                    {
                        car.PartCars.Add(new PartCar
                        {
                            PartId = part,
                            CarId = car.Id
                        });
                    }
                }
                dbCars.Add(car);

            }
            context.AddRange(dbCars);
            context.SaveChanges();



            return string.Format(ImportSuccsesfulyMessage, dbCars.Count);
        }


        private static void Seed(CarDealerContext dbContext)
        {
            var importSuppliersJson = File.ReadAllText(@"./Datasets/suppliers.json");
            var importPartsJson = File.ReadAllText(@"./Datasets/parts.json");
            var carsImportJson = File.ReadAllText(@"./Datasets/cars.json");
            var customersImportJson = File.ReadAllText(@"./Datasets/customers.json");
            var salesImportJson = File.ReadAllText(@"./Datasets/sales.json");



            Console.WriteLine(ImportSuppliers(dbContext, importSuppliersJson));
            Console.WriteLine(ImportParts(dbContext, importPartsJson));
            Console.WriteLine(ImportCars(dbContext, carsImportJson));
            Console.WriteLine(ImportCustomers(dbContext, customersImportJson));
            Console.WriteLine(ImportSales(dbContext, salesImportJson));

        }

        private static void ResetDatabase(CarDealerContext carDealerContext)
        {
            carDealerContext.Database.EnsureDeleted();
            Console.WriteLine("Deleted");
            carDealerContext.Database.EnsureCreated();
            Console.WriteLine("Created");
            Seed(carDealerContext);
        }

        private static IMapper InitializeMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });
            IMapper mapper = config.CreateMapper();
            return mapper;
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var mapper = InitializeMapper();

            var suppliersDesereliazed = JsonConvert.DeserializeObject<IEnumerable<SuppliersDto>>(inputJson);

            var suppliers = mapper.Map<IEnumerable<Supplier>>(suppliersDesereliazed);
            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();


            return string.Format(ImportSuccsesfulyMessage, suppliers.Count());

        }
        public static string ImportParts(CarDealerContext context, string inputJson)
        {

            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,

            };
            var partsDesereliazed = JsonConvert.DeserializeObject<IEnumerable<Part>>(inputJson);

            var validSuppliers = context.Suppliers.Select(x => x.Id).ToList();
            var validParts = partsDesereliazed.Where(x => validSuppliers.Contains(x.SupplierId)).ToList();

            context.AddRange(validParts);
            context.SaveChanges();

            return string.Format(ImportSuccsesfulyMessage, validParts.Count());
        }
    }
}