using ILibrary;
using ILibrary.AdminSide;
using ILibrary.HRM;
using ILibrary.Production;
using InspireERP.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InspireERP.Controllers
{
    public class SiteOwnerController : Controller
    {
        // GET: SiteOwner
          string path1 = "C:\\Inetpub\\vhosts\\fanco.com.pk\\httpdocs\\Content\\assets\\img\\";
        //string path1 = WebUtil.FilesPath;
        public ActionResult Index()
        {

            if (Session[WebUtil.sa] != null)
            {
                ViewBag.GetActiveBanners = new AdminHelper().GetAllBanners();
                ViewBag.GetDepartments = new ProductHandler().DepartmentAsync();
                ViewBag.country = ModelHelper.selectListItem(new HRHandler().GetCountry());
                return View();

            }
            else
            {
                string url = Url.RequestContext.HttpContext.Request.RawUrl;
                ViewData.Add("URL", url);
                return RedirectToAction("Index", "Person");
            }
        }

        public ActionResult AddBanner(string name, string url)
        {
            //Get Images Based on JPG Only
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
                        string Imageurl = "/Content/assets/img/banners/" + DateTime.Now.GetHashCode().ToString() + count + file.FileName.Substring(file.FileName.LastIndexOf('.'));
                       // product.Images.Add(new Images { URL = url, Caption = "Product-" + product.Name });
                        string path = Server.MapPath(Imageurl);
                        path1 = Path.Combine(path1, "banners");
                        if (!Directory.Exists(path1))
                        {
                            Directory.CreateDirectory(path1);
                        }
                        file.SaveAs(path);
                        BannerSlider banner = new BannerSlider();
                        banner.AddDateTime = DateTime.Now;
                        banner.Text = name;
                        banner.IsActive = true;
                        banner.ImageURL = Imageurl;
                        banner.URL = url;
                        new AdminHelper().AddBanner(banner);
                    }
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult DisableBanner(int Id)
        {
            new AdminHelper().DisableBanner(Id);
            return View();
        }

        public ActionResult EnableBanner(int Id)
        {
            new AdminHelper().EnableBanner(Id);
            return View();
        }
        public ActionResult TabbedName(string tab1,string tab2,string tab3,string tab4)
        {
            new AdminHelper().AddTags(tab1,tab2,tab3,tab4);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult UpdateConfiguration(string address,string city,string country,string email,string firstName,string lastName, string mobile,string note,string skype,string facebook,string twitter, string website)
        {
            string ImageUrl="" ;
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
                        string Imageurl = "/Content/assets/img/appimages/" + DateTime.Now.GetHashCode().ToString() + count + file.FileName.Substring(file.FileName.LastIndexOf('.'));
                        // product.Images.Add(new Images { URL = url, Caption = "Product-" + product.Name });
                        string path = Server.MapPath(Imageurl);
                        path1 = Path.Combine(path1, "appimages");
                        if (!Directory.Exists(path1))
                        {
                            Directory.CreateDirectory(path1);
                        }
                        file.SaveAs(path);
                        ImageUrl = Imageurl;
                    }
                }
            }
            using (MyDbContext con = new MyDbContext())
            {

                SiteConfiguration s = con.DbSetSiteConfiguration.First();
                s.CompanyAddress = address;
                s.CompanyCity = city;
                s.CompanyCountry = country;
                s.CompanyEmail = email;
                s.CompanyFirstName = firstName;
                s.CompanyLastName = lastName;
                s.CompanyMobile = mobile;
                s.CompanyNote = note;s.CompanySkype = skype;
                s.FaceBook = facebook;
                s.Twitter = twitter;
                s.WebSite = website;
                s.CompanyLogo = ImageUrl;
                con.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult AddPromo(string addName, string addDesc, string addURL, int addNumber)
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
                        string Imageurl = "/Content/assets/img/banners/" + DateTime.Now.GetHashCode().ToString() + count + file.FileName.Substring(file.FileName.LastIndexOf('.'));
                        string path = Server.MapPath(Imageurl);
                        path1 = Path.Combine(path1, "banners");
                        if (!Directory.Exists(path1))
                        {
                            Directory.CreateDirectory(path1);
                        }
                        file.SaveAs(path);
                        PaidPromotions promotions = new PaidPromotions();
                        promotions.DateCreated = DateTime.Now;
                        promotions.Description = addDesc;
                        promotions.AddVavlue = addNumber;
                        promotions.Name = addName;
                        promotions.IsActive = true;
                        promotions.ImageUrl = Imageurl;
                        promotions.Url = addURL;
                        new AdminHelper().AddPromotion(promotions);
                    }
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult DisablePromo(int Id)
        {
            new AdminHelper().DisablePromo(Id);
            return View();
        }
        public ActionResult EnablePromo(int Id)
        {
            new AdminHelper().EnablePromo(Id);
            return View();
        }
    }
}