using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccountEx.BussinessLogic
{
    public static class ActionManager
    {
        public static bool OnlyViewOwnRecord { get { return Get("User.CanViewOwnRecord"); } }
        public static bool JobExport { get { return Get("Job.Export"); } }
        public static bool JobSea { get { return Get("Job.Sea"); } }
        public static bool JobAir { get { return Get("Job.Air"); } }
        public static bool JobStatusAllJobs { get { return Get("Job.Status.AllJobs"); } }
        public static bool ManageJobPrices { get { return Get("Job.ManageJobPrices"); } }
        public static bool ViewProductSecondaryCategory { get { return Get("Product.SecondaryCategory"); } }
        public static bool ApplicationTestInterview { get { return Get("Application.TestInterview"); } }

        public static bool ApplicationMedicalTest { get { return Get("Application.MedicalTest"); } }

        public static bool ApplicationENumber { get { return Get("Application.ENumber"); } }
        public static bool HideGINAmount { get { return Get("GIN.HideAmount"); } }


        public static Dictionary<string, bool> GetAll()
        {
            if (SiteContext.Current.Actions != null) return SiteContext.Current.Actions;
            var roleActions = new RoleActionRepository().GetByRoleId(SiteContext.Current.UserRoles).ToDictionary(p => p.ActionId, q => q.Allowed);
            var actions = new ActionRepository().GetAll();
            actions.ForEach(item => item.Allowed = roleActions.ContainsKey(item.Id) ? roleActions[item.Id] : item.Allowed);
            //foreach (var item in actions)
            //    item.Allowed = roleActions.ContainsKey(item.Id) ? roleActions[item.Id] : item.Allowed;
            SiteContext.Current.Actions = actions.ToDictionary(p => p.Key, q => q.Allowed);
            return SiteContext.Current.Actions;
        }
        public static Dictionary<string, bool> Actions
        {
            get { return GetAll(); }
        }
        public static void RefreshSetting()
        {
            var roleActions = new RoleActionRepository().GetByRoleId(SiteContext.Current.UserRoles).ToDictionary(p => p.ActionId, q => q.Allowed);
            var actions = new ActionRepository().GetAll();
            actions.ForEach(item => item.Allowed = roleActions.ContainsKey(item.Id) ? roleActions[item.Id] : item.Allowed);
            SiteContext.Current.Actions = actions.ToDictionary(p => p.Key, q => q.Allowed);
        }
        public static bool Get(string key)
        {
            return Actions.ContainsKey(key) ? Actions[key] : false;
        }

        public static T Get<T>(string key, object defaultValue)
        {
            var value = Actions.ContainsKey(key) ? Actions[key] + "" : "";
            return (T)Convert.ChangeType(!string.IsNullOrWhiteSpace(value) ? value : defaultValue, typeof(T));
        }
        public static T Get<T>(string key)
        {
            return (T)Convert.ChangeType(Get(key), typeof(T));
        }



    }
}
