using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.COA;
using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Repositories
{
    public class RentAgreementScheduleRepository : GenericRepository<RentAgreementSchedule>
    {
        public RentAgreementScheduleRepository() : base() { }
        public RentAgreementScheduleRepository(BaseRepository repo)
        {
            Db = repo.GetContext();
        }

        public void UpdateSchedules(RentAgreement entity)
        {
            var query = "Delete from RentAgreementSchedules where RentAgreementId=" + entity.Id + " and CompanyId=" + SiteContext.Current.User.CompanyId;
            Db.Database.ExecuteSqlCommand(query);

            foreach (var item in entity.RentAgreementSchedules)
            {
                item.RentAgreementId = entity.Id;
                Db.RentAgreementSchedules.Add(item);
            }

        }

        public void RenewSchedules(RentAgreement entity)
        {
            var currentSchedules = Db.RentAgreementSchedules.Where(p => p.RentAgreementId == entity.Id && p.IsRenewed == false);

            foreach (var item in currentSchedules)
            {
                item.IsRenewed = true;
                item.ModifiedAt = DateTime.Now;
                item.ModifiedBy = SiteContext.Current.User.Id;
            }

            foreach (var item in entity.RentAgreementSchedules)
            {
                item.RentAgreementId = entity.Id;
                Db.RentAgreementSchedules.Add(item);
            }

        }



    }
}
