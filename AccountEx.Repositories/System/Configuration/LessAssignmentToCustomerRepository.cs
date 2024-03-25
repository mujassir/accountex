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
    public class LessAssignmentToCustomerRepository : GenericRepository<GroupItemCustomerLess>
    {



        public IQueryable<DataTableCustomerGroupPromotionExtra> GetCustomerLess()
        {
            var companyId = SiteContext.Current.User.CompanyId;
            var cutomerLess = Collection;
            var groups = AsQueryable<ItemGroup>().Where(p => p.GroupType == (byte)GroupType.CustomerIncentiveGroup && p.GroupSubType == (byte)GroupSubType.Less);
            var Itemgroups = AsQueryable<ItemGroup>().Where(p => p.GroupType == (byte)GroupType.ItemGroup && p.GroupSubType == (byte)GroupSubType.Less); ;

            var records = Itemgroups.Join(cutomerLess, ig => ig.Id, cl => cl.ItemGroupId, (ig, cl) => new { ItemGroup = ig, CustomerLess = cl }).
                Join(groups, igcl => igcl.CustomerLess.CustomerGroupId, g => g.Id, (igcl, g) => new { CustomerLessWithItemGroup = igcl, Group = g })
             .GroupBy(p => new { p.CustomerLessWithItemGroup.CustomerLess.CustomerGroupId, p.CustomerLessWithItemGroup.CustomerLess.ItemGroupId }).Select(x => new DataTableCustomerGroupPromotionExtra()
             {
                 Group = x.FirstOrDefault().Group.Name,
                 GroupId = x.FirstOrDefault().Group.Id,
                 ItemGroup = x.FirstOrDefault().CustomerLessWithItemGroup.ItemGroup.Name,
                 ItemGroupId = x.FirstOrDefault().CustomerLessWithItemGroup.ItemGroup.Id,

             }).AsQueryable();
            return records;
        }
        public dynamic GetAllByFiscal()
        {
            var promostions = Collection.ToList();
            return promostions.Select(p => new
            {
                p.ItemId,
                p.CustomerId,
                p.LessRate

            }).ToList();
        }
        public void DeleteByCustomerGroupId(int groupid, int itemgroupid)
        {
            string query = "delete from GroupItemCustomerLesses where ItemGroupId=" + itemgroupid + "and CustomerGroupId=" + groupid + " and CompanyId=" + SiteContext.Current.User.CompanyId;
            Db.Database.ExecuteSqlCommand(query);
            Db.SaveChanges();
        }


    }
}
