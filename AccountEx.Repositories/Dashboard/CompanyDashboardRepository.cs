using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;

namespace AccountEx.Repositories
{
    public class CompanyDashboardRepository : GenericRepository<CompanyDashboard>
    {
        public List<IdName> GetAvailableDashBoard()
        {
            var ids = Collection.Select(p => p.DashboardId).ToList();
            return Db.Dashboards.Where(p => ids.Contains(p.Id)).Select(p => new IdName
            {
                Id = p.Id,
                Name = p.Title,
            }).ToList();
        }
    }
}
