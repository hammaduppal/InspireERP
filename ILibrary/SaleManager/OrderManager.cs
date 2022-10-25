using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILibrary.SaleManager
{
    public class OrderManager
    {
        public List<OrderMaster> GetOrdersbyUser(int Id)
        {
            using (MyDbContext con = new MyDbContext())
            {
                return con.DbSetOrderMaster.Where(x=>x.Customer.LoginUsers.Id==Id).ToList();
            }
        }
        public OrderMaster GetOrder(int Id)
        {
            using (MyDbContext con = new MyDbContext())
            {
                
                return con.DbSetOrderMaster.Include("Product").Include("Product.Images").Where(x => x.Id==Id).FirstOrDefault();
            }
        }
        public List<OrderMaster> GetOrderbySupplier(int Id)
        {
            using (MyDbContext con = new MyDbContext())
            {

                return con.DbSetOrderMaster.Include("Product").Include("Product.Images").Include("Customer").Where(x => x.Product.Supplier.Id == Id).ToList();
            }
        }
        public int GetOrderCount(int Id)
        {
            int count = 0;
            using (MyDbContext con = new MyDbContext())
            {

                count= con.DbSetOrderMaster.Where(x => x.Product.Supplier.Id == Id).ToList().Count;
                return count;
            }
        }
        public void ChangeOrderStatus()
        {

        }
    }
}
