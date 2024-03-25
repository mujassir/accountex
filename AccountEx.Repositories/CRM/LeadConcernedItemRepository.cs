using System.Linq;
using AccountEx.CodeFirst.Models;
using System.Collections.Generic;
using AccountEx.Common;

namespace AccountEx.Repositories
{
    public class LeadConcernedItemRepository : GenericRepository<LeadConcernedItem>
    {
        public LeadConcernedItemRepository() : base() { }
        public LeadConcernedItemRepository(BaseRepository repo) {
            base.Db = repo.GetContext();
        }

        public override void Save(List<LeadConcernedItem> input)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var leadid = input.FirstOrDefault().LeadId;
                DeleteLeadConcernedItems(leadid);
                base.Save(input);
                scope.Complete();
            }
        }

        public void DeleteLeadConcernedItems(int leadid)
        {
            var items = Db.LeadConcernedItems.Where(p => p.LeadId == leadid && p.CompanyId == SiteContext.Current.User.CompanyId).ToList();
            foreach (var item in items)
            {
                Db.LeadConcernedItems.Remove(item);
            }
            Db.SaveChanges();
        }

        public List<LeadConcernedItem> GetByLeadId(int leadid)
        {
            return Collection.Where(p => p.LeadId == leadid).ToList();

        }
    }
}