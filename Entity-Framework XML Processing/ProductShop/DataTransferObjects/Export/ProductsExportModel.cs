﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.DataTransferObjects.Export
{
    [XmlType("Product")]
    public class ProductsExportModel
    {
        [XmlElement("name")]
        public string Name { get; set; }


        [XmlElement("price")]
        public decimal Price { get; set; }
        [XmlElement("buyer")]
        public string Buyer { get; set; }

    }
}
