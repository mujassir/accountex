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
    public class PromotionRepository : GenericRepository<Promotion>
    {
        public bool IsAlreadyExistInDates(int id, DateTime fromdate, DateTime todate, out string configName)
        {
            configName = "";
            if (id != 0) return false;
            var config = Collection.FirstOrDefault(p =>
                (fromdate >= p.FromDate && fromdate <= p.ToDate)
                || (todate >= p.FromDate && todate <= p.ToDate)
                );
            if (config != null)
            {
                configName = config.Name;
                return true;
            }
            return false;

        }

        public Promotion GetByDate(DateTime date)
        {
            return FiscalCollection.FirstOrDefault(p => p.FromDate > date && p.ToDate <= date);
        }

        public dynamic GetAllByFiscal()
        {
            var promostions = FiscalCollection.ToList();
            return promostions.SelectMany(p => p.PromotionItems).Select(p => new
            {
                p.ItemId,
                CustomerId=p.CustomerId,
                p.PromotionRatePurchase,
                p.PromotionRateSale,
                FromDate = promostions.First(q => q.Id == p.PromotionId).FromDate.Date,
                ToDate = promostions.First(q => q.Id == p.PromotionId).ToDate.Date
            }).ToList();
        }

        public override void Update(Promotion input)
        {
            string query = "delete from PromotionItems where PromotionId=" + input.Id + "and CustomerId is null and CompanyId=" + SiteContext.Current.User.CompanyId;
            Db.Database.ExecuteSqlCommand(query);
            foreach (var item in input.PromotionItems)
            {
                item.PromotionId = input.Id;
                item.ModifiedAt = DateTime.Now;
                item.ModifiedBy = SiteContext.Current.User.Id;
                Db.PromotionItems.Add(item);
            }
            input.PromotionItems = null;
            base.Update(input);
        }
        public void SaveCustomerPromotions(Promotion input)
        {

            new PromotionItemRepository().Save(input.PromotionItems.ToList());

        }


        public IQueryable<DataTableCustomerGroupPromotionExtra> GetCustomerPromotions()
        {
            var companyId = SiteContext.Current.User.CompanyId;
            var promotions = FiscalCollection;
            var groups = AsQueryable<ItemGroup>();

            var records = AsQueryable<PromotionItem>().Join(promotions, pi => pi.PromotionId, p => p.Id, (pi, p) => new { Promotion = p, PromotionItems = pi }).
                Join(groups, ppi => ppi.PromotionItems.CustomerGroupId, g => g.Id, (ppi, g) => new { PromotionWithItem = ppi, Group = g }).
             Where(p => p.PromotionWithItem.PromotionItems.CustomerGroupId.HasValue).GroupBy(p => new { p.PromotionWithItem.PromotionItems.CustomerGroupId, p.PromotionWithItem.PromotionItems.PromotionId }).Select(x => new DataTableCustomerGroupPromotionExtra()
               {
                   Group = x.FirstOrDefault().Group.Name,
                   GroupId = x.FirstOrDefault().Group.Id,
                   PromotionId = x.FirstOrDefault().PromotionWithItem.Promotion.Id,
                   Promotion = x.FirstOrDefault().PromotionWithItem.Promotion.Name,
                   FromDate = x.FirstOrDefault().PromotionWithItem.Promotion.FromDate,
                   ToDate = x.FirstOrDefault().PromotionWithItem.Promotion.ToDate,

               }).AsQueryable();
            return records;
        }
    }
}
