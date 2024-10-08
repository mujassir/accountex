﻿using System.Collections.Generic;
using System.Linq;
using AccountEx.CodeFirst.Models;

namespace AccountEx.Repositories
{
    public class AssetRepository : GenericRepository<Asset>
    {
        public void SyncIds(List<int> ids)
        {
            foreach (var account in Db.Accounts.Where(p => ids.Contains(p.Id)).ToList())
            {
                var item = Collection.FirstOrDefault(p => p.AccountId == account.Id);
                if (item != null)
                    account.ReferenceId = item.Id;
            }
            Db.SaveChanges();
        }
    }
}
