using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.CodeFirst;
using AccountEx.Common;
using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.Vehicles;

namespace AccountEx.Repositories
{
    public class SupplierCurrencyRepository : GenericRepository<SupplierCurrency>
    {

        public void Update(List<SupplierCurrency> currencis, int accountId)
        {
            var query = "Delete from SupplierCurrencies where AccountId=" + accountId + " and CompanyId=" + SiteContext.Current.User.CompanyId;
            Db.Database.ExecuteSqlCommand(query);
            foreach (var item in currencis)
            {
                item.ModifiedAt = DateTime.Now;
                item.ModifiedBy = SiteContext.Current.User.Id;
                Db.SupplierCurrencies.Add(item);
            }
        }

    }
}
