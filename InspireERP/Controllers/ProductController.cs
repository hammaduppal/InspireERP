using ILibrary.Production;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ILibrary.HRM;
using ILibrary;
using System.Drawing;
using Microsoft.Office.Interop.Excel;
using InspireERP.Models;
using Color = ILibrary.Production.Color;
using System.IO;
using System.Threading.Tasks;
using System.Data.Entity;
using ILibrary.AdminSide;
using OfficeOpenXml;
using Quartz.Util;

namespace InspireERP.Controllers
{
   
    public class ProductController : Controller
    {
        // GET: Product
        string path1 = WebUtil.BusinessPath;
        public async Task<ActionResult> GetInventory()
        {
            if ((LoginUsers)Session[WebUtil.supplier] != null)
            {
                using (MyDbContext con = new MyDbContext())
                {
                    ViewBag.Title = "Inventory Page";
                    LoginUsers loginUsers = (LoginUsers)Session[WebUtil.supplier];
                    Supplier s = new MyDbContext().DbSetSupplier.Where(x => x.LoginUser.UserName == loginUsers.UserName).FirstOrDefault();
                    ViewBag.ListofProduct = await con.DbSetProduct.Include("Color").Include("SubCategory").Include("SubCategory.Category").Where( x=>x.Supplier.Id==s.Id && x.IsActive==true).ToListAsync();
                    return PartialView("~/Views/Product/GetInventory.cshtml");
                }
            }
            else
            {
                TempData["URL"] = Url.RequestContext.HttpContext.Request.RawUrl;
                return RedirectToAction("Index", "Person");
            }
        }
        public async Task<ActionResult> GetInActive()
        {
            if ((LoginUsers)Session[WebUtil.supplier] != null)
            {
                using (MyDbContext con = new MyDbContext())
                {
                    ViewBag.Title = "Inventory Page";
                    LoginUsers loginUsers = (LoginUsers)Session[WebUtil.supplier];
                    Supplier s = new MyDbContext().DbSetSupplier.Where(x => x.LoginUser.UserName == loginUsers.UserName).FirstOrDefault();
                    ViewBag.ListofProduct = await con.DbSetProduct.Include("Color").Include("SubCategory").Include("SubCategory.Category").Where(x => x.Supplier.Id == s.Id && x.IsActive == false).ToListAsync();
                    return PartialView("~/Views/Product/GetInActiveProducts.cshtml");
                }
            }
            else
            {
                TempData["URL"] = Url.RequestContext.HttpContext.Request.RawUrl;
                return RedirectToAction("Index", "Person");
            }
        }
    
