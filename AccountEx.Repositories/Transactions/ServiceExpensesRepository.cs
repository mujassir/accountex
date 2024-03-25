using AccountEx.CodeFirst.Models;
using System.Collections.Generic;
using System.Linq;
using AccountEx.Common;

namespace AccountEx.Repositories
{
    public class ServiceExpensesRepository : GenericRepository<ServiceExpense>
    {
         public ServiceExpensesRepository() : base() { }
         public ServiceExpensesRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public void DeleteBySaleId(int saleId)
        {
            string query = "Delete from ServiceExpenses where SaleId=" + saleId + " and CompanyId=" + SiteContext.Current.User.CompanyId;
            Db.Database.ExecuteSqlCommand(query);
            Db.SaveChanges();
        }

    }
}
