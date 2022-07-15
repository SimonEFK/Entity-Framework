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

            //Console.WriteLine(ImportSuppliers(carDealerContext, importSuppliersJson));

            var importPartsJson = File.ReadAllText(@"../../../Datasets/parts.json");

            Console.WriteLine(ImportParts(carDealerContext, importPartsJson));


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
            var validParts = partsDesereliazed.Where(x => validSuppliers.Contains(x.SupplierId)).ToList() ;
            
            context.AddRange(validParts);
            context.SaveChanges();

            return $"Successfully imported {validParts.Count()}.";




        }
    }




}