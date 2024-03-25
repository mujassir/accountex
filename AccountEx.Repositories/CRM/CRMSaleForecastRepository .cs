using AccountEx.CodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Core.Objects;
using AccountEx.Common;
using AccountEx.CodeFirst.Models.CRM;
using AccountEx.Common.CRM;

namespace AccountEx.Repositories
{
    public class CRMSaleForecastRepository : GenericRepository<CRMSaleForecast>
    {
        public CRMSaleForecastRepository() : base() { }
        public CRMSaleForecastRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }


        public override void Save(CRMSaleForecast PMC)
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

        public override void Add(CRMSaleForecast PMC)
        {
            base.Add(PMC, true, false);
        }
        public List<CRMCalendarEventExtra> GetEvents(DateTime date)
        {
            var query = string.Format("EXEC [dbo].[GetCRMEventsByDate] @COMPANYID={0}, @Date={1}", SiteContext.Current.User.CompanyId, "'" + date.ToString("yyyy-MM-dd") + "'");
            return Db.Database.SqlQuery<CRMCalendarEventExtra>(query).ToList();
        }
        public override void Update(CRMSaleForecast PMC)
        {
            //var PMCItemsRepo = new PMCItemRepository(this);
            //var dbPMC = GetById(PMC.Id, true);

            ////add,update & remove services items
            //var Ids = PMC.PMCItems.Select(p => p.Id).ToList();
            //var deletedIds = dbPMC.PMCItems.Where(p => !Ids.Contains(p.Id)).Select(p => p.Id).ToList();
            //PMCItemsRepo.Delete(deletedIds);
            //PMCItemsRepo.Save(PMC.PMCItems.ToList());


            ////SaveChanges();
            //PMC.PMCItems = null;
            //base.Update(PMC, true, false);
        }




    }
}