using AccountEx.CodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Core.Objects;
using AccountEx.Common;
using AccountEx.CodeFirst.Models.CRM;

namespace AccountEx.Repositories
{
    public class CRMSaleInvoiceItemRepository : GenericRepository<CRMSaleInvoiceItem>
    {

        public CRMSaleInvoiceItemRepository() : base() { }
        public CRMSaleInvoiceItemRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public List<vw_CRMSaleInvoiceItems> GetInvoiceItemFromView(List<int> ids)
        {
            return AsQueryable<vw_CRMSaleInvoiceItems>(true).Where(p =>ids.Contains(p.Id)).ToList();
        }


    }
}
