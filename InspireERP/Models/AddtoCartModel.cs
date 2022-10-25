using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InspireERP.Models
{
    public class AddtoCartModel
    {
        public int PId { get; set; }
        public int Price { get; set; }
        public int Total { get; set; }
        public int DeliveryCharges { get; set; }
        public int Qty { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
    }
}