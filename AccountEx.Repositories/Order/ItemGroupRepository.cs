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
   public class ItemGroupRepository : GenericRepository<ItemGroup>
    {
       public override void Update(ItemGroup entity)
       {
           var query = "delete from ItemGroupItems where ItemGroupId=" + entity.Id + " and CompanyId=" + SiteContext.Current.User.CompanyId;
           Db.Database.ExecuteSqlCommand(query);
           foreach(var item in entity.ItemGroupItems)
           {
               item.ItemGroupId = entity.Id;
               item.ModifiedAt = DateTime.Now;
               item.ModifiedBy = SiteContext.Current.User.Id;
               Db.ItemGroupItems.Add(item);
           }
           entity.ItemGroupItems = null;
           base.Update(entity);
       }
       public List<ItemGroup> GetPromotionItemGroups() 
       {
          return Collection.Where(p => p.GroupType == (byte)GroupType.ItemGroup).Where(p => p.GroupSubType == (byte)GroupSubType.Promotion).ToList();
       }

       public List<ItemGroup> GetPromotionCustomerGroups() 
       {
          return Collection.Where(p => p.GroupType == (byte)GroupType.CustomerIncentiveGroup).Where(p => p.GroupSubType == (byte)GroupSubType.Promotion).ToList();
       }
       public List<ItemGroup> GetLessItemGroups()
       {
           return Collection.Where(p => p.GroupType == (byte)GroupType.ItemGroup).Where(p => p.GroupSubType == (byte)GroupSubType.Less).ToList();
       }
       public List<ItemGroup> GetLessCustomerGroups()
       {
           return Collection.Where(p => p.GroupType == (byte)GroupType.CustomerIncentiveGroup).Where(p => p.GroupSubType == (byte)GroupSubType.Less).ToList();
       }
      

    }
}
