using System.Xml.Serialization;

namespace CarDealer
{
    [XmlType("car")]
    public class CarXmlExportModel
    {
        [XmlElement("make")]
        public string Make { get; set; }
        [XmlElement("model")]
        public string Model { get; set; }
        [XmlElement("travelled-distance")]
        public long TraveledDistance { get; set; }
    }
}