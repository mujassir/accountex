using AccountEx.CodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Core.Objects;
using AccountEx.Common;

namespace AccountEx.Repositories
{
    public class InvoiceDcsRepository : GenericRepository<InvoiceDc>
    {
        public InvoiceDcsRepository() : base() { }
        public InvoiceDcsRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }

        public bool CheckIfSaleExistByDCId(int id)
        {
            return Collection.Any(p => p.DcId == id);
        }



    }
}