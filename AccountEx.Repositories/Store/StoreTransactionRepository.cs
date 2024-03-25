using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using AccountEx.CodeFirst.Models;
using AccountEx.DbMapping;

namespace AccountEx.Repositories
{
    public class StoreTransactionRepository : GenericRepository<Purchase>
    {


        public int GetNextVoucherNumber()
        {
            try
            {
                var maxnumber = 1;
                if (Collection.FirstOrDefault(p => p.PurchaseType == "StoreReceiving") == null)
                {
                    return maxnumber;
                }
                maxnumber = Collection.Where(p => p.PurchaseType == "StoreReceiving").Max(p => p.VoucherNumber) + 1;
                return maxnumber;
            }
            catch (Exception ex)
            {

                return -1;
            }
        }
        public new List<Purchase> GetById(int id)
        {
            var masterdetail = Collection.FirstOrDefault(p => p.Id == id);
            var data = Collection.Where(p => p.VoucherNumber == masterdetail.VoucherNumber && p.TransactionType == masterdetail.TransactionType).ToList();
            return data;
        }
        public void Save(PurchaseExtra obj) { }
        public void Update(PurchaseExtra obj) { }


    }
}