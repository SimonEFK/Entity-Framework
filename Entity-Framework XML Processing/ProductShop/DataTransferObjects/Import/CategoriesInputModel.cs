using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.DataTransferObjects.Import
{
    [XmlType("Category")]
    public class CategoriesInputModel
    {
        [XmlElement("name")]
        public string Name { get; set; }

    }
}
