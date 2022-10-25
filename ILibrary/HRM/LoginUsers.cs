using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILibrary.HRM
{
    public class LoginUsers
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Logintype LoginType { get; set; }
        public string Email { get; set; }
        public enum Logintype
        {
            user,
            supplier,
            master
        }
    }
}
