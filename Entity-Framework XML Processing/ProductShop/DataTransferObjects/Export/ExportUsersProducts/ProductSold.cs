using System.Xml.Serialization;

namespace ProductShop.DataTransferObjects.Export.ExportUsersProducts
{
    [XmlType("Product")]
    public class ProductSold
    {
        [XmlElement("name")]
        public string Name { get; set; }
        [XmlElement("price")]
        public decimal Price { get; set; }
    }
}