using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILibrary.HRM
{
    public class ResetPass
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public int HashCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime RequestedTime { get; set; }

    }
}