        public async Task< ActionResult> GetProduct(int Id)
        {
            using (MyDbContext con = new MyDbContext())
            {
                LoginUsers loginUsers = (LoginUsers)Session[WebUtil.supplier];
                int sId = con.DbSetSupplier.Where(x => x.LoginUser.Id == loginUsers.Id).FirstOrDefault().Id;
                var Product = await con.DbSetProduct.Include("Size").Include("Material").Include("Color").Include("Images").Include("SubCategory")
                   .Include("SubCategory.Category").Include("SubCategory.Category.Department").Where(x => x.Id == Id && x.Supplier.Id==sId ).FirstOrDefaultAsync();
                return PartialView("~/Views/_Shared/_VSP.cshtml", Product);
            }
        }
        public async Task<ActionResult> RemoveProduct(int Id)
        {
            if ((LoginUsers)Session[WebUtil.supplier] != null)
            {

                using (MyDbContext con = new MyDbContext())
                {
                    LoginUsers loginUsers = (LoginUsers)Session[WebUtil.supplier];
                    int sId = con.DbSetSupplier.Where(x => x.LoginUser.Id == loginUsers.Id).FirstOrDefault().Id;
                    Product p = await con.DbSetProduct.Where(x => x.Id == Id && x.Supplier.Id == sId).FirstOrDefaultAsync();
                    p.IsActive = false;
                    p.LastModified = DateTime.Now;
                    con.SaveChanges();
                    return RedirectToAction("GetInventory", "Product");

                }
            }
            else
            {
                TempData["URL"] = Url.RequestContext.HttpContext.Request.RawUrl;
                return RedirectToAction("Index", "Person");
            }
        }
        public async Task<ActionResult> ActivateProduct(int Id)
        {
            if ((LoginUsers)Session[WebUtil.supplier] != null)
            {

                using (MyDbContext con = new MyDbContext())
                {
                    LoginUsers loginUsers = (LoginUsers)Session[WebUtil.supplier];
                    int sId = con.DbSetSupplier.Where(x => x.LoginUser.Id == loginUsers.Id).FirstOrDefault().Id;
                    Product p = await con.DbSetProduct.Where(x => x.Id == Id && x.Supplier.Id == sId).FirstOrDefaultAsync();
                    p.IsActive = true;
                    p.LastModified = DateTime.Now;
                    con.SaveChanges();
                    return RedirectToAction("GetInventory", "Product");
                }
            }
            else
            {
                TempData["URL"] = Url.RequestContext.HttpContext.Request.RawUrl;
                return RedirectToAction("Index", "Person");
            }
        }
        [HttpGet]
        public ActionResult EditProduct(int Id)
        {
            return PartialView("~/Views/_Shared/_EditProduct.cshtml", new ProductHandler().GetProductbyId(Id));
        }
        [HttpPost]
        public ActionResult EditProduct(Product p)
        {
            using (MyDbContext con = new MyDbContext())
            {
                LoginUsers loginUsers = (LoginUsers)Session[WebUtil.supplier];
                Supplier s = con.DbSetSupplier.Include("LoginUser").Where(x => x.LoginUser.UserName == loginUsers.UserName).FirstOrDefault();
                if (s.LoginUser.UserName == loginUsers.UserName)
                {
                    new ProductHandler().EditProduct(p);
                    return RedirectToAction("GetInventory", "Product");
                }
                else
                {
                    return RedirectToAction("ErrorPage", "Master");
                }
            }
        }
        public ActionResult ChangeImages(Product product)
        {
            using (MyDbContext con = new MyDbContext())
            {
                List<Images> images = new List<Images>();
                // List<Images> images = con.DBSetImages.Where(x => x.Id == product.Id).ToList();
                Product toremove = con.DbSetProduct.Include("Images").Where(x => x.Id == product.Id).FirstOrDefault();
                con.DbSetImages.RemoveRange(toremove.Images);
                con.SaveChanges();
                Product p = con.DbSetProduct.Where(x => x.Id == product.Id).FirstOrDefault();
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
                            string url = "/Content/assets/img/pimages/" + DateTime.Now.GetHashCode().ToString() + count + file.FileName.Substring(file.FileName.LastIndexOf('.'));
                            images.Add(new Images { URL = url, Caption = "Product-" + product.Name });
                            string path = Server.MapPath(url);
                            string path2 = Path.Combine(path1, "pimages");
                            if (!Directory.Exists(path2))
                            {
                                Directory.CreateDirectory(path2);
                            }
                            file.SaveAs(path);
                        }
                    }
                }
                p.Images = images;
                con.SaveChanges();
                return RedirectToAction("GetInventory", "Product");
            }
        }
        [HttpGet]
        public ActionResult AddProduct()
        {
            if ((LoginUsers)Session[WebUtil.supplier] != null)
            {
                // LoginUsers loginUsers = (LoginUsers)Session[WebUtil.supplier];
                //int Id = loginUsers.Id;
                //Supplier s = new MyDbContext().DbSetSupplier.Where(x => x.LoginUser.UserName == loginUsers.UserName).FirstOrDefault();
                //ViewBag.ListofProduct = new ProductHandler().GetAllActiveProducts(s.Id);
                ViewBag.ListofDepartment = ModelHelper.selectListItem(new ProductHandler().DepartmentAsync());
                ViewBag.ListofColors = ModelHelper.selectListItem(new ProductHandler().ColorAsync());
                ViewBag.CategorybyID = ModelHelper.selectListItem(new ProductHandler().CategoriesAsync(0));
                ViewBag.brands = ModelHelper.selectListItem(new ProductHandler().Brand());
                ViewBag.subCategorybyID = ModelHelper.selectListItem(new ProductHandler().subCategories(0));
                ViewBag.material = ModelHelper.selectListItem(new ProductHandler().Materials());
                ViewBag.sizes = ModelHelper.selectListItem(new ProductHandler().Size());
                ViewBag.Title = "Add Product";
                return PartialView("~/Views/_Shared/_AddSP.cshtml");
            }
            else
            {
                string url = Url.RequestContext.HttpContext.Request.RawUrl;
                TempData["URL"] = url;
                return RedirectToAction("Index", "Person");
            }
        }
        [HttpPost]
        public ActionResult AddProduct(Product product)
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
                        string url = "/Content/assets/img/pimages/" + DateTime.Now.GetHashCode().ToString() + count + file.FileName.Substring(file.FileName.LastIndexOf('.'));
                        product.Images.Add(new Images { URL = url, Caption = "Product-" + product.Name });
                        string path = Server.MapPath(url);
                        string path2 = Path.Combine(path1, "pimages");
                        if (!Directory.Exists(path1))
                        {
                            Directory.CreateDirectory(path1);
                        }

                        file.SaveAs(path);
                    }
                }
            }
            //Get Id of Associated Classes
            int departmentId = int.Parse(product.SubCategory.Category.Department.Name);
            int categoryId = int.Parse(product.SubCategory.Category.Name);
            int subcatId = int.Parse(product.SubCategory.Name);
            int colorId = int.Parse(product.Color.Name);
            int brandId = int.Parse(product.Brand.Name);
            int materialId = int.Parse(product.Material.Name);

            LoginUsers lu = (LoginUsers)Session[WebUtil.supplier];
            if (product.Name != null && product.Description != null)
            {
                using (MyDbContext con = new MyDbContext())
                {
                    //Getting Classes by ID
                    var department = con.DbSetDepartment.Where(x => x.Id == departmentId).FirstOrDefault();
                    var category = con.DbSetCategory.Where(x => x.Id == categoryId).FirstOrDefault();
                    var subCategoryId = con.DbSetSubCategory.Where(x => x.Id == subcatId).FirstOrDefault();
                    var supplier = con.DbSetSupplier.Where(x => x.LoginUser.Id == lu.Id).FirstOrDefault();
                    var brand = con.DbSetBrand.Where(x => x.Id == brandId).FirstOrDefault();
                    var color = con.DbSetColor.Where(x => x.Id == colorId).FirstOrDefault();
                    var material = con.DbSetMaterial.Where(x => x.Id == materialId).FirstOrDefault();
                    //Assigning
                    product.SubCategory = subCategoryId;
                    product.SubCategory.Category = category;
                    product.SubCategory.Category.Department = department;
                    product.Supplier = supplier;
                    string[] split = supplier.CompanyName.Split(' ');
                    product.BarCode = supplier.Id + "-" + product.BarCode;
                    product.Color = color;
                    product.Brand = brand;
                    product.Material = material;
                    product.Description = product.Description + "  " + product.Material.Name + "  " +  product.Size.Name + "  " +
                        product.SubCategory.Name + "  " + product.SubCategory.Category.Name;
                    //Finalizing
                    product.IsActive = true;
                    product.LastModified = DateTime.Now;
                    product.LastSold = new DateTime(2022, 01, 01);
                    product.LastPurchased= new DateTime(2022, 01, 01);
                    con.DbSetProduct.Add(product);
                    con.Entry(product.SubCategory).State = System.Data.Entity.EntityState.Unchanged;
                    con.Entry(product.Supplier).State = System.Data.Entity.EntityState.Unchanged;
                    con.Entry(product.Color).State = System.Data.Entity.EntityState.Unchanged;
                    con.Entry(product.Brand).State = System.Data.Entity.EntityState.Unchanged;
                    con.Entry(product.Material).State = System.Data.Entity.EntityState.Unchanged;
                    con.SaveChanges();
                }
            }
            return RedirectToAction("GetInventory", "Product");
        }
        public ActionResult GetCategory(int Id)
        {
            ViewBag.CategorybyID = ModelHelper.selectListItem(new ProductHandler().CategoriesAsync(Id));
            return PartialView("~/Views/_Shared/_GetCategory.cshtml");
        }
        public ActionResult GetCategorybyName(string name)
        {
            ViewBag.category = ModelHelper.selectListItem(new ProductHandler().CategoriesAsync(name));
            return PartialView("~/Views/_Shared/_DDLView.cshtml");
        }
        public ActionResult GetSubCategory(int Id)
        {
            ViewBag.subCategorybyID = ModelHelper.selectListItem(new ProductHandler().subCategories(Id));
            return PartialView("~/Views/_Shared/_GetSubCategory.cshtml");
        }
        public string CheckDepartment(string name)
        {
            using (MyDbContext con = new MyDbContext())
            {
                if (con.DbSetDepartment.Where(x=>x.Name==name).FirstOrDefault()!=null)
                {
                    return "true";
                }
                else
                {
                    return "false";
                }
            }
        }
        public ActionResult AddDepartment (string name)
        {
            new ProductHandler().AddDepartment(name);
            return RedirectToAction("Index", "SiteOwner");
        }
        public string CheckCategory(ObjectModel obj)
        {
            using (MyDbContext con = new MyDbContext())
            {
                Department d = con.DbSetDepartment.Where(x => x.Name == obj.DepValue).FirstOrDefault();
                if (con.DbSetCategory.Where(x=>x.Name==obj.Name && x.Department.Name==d.Name).FirstOrDefault()!=null)
                {
                    return "true";
                }
                else
                {
                    return "false";
                }
            }
        }
        public ActionResult AddCategory(string department ,string name)
        {
            using (MyDbContext con = new MyDbContext())
            {
                Department d = con.DbSetDepartment.Where(x => x.Name == department).FirstOrDefault();
                Category c = new Category();
                c.Name = name;
                c.Department = d;
                con.Entry(c.Department).State = System.Data.Entity.EntityState.Unchanged;
                con.DbSetCategory.Add(c);
                con.SaveChanges();
                return RedirectToAction("Index", "SiteOwner");
            }

        }
        public string CheckSubCategory(ObjectModel obj)
        { 
            using (MyDbContext con = new MyDbContext())
            {
                Department d = con.DbSetDepartment.Where(x => x.Name == obj.CatValue).FirstOrDefault();
                Category c = con.DbSetCategory.Where(x => x.Name == obj.DepValue).FirstOrDefault();

                if (con.DbSetSubCategory.Where(x => x.Name == obj.Name && x.Category.Id==c.Id&& x.Category.Department.Id == d.Id).FirstOrDefault() != null)
                {
                    return "true";
                }
                else
                {
                    return "false";
                }
            }
        }
        public ActionResult AddSubCategory( string departmentwo, string damage, string subcategory)
        {
            using (MyDbContext con = new MyDbContext())
            {
                Category c = con.DbSetCategory.Where(x => x.Name == damage).FirstOrDefault();
                Department d = con.DbSetDepartment.Where(x => x.Name == departmentwo).FirstOrDefault();
                SubCategory s = new SubCategory();
                s.Name = subcategory;
                s.Category = c;
                s.Category.Department = d;
                con.Entry(s.Category).State = System.Data.Entity.EntityState.Unchanged;
                con.Entry(s.Category.Department).State = System.Data.Entity.EntityState.Unchanged;
                con.DbSetSubCategory.Add(s);
                con.SaveChanges();

            }
            return RedirectToAction("Index", "SiteOwner");
        }
        public string CheckColor(string name)
        {
            using (MyDbContext con = new MyDbContext())
            {
                if (con.DbSetColor.Where(x => x.Name == name).FirstOrDefault() != null)
                {
                    return "true";
                }
                else
                {
                    return "false";
                }
            }

        }
        public ActionResult AddColor(string color)
        {
            using (MyDbContext con = new MyDbContext())
            {
                Color c = new Color();
                c.Name = color;
                con.DbSetColor.Add(c);
                con.SaveChanges();
                return RedirectToAction("Index", "SiteOwner");
            }
        }
        public string CheckBrand(string name)
        {
            using (MyDbContext con = new MyDbContext())
            {
                if (con.DbSetBrand.Where(x => x.Name == name).FirstOrDefault() != null)
                {
                    return "true";
                }
                else
                {
                    return "false";
                }
            }

        }
        public ActionResult AddBrand(string brand)
        {
            using (MyDbContext con = new MyDbContext())
            {
                Brand b = new Brand();
                b.Name = brand;
                con.DbSetBrand.Add(b);
                con.SaveChanges();
                return RedirectToAction("Index", "SiteOwner");
            }
        }
        public string CheckMaterial(string name)
        {
            using (MyDbContext con = new MyDbContext())
            {
                if (con.DbSetMaterial.Where(x => x.Name == name).FirstOrDefault() != null)
                {
                    return "true";
                }
                else
                {
                    return "false";
                }
            }

        }
        public ActionResult AddMaterial(string material)
        {
            using (MyDbContext con = new MyDbContext())
            {
                Material b = new Material();
                b.Name = material;
                con.DbSetMaterial.Add(b);
                con.SaveChanges();
                return RedirectToAction("Index", "SiteOwner");
            }
        }
        public string CheckSize(string name)
        {
            using (MyDbContext con = new MyDbContext())
            {
                if (con.DbSetSize.Where(x => x.Name == name).FirstOrDefault() != null)
                {
                    return "true";
                }
                else
                {
                    return "false";
                }
            }

        }
        public ActionResult AddSize(string size)
        {
            using (MyDbContext con = new MyDbContext())
            {
                ILibrary.Production.Size b = new ILibrary.Production.Size();
                b.Name = size;
                con.DbSetSize.Add(b);
                con.SaveChanges();
                return RedirectToAction("Index", "SiteOwner");
            }
        }


        public async Task<ActionResult> ReadExcel()
        {
            if (Session[WebUtil.supplier] != null)
            {
                LoginUsers lu = (LoginUsers)Session[WebUtil.supplier];
                Supplier s = new HRHandler().GetSupplierbyLoginID(lu.Id);
                List<string> excelData = new List<string>();
                Product product = new Product();
                double value = 0;
                string path = new AdminHelper().GetSupplierConfig(s.Id).BulkUploadFilePath;
                FileInfo file = new FileInfo(path);
                if (file.Exists == true)
                {
                    using (ExcelPackage excelPackage = new ExcelPackage(file))
                    {

                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        using (MyDbContext con = new MyDbContext())
                        {


                            foreach (ExcelWorksheet worksheet in excelPackage.Workbook.Worksheets)
                            {
                                //loop all rows
                                for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
                                {
                                    //loop all columns in a row
                                    for (int j = worksheet.Dimension.Start.Column; j <= worksheet.Dimension.End.Column; j++)
                                    {
                                        //add the cell data to the List
                                        if (worksheet.Cells[i, j].Value != null)
                                        {
                                            var temp = worksheet.Cells[i, j].Value.ToString();

                                            if (j == 1)
                                            {
                                                string subcat = temp;
                                                SubCategory cn = con.DbSetSubCategory.Where(x => x.Name == subcat).FirstOrDefault();
                                                if (cn != null)
                                                {
                                                    product.SubCategory = cn;
                                                }
                                                else
                                                {
                                                    SubCategory subCategory = new SubCategory();
                                                    subCategory.Name = subcat;
                                                    con.DbSetSubCategory.Add(subCategory);
                                                    con.SaveChanges();
                                                    SubCategory cn2 = con.DbSetSubCategory.Where(x => x.Name == subcat).FirstOrDefault();
                                                    product.SubCategory = cn2;
                                                }
                                            }

                                            else if (j == 2)
                                            {
                                                string categoryName = temp;
                                                Category cn = con.DbSetCategory.Where(x => x.Name == categoryName).FirstOrDefault();
                                                if (cn != null)
                                                {
                                                    product.SubCategory.Category = cn;
                                                }
                                                else
                                                {

                                                    Category category = new Category();
                                                    category.Name = categoryName;
                                                    con.DbSetCategory.Add(category);
                                                    con.SaveChanges();
                                                    Category cn2 = con.DbSetCategory.Where(x => x.Name == categoryName).FirstOrDefault();
                                                    product.SubCategory.Category = cn2;
                                                }
                                            }
                                            else if (j == 3)
                                            {
                                                string departmetnName = temp;
                                                Department dp = con.DbSetDepartment.Where(x => x.Name == departmetnName).FirstOrDefault();
                                                if (dp != null)
                                                {
                                                    product.SubCategory.Category.Department = dp;
                                                }
                                                else
                                                {
                                                    Department dep = new Department();
                                                    dep.Name = departmetnName;
                                                    con.DbSetDepartment.Add(dep);
                                                    con.SaveChanges();
                                                    dp = con.DbSetDepartment.Where(x => x.Name == departmetnName).FirstOrDefault();
                                                    product.SubCategory.Category.Department = dp;
                                                }
                                            }
                                            else if (j == 4)
                                            {
                                                product.Name = temp;
                                            }
                                            else if (j == 5)
                                            {
                                                product.Description = temp;
                                            }
                                            else if (j == 6)
                                            {
                                                value = int.Parse(temp);
                                                //int.TryParse(temp,out value );
                                                product.SalePrice = (int)value;
                                            }
                                            else if (j == 7)
                                            {
                                                value = int.Parse(temp);

                                                product.DiscountPrice = (int)value;
                                            }
                                            else if (j == 8)
                                            {
                                                // int.TryParse(temp, out value); ;
                                                value = int.Parse(temp);

                                                product.RetailPrice = (int)value;
                                            }
                                            else if (j == 9)
                                            {
                                                product.BarCode = temp.ToString();
                                            }
                                            else if (j == 10)
                                            {
                                                string brandName = temp;
                                                Brand br = con.DbSetBrand.Where(x => x.Name == brandName).FirstOrDefault();
                                                if (br != null)
                                                {
                                                    product.Brand = br;
                                                }
                                                else
                                                {
                                                    Brand brand = new Brand();
                                                    brand.Name = brandName;
                                                    con.DbSetBrand.Add(brand);
                                                    con.SaveChanges();
                                                    br = con.DbSetBrand.Where(x => x.Name == brandName).FirstOrDefault();
                                                    product.Brand = br;
                                                }
                                            }
                                            else if (j == 11)
                                            {
                                                string color = temp;
                                                Color colr = con.DbSetColor.Where(x => x.Name == color).FirstOrDefault();
                                                if (colr != null)
                                                {
                                                    product.Color = colr;
                                                }
                                                else
                                                {
                                                    Color c = new Color();
                                                    c.Name = color;
                                                    con.DbSetColor.Add(c);
                                                    con.SaveChanges();
                                                    colr = con.DbSetColor.Where(x => x.Name == color).FirstOrDefault();
                                                    product.Color = colr;
                                                }
                                            }
                                            else if (j == 12)
                                            {
                                                // int.TryParse(temp, out value); ;
                                                // value = temp.ToString();
                                                product.ModelNumber = temp.ToString();
                                            }
                                            else if (j == 13)
                                            {
                                                //value = temp;
                                                product.SerialNumber = temp.ToString();

                                            }
                                            else if (j == 14)
                                            {
                                                string size = temp;
                                                ILibrary.Production.Size size1 = con.DbSetSize.Where(x => x.Name == size).FirstOrDefault();
                                                if (size1 != null)
                                                {
                                                    product.Size = size1;
                                                }
                                                else
                                                {
                                                    ILibrary.Production.Size c = new ILibrary.Production.Size();
                                                    c.Name = size;
                                                    con.DbSetSize.Add(c);
                                                    con.SaveChanges();
                                                    size1 = con.DbSetSize.Where(x => x.Name == size).FirstOrDefault();
                                                    product.Size = size1;
                                                }
                                            }
                                            else if (j == 15)
                                            {
                                                string material = temp;
                                                Material Matt = con.DbSetMaterial.Where(x => x.Name == material).FirstOrDefault();
                                                if (Matt != null)
                                                {
                                                    product.Material = Matt;
                                                }
                                                else
                                                {
                                                    Material c = new Material();
                                                    c.Name = material;
                                                    con.DbSetMaterial.Add(c);
                                                    con.SaveChanges();
                                                    Matt = con.DbSetMaterial.Where(x => x.Name == material).FirstOrDefault();
                                                    product.Material = Matt;
                                                }
                                            }
                                            else if (j == 16)
                                            {
                                                value = int.Parse(temp);

                                                //int.TryParse(temp,out value );
                                                product.CostPrice = (int)value;
                                            }
                                            else if (j == 17)
                                            {
                                                value = int.Parse(temp);
                                                //int.TryParse(temp,out value );
                                                product.Qty = int.Parse(value.ToString());
                                            }
                                            else if (j == 18)
                                            {
                                                value = int.Parse(temp);
                                                //int.TryParse(temp,out value );
                                                product.DeliveryCharges = int.Parse(value.ToString());
                                            }
                                        }

                                    }
                                    product.IsActive = true;
                                    product.LastModified = new DateTime(2001, 01, 01);
                                    product.LastPurchased = new DateTime(2001, 01, 01);
                                    product.LastSold = new DateTime(2001, 01, 01);
                                    List<Images> images = new List<Images>();
                                    images.Add(new Images { URL = "/Content/assets/img/nophoto.png", Caption = "First Image", Priority = "0" });
                                    images.Add(new Images { URL = "/Content/assets/img/shoppingimage.jpg", Caption = "Second Image", Priority = "1" });

                                    product.Images = images;
                                    product.Supplier = s;
                                    con.Entry(product.Brand).State = System.Data.Entity.EntityState.Unchanged;
                                    con.Entry(product.SubCategory.Category.Department).State = System.Data.Entity.EntityState.Unchanged;
                                    con.Entry(product.SubCategory.Category).State = System.Data.Entity.EntityState.Unchanged;
                                    con.Entry(product.SubCategory).State = System.Data.Entity.EntityState.Unchanged;
                                    con.Entry(product.Color).State = System.Data.Entity.EntityState.Unchanged;
                                    con.Entry(product.Supplier).State = System.Data.Entity.EntityState.Unchanged;
                                    con.DbSetProduct.Add(product);
                                    await con.SaveChangesAsync();
                                }
                            }
                        }
                    }
                }
                else if (file.Exists == false)
                {
                    return RedirectToAction("ErrorPage", "Person");

                }
                return RedirectToAction("ErrorPage", "Person");

            }
            else
            {
                TempData["URL"] = Url.RequestContext.HttpContext.Request.RawUrl;
                return RedirectToAction("Index", "Person");
            }
        }
        public ActionResult FilterSearch(string trig, int dId, bool scb, int minprice, int maxprice, int bId, bool bcb, string barray, string sort)
        {

            using (MyDbContext con = new MyDbContext())
            {
                
                
                List<Product> p = new List<Product>();
                if (trig == "filterdepartment" && dId == 0 && scb == false || trig == "brandname" && dId == 0 && scb == false || trig == "salecheckbox" && dId == 0 && scb == false || trig == "pricemin" && dId == 0 && scb == false || trig == "pricemax" && dId == 0 && scb == false)
                {
                    p = new List<Product>();
                    int aim = 0;
                    if (barray == "0")
                    {
                        //aim = int.Parse(item);
                        List<Product> products = new MyDbContext().DbSetProduct.Include("Color").Include("Images").Include("SubCategory")
                        .Include("SubCategory.Category").Where(x => x.RetailPrice >= minprice && x.RetailPrice <= maxprice).ToList();
                        p.AddRange(products);
                    }
                    else
                    {
                        string[] bar = barray.Split(',');
                        foreach (var item in bar)
                        {
                            aim = int.Parse(item);
                            List<Product> products = new MyDbContext().DbSetProduct.Include("Color").Include("Images").Include("SubCategory")
                             .Include("SubCategory.Category").Where(x => x.RetailPrice >= minprice && x.RetailPrice <= maxprice && x.Brand.Id == aim).ToList();
                            p.AddRange(products);
                        }
                    }
                    ViewBag.SearchbyRequirment = p;
                    return View("~/Views/_Shared/_SearchedProducts.cshtml");
                }
                else if (trig == "filterdepartment" && dId != 0 && scb == true || trig == "brandname" && dId != 0 && scb == true || trig == "salecheckbox" && dId != 0 && scb == true || trig == "pricemin" && dId != 0 && scb == true || trig == "pricemax" && dId != 0 && scb == true)
                {
                    int aim = 0;
                    if (barray == "0")
                    {
                        //aim = int.Parse(item);
                        List<Product> products = new MyDbContext().DbSetProduct.Include("Color").Include("Images").Include("SubCategory")
                                             .Include("SubCategory.Category").Where(x => x.SubCategory.Category.Department.Id == dId
                                             && x.RetailPrice >= minprice && x.RetailPrice <= maxprice && x.IsSale == scb
                                             ).ToList();
                        p.AddRange(products);
                    }
                    else
                    {
                        string[] bar = barray.Split(',');
                        foreach (var item in bar)
                        {
                            aim = int.Parse(item);
                            List<Product> products = new MyDbContext().DbSetProduct.Include("Color").Include("Images").Include("SubCategory")
                                                 .Include("SubCategory.Category").Where(x => x.SubCategory.Category.Department.Id == dId
                                                 && x.RetailPrice >= minprice && x.RetailPrice <= maxprice && x.Brand.Id == aim && x.IsSale == scb
                                                 ).ToList();
                            p.AddRange(products);
                        }
                    }
                    ViewBag.SearchbyRequirment = p;
                    return View("~/Views/_Shared/_SearchedProducts.cshtml");
                }
                else if (trig == "filterdepartment" && dId != 0 && scb == false || trig == "brandname" && dId != 0 && scb == false || trig == "salecheckbox" && dId != 0 && scb == false || trig == "pricemin" && dId != 0 && scb == false || trig == "pricemax" && dId != 0 && scb == false)
                {
                    int aim = 0;
                    if (barray == "0")
                    {
                        //aim = int.Parse(item);
                        List<Product> products = new MyDbContext().DbSetProduct.Include("Color").Include("Images").Include("SubCategory")
                                             .Include("SubCategory.Category").Where(x => x.SubCategory.Category.Department.Id == dId
                                             && x.RetailPrice >= minprice && x.RetailPrice <= maxprice
                                             ).ToList();
                        p.AddRange(products);
                    }
                    else
                    {
                        string[] bar = barray.Split(',');
                        foreach (var item in bar)
                        {

                            aim = int.Parse(item);
                            List<Product> products = new MyDbContext().DbSetProduct.Include("Color").Include("Images").Include("SubCategory")
                                                 .Include("SubCategory.Category").Where(x => x.SubCategory.Category.Department.Id == dId
                                                 && x.RetailPrice >= minprice && x.RetailPrice <= maxprice && x.Brand.Id == aim
                                                 ).ToList();
                            p.AddRange(products);
                        }
                    }
                    ViewBag.SearchbyRequirment = p;
                    return View("~/Views/_Shared/_SearchedProducts.cshtml");
                }
                else if (trig == "filterdepartment" && dId == 0 && scb == true || trig == "brandname" && dId == 0 && scb == true || trig == "salecheckbox" && dId == 0 && scb == true || trig == "pricemin" && dId == 0 && scb == true || trig == "pricemax" && dId != 0 && scb == true)
                {
                    int aim = 0;
                    if (barray == "0")
                    {
                        //aim = int.Parse(item);
                        List<Product> products = new MyDbContext().DbSetProduct.Include("Color").Include("Images").Include("SubCategory")
                        .Include("SubCategory.Category").Where(x => x.RetailPrice >= minprice && x.RetailPrice <= maxprice && x.IsSale == true).ToList();
                        p.AddRange(products);
                    }
                    else
                    {
                        string[] bar = barray.Split(',');
                        foreach (var item in bar)
                        {
                            aim = int.Parse(item);
                            List<Product> products = new MyDbContext().DbSetProduct.Include("Color").Include("Images").Include("SubCategory")
                             .Include("SubCategory.Category").Where(x => x.RetailPrice >= minprice && x.RetailPrice <= maxprice && x.Brand.Id == aim && x.IsSale == true).ToList();
                            p.AddRange(products);
                        }
                    }
                    ViewBag.SearchbyRequirment = p;
                    return View("~/Views/_Shared/_SearchedProducts.cshtml");
                }
                else if (trig=="sort")
                {
                    if (sort== "Price High to Low")
                    {
                        int aim = 0;
                        if (barray == "0")
                        {
                            //aim = int.Parse(item);
                            List<Product> products = new MyDbContext().DbSetProduct.Include("Color").Include("Images").Include("SubCategory")
                            .Include("SubCategory.Category").Where(x => x.RetailPrice >= minprice && x.RetailPrice <= maxprice && x.IsSale == scb && x.SubCategory.Category.Department.Id==dId).OrderByDescending(x => x.RetailPrice).ToList();
                            p.AddRange(products);
                        }
                        else
                        {
                            string[] bar = barray.Split(',');
                            foreach (var item in bar)
                            {
                                aim = int.Parse(item);
                                List<Product> products = new MyDbContext().DbSetProduct.Include("Color").Include("Images").Include("SubCategory")
                                 .Include("SubCategory.Category").Where(x => x.RetailPrice >= minprice && x.RetailPrice <= maxprice && x.Brand.Id == aim && x.IsSale == scb).OrderByDescending(x=>x.RetailPrice).ToList();
                                p.AddRange(products);
                            }
                        }
                        ViewBag.SearchbyRequirment = p;
                        return View("~/Views/_Shared/_SearchedProducts.cshtml");
                    }
                    else
                    {
                        int aim = 0;
                        if (barray == "0")
                        {
                            //aim = int.Parse(item);
                            List<Product> products = new MyDbContext().DbSetProduct.Include("Color").Include("Images").Include("SubCategory")
                            .Include("SubCategory.Category").Where(x => x.RetailPrice >= minprice && x.RetailPrice <= maxprice && x.IsSale == scb).OrderBy(x => x.RetailPrice).ToList();
                            p.AddRange(products);
                        }
                        else
                        {
                            string[] bar = barray.Split(',');
                            foreach (var item in bar)
                            {
                                aim = int.Parse(item);
                                List<Product> products = new MyDbContext().DbSetProduct.Include("Color").Include("Images").Include("SubCategory")
                                 .Include("SubCategory.Category").Where(x => x.RetailPrice >= minprice && x.RetailPrice <= maxprice && x.Brand.Id == aim && x.IsSale == true).ToList();
                                p.AddRange(products);
                            }
                        }
                        ViewBag.SearchbyRequirment = p;
                        return View("~/Views/_Shared/_SearchedProducts.cshtml");
                    }

                   
                }
                else
                {
                    return View("~/Views/_Shared/_SearchedProducts.cshtml");
                }
            }
        }
        public int GetCounter(string trig, int dId)
        {
            using (MyDbContext con = new MyDbContext())
            {
                if (trig == "filterdepartment" && dId == 0 || trig == "salecheckbox" && dId == 0)
                {
                    return con.DbSetProduct.Where(x => x.IsSale == true).ToList().Count;
                }
                else if (trig == "filterdepartment" || trig == "salecheckbox")
                {
                    return con.DbSetProduct.Where(x => x.SubCategory.Category.Department.Id == dId && x.IsSale == true).ToList().Count;
                }
                else
                {
                    return 0;
                }

            }
        }

        //public ActionResult ReadExcel()
        //{
        //    if ((LoginUsers)Session[WebUtil.supplier] != null)
        //    {
        //        LoginUsers lu = (LoginUsers)Session[WebUtil.supplier];
        //        Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
        //        FileInfo file = new FileInfo("D:\\myinvn.xlsx");
        //        if (file.Exists == true)
        //        {
        //            Microsoft.Office.Interop.Excel.Workbook xlWorkbook = xlApp.Workbooks.Open("D:\\myinvn.xlsx", 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
        //            Microsoft.Office.Interop.Excel._Worksheet xlWorksheet = (Microsoft.Office.Interop.Excel._Worksheet)xlWorkbook.Sheets[1];
        //            Microsoft.Office.Interop.Excel.Range xlRange = xlWorksheet.UsedRange;
        //            int rowCount = xlRange.Rows.Count;
        //            int colCount = xlRange.Columns.Count;
        //            // List<Product> p = new List<Product>();
        //            double value = 0;
        //            Product product = new Product();
        //            using (MyDbContext con = new MyDbContext())
        //            {
        //                Supplier s = con.DbSetSupplier.Where(x => x.LoginUser.Id == lu.Id).FirstOrDefault();
        //                for (int i = 2; i <= rowCount; i++)
        //                {

        //                    for (int j = 1; j <= colCount; j++)
        //                    {

        //                        var temp = (dynamic)(xlRange.Cells[i, j] as Microsoft.Office.Interop.Excel.Range).Value2;
        //                        if (j == 1)
        //                        {
        //                            string subcat = temp;
        //                            SubCategory cn = con.DbSetSubCategory.Where(x => x.Name == subcat).FirstOrDefault();
        //                            if (cn != null)
        //                            {
        //                                product.SubCategory = cn;
        //                            }
        //                            else
        //                            {
        //                                product.SubCategory.Name = "TEMP";
        //                            }
        //                        }

        //                        else if (j == 2)
        //                        {
        //                            string categoryName = temp;
        //                            Category cn = con.DbSetCategory.Where(x => x.Name == categoryName).FirstOrDefault();
        //                            if (cn != null)
        //                            {
        //                                product.SubCategory.Category = cn;
        //                            }
        //                            else
        //                            {
        //                                product.SubCategory.Category.Name = "TEMP";
        //                            }
        //                        }
        //                        else if (j == 3)
        //                        {
        //                            string departmetnName = temp;
        //                            Department dp = con.DbSetDepartment.Where(x => x.Name == departmetnName).FirstOrDefault();
        //                            if (dp != null)
        //                            {
        //                                product.SubCategory.Category.Department = dp;
        //                            }
        //                            else
        //                            {
        //                                product.SubCategory.Category.Department.Name = "TEMP";
        //                            }
        //                        }
        //                        else if (j == 4)
        //                        {
        //                            product.Name = temp;
        //                        }
        //                        else if (j == 5)
        //                        {
        //                            product.Description = temp;
        //                        }
        //                        else if (j == 6)
        //                        {
        //                            value = temp;
        //                            //int.TryParse(temp,out value );
        //                            product.SalePrice = (int)value;
        //                        }
        //                        else if (j == 7)
        //                        {
        //                            value = temp;
        //                            product.DiscountPrice = (int)value;
        //                        }
        //                        else if (j == 8)
        //                        {
        //                            // int.TryParse(temp, out value); ;
        //                            value = temp;
        //                            product.RetailPrice = (int)value;
        //                        }
        //                        else if (j == 9)
        //                        {
        //                            product.BarCode = s.Id + "-" + temp;
        //                        }
        //                        else if (j == 10)
        //                        {
        //                            string brandName = temp;
        //                            Brand cn = con.DbSetBrand.Where(x => x.Name == brandName).FirstOrDefault();
        //                            if (cn != null)
        //                            {
        //                                product.Brand = cn;
        //                            }
        //                            else
        //                            {
        //                                product.SubCategory.Category.Name = "TEMP";
        //                            }
        //                        }
        //                        else if (j == 11)
        //                        {
        //                            string color = temp;
        //                            Color colr = con.DbSetColor.Where(x => x.Name == color).FirstOrDefault();
        //                            if (colr != null)
        //                            {
        //                                product.Color = colr;
        //                            }
        //                            else
        //                            {
        //                                product.Color.Name = "TEMP";
        //                            }
        //                        }
        //                        else if (j == 12)
        //                        {
        //                            // int.TryParse(temp, out value); ;
        //                            // value = temp.ToString();
        //                            product.ModelNumber = temp;
        //                        }
        //                        else if (j == 13)
        //                        {
        //                            //value = temp;
        //                            product.SerialNumber = temp;

        //                        }
        //                        else if (j == 14)
        //                        {
        //                            string size = temp;
        //                            ILibrary.Production.Size size1 = con.DbSetSize.Where(x => x.Name == size).FirstOrDefault();
        //                            if (size1 != null)
        //                            {
        //                                product.Size = size1;
        //                            }
        //                            else
        //                            {
        //                                product.Color.Name = "TEMP";
        //                            }
        //                        }
        //                        else if (j == 15)
        //                        {
        //                            string material = temp;
        //                            Material Matt = con.DbSetMaterial.Where(x => x.Name == material).FirstOrDefault();
        //                            if (Matt != null)
        //                            {
        //                                product.Material = Matt;
        //                            }
        //                            else
        //                            {
        //                                product.Color.Name = "TEMP";
        //                            }
        //                        }
        //                    }
        //                    product.IsActive = true;
        //                    product.Supplier = s;
        //                    con.Entry(product.Brand).State = System.Data.Entity.EntityState.Unchanged;
        //                    con.Entry(product.SubCategory.Category.Department).State = System.Data.Entity.EntityState.Unchanged;
        //                    con.Entry(product.SubCategory.Category).State = System.Data.Entity.EntityState.Unchanged;
        //                    con.Entry(product.SubCategory).State = System.Data.Entity.EntityState.Unchanged;
        //                    con.Entry(product.Color).State = System.Data.Entity.EntityState.Unchanged;
        //                    con.Entry(product.Supplier).State = System.Data.Entity.EntityState.Unchanged;
        //                    con.DbSetProduct.Add(product);
        //                    con.SaveChanges();
        //                }
        //            }
        //            return RedirectToAction("GetAllProducts", "Master");
        //        }
        //        else if (file.Exists == false)
        //        {
        //            return RedirectToAction("ErrorPage", "Person");
        //        }
        //        return RedirectToAction("ErrorPage", "Person");
        //        //return RedirectToAction("GetAllProducts", "Master");

        //    }
        //    else
        //    {
        //        return RedirectToAction("Index", "Person");
        //    }
        //}


    }
}