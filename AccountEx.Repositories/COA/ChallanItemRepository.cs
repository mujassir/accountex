using AccountEx.CodeFirst.Models.COA;
using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace AccountEx.Repositories
{
    public class ChallanItemRepository : GenericRepository<ChallanItem>
    {
        public ChallanItemRepository() : base() { }
        public ChallanItemRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public List<ChallanItem> GetUnReceivedChallans(bool IsReceived)
        {
            return Collection.Where(p => p.IsReceived == IsReceived).ToList();
        }

       

      
        public List<ChallanItem> GetUnReceivedChallans()
        {
            var items = Collection.Where(p => p.IsReceived == false && p.EntryType == (byte)EntryType.Manual).ToList();
            items.Where(p => p.TransactionType  == VoucherType.PossessionCharges ||
                        p.TransactionType  == VoucherType.SecurityMoney ||
                        p.TransactionType  == VoucherType.MiscCharges ||
                        p.TransactionType  == VoucherType.RentMonthlyLiability).ToList();
            return items;
        }
        //public ChallanExtra GetChallansByChallanNo(int challanNo)
        //{
        //    var query = string.Format("EXEC [DBO].[GetChallansByChallanNo] @ChallanNo = {0}", challanNo);
        //    var record = Db.Database.SqlQuery<ChallanExtra>(query).FirstOrDefault();
        //    return record;
        //}


        public bool CheckIfChallanReceived(int id)
        {
            return Collection.Any(p => p.Id == id && p.IsReceived);
        }

    }
}
