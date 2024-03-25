using AccountEx.CodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Core.Objects;
using AccountEx.Common;
using AccountEx.CodeFirst.Models.CRM;
using AccountEx.Common.CRM;
using System.Data.Entity;

namespace AccountEx.Repositories
{
    public class CRMCompliantRepository : GenericRepository<CRMComplaint>
    {
        public CRMCompliantRepository() : base() { }
        public CRMCompliantRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }


        public override void Save(CRMComplaint PMC)
        {

            if (PMC.Id == 0)
            {
                Add(PMC);
            }
            else
            {
                Update(PMC);
            }
        }

        public override void Add(CRMComplaint PMC)
        {
            base.Add(PMC, true, false);
        }
        public int GetNextVoucherNumber()
        {
            var maxnumber = ConfigurationReader.GetConfigKeyValue<int>("VoucherStartNumber", 1001);
            if (!Collection.Any())
                return maxnumber;
            return Collection.OrderByDescending(p => p.VoucherNo).FirstOrDefault().VoucherNo + 1;
        }
        public void ChangeStatus(int id, CRMComplaintStatus statusId)
        {

            var c = Collection.FirstOrDefault(p => p.Id == id);
            if (c != null)
            {
                c.StatusId = statusId;
                if (statusId == CRMComplaintStatus.Resolved)
                {
                    c.ResolvedById = SiteContext.Current.User.Id;
                    c.ResolvedDate = DateTime.Now;
                }
                else if (statusId == CRMComplaintStatus.Closed)
                {
                    c.ClosedById = SiteContext.Current.User.Id;
                    c.ClosedDate = DateTime.Now;
                }
            }



        }
        public List<vw_CRMCompliants> GetPendingComplaints(int regionId)
        {
            return AsQueryable<vw_CRMCompliants>().AsNoTracking().Where(p => p.RegionId == regionId && p.StatusId != CRMComplaintStatus.Closed).ToList();
        }
        public void Update(List<CRMComplaint> compliants)
        {
            var ids = compliants.Select(p => p.Id).ToList();
            var dbComplaints = Collection.Where(p => ids.Contains(p.Id)).ToList();

            foreach (var c in compliants)
            {
                var dbComplaint = dbComplaints.FirstOrDefault(p => p.Id == c.Id);
                Db.Entry(dbComplaint).CurrentValues.SetValues(c);
                Db.Entry(dbComplaint).State = EntityState.Modified;
            }

        }




    }
}