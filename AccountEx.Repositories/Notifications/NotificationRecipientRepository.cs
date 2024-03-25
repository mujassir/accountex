using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.CodeFirst.Models;

namespace AccountEx.Repositories
{
    public class NotificationRecipientRepository : GenericRepository<NotificationRecipient>
    {
        public NotificationRecipientRepository(BaseRepository baseRepo)
        {
            this.Db = baseRepo.GetContext();
        }
    }
}
