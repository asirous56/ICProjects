using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestCloudFrontTest.Models
{
    public class User:Person
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime ServiceStartDate { get; set; }

    }

}
