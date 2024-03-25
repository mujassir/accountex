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
    public class CurrecnyRateRepository : GenericRepository<CurrencyRate>
    {
        public CurrecnyRateRepository() : base() { }
        public CurrecnyRateRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }

       
    }
}