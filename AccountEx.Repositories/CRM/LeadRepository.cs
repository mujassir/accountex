using AccountEx.CodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Core.Objects;
using AccountEx.Common;

namespace AccountEx.Repositories
{
    public class LeadRepository : GenericRepository<Lead>
    {
        public int GetNextLeadNo()
        {
            int LeadNo;
            if (Collection.Any(Lead => Lead.LeadNo >= 0))
            {
                LeadNo = Collection.Max(Lead => Lead.LeadNo) + 1;
            }
            else
            {
                LeadNo = 1;
            }
            return LeadNo;
        }

        public override void Update(Lead lead)
        {
            //var query = "Delete from LeadConcernedItems where LeadId=" + lead.Id + " and CompanyId=" + SiteContext.Current.User.CompanyId;
            //Db.Database.ExecuteSqlCommand(query);
            //foreach (var item in lead.LeadConcernedItems)
            //{
            //    item.LeadId = lead.Id;
            //    Db.LeadConcernedItems.Add(item);
            //}

            //lead.LeadConcernedItems = null;

            //var query2 = "Delete from LeadExpectedItems where LeadId=" + lead.Id + " and CompanyId=" + SiteContext.Current.User.CompanyId;
            //Db.Database.ExecuteSqlCommand(query2);
            //foreach (var item in lead.LeadExpectedItems)
            //{
            //    item.LeadId = lead.Id;
            //    Db.LeadExpectedItems.Add(item);
            //}

            //lead.LeadExpectedItems = null;
            //base.Update(lead,true,false);

            var dbLeadConcItem = GetById(lead.Id, true);
            var ExpIds = lead.LeadConcernedItems.Select(p => p.Id).ToList();
            var expDeletedIds = dbLeadConcItem.LeadConcernedItems.Where(p => !ExpIds.Contains(p.Id)).Select(p => p.Id).ToList();
            new LeadConcernedItemRepository(this).Delete(expDeletedIds);
            new LeadConcernedItemRepository(this).Save(false, lead.LeadConcernedItems.ToList());

            var dbLeadExpItem = GetById(lead.Id, true);
            var ConcIds = lead.LeadExpectedItems.Select(p => p.Id).ToList();
            var concDeletedIds = dbLeadExpItem.LeadExpectedItems.Where(p => !ConcIds.Contains(p.Id)).Select(p => p.Id).ToList();
            new LeadExpectedItemRepository(this).Delete(concDeletedIds);
            new LeadExpectedItemRepository(this).Save(false, lead.LeadExpectedItems.ToList());
            base.Update(lead, true, false);


        }
        public List<Lead> GetByLeadId(int leadid)
        {
            return Collection.Where(p => p.Id == leadid).ToList();

        }
        public void DeleteByLeadId(int leadId)
        {
            string query = "Delete from Leads where LeadId=" + leadId + " and CompanyId=" + SiteContext.Current.User.CompanyId;
            Db.Database.ExecuteSqlCommand(query);
            Db.SaveChanges();
        }
    }
}
