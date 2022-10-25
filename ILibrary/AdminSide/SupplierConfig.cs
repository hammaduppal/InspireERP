using ILibrary.HRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILibrary.AdminSide
{
    public class SupplierConfig
    {
        public int Id { get; set; }
        public DateTime SupplierCreatedOn { get; set; }
        public string BulkUploadFilePath { get; set; }
        public DateTime NewfileTime { get; set; }
        public Supplier  Supplier { get; set; }







    }
}
