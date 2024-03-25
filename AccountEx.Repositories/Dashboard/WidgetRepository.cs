using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.CodeFirst.Models;

namespace AccountEx.Repositories
{
    public class WidgetRepository : GenericRepository<Widget>
    {
          public WidgetRepository() : base() { }
          public WidgetRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public List<Widget> GetByDashboard(int companyId, int dashboardId, DateTime lastLoadTime)
        {
            var query = string.Format("EXEC GetDashboardWidgets @CompanyId = {0}, @DashboardId = {1}, @LastLoadTime = '{2}'"
                                    , companyId, dashboardId, lastLoadTime.ToString("yyyy-MM-dd hh:mm:ss"));

            return Db.Database.SqlQuery<Widget>(query).ToList();
        }

        public List<Widget> GetByDashboard(int dashboardId)
        {
          
            var query = from dw in Db.DashboardWidgets
                        join w in Db.Widgets on dw.WidgetId equals w.Id
                        where dw.DashboardId == dashboardId
                        select w;
            return query.ToList();

        }

        public override Widget GetById(int id)
        {
            return Db.Widgets.FirstOrDefault(p => p.Id == id);
        }

        public List<WidgetParameter> GetParameters(int widgetId)
        {
            return Db.WidgetParameters.Where(p => p.WidgetId == widgetId).ToList();
        }
    }
}
