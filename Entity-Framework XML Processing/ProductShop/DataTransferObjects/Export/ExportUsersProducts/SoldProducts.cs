using System.Collections.Generic;
using System.Xml.Serialization;

namespace ProductShop.DataTransferObjects.Export.ExportUsersProducts
{

    [XmlType("SoldProducts")]
    public class SoldProducts
    {
        [XmlElement("count")]
        public int Count { get; set; }


        [XmlArray("products")]
        public ProductSold[] Products { get; set; }
    }
}