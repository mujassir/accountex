using AccountEx.Common;
using System.Collections.Generic;
using System.Linq;
using System;
using AccountEx.CodeFirst.Models;

namespace AccountEx.Repositories
{
    public class FiscalSettingRepository : GenericRepository<FiscalSetting>
    {
        public FiscalSettingRepository() : base() { }
        public FiscalSettingRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public List<FiscalSetting> GetByKeys(List<string> keys)
        {
            return Collection.Where(p => keys.Contains(p.Key)).ToList();
        }
        public bool IsPmcLocked()
        {
            var key = "Fiscal.IsPmcLocked";
            var record = FiscalCollection.FirstOrDefault(p => p.Key.ToLower() == key.ToLower());
            if (record != null)
                return Numerics.GetBool(record.Value);
            else
                return false;
        }
        public List<FiscalSetting> GetByCompanyId(int companyId, int fiscalId)
        {
            return Db.FiscalSettings.Where(p => p.CompanyId == companyId && p.FiscalId == fiscalId).ToList();
        }
        public List<FiscalSetting> GetByCompanyId(int companyId, List<string> keys)
        {
            return Db.FiscalSettings.Where(p => p.CompanyId == companyId && keys.Contains(p.Key)).ToList();
        }
        public bool CheckIfAccountExist(string accountTitle)
        {
            return Collection.Any(p => p.Value.ToLower() == accountTitle);
        }
    }
}
