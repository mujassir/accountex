using AccountEx.CodeFirst.Models;
using System;
using System.Linq;
using AccountEx.Common;
using System.Collections.Generic;

namespace AccountEx.Repositories
{
    public class InvoiceDcRepository : GenericRepository<InvoiceDc>
    {
        public InvoiceDcRepository() : base() { }
        public InvoiceDcRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }



        public bool CheckIfSaleExistByDCId(int dcId, int saleId)
        {
            return Collection.Any(p => p.DcId == dcId && p.SaleId != saleId);
        }




    }
}