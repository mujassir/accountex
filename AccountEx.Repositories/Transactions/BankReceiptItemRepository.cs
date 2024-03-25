using AccountEx.CodeFirst.Models.COA;
using AccountEx.CodeFirst.Models.Transactions;
using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Repositories
{
    public class BankReceiptItemRepository : GenericRepository<BankReceiptItem>
    {
        public BankReceiptItemRepository() : base() { }
        public BankReceiptItemRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
    }
}
