using AccountEx.CodeFirst.Models;
using System;
using System.Linq;
using AccountEx.Common;
using System.Collections.Generic;

namespace AccountEx.Repositories
{
    public class DCItemRepository : GenericRepository<DCItem>
    {
        public DCItemRepository() : base() { }
        public DCItemRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public List<DCItem> GetByOrderNo(int orderno, VoucherType vtype)
        {
            return AsQueryable<DeliveryChallan>(true).SelectMany(p => p.DCItems).Where(p => p.OrderNo == orderno && p.TransactionType == vtype).ToList();
        }





    }
}