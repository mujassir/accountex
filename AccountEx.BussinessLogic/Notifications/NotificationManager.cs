using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;

namespace AccountEx.BussinessLogic
{
    public static class NotificationManager
    {
        public static void AddTrigger(int referenceId, byte transactionTypeId, NotificationActions action, BaseRepository repo)
        {
            AddTrigger(new NotificationJobTrigger()
            {
                CompanyId = SiteContext.Current.User.CompanyId,
                CreatedAt = DateTime.Now,
                CreatedBy = SiteContext.Current.User.Id,
                ReferenceId = referenceId,
                TransactionTypeId = transactionTypeId,
                NotificationActionId = (byte)action

            }, repo);

        }

        public static void AddTrigger(NotificationJobTrigger trigger, BaseRepository repo)
        {

            //commented due to id not generating becouse save changes is set to false
            //will discuss it later 


            //new NotificationJobTriggerRepository(repo).Add(trigger, false, false);
        }


        public static void AddNotification(Notification notification, List<int> recipientUserIds)
        {
            var notificationRepo = new NotificationRepository();
            using (var scope = TransactionScopeBuilder.Create())
            {
                notificationRepo.Add(notification);

                notificationRepo.SaveChanges();

                var recipientRepo = new NotificationRecipientRepository(notificationRepo);
                foreach (var userId in recipientUserIds)
                {
                    recipientRepo.Add(new NotificationRecipient() { NotificationId = notification.Id, RecipientUserId = userId });

                }

                recipientRepo.SaveChanges();
                scope.Complete();
            }
        }

    }
}
