
using AccountEx.CodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Repositories
{
    public class CompanyPartnerRepository : GenericRepository<CompanyPartner>
    {
         public CompanyPartnerRepository() : base() { }
         public CompanyPartnerRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
    }
}

