using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.DataTransferObjects.Export
{

    [XmlType("Category")]
    public class CategoriesExportModel
    {
        [XmlElement("name")]
        public string Name { get; set; }
        [XmlElement("count")]
        public int Count { get; set; }
        [XmlElement("avergePrice")]
        public decimal AvergePrice { get; set; }
        [XmlElement("totalRevenue")]
        public decimal TotalRevenue { get; set; }
    }
}
