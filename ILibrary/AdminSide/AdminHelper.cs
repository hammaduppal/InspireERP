using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILibrary.AdminSide
{
    public class AdminHelper
    {
        #region Banners
        public List<BannerSlider> BannerSlides()
        {
            using (MyDbContext con = new MyDbContext())
            {
                return con.DbSetBanners.Where(x => x.IsActive == true).ToList();
            }
        }
        public List<BannerSlider> GetAllBanners()
        {
            using (MyDbContext con = new MyDbContext())
            {
                return con.DbSetBanners.ToList();
            }
        }
        public void DisableBanner(int Id)
        {
            using (MyDbContext con = new MyDbContext())
            {
                BannerSlider b = con.DbSetBanners.Where(x => x.Id == Id).FirstOrDefault();
                b.IsActive = false;
                con.SaveChanges();
            }
        }
        public void EnableBanner(int Id)
        {
            using (MyDbContext con = new MyDbContext())
            {
                BannerSlider b = con.DbSetBanners.Where(x => x.Id == Id).FirstOrDefault();
                b.IsActive = true;
                con.SaveChanges();
            }
        }
        public void AddBanner(BannerSlider banner)
        {
            using (MyDbContext con = new MyDbContext())
            {
                con.DbSetBanners.Add(banner);
                con.SaveChanges();
            }
        }

        #endregion

        #region Promos
        public void AddPromotion(PaidPromotions promotion)
        {
            using (MyDbContext con = new MyDbContext())
            {
                con.DbSetPaidPromotions.Add(promotion);
                con.SaveChanges();
            }
        }
        public List<PaidPromotions> GetActivePromotions()
        {
            using (MyDbContext con = new MyDbContext())
            {
                return con.DbSetPaidPromotions.Where(x => x.IsActive == true).ToList();
            }
        }
        public List<PaidPromotions> GetAllPromotions()
        {
            using (MyDbContext con = new MyDbContext())
            {
                return con.DbSetPaidPromotions.ToList();
            }
        }
        public void DisablePromo(int Id)
        {
            using (MyDbContext con = new MyDbContext())
            {
                PaidPromotions b = con.DbSetPaidPromotions.Where(x => x.Id == Id).FirstOrDefault();
                b.IsActive = false;
                con.SaveChanges();
            }
        }
        public void EnablePromo(int Id)
        {
            using (MyDbContext con = new MyDbContext())
            {
                PaidPromotions b = con.DbSetPaidPromotions.Where(x => x.Id == Id).FirstOrDefault();
                b.IsActive = true;
                con.SaveChanges();
            }
        }
        #endregion

        public void AddTags(string tab1, string tab2, string tab3, string tab4)
        {
            string tags = $"{tab1}|{tab2}|{tab3}|{tab4} ";
            using (MyDbContext con = new MyDbContext())
            {
               SiteConfiguration s =  con.DbSetSiteConfiguration.First();
                s.tabtags = tags;
                con.SaveChanges();
            }
        }
        public SiteConfiguration GetSiteConfiguration()
        {
            using (MyDbContext con = new MyDbContext())
            {
                return con.DbSetSiteConfiguration.First();
            }
        }
    
    public SupplierConfig GetSupplierConfig(int Id)
        {
            using (MyDbContext con = new MyDbContext())
            {
                return con.DbSetSupplierConfig.Where(x => x.Supplier.Id == Id).FirstOrDefault();
            }
        }
    
    }
}
