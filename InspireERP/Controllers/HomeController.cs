using ILibrary;
using ILibrary.AdminSide;
using ILibrary.HRM;
using ILibrary.Production;
using ILibrary.SaleManager;
using InspireERP.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace InspireERP.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.Featured = new ProductHandler().FeaturedTENProducts();
            ViewBag.SaleProducts = new ProductHandler().SalesTENProduct();
            ViewBag.IsDiscountProducts = new ProductHandler().DiscountTenProducts();
            return View();
        }
        public ActionResult GetProduct(int? Id)
        {
            if (Id == null)
            {
                return RedirectToAction("ErrorPage", "Person");
            }
            else
            {
                ViewBag.Featured = new ProductHandler().FeaturedTENProducts();
                return PartialView("~/Views/_Shared/_ViewProduct.cshtml",new ProductHandler().GetProductbyId((int)Id));
            }
        }
        public ActionResult GetbySubCat(int? Id)
        {
            
            if (Id==null)
            {
                return RedirectToAction("ErrorPage","Person");

            }
            else
            {
                return View(new ProductHandler().ProductbySubCat((int)Id));
            }
            
        }
        public ActionResult SearchbySale()
        {
           return View("~/Views/Home/GetbySubCat.cshtml", new ProductHandler().GetSaleProduct());
        }
        public ActionResult Help()
        {
            return View();
        }
        public ActionResult Privacy()
        {
            return View();
        }
        public ActionResult SearchbyMostViewed()
        {
            return View("~/Views/Home/GetbySubCat.cshtml", new ProductHandler().GetPopularProducts());
        }
        public ActionResult CustomerDashBoard()
        {
            if (Session[WebUtil.User]!=null)
            {
                LoginUsers lu = (LoginUsers)Session[WebUtil.User];
                ViewBag.Customer = new HRHandler().GetCustomer(lu);
                ViewBag.ListofOrders = new OrderManager().GetOrdersbyUser(lu.Id);
                ViewBag.Countries = ModelHelper.selectListItem(new HRHandler().GetCountry());
                return View();
            }
            else
            {
                return RedirectToAction("Index","Person");
            }
        }
        public ActionResult GetSingleOrder(int Id)
        {
            if (Session[WebUtil.User]!=null)
            {
                LoginUsers l = (LoginUsers)Session[WebUtil.User];
                using (MyDbContext con = new MyDbContext())
                {
                  OrderMaster om= con.DbSetOrderMaster.Include("Customer.LoginUsers").Where(x => x.Id == Id).FirstOrDefault();
                    if (om.Customer.LoginUsers.Id==l.Id)
                    {
                        ViewBag.Order = new OrderManager().GetOrder(Id);
                        return View("/Views/_Shared/_OrderDetail.cshtml");
                    }
                    else
                    {
                        return RedirectToAction("ErrorPage","Person");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Person");
            }
        }
        public async Task<ActionResult> ManualSearch(string name)
        {
            using (MyDbContext con = new MyDbContext())
            {
                ViewBag.Title = "Search By: "+ name;
                return View("~/Views/Home/GetbySubCat.cshtml", await con.DbSetProduct.Include("SubCategory").Include("Images").Where(x => x.Description.Contains(name)).ToListAsync());
            }
        }

    }
}