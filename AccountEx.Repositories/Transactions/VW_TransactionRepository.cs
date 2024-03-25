using System.Collections.Generic;
using System.Linq;
using AccountEx.CodeFirst.Models;

namespace AccountEx.Repositories
{
    public class VwTransactionRepository : GenericRepository<vw_Transaction>
    {


        public List<vw_Transaction> GetById(int id)
        {
            var masterdetail = FiscalCollection.FirstOrDefault(p => p.Id == id);
            var data = FiscalCollection.Where(p => p.VoucherNumber == masterdetail.VoucherNumber && p.TransactionType == masterdetail.TransactionType).ToList();
            return data;
        }

    }
}