using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.BussinessLogic;

namespace AccountEx.NotificationServices
{
    partial class NotificationService : ServiceBase
    {

        Timer dataRefreshTimer;
        Timer triggerTimer;
        Timer queueTimer;


        static BlockingCollection<NotificationJobTrigger> TriggerQueue { get; set; }

        public NotificationService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Logger.ApplicationName = "NotificationService";
            InitTimers();
        }


        private void InitTimers()
        {

            try
            {
                dataRefreshTimer = new Timer(ConfigurationReader.GetConfigKeyValue("dataRefreshTimer_Interval", 20 * 1000));
                dataRefreshTimer.Elapsed += dataRefreshTimer_Elapsed;

                triggerTimer = new Timer(ConfigurationReader.GetConfigKeyValue("triggerTimer_Interval", 5 * 1000));
                triggerTimer.Elapsed += triggerTimer_Elapsed;

                dataRefreshTimer.Start();
                triggerTimer.Start();

                queueTimer = new Timer(100);
                queueTimer.Elapsed += queueTimer_Elapsed;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }


        }

        void queueTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                triggerTimer.Stop();
                ProcessQueue();

            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            finally
            {

                //triggerTimer.Start();
            }
        }

        void triggerTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                triggerTimer.Stop();
                var triggers = new NotificationJobTriggerRepository().GetAll(p => p.IsProcessed == false);
                foreach (var item in triggers)
                {
                    TriggerQueue.Add(item);
                }

            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            finally
            {

                triggerTimer.Start();
            }
        }

        void dataRefreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                dataRefreshTimer.Stop();

                if (DataCache.IsInitialized == false)
                    DataCache.InitCache();
                else
                    DataCache.RefreshCache();
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            finally
            {

                dataRefreshTimer.Start();
            }
        }


        // Process Queue

        public void ProcessQueue()
        {
            var options = new ParallelOptions() { MaxDegreeOfParallelism = ConfigurationReader.GetConfigKeyValue("MaxNoOfThreads", 4) };

            Parallel.ForEach(TriggerQueue.GetConsumingEnumerable(), options, trigger =>
            {
                try
                {
                    if (DataCache.Jobs.ContainsKey(trigger.CompanyId.Value))
                    {
                        var jobs = DataCache.Jobs[trigger.CompanyId.Value].Select(p => p.Value).ToList();
                        foreach (var job in jobs)
                        {
                            if (DataCache.IsUserMonitored(trigger, job) == false) continue;

                            if (job.NotificationJobTypeId == (byte)NotificationJobTypes.ActionBased)
                            {
                                switch ((NotificationJobTypes)job.NotificationJobTypeId)
                                { 
                                    case  NotificationJobTypes.ActionBased:
                                        break;
                                    case NotificationJobTypes.Scheduled:
                                        break;
                                    case NotificationJobTypes.Threshold:
                                        break;

                                }
                                if (DataCache.Actions[job.Id].Any(p => p.Value.TransactionTypeId == trigger.TransactionTypeId
                                    && p.Value.NotificationActionId == trigger.NotificationActionId))
                                {

                                    var notification = GetNotification(trigger);
                                    var userIds = GetRecipientUserIds(job.Id);
                                    NotificationManager.AddNotification(notification, userIds);

                                }
                            }

                        }
                    }



                    // Update trigger as processed so doesn't get back again
                    trigger.IsProcessed = true;
                    trigger.ModifiedAt = DateTime.Now;
                    trigger.ModifiedBy = 1;
                    new NotificationJobTriggerRepository().Update(trigger);
                }
                catch (Exception ex)
                {

                    Logger.Log(ex);
                }
            });

        }



        #region private methods

        private Notification GetNotification(NotificationJobTrigger trigger)
        {
            var action = DataCache.NotificationActions[trigger.NotificationActionId];
            var user = DataCache.Users[trigger.CreatedBy];
            var transactionType = DataCache.TransactionTypes[trigger.TransactionTypeId];


            var notification = new Notification();

            notification.Subject = action.NotificationTemplate.Replace("[Username]", user.GetFullName())
                                                              .Replace("[Id]", trigger.ReferenceId + "");

            notification.Subject += " in module " + transactionType.Description;

            return notification;
        }

        private List<int> GetRecipientUserIds(int jobId)
        {

            var contacts = DataCache.Contacts[jobId].Select(p => p.Value).ToList();

            var userIds = new List<int>();

            foreach (var item in contacts)
            {
                if (item.IsAllUsers)
                {
                    return DataCache.Users.Select(p => p.Value.Id).ToList();
                }

                if (item.UserId.HasValue && DataCache.Users.ContainsKey(item.UserId.Value) == true)
                {
                    userIds.Add(item.UserId.Value);
                }

                if (item.RoleId.HasValue && DataCache.RoleUsers.ContainsKey(item.RoleId.Value))
                {
                    userIds.AddRange(DataCache.RoleUsers[item.RoleId.Value].Select(p => p.Value.Id).ToList());
                }

                if (item.IsAdminRole)
                {
                    userIds.AddRange(DataCache.Users.Where(p => p.Value.IsAdmin).Select(p => p.Value.Id).ToList());
                }

            }

            return userIds;

        }


        #endregion



        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }
    }
}
