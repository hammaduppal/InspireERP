using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
namespace ILibrary.Production
{
    public class ProductHandler
    {
        //Get Product by ID
        public Product GetProductbyId(int Id)
        {
            using (MyDbContext con = new MyDbContext())
            {
                Product p = con.DbSetProduct.Include("Size").Include("Material").Include("Color").Include("Images").Include("SubCategory")
                    .Include("SubCategory.Category").Include("SubCategory.Category.Department")
                    .Where(x => x.Id == Id & x.IsActive == true).FirstOrDefault();
                p.ViewCounts ++;
                con.SaveChanges();
                return p;
            }
        }

        public List<Product> GetSimilarProducts(int Id)
        {
            using (MyDbContext con = new MyDbContext())
            {
                return con.DbSetProduct.Include("Color").Include("Material").Include("Size").Include("SubCategory").Include("SubCategory.Category").Include("SubCategory.Category.Department").Include("Images")
                    .Where(x => x.SubCategory.Id==Id).Where(x => x.IsActive==true).Take(20).ToList();
            }
        }
        //GetAll Product
        public List<Product> GetAllActiveProducts(int Id)
        {
            using (MyDbContext con = new MyDbContext())
            {
                return con.DbSetProduct.Include("Size").Include("Material").Include("Color").Include("Images").Include("SubCategory")
                    .Include("SubCategory.Category").Include("SubCategory.Category.Department")
                    .Where(x=>x.Supplier.Id==Id).Where(x=>x.IsActive==true).ToList();
            }
        }
        public int ProductCount()
        {
            int count = 0;
            using (MyDbContext con = new MyDbContext())
            {
                count= con.DbSetProduct
                    .Where(x => x.IsActive == true).ToList().Count;
                return count;
            }
        }
        public int ProductCount(int Id)
        {
            int count = 0;
            using (MyDbContext con = new MyDbContext())
            {
                count = con.DbSetProduct
                    .Where(x=>x.Supplier.Id==Id).Where(x => x.IsActive == true).ToList().Count;
                return count;
            }
        }
        public int CustomerCount()
        {
            using (MyDbContext con = new MyDbContext())
            {
               return con.DbSetCustomer
                    
                    .ToList().Count;
            }
        }
        public List<Product> ProductbySubCat(int Id)
        {
            using (MyDbContext con = new MyDbContext())
            {
                return con.DbSetProduct.Include("SubCategory").Include("Images").Where(x => x.SubCategory.Id == Id).ToList();
            }

        }
        public List<Product> GetSaleProduct()
        {
            using (MyDbContext con = new MyDbContext())
            {
                return con.DbSetProduct.Include("Color").Include("Material").Include("Size").Include("SubCategory").Include("SubCategory.Category").Include("SubCategory.Category.Department").Include("Images")
                    .Where(x => x.IsActive).Where(x => x.IsSale == true).Take(5).ToList();
            }
        }
        public List<Product> GetPopularProducts()
        {
            using (MyDbContext con = new MyDbContext())
            {
                return con.DbSetProduct.Include("Color").Include("Material").Include("Size").Include("SubCategory").Include("SubCategory.Category").Include("SubCategory.Category.Department").OrderBy(x=>x.ViewCounts).Include("Images")
                    .Where(x => x.IsActive ).Take(20).ToList();
            }
        }
        //public List<Product> GetSaleProduct(string name)
        //{
        //    using (MyDbContext con = new MyDbContext())
        //    {
        //        return con.DbSetProduct.Include("Color").Include("Material").Include("Size").Include("SubCategory").Include("SubCategory.Category").Include("SubCategory.Category.Department").Include("Images")
        //            .Where(x => x.IsActive).Where(x => x.IsSale == true).ToList();
        //    }
        //}

        public List<Product> FeaturedTENProducts()
        {
            using (MyDbContext con = new MyDbContext())
            {
               return con.DbSetProduct.Include("Images").Where(x => x.IsFeatured == true).Take(10).ToList();
            }
        }
        public List<Product> SalesTENProduct()
        {
            using (MyDbContext con = new MyDbContext())
            {
                return con.DbSetProduct.Include("Images").Where(x => x.IsSale == true).Take(10).ToList();
            }
        }
        public List<Product> DiscountTenProducts()
        {
            using (MyDbContext con = new MyDbContext())
            {
                return con.DbSetProduct.Include("Images").Where(x => x.IsDiscount == true).Take(10).ToList();
            }
        }


