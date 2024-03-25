using AccountEx.CodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Core.Objects;
using AccountEx.Common;

namespace AccountEx.Repositories
{
    public class InvoiceClearingRepository : GenericRepository<InvoiceClearing>
    {
        public InvoiceClearingRepository() : base() { }
        public InvoiceClearingRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
    }
}
