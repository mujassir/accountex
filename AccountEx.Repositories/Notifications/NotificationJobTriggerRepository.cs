using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.CodeFirst.Models;

namespace AccountEx.Repositories
{
    public class NotificationJobTriggerRepository : GenericRepository<NotificationJobTrigger>
    {
         public NotificationJobTriggerRepository() : base() { }
         public NotificationJobTriggerRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
    }
}
