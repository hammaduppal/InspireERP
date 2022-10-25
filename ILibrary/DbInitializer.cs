using ILibrary.AdminSide;
using ILibrary.HRM;
using ILibrary.Production;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILibrary
{
    public class DbInitializer : DropCreateDatabaseIfModelChanges<MyDbContext>
    {
        protected override void Seed(MyDbContext context)
        {
            List<PaidPromotions> paid = new List<PaidPromotions>();
            paid.Add(new PaidPromotions { AddVavlue = 0, DateCreated = DateTime.Now, ImageUrl = "/Content/assets/img/banners/promo1.jpg", IsActive = true, Url = "/Home/GetProduct/1", Name = "Promo1", Description = "Promo1" });
            paid.Add(new PaidPromotions { AddVavlue = 1, DateCreated = DateTime.Now, ImageUrl = "/Content/assets/img/banners/promo2.jpg", IsActive = true, Url = "/Home/GetProduct/1", Name = "Promo1", Description = "Promo1" });
            paid.Add(new PaidPromotions { AddVavlue = 2, DateCreated = DateTime.Now, ImageUrl = "/Content/assets/img/banners/promo3.jpg", IsActive = true, Url = "/Home/GetProduct/1", Name = "Promo1", Description = "Promo1" });
            paid.Add(new PaidPromotions { AddVavlue = 3, DateCreated = DateTime.Now, ImageUrl = "/Content/assets/img/banners/promo3.jpg", IsActive = true, Url = "/Home/GetProduct/1", Name = "Promo1", Description = "Promo1" });
            context.DbSetPaidPromotions.AddRange(paid);


            LoginUsers l = new LoginUsers();
            l.Email = "sa@sa.com";
            l.Password = "12345";
            l.UserName = "sa";
            l.LoginType = LoginUsers.Logintype.master;
            List<StretAddress> suplierAdress = new List<StretAddress>();
            suplierAdress.Add(new StretAddress { StAddress = "MyHome", City = new City { Name = "Lahore", Country = new Country { Name = "Pakistan" } }, AddressType= AddressType.Shipping });
            suplierAdress.Add(new StretAddress { StAddress = "My Office", City = new City { Name = "Lahore", Country = new Country { Name = "Pakistan" } }, AddressType = AddressType.Billing });
            List<StretAddress> custAddress = new List<StretAddress>();
            custAddress.Add(new StretAddress { StAddress = "MyHome", City = new City { Name = "Lahore", Country = new Country { Name = "Pakistan" } }, AddressType = AddressType.Shipping });
            custAddress.Add(new StretAddress { StAddress = "My Office", City = new City { Name = "Lahore", Country = new Country { Name = "Pakistan" } }, AddressType = AddressType.Billing });

            Supplier supplier = new Supplier();
            supplier.CompanyName = "Inspire Nation";
            supplier.NTN = "9876543";
            supplier.DateCreated = new DateTime(2022, 06,22);
            supplier.DateEdited = new DateTime(2022, 06, 22);

            supplier.CompanyLogo = "/Content/Images/BusinessData/clogo.jpg";
            supplier.Person = new Person
            {
                Email = "uppal.hammad@gmail.com",
                FirstName = "Hammad",
                LastName = "Uppal",
                MobileNumber = 3334048470,
                //SellerProfilePIC = "/Content/Images/ProfileIMages/hdpic.jpg",
                StretAddress = suplierAdress
            };
            supplier.LoginUser = new LoginUsers { UserName = "hammad", Password = "12345", LoginType = LoginUsers.Logintype.supplier, Email="uppal.hammad@gmail.com" };
            SupplierConfig supplierConfig = new SupplierConfig();
            supplierConfig.Supplier = supplier;
            supplierConfig.SupplierCreatedOn = DateTime.Now;
            supplierConfig.NewfileTime = DateTime.Now;
            Customer customer = new Customer();
            customer.Person = new Person
            {
                
                FirstName = "Jawwad",
                LastName = "Uppal",
                Email = "jawad_uppal@yahoo.com",
                //CNIC = "35201170257555",
                MobileNumber = 3334048470,
                StretAddress = custAddress
            };
            customer.LoginUsers = new LoginUsers { UserName = "jawad", Password = "12345", LoginType = LoginUsers.Logintype.user, Email="jawad_uppal@yahoo.com" };
            customer.CustomerNote = "I am The Customer1";
           
            List<Images> images = new List<Images>();
            images.Add(new Images { Caption = "FirstImage", URL = "/Content/assets/img/pimages/p1.jpg" });
            images.Add(new Images { Caption = "2nd Image", URL = "/Content/assets/img/pimages/p2.jpg" });
            images.Add(new Images { Caption = "3rd Image", URL = "/Content/assets/img/pimages/p3.jpg" });
            // string n = supplier.CompanyName.Substring(0, 5);

            Product product = new Product
            {
                BarCode = supplier.NTN + "-123456",
                Supplier = supplier,
                Brand = new Brand { Name = "Dell Corp" },
                //IsDiscount = false,
                //IsSale = true,
                //SerialNumber = "A153P012NAM6",
                Qty = 5,
                Color = new Color { Name = "Blue" },
                Material = new Material { Name = "Plastic" },
                Size = new Size { Name = "Large" }
                 ,
                SalePrice = 150, LastModified = new DateTime(2022, 01, 01), LastSold = new DateTime(2022, 01, 01), CostPrice = 150, LastPurchased = new DateTime(2022, 01, 01),
                Description = "Hammer That Can Destory",
                Images = images,
                DiscountPrice = 170,
                RetailPrice = 200, DeliveryCharges = 300, ModelNumber="DW123456", SerialNumber="654321",  IsSale=true,
                Name = "Noyan Hammer",
                IsActive = true,
                SubCategory = new SubCategory
                {
                    Name = "RubberHammer",
                    Category = new Category { Name = "Hammer",  Department = new Department { Name = "Hardware" } }
                }
            };
            // SliderBanner sb = new SliderBanner();
            // sb.NameOne = "Our Best Collection";
            // sb.NameTwo = "On Sale";
            // sb.NameThree = "This is Featured Product and Very Speical Efficency";
            // sb.ProductLink = "https://localhost:44323/Home/GetProductDetail/1";
            // sb.URL = "/Content/Images/BusinessData/BImages/b1.jpg";
            // sb.IsActive = true;
            //// context.DbSetBanner.Add(sb);
            ///
            SiteConfiguration s = new SiteConfiguration();

            s.tabtags = "Hardware|prf|baby|Undergarments ";
            s.CompanyAddress = "MyCompany Address What is Your Problem";
            s.CompanyCity = "Lahore";
            s.CompanyCountry = "Pakistan";
            s.CompanyEmail = "myEmail@YourProblem.com";
            s.CompanyMobile = "0300-4005006";
            s.CompanyFirstName = "Inspire";
            s.CompanyLastName = "Nation";
            s.CompanyLogo = "/Content/assets/img/bimages/inspire.png";

            s.CompanyNote = "We Are becoming #1 Online Seller in Pakistan we try our best to satisfy our customers";
            s.CompanySkype = "MySkype";
            s.FaceBook = "facebook.com/quakerquakest";
            s.Twitter = "twitter.com";
            s.WebSite = "whois.com";
            BannerSlider bannerSlider = new BannerSlider();
            bannerSlider.URL = "/Home/GetProduct/1";
            bannerSlider.Text = "PriorityProduct";
            bannerSlider.IsActive = true;
            bannerSlider.ImageURL = "/Content/assets/img/banners/Banner1.jpg";
            bannerSlider.AddDateTime = DateTime.Now;
            context.DbSetBanners.Add(bannerSlider);
            context.DbSetLoginUser.Add(l);
            context.DbSetSiteConfiguration.Add(s);
             context.DbSetSupplier.Add(supplier);
           
            context.DbSetCustomer.Add(customer);
            context.DbSetSupplierConfig.Add(supplierConfig);
            context.DbSetProduct.Add(product);
            context.SaveChanges();

            //context.Entry(customer.Person.StretAddress.City).State = EntityState.Unchanged;
            //context.Entry(customer.Person.StretAddress.City.Country).State = EntityState.Unchanged;
            context.SaveChanges();
            base.Seed(context);
            {
            }

        }
    }
}

