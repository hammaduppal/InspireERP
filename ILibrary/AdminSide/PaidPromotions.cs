using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILibrary.AdminSide
{
    public class PaidPromotions
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public DateTime DateCreated { get; set; }
        public  bool IsActive { get; set; }
        public int AddVavlue { get; set; }

    }
}
