using ILibrary.HRM;
using System;
using System.Collections.Generic;

namespace ILibrary.Production
{
    public class Product
    {
        public int Id
        {
            get; set;
        }
        public string Name{get; set;}
        public string Description {get; set;}
        public int RetailPrice{get; set;}
        public int CostPrice{get; set;}
        public int DiscountPrice{get; set;}
        public int SalePrice{get; set;}
        public int DeliveryCharges { get; set; }
        public string ModelNumber { get; set; }
        public string SerialNumber { get; set; }
        public SubCategory SubCategory { get; set; }
        public Color Color { get; set; }
        public List<Images> Images{ get; set; }
        public Size Size { get; set; }
        public Material Material { get; set; }
        public int Qty { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime LastSold { get; set; }
        public DateTime LastPurchased { get; set; }
        public bool IsActive { get; set; }

        public Supplier Supplier { get; set; }
        public string BarCode { get; set; }
        public Brand Brand { get; set; }
        public bool IsSale { get; set; }
        public bool IsDiscount { get; set; }
        public bool IsFeatured { get; set; }
        public int ViewCounts { get; set; }







    }
}
