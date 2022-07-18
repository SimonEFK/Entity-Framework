using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarDealer.DTO
{
    public class CarImportModel
    {
        [JsonProperty("make")]
        public string Make { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("travelledDistance")]
        public long TravelledDistance { get; set; }

        public int[] PartsId { get; set; }
        //[JsonProperty("partsId")]
        //public CarPartsImportModel[] PartsId { get; set; }

    }
}

/*
 {
    "make": "Opel",
    "model": "Omega",
    "travelledDistance": 176664996,
    "partsId": [
      38,
      102,
      23,
      116,
      46,
      68,
      88,
      104,
      71,
      32,
      114
    ]
  }
 */


