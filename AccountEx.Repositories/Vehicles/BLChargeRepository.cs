using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Core.Objects;
using Transaction = AccountEx.CodeFirst.Models.Transaction;
using System.Configuration;
using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.Transactions;
using AccountEx.Common.VehicleSystem;
using AccountEx.CodeFirst.Models.Vehicles;

namespace AccountEx.Repositories
{
    public class BLChargeRepository : GenericRepository<BLCharge>
    {
        public BLChargeRepository() : base() { }
        public BLChargeRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }

        public bool CheckIfPaidcharge(int blId)
        {
            return Collection.Any(p => p.BLId == blId && p.IsPaid);
        }

    }
}