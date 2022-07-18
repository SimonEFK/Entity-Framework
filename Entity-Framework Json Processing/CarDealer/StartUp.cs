using System;
using System.Collections.Generic;
using System.Globalization;
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
            //ResetDatabase(carDealerContext);
            Console.WriteLine(GetCarsFromMakeToyota(carDealerContext));


        }


        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var carMake = "Toyota";

            var cars = context.Cars.Where(x => x.Make.ToUpper() == carMake.ToUpper()).Select(x => new
            {
                x.Id,
                x.Make,
                x.Model,
                x.TravelledDistance,
            })
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .ToList();
            var jsonCarsExport = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return jsonCarsExport;
        }
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new CustomersDtoExport
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    IsYoungDriver = c.IsYoungDriver,

                })

                .ToList();

            var jsonExport = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return jsonExport;
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

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<IEnumerable<Customer>>(inputJson);


            context.AddRange(customers);
            context.SaveChanges();


            return string.Format(ImportSuccsesfulyMessage, customers.Count());
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



        private static IMapper InitializeMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });
            IMapper mapper = config.CreateMapper();
            return mapper;
        }

    }
}