using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ILibrary.HRM
{
    public class HRHandler
    {
        public Supplier GetSupplierbyLoginID(int Id)
        {
            using (MyDbContext con = new MyDbContext())
            {
                return con.DbSetSupplier.Where(x => x.LoginUser.Id == Id).FirstOrDefault();
            }
        }
       public int CheckSupplier(int Id)
        {
            using (MyDbContext con = new MyDbContext())
            {
               return con.DbSetSupplier.Where(x => x.LoginUser.Id == Id).FirstOrDefault().Id;
               
            }
        }
        
        public LoginUsers GetLogin(string email, string password)
        {
            using (MyDbContext con = new MyDbContext())
            {
                return con.DbSetLoginUser.Where(x => x.Email == email).Where(x => x.Password == password).FirstOrDefault();
            }

        }
        public List<City> GetCity()
        {
            using (MyDbContext con = new MyDbContext())
            {
                return con.DbSetCity.ToList();
            }

        }
        public List<City> GetCity(int Id)
        {
            using (MyDbContext con = new MyDbContext())
            {
                return con.DbSetCity.Where(x=>x.Country.Id==Id).ToList();
            }

        }
        public List<Country> GetCountry()
        {
            using (MyDbContext con = new MyDbContext())
            {
                return con.DbSetCountry.ToList();
            }

        }
        public void AddCountry(string name)
        {
            using (MyDbContext con = new MyDbContext())
            {
                Country cc = con.DbSetCountry.Where(x => x.Name == name).FirstOrDefault();
                if (cc == null)
                {
                    Country c = new Country();
                    c.Name = name;
                    con.DbSetCountry.Add(c);
                    con.SaveChanges();
                }
            }
        }
        public void AddCity(string countryName, string CityName)
        {
            using (MyDbContext con = new MyDbContext())
            {
                Country c = con.DbSetCountry.Where(x => x.Name == countryName).FirstOrDefault();
                City city = new City();
                city.Name = CityName;
                city.Country = c;
                con.Entry(city.Country).State = System.Data.Entity.EntityState.Unchanged;
                con.DbSetCity.Add(city);
                con.SaveChanges();
            }

        }
        public Customer GetCustomer(string username, string password)
        {
            using (MyDbContext con = new MyDbContext())
            {
                return  con.DbSetCustomer.Include("LoginUsers").Include("Person").Include("Person.StretAddress").Include("Person.StretAddress.City").Include("Person.StretAddress.City.Country").Where(x => x.LoginUsers.UserName == username && x.LoginUsers.Password == password).FirstOrDefault();
            }
        }
        public Customer GetCustomer(LoginUsers lu)
        {
            using (MyDbContext con = new MyDbContext())
            {
                return con.DbSetCustomer.Include("LoginUsers").Include("Person").Include("Person.StretAddress").Include("Person.StretAddress.City").Include("Person.StretAddress.City.Country").Where(x => x.LoginUsers.Id == lu.Id).FirstOrDefault();
            }
        }

       // [Obsolete("Do not use this in Production code!!!", true)]
        static void NEVER_EAT_POISON_Disable_CertificateValidation()
        {
            //Disabling certificate validation can expose you to a man -in-the - middle attack
            //   which may allow your encrypted message to be read by an attacker
            //   https://stackoverflow.com/a/14907718/740639
            ServicePointManager.ServerCertificateValidationCallback =
                delegate (
                    object s,
                    X509Certificate certificate,
                    X509Chain chain,
                    SslPolicyErrors sslPolicyErrors
                )
                {
                    return true;
                };
        }
        public void SendEmail(string Email, string UserName)
        {
            
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            int Hascode = DateTime.Now.GetHashCode();
            using (MyDbContext con = new MyDbContext())
            {
                ResetPass rp = new ResetPass();
                rp.IsActive = true;
                rp.Email = Email;
                rp.HashCode = Hascode;
                rp.RequestedTime = DateTime.Now;
                con.DbSetResetPass.Add(rp);
                con.SaveChanges();
            }

            NEVER_EAT_POISON_Disable_CertificateValidation();

            MailMessage m = new MailMessage();
            SmtpClient s = new SmtpClient("fanco.com.pk", 465);
            //var random = new Random(2006);
            m.From = new MailAddress("noreply@fanco.com.pk", "System Generated E-Mail");
            m.To.Add(new MailAddress(Email, "Dear User"));
            m.Subject = "Reset Password Request";
            m.Body = $"We have recieved Request for Reset Password if you want to proceed Please Click on The Link <a href=\"https://fanco.com.pk/Person/ResetPassword/?code={Hascode} \">Click Here<//a><\\br> and if you havent requested Delete this Email. Donot Share This Password with anyone. ";
            s.Port = 465;
            s.Host = "fanco.com.pk";
            s.EnableSsl = true;
            s.UseDefaultCredentials = true;
            s.DeliveryMethod = SmtpDeliveryMethod.Network;
            s.DeliveryFormat = SmtpDeliveryFormat.International;
            s.Credentials = CredentialCache.DefaultNetworkCredentials;
            s.Credentials = new System.Net.NetworkCredential("noreply@fanco.com.pk", "Lahore@123");
            s.Send(m);
        }
     
    }
}
