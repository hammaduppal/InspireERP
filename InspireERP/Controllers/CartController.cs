using ILibrary.Production;
using InspireERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ILibrary.HRM;
using ILibrary.SaleManager;
using ILibrary;
using System.Threading.Tasks;
using System.Data.Entity;

namespace InspireERP.Controllers
{
    public class CartController : Controller
    {
        public async Task< ActionResult> GetCustomerInfo(int Id)
        {
            using (MyDbContext con = new MyDbContext())
            {
                ViewBag.OrderMaster = await con.DbSetOrderMaster.Include("Customer").Include("Customer.Person.StretAddress").Include("Customer.Person.StretAddress.City.Country").Include("Customer.Person.StretAddress.City").Include("Product").Where(x => x.Id == Id).FirstOrDefaultAsync();
                return PartialView("~/Views/_Shared/_PrintData.cshtml");
            }
            
        }
        public ActionResult AddtoCart(string data)
        {
            List<AddtoCartModel> cartModel;
            List<AddtoCartModel> cart = (List<AddtoCartModel>)Session[WebUtil.cartitem];
            int amount = 0;
            string[] vs = data.Split('|');
            int itemId = int.Parse(vs[1]);
            int Qty = int.Parse(vs[0]);
            Product p = new ProductHandler().GetProductbyId(itemId);
            if (p.IsSale == true)
            {
                amount = p.SalePrice;
            }
            else if (p.IsDiscount == true)
            {
                amount = p.DiscountPrice;
            }
            else if (p.IsSale == false && p.IsDiscount == false)
            {
                amount = p.RetailPrice;
            }
            int total = amount * Qty;
            if (Session[WebUtil.cartitem] == null)
            {
                cartModel = new List<AddtoCartModel>();
                cartModel.Add(new AddtoCartModel { PId = itemId, DeliveryCharges=p.DeliveryCharges, Price = amount, Qty = Qty, Total = total, Name = p.Name, URL = p.Images.First().URL });
                // Session[WebUtil.cartitem] = cartModel;
                Session.Add(WebUtil.cartitem, cartModel);
            }
            else
            {
                if (cart.Find(x => x.PId == itemId) != null)
                {
                    AddtoCartModel model = cart.Where(x => x.PId == itemId).FirstOrDefault();
                    model.Qty = model.Qty + Qty;
                    model.Total = model.Qty * model.Price;
                    //cart.Add(model);
                    // Session.Add(WebUtil.cartitem, cart);
                }
                else
                {
                    AddtoCartModel cm = new AddtoCartModel();
                    cm.Name = p.Name;
                    cm.PId = itemId;
                    cm.Price = amount;
                    cm.DeliveryCharges = p.DeliveryCharges;
                    cm.Qty = Qty;
                    cm.URL = p.Images.First().URL;
                    cm.Total = cm.Price * cm.Qty;
                    cart.Add(cm);

                }
            }
            return PartialView("~/Views/_Shared/_CartCounter.cshtml");
            //return Redirect($"localhost:53317/Home/GetProduct/{itemId}");
        }

