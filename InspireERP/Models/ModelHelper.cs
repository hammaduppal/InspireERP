using ILibrary.Production;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InspireERP.Models
{
    public class ModelHelper
    {
        public static List<SelectListItem> selectListItem(dynamic value)
        {
            List<SelectListItem> ls = new List<SelectListItem>();
            foreach (var item in value)
            {
                ls.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            }
            return ls;
        }
    }
}