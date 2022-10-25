using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILibrary.HRM
{
    public class Customer
    {
        public int Id { get; set; }
        public Person Person { get; set; }
        public LoginUsers LoginUsers { get; set; }
        public string CustomerNote { get; set; }
    }
}
