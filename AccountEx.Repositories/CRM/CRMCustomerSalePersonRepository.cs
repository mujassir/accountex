using AccountEx.CodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Core.Objects;
using AccountEx.Common;
using AccountEx.CodeFirst.Models.CRM;

namespace AccountEx.Repositories
{
    public class CRMCustomerSalePersonRepository : GenericRepository<CRMCustomerSalePerson>
    {

        public CRMCustomerSalePersonRepository() : base() { }
        public CRMCustomerSalePersonRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public List<IdName> GetSalePersonByCatgeoryId(int customerId, int categoryId)
        {
            var salePersonIds = Collection.Where(p => p.CRMCustomerId == customerId && p.CategroyId == categoryId).Select(p => p.UserId).ToList();
            return new UserRepository().GetIdNames(salePersonIds);
        }


    }
}
