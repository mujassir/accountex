using AccountEx.Common;
using System.Collections.Generic;
using System.Linq;
using System;
using AccountEx.CodeFirst.Models;

namespace AccountEx.Repositories
{
    public class SettingRepository : GenericRepository<Setting>
    {
        public SettingRepository() : base() { }
        public SettingRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public List<Setting> GetByKeys(List<string> keys)
        {
            return Collection.Where(p => keys.Contains(p.Key)).ToList();
        }
        public List<Setting> GetByCompanyId(int companyId)
        {
            return Db.Settings.Where(p => p.CompanyId == companyId).ToList();
        }
        public List<Setting> GetByCompanyId(int companyId, List<string> keys)
        {
            return Db.Settings.Where(p => p.CompanyId == companyId && keys.Contains(p.Key)).ToList();
        }
        public bool CheckIfAccountExist(string accountTitle)
        {
            return Collection.Any(p => p.Value.ToLower() == accountTitle);
        }
    }
}
