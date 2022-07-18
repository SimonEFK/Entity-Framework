using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CarDealer.DTO
{
    public class CustomersDtoExport
    {
        //[JsonProperty("name")]
        public string Name { get; set; }
        //[JsonProperty("birthDate")]
        public string BirthDate { get; set; }
        //[JsonProperty("isYoungDriver")]
        
        public bool IsYoungDriver { get; set; }
    }
}
