using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;

namespace AccountEx.Repositories
{
    public class DashboardRepository : GenericRepository<Dashboard>
    {
        public Dashboard GetByName(string name)
        {
            return Db.Dashboards.FirstOrDefault(p => p.Name == name);
        }
        public Dashboard GetDashboardById(int id)
        {
            return Db.Dashboards.FirstOrDefault(p => p.Id == id);
        }



    }
}
