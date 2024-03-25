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
    public class CalendarEventRepository : GenericRepository<CalendarEvent>
    {
        public CalendarEventRepository() : base() { }
        public CalendarEventRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }


        public override void Save(CalendarEvent PMC)
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
        public void UpdateGoogleEventId(int id, string eventId, string link = "", bool saveChanges = false)
        {
            var dbEvent = Collection.FirstOrDefault(p => p.Id == id);
            if (dbEvent != null)
            {
                dbEvent.GoogleCalendarEeventId = eventId;
                dbEvent.GoogleCalendarEeventLink = link;
            }
            if (saveChanges)
                SaveChanges();
        }

        public override void Add(CalendarEvent PMC)
        {
            base.Add(PMC, true, false);
        }
        public int GetNextVoucherNumber()
        {
            var maxnumber = ConfigurationReader.GetConfigKeyValue<int>("VoucherStartNumber", 1001);
            if (!Collection.Any())
                return maxnumber;
            return Collection.OrderByDescending(p => p.VisitNo).FirstOrDefault().VisitNo + 1;
        }
        public List<CRMCalendarEventExtra> GetEvents(DateTime date)
        {
            var query = string.Format("EXEC [dbo].[GetCRMEventsByDate] @COMPANYID={0}, @Date={1}, @UserId={2}", SiteContext.Current.User.CompanyId, "'" + date.ToString("yyyy-MM-dd") + "'", SiteContext.Current.User.Id);
            return Db.Database.SqlQuery<CRMCalendarEventExtra>(query).ToList();
        }
        public List<vw_CRMCalendarEvents> GetUnsyncPendingEvents(int userId, int companyId)
        {
            var date = DateTime.Now.AddHours(-1);
            return Db.vw_CRMCalendarEvents.Where(p => p.CompanyId == companyId && ((p.GoogleCalendarEeventId == null && userId == p.CreatedBy)) || (userId == p.CreatedBy && p.ModifiedAt > date)).ToList();
        }
        public List<int> GetUseIdsforUnsyncEvents(int companyId)
        {
            var date = DateTime.Now.AddHours(-1);
            var userIdsWithToken = Db.UserTokens.Where(p => p.CompanyId == companyId && p.Type == TokenType.Refresh && p.Token != null).Select(p => p.UserId).Distinct().ToList();
            if (userIdsWithToken.Count > 0)
                return Db.CalendarEvents.Where(p => p.CompanyId == companyId && ((p.GoogleCalendarEeventId == null && userIdsWithToken.Contains(p.CreatedBy)) || (userIdsWithToken.Contains(p.CreatedBy) && p.ModifiedAt > date))


                ).Select(p => p.CreatedBy).Distinct().ToList();
            else return new List<int>();
        }




    }
}