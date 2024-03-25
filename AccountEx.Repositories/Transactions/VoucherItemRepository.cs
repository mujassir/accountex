using System.Linq;
using AccountEx.CodeFirst.Models;
using System.Collections.Generic;
using AccountEx.Common;

namespace AccountEx.Repositories
{
    public class VoucherItemRepository : GenericRepository<VoucherItem>
    {
        public VoucherItemRepository() : base() { }
        public VoucherItemRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
       
    }
}