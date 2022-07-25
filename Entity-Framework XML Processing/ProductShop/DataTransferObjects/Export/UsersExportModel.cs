using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.DataTransferObjects.Export
{

    [XmlType("User")]
    public class UsersExportModel
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }


        [XmlElement("lastName")]
        public string LastName { get; set; }


        [XmlArray("soldProducts")]
        public UsersProductsSoldExportModel[] ProductsSold { get; set; }

    }
}
