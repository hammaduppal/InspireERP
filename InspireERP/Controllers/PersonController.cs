using ILibrary;
using ILibrary.HRM;
using InspireERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ILibrary.AdminSide;
using System.Data.Entity;
using System.IO;

namespace InspireERP.Controllers
{
    public class PersonController : Controller
    {
       // string path1 = "C:\\Inetpub\\vhosts\\fanco.com.pk\\httpdocs\\Content\\assets\\img\\";

        // GET: Person
        public ActionResult Index()
        {
            ViewBag.Title = "Inspire ERP";
            if ((LoginUsers)Session[WebUtil.User] != null)
            {
                ViewBag.Title = "Welcome to Inspire WholeSale Suppliers";
                return RedirectToAction("Index", "Home");
            }
            else if ((LoginUsers)Session[WebUtil.supplier] != null)
            {
                ViewBag.Title = "Welcome Supplier";
                return RedirectToAction("Index", "Master");
            }
            else
            {
                return View();
            }
        }
        
        //If Problem occurs only change string email to string username
        public ActionResult GetLogin(string email, string password)
        {
            string thisURL = "";
            if (TempData.ContainsKey("URL"))
            {
                thisURL = TempData["URL"].ToString();
            }
            
            if ((LoginUsers)Session[WebUtil.supplier] != null || (LoginUsers)Session[WebUtil.User] != null)
            {
                ViewBag.Title = "Welcome to Fanco Group";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (email != null && password != null)
                {
                    //con.DbSetLoginUser.Where(x => x.UserName == username).Where(x => x.Password == password).FirstOrDefault() != null
                    if (new HRHandler().GetLogin(email, password) != null)
                    {
                        LoginUsers l = new HRHandler().GetLogin(email, password);
                        if (l.LoginType == LoginUsers.Logintype.user)
                        {
                            Session.Add(WebUtil.User, l);
                            if (string.IsNullOrEmpty(thisURL))
                            {
                                return RedirectToAction("Index", "Home");
                            }
                            return Redirect(thisURL);
                        }
                        else if (l.LoginType == LoginUsers.Logintype.supplier)
                        {
                            Session.Add(WebUtil.supplier, l);
                            if (string.IsNullOrEmpty(thisURL))
                            {
                                return RedirectToAction("Index", "AdminPanel");
                            }
                            return Redirect(thisURL);
                        }
                        else if (l.LoginType== LoginUsers.Logintype.master)
                        {
                            Session.Add(WebUtil.sa, l);
                            return RedirectToAction("Index","SiteOwner");
                        }
                       
                        else
                        {
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        return View("Index");
                    }
                }
                else
                {
                    return View("Index");
                }
            }
        }
        public ActionResult LogOut()
        {
            Session.Abandon();
            return RedirectToAction("Index","Home");
        }
        public string CheckCountry(string name)
        {
            using (MyDbContext con = new MyDbContext())
            {
                if (con.DbSetCountry.Where(x=>x.Name==name).FirstOrDefault()!=null)
                {
                    return "true";
                }
                else
                {
                    return "false";
                }
            }
        }
        public ActionResult AddCountry(string name)
        {
            new HRHandler().AddCountry(name);
            return RedirectToAction("Index", "SiteOwner");
        }
        public string CheckCity(ObjectModel obj)
        {
            using (MyDbContext con = new MyDbContext())
            {

                Country c = con.DbSetCountry.Where(x => x.Name == obj.DepValue).FirstOrDefault();
                if (con.DbSetCity.Where(x => x.Name == obj.Name && x.Country.Name==obj.DepValue).FirstOrDefault() != null)
                {
                    return "true";
                }
                else
                {
                    return "false";
                }
            }
        }
        public ActionResult AddCity(string countrylist, string cityName)
        {
            new HRHandler().AddCity(countrylist, cityName);

            return RedirectToAction("Index","SiteOwner");
        }
        public ActionResult RegisterUser(string firstName, string lastName, string stAddress, string country, string city, string email, string mobile, string password)
        {
            string thisURL = "";
            if (TempData.ContainsKey("URL"))
            {
                thisURL = TempData["URL"].ToString();
            }
            List<StretAddress> stretAddresses = new List<StretAddress>();
            Int64 m = Int64.Parse(mobile);
            int cId = int.Parse(city);
            int coId = int.Parse(country);
            using (MyDbContext con = new MyDbContext())
            {
                City cIty = con.DbSetCity.Where(x => x.Id == cId).FirstOrDefault();
                Country coty = con.DbSetCountry.Where(x => x.Id == coId).FirstOrDefault();
                cIty.Country = coty;
                stretAddresses.Add(new StretAddress { StAddress = stAddress, City=cIty,  AddressType = AddressType.Billing } );
                stretAddresses.Add(new StretAddress { StAddress = stAddress, City = cIty, AddressType = AddressType.Shipping });


                Customer c = new Customer();
                LoginUsers l = new LoginUsers();
                c.Person = new Person { FirstName = firstName, LastName = lastName, MobileNumber = m, Email = email, StretAddress = stretAddresses
                    };
              //  c.Person.StretAddress.City = cIty;
                //c.Person.StretAddress.City.Country = coty;
                c.LoginUsers = new LoginUsers();
                l.UserName = firstName;l.Password = password;l.Email = email; l.LoginType= LoginUsers.Logintype.user;
                c.LoginUsers = l;
                con.Entry(cIty).State = System.Data.Entity.EntityState.Unchanged;
                //con.Entry(c.Person.StretAddress.City.Country).State = System.Data.Entity.EntityState.Unchanged;
                con.DbSetCustomer.Add(c);
                con.SaveChanges();
                Session.Add(WebUtil.User, l);
                if (string.IsNullOrEmpty(thisURL))
                {
                    return RedirectToAction("Index", "Home");
                }
                return Redirect(thisURL);
            }
           
        }
        [HttpGet]
        public ActionResult ResetPassword(int code)
        {
            using (MyDbContext con = new MyDbContext())
            {
                ResetPass resetPass = con.DbSetResetPass.Where(x => x.HashCode == code).Where(x => x.IsActive == true).FirstOrDefault();
                if (resetPass==null)
                {
                    return RedirectToAction("ErrorPage", "Person");
                }
                else
                {
                    ViewBag.ResetPass = resetPass;
                    return View();
                }
            }
        }
        [HttpPost]
        public ActionResult ResetPassword(string hashCode, string email, string password, string password2)
        {
            using (MyDbContext con = new MyDbContext())
            {
                int p = int.Parse(hashCode);
                if (con.DbSetResetPass.Where(x => x.HashCode == p) != null && con.DbSetResetPass.Where(x => x.Email == email) != null)
                {
                    ResetPass rp = con.DbSetResetPass.Where(x => x.HashCode == p).Where(x => x.Email == email).FirstOrDefault();
                    rp.IsActive = false;
                    con.DbSetResetPass.Remove(rp);
                    con.SaveChanges();
                    LoginUsers lu = con.DbSetLoginUser.Where(x => x.Email == email).FirstOrDefault();
                    lu.Password = password;
                    con.DbSetLoginUser.Add(lu);
                    con.Entry(lu).State = EntityState.Modified;
                    con.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }
        public string ResetRequest(string ForGetEmail)
        {
            using (MyDbContext con = new MyDbContext())
            {
                LoginUsers l = con.DbSetLoginUser.Where(x => x.Email == ForGetEmail).FirstOrDefault();
                if (l != null)
                {
                    new HRHandler().SendEmail(ForGetEmail, "Hello User");

                    return ("true");
                }
                else
                {
                    return ("false");
                }
            }
        }
        public ActionResult ErrorPage()
        {
           
            return View();
        }
        public ActionResult UpdateCustomer(string id, string firstName, string lastName, string email, string mobile, string password)
        {
            int cId = int.Parse(id);
            long m = long.Parse(mobile);
            if (Session[WebUtil.User] != null)
            {
                LoginUsers lu = (LoginUsers)Session[WebUtil.User];
                Customer cc = new HRHandler().GetCustomer(lu);

                if (cc.Id==cId)
                {
                    using (MyDbContext con = new MyDbContext())
                    {
                        Customer c = con.DbSetCustomer.Include("LoginUsers").Include("Person").Where(x => x.Id == cId).FirstOrDefault();
                        c.Person.FirstName = firstName;
                        c.Person.LastName = lastName;
                        c.Person.MobileNumber = m;
                        c.Person.Email = email;
                        c.LoginUsers.Password = password;
                        con.SaveChanges();
                        return RedirectToAction("CustomerDashBoard", "Home");
                    }
                }
                else
                {
                    return RedirectToAction("ErrorPage","Person");
                }
             
            }
            else
            {
                return RedirectToAction("Index", "Person");
            }
        }
        public ActionResult GetAddressShipping()
        {
            if (Session[WebUtil.User]!=null)
            {
                LoginUsers lu = (LoginUsers)Session[WebUtil.User];
                Customer cc = new HRHandler().GetCustomer(lu);
                ViewBag.Address = cc.Person.StretAddress.Where(x => x.AddressType == AddressType.Shipping).FirstOrDefault();
                ViewBag.City = ModelHelper.selectListItem(new HRHandler().GetCity());
                ViewBag.Country = ModelHelper.selectListItem(new HRHandler().GetCountry());

                return PartialView("~/Views/_Shared/_ShippingAddress.cshtml");

            }
            else
            {
                return RedirectToAction("Index","Person");
            }
        }
        public ActionResult UpdateShippingAddress(string Id, string stAddress,int cityName, int countryName)
        {
            int SId = int.Parse(Id);
            if (Session[WebUtil.User] != null)
            {
                LoginUsers lu = (LoginUsers)Session[WebUtil.User];
                Customer cc = new HRHandler().GetCustomer(lu);
                using (MyDbContext con = new MyDbContext())
                {
                    City c = con.DbSetCity.Where(x => x.Id == cityName).FirstOrDefault();
                    Country co = con.DbSetCountry.Where(x => x.Id == countryName).FirstOrDefault();

                    StretAddress stretAddress = con.DbSetStreetAddresses.Where(x => x.Id == SId).FirstOrDefault();
                    stretAddress.City = c;
                    stretAddress.City.Country = co;
                    stretAddress.StAddress = stAddress;
                    con.Entry(stretAddress.City).State = System.Data.Entity.EntityState.Unchanged;
                    con.Entry(stretAddress.City.Country).State = System.Data.Entity.EntityState.Unchanged;
                    con.SaveChanges();
                }
              //ViewBag.Address = cc.Person.StretAddress.Where(x => x.AddressType == AddressType.Shipping).FirstOrDefault();
                return RedirectToAction("CustomerDashBoard","Home");

            }
            else
            {
                return RedirectToAction("Index", "Person");
            }
        }
        public ActionResult GetAddressBilling()
        {
            if (Session[WebUtil.User] != null)
            {
                LoginUsers lu = (LoginUsers)Session[WebUtil.User];
                Customer cc = new HRHandler().GetCustomer(lu);
                ViewBag.Address = cc.Person.StretAddress.Where(x => x.AddressType == AddressType.Billing).FirstOrDefault();
                ViewBag.City = ModelHelper.selectListItem(new HRHandler().GetCity());
                ViewBag.Country = ModelHelper.selectListItem(new HRHandler().GetCountry());

                return PartialView("~/Views/_Shared/_BillingAddress.cshtml");

            }
            else
            {
                return RedirectToAction("Index", "Person");
            }
        }
        public ActionResult UpdateBillingAddress(string Id, string stAddress, int cityName, int countryName)
        {
            int SId = int.Parse(Id);
            if (Session[WebUtil.User] != null)
            {
                LoginUsers lu = (LoginUsers)Session[WebUtil.User];
                Customer cc = new HRHandler().GetCustomer(lu);
                using (MyDbContext con = new MyDbContext())
                {
                    City c = con.DbSetCity.Where(x => x.Id == cityName).FirstOrDefault();
                    Country co = con.DbSetCountry.Where(x => x.Id == countryName).FirstOrDefault();

                    StretAddress stretAddress = con.DbSetStreetAddresses.Where(x => x.Id == SId).FirstOrDefault();
                    stretAddress.City = c;
                    stretAddress.City.Country = co;
                    stretAddress.StAddress = stAddress;
                    con.Entry(stretAddress.City).State = System.Data.Entity.EntityState.Unchanged;
                    con.Entry(stretAddress.City.Country).State = System.Data.Entity.EntityState.Unchanged;

                    con.SaveChanges();

                }
               // ViewBag.Address = cc.Person.StretAddress.Where(x => x.AddressType == AddressType.Shipping).FirstOrDefault();
                return RedirectToAction("CustomerDashBoard", "Home");

            }
            else
            {
                return RedirectToAction("Index", "Person");
            }
        }
        public ActionResult Dummy()
        {
         
            return View();
        }
        public string CheckEmail(string email)
        {
            using (MyDbContext con = new MyDbContext())
            {
                LoginUsers l = con.DbSetLoginUser.Where(x => x.Email == email).FirstOrDefault();
                if (l==null)
                {
                    return "false";
                }
                else
                {
                    return "true";
                }
            }
        }
        public string CheckMobile(string mobile)
        {
            long m = long.Parse(mobile);
            using (MyDbContext con = new MyDbContext())
            {
                Customer l = con.DbSetCustomer.Where(x => x.Person.MobileNumber == m).FirstOrDefault();
                if (l == null)
                {
                    return "false";
                }
                else
                {
                    return "true";
                }
            }
        }
        [HttpGet]
        public ActionResult SupplierSignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SupplierSignUp(Supplier supplier, string address1,string address2, string country, string city)
        {
            int count = 0;
            foreach (string item in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[count];
                string filename = file.FileName;
                int l = filename.IndexOf('.');
                ++l;
                string b = filename.Substring(l);
                if (b == "jpg")
                {
                    if (!string.IsNullOrEmpty(file.FileName))
                    {
                        ++count;
                        string url = "/Content/assets/img/appimages/" + DateTime.Now.GetHashCode().ToString() + count + file.FileName.Substring(file.FileName.LastIndexOf('.'));
                        //if (!Directory.Exists(path1))
                        //{
                        //    Directory.CreateDirectory(path1);
                        //}
                        if (item=="logo")
                        {
                            supplier.NTNPicture = url;
                            string path = Server.MapPath(url);
                            file.SaveAs(path);


                        }
                        else if (item=="ntn")
                        {
                            supplier.CompanyLogo = url;
                            string path = Server.MapPath(url);
                            file.SaveAs(path);
                        }
                    }
                }
            }
            using (MyDbContext con = new MyDbContext())
            {
                City c = con.DbSetCity.Include("Country").Where(x => x.Name == city).FirstOrDefault();
                //Country co = con.DbSetCountry.Where(x => x.Name == country).FirstOrDefault();

                Supplier s = new Supplier();
                SupplierConfig sc = new SupplierConfig();
                s.CompanyName = supplier.CompanyName;
                s.NTN = supplier.NTN;
                s.DateEdited = DateTime.Now;
                s.DateCreated = DateTime.Now;
                s.SCode = $"{supplier.CompanyLogo.Substring(0, 3)}-{DateTime.Now.GetHashCode().ToString()}";
                s.Person = new Person { Email = supplier.Person.Email, FirstName = supplier.Person.FirstName, LastName = supplier.Person.LastName,
                    MobileNumber = supplier.Person.MobileNumber };
                List<StretAddress> address = new List<StretAddress>();
                address.Add(  new StretAddress {  StAddress= address1, AddressType= AddressType.Billing, City=c});
                address.Add(new StretAddress { StAddress = address2, AddressType = AddressType.Shipping, City = c });
                s.Person.StretAddress = address;
                s.Person.FirstName = supplier.Person.FirstName;
                s.Person.LastName = supplier.Person.LastName;
                s.Person.Email = supplier.Person.Email;
                s.LoginUser = new LoginUsers { Email = supplier.Person.Email, 
                    UserName = supplier.LoginUser.UserName, Password = supplier.LoginUser.Password, LoginType = LoginUsers.Logintype.supplier };
                sc.SupplierCreatedOn = DateTime.Now;
                sc.NewfileTime = DateTime.Now;
                sc.Supplier = s;
                
                con.Entry(c).State = EntityState.Unchanged;
                con.DbSetSupplier.Add(s);
                con.DbSetSupplierConfig.Add(sc);
                con.SaveChanges();






                return View();

            }
        }

    }
}