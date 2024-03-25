using System.Linq;
using AccountEx.CodeFirst.Models;
using System.Collections.Generic;
using AccountEx.Common;

namespace AccountEx.Repositories
{
    public class LeadExpectedItemRepository : GenericRepository<LeadExpectedItem>
    {
        public LeadExpectedItemRepository() : base() { }
        public LeadExpectedItemRepository(BaseRepository repo) {
            base.Db = repo.GetContext();
        }

        public override void Save(List<LeadExpectedItem> input)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var leadid = input.FirstOrDefault().LeadId;
                DeleteLeadExpectedItems(leadid);
                base.Save(input);
                scope.Complete();
            }
        }
        public void DeleteLeadExpectedItems(int leadid)
        {
            var items = Db.LeadExpectedItems.Where(p => p.LeadId == leadid && p.CompanyId == SiteContext.Current.User.CompanyId).ToList();
            foreach (var item in items)
            {
                Db.LeadExpectedItems.Remove(item);
            }
            Db.SaveChanges();
        }
        public List<LeadExpectedItem> GetByLeadId(int leadid)
        {
            return Collection.Where(p => p.LeadId == leadid).ToList();
            
        }
    }
}