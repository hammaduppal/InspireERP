using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILibrary.AdminSide
{
    public class BannerSlider
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string URL { get; set; }
        public string ImageURL { get; set; }
        public DateTime AddDateTime { get; set; }
        public bool IsActive { get; set; }

    }
}
