using ILibrary;
using ILibrary.AdminSide;
using ILibrary.HRM;
using ILibrary.Production;
using ILibrary.SaleManager;
using InspireERP.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InspireERP.Controllers
{
   
    public class AdminPanelController : Controller
    {
        // GET: AdminPanel
        string filePath = WebUtil.FilesPath;
        public ActionResult Index()
        {
            if (Session[WebUtil.supplier]!=null)
            {
                    LoginUsers lu = (LoginUsers)Session[WebUtil.supplier];
                    int sId = new HRHandler().CheckSupplier(lu.Id);
                    ViewBag.OrderCount = new OrderManager().GetOrderCount(sId);
                    ViewBag.ListofProduct = new ProductHandler().GetAllActiveProducts(sId);
                    ViewBag.ProductCount = new ProductHandler().ProductCount();
                    ViewBag.YourProducts = new ProductHandler().ProductCount(sId);

                ViewBag.TotalCustomers = new ProductHandler().CustomerCount();
                    return View();
            }
            else
            {
                TempData["URL"] = Url.RequestContext.HttpContext.Request.RawUrl;
                return RedirectToAction("Index","Person");
            }
        }
        //Needed to Move
        public ActionResult AddAttrb()
        {
          
                if (Session[WebUtil.sa]!=null)
                {
                ViewBag.country = ModelHelper.selectListItem(new HRHandler().GetCountry());
                ViewBag.department = ModelHelper.selectListItem(new ProductHandler().DepartmentAsync());
                ViewBag.category= ModelHelper.selectListItem(new ProductHandler().CategoriesAsync(0));
                return View();
                }
                else
                {
                TempData["URL"] = Url.RequestContext.HttpContext.Request.RawUrl;
                return RedirectToAction("Index", "Person");
                }
        }
        public ActionResult BulkUpload()
        {
            return View();
        }
        public ActionResult ViewOrders()
        {
            if (Session[WebUtil.supplier]!=null)
            {
                LoginUsers lu = (LoginUsers)Session[WebUtil.supplier];
                int sId = new HRHandler().CheckSupplier(lu.Id);
                ViewBag.OrdersSupplier = new OrderManager().GetOrderbySupplier(sId);
                return View();
            }
            else
            {
                TempData["URL"] = Url.RequestContext.HttpContext.Request.RawUrl;
                return RedirectToAction("Index", "Person");
            }
            
        }
       public ActionResult UploadFile()
        {
            int count = 0;
            if (Session[WebUtil.supplier] != null)
            {
                using (MyDbContext con = new MyDbContext())
                {
                    LoginUsers lu = (LoginUsers)Session[WebUtil.supplier];
                    int sId = new HRHandler().CheckSupplier(lu.Id);
                    SupplierConfig sc = con.DbSetSupplierConfig.Where(x => x.Supplier.Id == sId).FirstOrDefault();
                    foreach (string item in Request.Files)
                    {
                        HttpPostedFileBase file = Request.Files[count];
                        string filename = file.FileName;
                        int l = filename.IndexOf('.');
                        ++l;
                        string b = filename.Substring(l);
                        if (b == "xlsx")
                        {
                            if (!string.IsNullOrEmpty(file.FileName))
                            {
                                ++count;
                                string Imageurl = "/Content/assets/Files/" + DateTime.Now.GetHashCode().ToString() + count + file.FileName.Substring(file.FileName.LastIndexOf('.'));
                                string path = Server.MapPath(Imageurl);
                                if (!Directory.Exists(filePath))
                                {
                                    Directory.CreateDirectory(filePath);
                                }
                                file.SaveAs(path);
                                sc.NewfileTime= DateTime.Now;
                                sc.BulkUploadFilePath = $"{path}";
                                con.SaveChanges();
                            }
                        }
                    }
                }

                return RedirectToAction("BulkUpload");

            }
            else
            {
                TempData["URL"] = Url.RequestContext.HttpContext.Request.RawUrl;
                return RedirectToAction("Index", "Person");
            }
          
        
        }
    }
}