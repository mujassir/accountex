using System.Linq;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using AccountEx.Repositories.Integrations;
using AccountEx.BussinessLogic.Integrations.Google.Calendar;
using AccountEx.Common.Integrations;

namespace AccountEx.BussinessLogic
{
    public static class GoogleCalendarManager
    {



        public static void SyncRudolfCalendarEventUsingHangfire()
        {
            try
            {
                var repo = new CalendarEventRepository();
                int companyId = 91;
                var users = repo.GetUseIdsforUnsyncEvents(companyId);
                foreach (var userId in users)
                {
                    try
                    {
                        SyncEvents(companyId, userId);
                    }
                    catch (Exception ex)
                    {

                        ErrorManager.Log(ex);
                    }

                }
            }
            catch (Exception ex)
            {

                ErrorManager.Log(ex);
            }
        }


        public static void SyncEvents(int companyId, int userId)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {

                var repo = new CalendarEventRepository();
                var tokenRepo = new UserTokenRepository(repo);
                OAuth2Helper OAuthhelper = new OAuth2Helper(AuthenticationType.googlecalendar);
                var userToken = tokenRepo.GetRefreshTokenByUserId(userId, companyId);
                if (userToken == null || userToken.Token == null)
                {
                    throw new OwnException("Please connect with google calendar.");
                }
                else
                {
                    OAuthhelper.GetAccessTokenFromRefreshToken(userToken.Token);
                    if (string.IsNullOrWhiteSpace(OAuthhelper.AccessToken))
                    {
                        throw new OwnException("Please connect again.Conenction has expired.");
                    }
                }

                var pendingEvents = repo.GetUnsyncPendingEvents(userId, companyId);
                var groupByCustomer = pendingEvents.GroupBy(p => new { p.CustomerId, p.Date }).ToList();
                var googleCalendarEvents = new List<GoogleCalendarEvent>();
                var reminderOverrides = new List<ReminderOverrides>() { new ReminderOverrides() { Method = "email", Minutes = 60 } };
                var reminder = new Reminders()
                {
                    UseDefault = false,
                    Overrides = reminderOverrides
                };
                foreach (var evnt in groupByCustomer)
                {
                    var googleEventId = "";
                    var crmEvent = evnt.FirstOrDefault();
                    var description = "";
                    var Ids = "";
                    foreach (var item in evnt.ToList())
                    {
                        description += item.Product + "," + item.Description + Environment.NewLine;
                        Ids += item.Id + ",";
                    }
                    if (evnt.Where(p => p.GoogleCalendarEeventId != null).Any())
                        googleEventId = evnt.Where(p => p.GoogleCalendarEeventId != null).FirstOrDefault().GoogleCalendarEeventId;
                    Ids = Ids.TrimEnd(',');
                    var googleCalendarEvent = new GoogleCalendarEvent()
                    {
                        Id = googleEventId,
                        summary = crmEvent.Customer + " (" + crmEvent.Mode + ")",
                        location = crmEvent.CustomerAddress,
                        description = description,
                        start = new GoogleCalendarEventDateTime() { dateTime = crmEvent.StartTime.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"), timeZone = "" },
                        end = new GoogleCalendarEventDateTime() { dateTime = crmEvent.EndTime.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"), timeZone = "" },
                        Ids = Ids,
                        reminders = new List<Reminders>() { reminder }

                    };
                    googleCalendarEvents.Add(googleCalendarEvent);

                }

                var query = "";
                foreach (var evnt in googleCalendarEvents)
                {
                    var googleCalendarEvent = OAuthhelper.CreateGoogleCalendarEvent(evnt);
                    if (googleCalendarEvent != null && !string.IsNullOrWhiteSpace(googleCalendarEvent.Id))
                        query += "UPDATE dbo.CalendarEvents SET GoogleCalendarEeventId='" + googleCalendarEvent.Id + "',GoogleCalendarEeventLink='" + googleCalendarEvent.HtmlLink + "' WHERE  CompanyId='" + companyId + "' and Id IN(" + evnt.Ids + ");";

                }
                if (!string.IsNullOrWhiteSpace(query))
                    repo.ExecuteQuery(query);
                repo.SaveChanges();
                scope.Complete();
            }

        }

    }

}
