using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AccountEx.CodeFirst.Models;
using AccountEx.Repositories;

namespace AccountEx.NotificationServices
{
    public static class DataCache
    {
        private static DateTime LastModified { get; set; }
        public static bool IsInitialized { get; private set; }
        //public static Dictionary<int, NotificationJobType> NotificationJobTypes { get; set; }
        //public static Dictionary<int, NotificationAction> NotificationActions { get; set; }


        #region Public Properties

        // Key CompanyId and Dictionary of Jobs of that company
        public static Dictionary<int, Dictionary<int, NotificationJob>> Jobs { get; set; }
        // Key JobId and Dictionary of Actions of that Job
        public static Dictionary<int, Dictionary<int, NotificationJobAction>> Actions { get; set; }
        // Key JobId and Dictionary of Actors of that Job
        public static Dictionary<int, Dictionary<int, NotificationJobActor>> Actors { get; set; }
        // Key JobId and Dictionary of Actors of that Job
        public static Dictionary<int, Dictionary<int, NotificationJobContact>> Contacts { get; set; }

        public static Dictionary<int, User> Users { get; set; }

        // Key RoleId and Dictionary of Users of that Role
        public static Dictionary<int, Dictionary<int, User>> RoleUsers { get; set; }

        public static Dictionary<int, NotificationAction> NotificationActions { get; set; }
        public static Dictionary<int, TransactionType> TransactionTypes { get; set; }


        #endregion

        // constructor
        //static DataCache()
        //{
        //    Jobs = new Dictionary<int, NotificationJob>();
        //    Actions = new Dictionary<int, List<NotificationJobAction>>();
        //    Actors = new Dictionary<int, List<NotificationJobActor>>();
        //    Contacts = new Dictionary<int, List<NotificationJobContact>>();
        //}

        public static void InitCache()
        {
            Jobs = new NotificationJobRepository().GetAll(p => p.CompanyId.HasValue == true && p.IsActive)
                        .GroupBy(p => p.CompanyId)
                        .ToDictionary(p => p.Key.Value, q => q.ToDictionary(a => a.Id, b => b));

            Actions = new NotificationJobActionRepository().GetAll()
                        .GroupBy(p => p.NotificationJobId)
                        .ToDictionary(p => p.Key, q => q.ToDictionary(a => a.Id, b => b));

            Actors = new NotificationJobActorRepository().GetAll()
                        .GroupBy(p => p.NotificationJobId)
                        .ToDictionary(p => p.Key, q => q.ToDictionary(a => a.Id, b => b));
            Contacts = new NotificationJobContactRepository().GetAll()
                        .GroupBy(p => p.NotificationJobId)
                        .ToDictionary(p => p.Key, q => q.ToDictionary(a => a.Id, b => b));

            Users = new UserRepository().GetAll().ToDictionary(p => p.Id, q => q);
            RoleUsers = new UserRoleRepository().GetAll().Join(Users, p => p.UserId, u => u.Value.Id, (a, b) => new { RoleId = a.RoleId, User = b })
                            .GroupBy(p => p.RoleId).ToDictionary(p => p.Key, q => q.ToDictionary(p => p.User.Value.Id, p => p.User.Value));


            NotificationActions = new NotificationActionRepository().GetAll().ToDictionary(p => p.Id, q => q);
            TransactionTypes = new GenericRepository<TransactionType>().GetAll().ToDictionary(p => p.Id, q => q);

            LastModified = DateTime.Now;
            IsInitialized = true;
        }


        public static void RefreshCache()
        {
            var updatedJobs = new NotificationJobRepository().GetAll(p => p.CreatedAt > LastModified || p.ModifiedAt > LastModified);

            foreach (var item in updatedJobs)
            {
                if (Jobs.ContainsKey(item.CompanyId.Value))
                {
                    if (Jobs[item.CompanyId.Value].ContainsKey(item.Id))
                    {
                        if (item.IsActive)
                            Jobs[item.CompanyId.Value][item.Id] = item;
                        else
                            Jobs[item.CompanyId.Value].Remove(item.Id);
                    }
                    else
                    {
                        Jobs[item.CompanyId.Value].Add(item.Id, item);
                    }
                }
                else
                {
                    Jobs.Add(item.CompanyId.Value, new Dictionary<int, NotificationJob> { { item.Id, item } });
                }
            }

            var updatedActions = new NotificationJobActionRepository().GetAll(p => p.CreatedAt > LastModified || p.ModifiedAt > LastModified);
            var updatedActors = new NotificationJobActorRepository().GetAll(p => p.CreatedAt > LastModified || p.ModifiedAt > LastModified);
            var updatedContacts = new NotificationJobContactRepository().GetAll(p => p.CreatedAt > LastModified || p.ModifiedAt > LastModified);

            AddUpdateItemsInDictionary(Actions, updatedActions);
            AddUpdateItemsInDictionary(Actors, updatedActors);
            AddUpdateItemsInDictionary(Contacts, updatedContacts);

        }

        private static void AddUpdateItemsInDictionary<T>(Dictionary<int, Dictionary<int, T>> collection, List<T> items)
        {
            foreach (var item in items)
            {
                var id = ((dynamic)item).Id;
                var jobId = ((dynamic)item).NotificationJobId;
                if (collection.ContainsKey(jobId))
                {
                    if (collection[jobId].ContainsKey(id)) { collection[jobId][id] = item; }
                    else
                        collection[jobId].Add(id, item);
                }
                else
                {
                    collection.Add(jobId, new Dictionary<int, T> { { id, item } });
                }
            }
        }

        public static bool IsUserMonitored(NotificationJobTrigger trigger, NotificationJob job)
        {

            //if(DataCache.Actors[job.Id]

            return true;
        }


    }
}
