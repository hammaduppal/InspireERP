using ILibrary.HRM;
using ILibrary.Production;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILibrary.SaleManager
{
    public class OrderMaster
    {
        public int Id { get; set; }
        public DateTime OrderDateTime { get; set; }
        public Customer Customer { get; set; }
        public OrderStatus Status { get; set; }
        public int GrandTotal { get; set; }
        public Product Product { get; set; }
        public int Qty { get; set; }
        public int Price { get; set; }
        public int DeliveryCharges { get; set; }
        public int Total { get; set; }
        public string OrderNote { get; set; }



        public enum OrderStatus
        {
            Select,
            OrderRecieved,
            OrderAccepted,
            OrderRejected,
            OrderInProcess,
            ReadyForDelivery,
            OrderDelivered
        }
    }
}
