using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using ILibrary.Production;
using ILibrary.HRM;
using ILibrary.SaleManager;
using ILibrary.AdminSide;

namespace ILibrary
{
    public class MyDbContext :DbContext
    {
        public MyDbContext() :base("ConStrng")
        {

        }
        public DbSet<Product> DbSetProduct { get; set; }
        public DbSet<SubCategory> DbSetSubCategory { get; set; }
        public DbSet<Category> DbSetCategory { get; set; }
        public DbSet<Department> DbSetDepartment { get; set; }
        public DbSet<Color> DbSetColor { get; set; }
        public DbSet<Size> DbSetSize{ get; set; }
        public DbSet<Brand> DbSetBrand { get; set; }
        public DbSet<SupplierConfig> DbSetSupplierConfig { get; set; }
        public DbSet<Material> DbSetMaterial { get; set; }
        public DbSet<BannerSlider> DbSetBanners { get; set; }
        public DbSet<SiteConfiguration> DbSetSiteConfiguration { get; set; }
        public DbSet<PaidPromotions> DbSetPaidPromotions { get; set; }
        public DbSet<StretAddress> DbSetStreetAddresses { get; set; }


        public DbSet<Person> DbSetPerson { get; set; }


        public DbSet<LoginUsers> DbSetLoginUser { get; set; }
        public DbSet<City> DbSetCity { get; set; }
        public DbSet<Country> DbSetCountry { get; set; }
        public DbSet<Supplier> DbSetSupplier { get; set; }
        public DbSet<Customer> DbSetCustomer { get; set; }
        public DbSet<Images> DbSetImages { get; set; }
        public DbSet<OrderMaster> DbSetOrderMaster { get; set; }
        public DbSet<OrderDetails> DbSetOrderDetail { get; set; }
        public DbSet<ResetPass> DbSetResetPass { get; set; }



    }
}
