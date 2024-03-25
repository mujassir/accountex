using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.CodeFirst.Models;

namespace AccountEx.Repositories
{
    public class WidgetColumnRepository : GlobalGenericRepository<WidgetColumn>
    {
          public WidgetColumnRepository() : base() { }
          public WidgetColumnRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }






          public List<WidgetColumn> GetColumn(int widgetId)
        {
            return Collection.Where(p => p.WidgetId == widgetId).ToList();
        }
    }
}