        //Remove Product Code



        public void EditProduct(Product p)
        {
            using (MyDbContext con = new MyDbContext())
            {
                Product pr =  con.DbSetProduct.Where(x => x.Id == p.Id).FirstOrDefault();
                pr.Name = p.Name;
                pr.Description = p.Description;
                pr.SalePrice = p.SalePrice;
                pr.RetailPrice = p.RetailPrice;
                pr.DiscountPrice = p.DiscountPrice;
                pr.IsDiscount = p.IsDiscount;
                pr.IsFeatured = p.IsFeatured;
                pr.IsSale = p.IsSale;
                con.Entry(pr).State = System.Data.Entity.EntityState.Modified;
                con.SaveChanges();
            }
        }
       
        public List<Product> TakeTwenty(string dep1, string dep2, string dep3, string dep4)
        {
            List<Product> p = new List<Product>();
            using (MyDbContext con = new MyDbContext())
            {
                p.AddRange(con.DbSetProduct.Include("SubCategory.Category.Department").Include("Images").Where(x => x.IsActive && x.SubCategory.Category.Department.Name==dep1).Take(3).ToList());
                p.AddRange(con.DbSetProduct.Include("SubCategory.Category.Department").Include("Images").Where(x => x.IsActive == true && x.SubCategory.Category.Department.Name == dep2).Take(3).ToList());
                p.AddRange(con.DbSetProduct.Include("SubCategory.Category.Department").Include("Images").Where(x => x.IsActive == true && x.SubCategory.Category.Department.Name == dep3).Take(3).ToList());
                p.AddRange(con.DbSetProduct.Include("SubCategory.Category.Department").Include("Images").Where(x => x.IsActive == true && x.SubCategory.Category.Department.Name == dep4).Take(3).ToList());
                return p;
            }

        }
        public List<SubCategory> subCategories(int Id)
        {
            using (MyDbContext con = new MyDbContext())
            {
                return con.DbSetSubCategory.Where(x => x.Category.Id == Id).ToList();
            }
        }
        public List<SubCategory> subCategories()
        {
            using (MyDbContext con = new MyDbContext())
            {
                return  con.DbSetSubCategory.Include("Category").Include("Category.Department").ToList();
            }
        }
        public List<Category> CategoriesAsync(int Id)
        {
            using (MyDbContext con = new MyDbContext())
            {
                return con.DbSetCategory.Where(x=>x.Department.Id==Id).ToList();
            }
        }
        public List<Category> CategoriesAsync(string name)
        {
            using (MyDbContext con = new MyDbContext())
            {
                Department d = con.DbSetDepartment.Where(x => x.Name == name).FirstOrDefault();
                return  con.DbSetCategory.Where(x => x.Department.Id == d.Id).ToList();
            }
        }
        public List<Category> CategoriesAsync()
        {
            using (MyDbContext con = new MyDbContext())
            {
                return  con.DbSetCategory.Include("Department").ToList();
            }
        }
       
        public List<Department> DepartmentAsync()
        {
            using (MyDbContext con = new MyDbContext())
            {
                return  con.DbSetDepartment.ToList();
            }
        }
        public List<Color> ColorAsync()
        {
            using (MyDbContext con = new MyDbContext())
            {
                return  con.DbSetColor.ToList();
            }
        }
        public List<Size> Size()
        {
            using (MyDbContext con = new MyDbContext())
            {
                return con.DbSetSize.ToList();
            }
        }
        public List<Brand> Brand()
        {
            using (MyDbContext con = new MyDbContext())
            {
                return con.DbSetBrand.ToList();
            }
        }
        public List<Material> Materials()
        {
            using (MyDbContext con = new MyDbContext())
            {
                return con.DbSetMaterial.ToList();
            }
        }
        public void AddDepartment(string name)
        {
            using (MyDbContext con = new MyDbContext())
            {
                Department d = new Department();
                d.Name = name;
                con.DbSetDepartment.Add(d);
                con.SaveChanges();
            }
        }
       
    }
}
