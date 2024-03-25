using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.CodeFirst;
using AccountEx.Common;
using AccountEx.CodeFirst.Models;

namespace AccountEx.Repositories
{
    public class PromotionItemRepository : GenericRepository<PromotionItem>
    {
        public void DeleteByCustomerGroupId(int groupid,int promotionid) 
        {
            string query = "delete from PromotionItems where PromotionId=" + promotionid + "and CustomerGroupId=" + groupid + " and CompanyId=" + SiteContext.Current.User.CompanyId;
            Db.Database.ExecuteSqlCommand(query);
            Db.SaveChanges();
        }

    }
}
