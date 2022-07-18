using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var carDealerContext = new CarDealerContext();
            //ResetDatabase(carDealerContext);

            //var importSuppliersJson = File.ReadAllText(@"../../../Datasets/suppliers.json");
            //ImportSuppliers(carDealerContext, importSuppliersJson);

            //var importPartsJson = File.ReadAllText(@"../../../Datasets/parts.json");

            //ImportParts(carDealerContext, importPartsJson);

            var carsImportJson = File.ReadAllText(@"./Datasets/cars.json");
            Console.WriteLine(ImportCars(carDealerContext, carsImportJson));
            ;
        }


        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var cars = JsonConvert.DeserializeObject<IEnumerable<CarImportModel>>(inputJson);


            var dbCars = new List<Car>();
            var dbCarParts = new List<PartCar>();
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



            return $"Successfully imported {dbCars.Count}.";
        }


        private static void ResetDatabase(CarDealerContext carDealerContext)
        {
            carDealerContext.Database.EnsureDeleted();
            Console.WriteLine("Deleted");
            carDealerContext.Database.EnsureCreated();
            Console.WriteLine("Created");
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


            return $"Successfully imported {suppliers.Count()}.";

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

            return $"Successfully imported {validParts.Count()}.";




        }
    }




}