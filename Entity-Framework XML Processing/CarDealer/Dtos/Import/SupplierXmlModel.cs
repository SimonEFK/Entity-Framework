using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DataTransferObject.Import
{
    [XmlType("Supplier")]
    public class SupplierXmlModel
    {
        [XmlElement("name")]
        public string  Name { get; set; }
        [XmlElement("isImporter")]
        public bool IsImporter { get; set; }
    }
}
