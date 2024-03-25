using AccountEx.CodeFirst.Models;

namespace AccountEx.DbMapping
{

    public class CompanyExtra : Company
    {
        public string Type { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }


    }
}