        public ActionResult GotoCart()
        {
            ViewBag.subCat = new ProductHandler().subCategories();
            ViewBag.Cat = new ProductHandler().CategoriesAsync();
            ViewBag.Department = new ProductHandler().DepartmentAsync();
            ViewBag.CartItem = (List<AddtoCartModel>)Session[WebUtil.cartitem];
            return View();
        }
        public ActionResult RemoveItem(int Id)
        {
            List<AddtoCartModel> cartModels = (List<AddtoCartModel>)Session[WebUtil.cartitem];
            AddtoCartModel cm = cartModels.Where(x => x.PId == Id).FirstOrDefault();
            cartModels.Remove(cm);
            return RedirectToAction("GotoCart","Cart");
        }
        public ActionResult CheckOut()
        {
            
            ViewBag.Countries = ModelHelper.selectListItem(new HRHandler().GetCountry());
            ViewBag.subCat = new ProductHandler().subCategories();
            ViewBag.Cat = new ProductHandler().CategoriesAsync();
            ViewBag.Department = new ProductHandler().DepartmentAsync();
            if (Session[WebUtil.cartitem]!=null&&((List<AddtoCartModel>)Session[WebUtil.cartitem]).Count!=0)
            {
                if (Session[WebUtil.User] != null)
                {
                    LoginUsers l = (LoginUsers)Session[WebUtil.User];
                    ViewBag.Customer = new HRHandler().GetCustomer(l.UserName, l.Password);
                    ViewBag.CartItems=(List<AddtoCartModel>)Session[WebUtil.cartitem];
                    return View();
                }
                else
                {
                    string url = Url.RequestContext.HttpContext.Request.RawUrl;
                    TempData["URL"]= url;
                    return RedirectToAction("UserLogin");
                }
            }
            else
            {
                return RedirectToAction("GotoCart");
            }
         
           
        }
        public ActionResult ConfirmOrder(string name)
        {
            int incdelivery = 0;
            int gTotal = 0;
            List<OrderDetails> orderDetails = new List<OrderDetails>();
            List<AddtoCartModel> cartModel = (List<AddtoCartModel>)Session[WebUtil.cartitem];
            LoginUsers lu =(LoginUsers) Session[WebUtil.User];
            Customer customer = new HRHandler().GetCustomer(lu);
            Product p;
            using (MyDbContext con = new MyDbContext())
            {
                foreach (var item in cartModel)
                {
                    OrderMaster om = new OrderMaster();
                    om.Customer = customer;
                    p = con.DbSetProduct.Where(x => x.Id == item.PId).FirstOrDefault();
                    incdelivery = item.Total+ p.DeliveryCharges;
                    om.DeliveryCharges = p.DeliveryCharges;
                    om.OrderDateTime = DateTime.Now;
                    om.Status = OrderMaster.OrderStatus.OrderRecieved;
                    om.Price = item.Price;
                    om.Total = incdelivery;
                    gTotal = gTotal + incdelivery;
                    om.GrandTotal = gTotal;
                    om.Product = p;
                    om.Qty = item.Qty;
                    om.OrderNote = name;
                    con.Entry(om.Customer).State = System.Data.Entity.EntityState.Unchanged;
                    con.Entry(om.Product).State = System.Data.Entity.EntityState.Unchanged;

                    con.DbSetOrderMaster.Add(om);
                    p.LastSold = DateTime.Now;
                    con.SaveChanges();
                }
                cartModel.Clear();
            }
            return RedirectToAction("ThankYouPage","Cart");
        }
        public ActionResult ThankYouPage()
        {
            ViewBag.subCat = new ProductHandler().subCategories();
            ViewBag.Cat = new ProductHandler().CategoriesAsync();
            ViewBag.Department = new ProductHandler().DepartmentAsync();
            return View();
        }

        public ActionResult ChangeOrderStatus(string data)
        {
            using (MyDbContext con = new MyDbContext())
            {
                string[] vs = data.Split('|');
                int orderId = int.Parse(vs[1]);
                int statusId = int.Parse(vs[0]);
                OrderMaster om = con.DbSetOrderMaster.Where(x => x.Id == orderId).FirstOrDefault();
                om.Status = (OrderMaster.OrderStatus)statusId;
                con.SaveChanges();
                return View();
            }
        }

        public ActionResult UserLogin()
        {
            //int Id = 1;
            ViewBag.subCat = new ProductHandler().subCategories();
            ViewBag.Cat = new ProductHandler().CategoriesAsync();
            ViewBag.Countries = ModelHelper.selectListItem(new HRHandler().GetCountry());
            ViewBag.cities = ModelHelper.selectListItem(new HRHandler().GetCity());
            ViewBag.Department = new ProductHandler().DepartmentAsync();
            return View();
        }
    }
}