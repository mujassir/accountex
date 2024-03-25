using System;
using System.Collections.Generic;
using System.Linq;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.CodeFirst.Models;
using System.Data.Entity;

namespace AccountEx.BussinessLogic
{
    public static class FiscalSettingManager
    {
        #region Properties

        public static string ProjectPmcTransferredKey = "Fiscal.IsProjectPmcTransferred";
        public static string ProjectLockedKey = "Fiscal.IsProjectLocked";
        public static string PmcLockedKey = "Fiscal.IsPmcLocked";
        public static bool IsProjectPmcTransferred
        {
            get { return Get<bool>(ProjectPmcTransferredKey, false); }
        }
        public static bool IsProjectLocked
        {
            get { return Get<bool>(ProjectLockedKey, false); }
        }
        public static bool IsPmcLocked
        {
            get { return Get<bool>(PmcLockedKey, false); }
        }

        #endregion






        public static void SaveFiscalSettings(List<FiscalSetting> FiscalSettings)
        {
            var repo = new FiscalSettingRepository();
            using (var scope = TransactionScopeBuilder.Create(new System.TimeSpan(0, 15, 0)))
            {
                repo.Save(FiscalSettings);
                repo.SaveChanges();
                scope.Complete();
            }
            SiteContext.Current.FiscalSettings = FiscalSettings.ToDictionary(p => p.Key, q => q.Value);
        }
        public static void SaveFiscalSettings(string key, string value)
        {
            var repo = new FiscalSettingRepository();
            using (var scope = TransactionScopeBuilder.Create(new System.TimeSpan(0, 15, 0)))
            {
                var fiscalId = SiteContext.Current.Fiscal.Id;
                var fs = repo.AsQueryable(true).AsNoTracking().FirstOrDefault(p => p.Key.ToLower() == key);
                if (fs != null)
                {
                    fs.Value = value;
                }
                else
                {
                    fs = new FiscalSetting() { Key = key, Value = value, FiscalId = fiscalId };
                }

                repo.Save(fs);
                repo.SaveChanges();
                scope.Complete();
            }
        }
        public static int GetByTitle(string title)
        {
            var cacheKey = string.Format("AccountId_{0}_{1}", SiteContext.Current.User.CompanyId, title);
            var value = SiteContext.Current.Cache(cacheKey);
            if (value != null)
            {
                var id = Cast.To<int>(value);
                if (id > 0) return id;
            }

            var ac = new AccountRepository().GetIdByName(title);
            SiteContext.Current.Cache(cacheKey, ac);
            return ac;
        }
        public static int GetAllByTitle(string titless)
        {
            return new AccountRepository().GetIdByName(titless);
        }
        public static Dictionary<string, string> GetAll()
        {
            return SiteContext.Current.FiscalSettings ?? (SiteContext.Current.FiscalSettings = new FiscalSettingRepository().GetAll(p => p.FiscalId == SiteContext.Current.Fiscal.Id).ToDictionary(p => p.Key, q => q.Value));
        }
        public static Dictionary<string, string> FiscalSettings
        {
            get { if (SiteContext.Current.User != null) return GetAll(); else return null; }
        }

        public static void RefreshSetting()
        {
            SiteContext.Current.FiscalSettings = new FiscalSettingRepository().GetAll(p => p.FiscalId == SiteContext.Current.Fiscal.Id).ToDictionary(p => p.Key, q => q.Value);
        }
        public static void RefreshSetting(int CompanyId, int fiscalId)
        {
            SiteContext.Current.FiscalSettings = new FiscalSettingRepository().GetByCompanyId(CompanyId, fiscalId).ToDictionary(p => p.Key, q => q.Value);
        }
        public static string Get(string key)
        {
            if (FiscalSettings == null)
                RefreshSetting();
            return FiscalSettings != null && FiscalSettings.ContainsKey(key) ? FiscalSettings[key] : "";
        }
        public static string Get(string key, string defaultValue)
        {
            if (FiscalSettings == null)
                RefreshSetting();
            return FiscalSettings != null && FiscalSettings.ContainsKey(key) && !string.IsNullOrWhiteSpace(FiscalSettings[key]) ? FiscalSettings[key] : defaultValue;
        }
        public static T Get<T>(string key, object defaultValue)
        {

            var value = Get(key);
            return (T)Convert.ChangeType(!string.IsNullOrEmpty(value) ? value : defaultValue, typeof(T));
        }
        public static T Get<T>(string key)
        {
            return (T)Convert.ChangeType(Get(key), typeof(T));
        }

    }
}
