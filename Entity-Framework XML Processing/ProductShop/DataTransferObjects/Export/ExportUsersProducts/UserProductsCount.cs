using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.DataTransferObjects.Export.ExportUsersProducts
{

    [XmlType("Change Type")]
    public class UserProductsCount
    {

        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("users")]
        public UsersExportModel2[] Users { get; set; }
    }
}
