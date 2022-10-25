using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILibrary.HRM
{
    public class Supplier
    {
        public int Id { get; set; }
        public string SCode { get; set; }
        public string NTN { get; set; }
        public string NTNPicture { get; set; }
        public string CompanyLogo { get; set; }
        public string CompanyName { get; set; }
        public Person Person { get; set; }
        public LoginUsers LoginUser { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateEdited { get; set; }
    }
}
