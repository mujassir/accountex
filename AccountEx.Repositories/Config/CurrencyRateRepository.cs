using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Repositories
{
    public class CurrencyRateRepository : GenericRepository<CurrencyRate>
    {
        public List<CurrencyRateExtra> GetByDate(DateTime date)
        {
            return FiscalCollection.Where(p => p.FromDate >= date).GroupBy(p => p.CurrencyId).Select(p => new CurrencyRateExtra()
            {
                CurrencyId = p.Key,
                Rate = p.OrderByDescending(q => q.FromDate).FirstOrDefault().Rate
            }).ToList();
        }
    }
}
