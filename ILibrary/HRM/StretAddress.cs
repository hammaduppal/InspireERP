using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILibrary.HRM
{
    public class StretAddress
    {
        public int Id { get; set; }
        public string StAddress { get; set; }
        public City City { get; set; }
        public AddressType AddressType { get; set; }
    }
    public enum AddressType
    {
        Shipping,
        Billing
    }
}
