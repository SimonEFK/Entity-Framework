using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.Dtos.Import
{
    [XmlType("Car")]
    public class CarXmlModel
    {
        [XmlElement("make")]
        public string Make { get; set; }
        [XmlElement("model")]
        public string Model { get; set; }

        [XmlElement("TraveledDistance")]
        public long TraveledDistance { get; set; }
        [XmlArray("parts")]
        public CarPartXmlModel[] CarPartXmlModel { get; set; }
    }
}
