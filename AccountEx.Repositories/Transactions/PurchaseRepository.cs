using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using AccountEx.CodeFirst.Models;
using AccountEx.DbMapping;

namespace AccountEx.Repositories
{
    public class PurchaseRepository : GenericRepository<Purchase>
    {


        public int GetNextVoucherNumber()
        {
            try
            {
                var maxnumber = 1;
                if (FiscalCollection.FirstOrDefault(p => p.PurchaseType == "RoughReceiving") == null)
                {
                    return maxnumber;
                }
                maxnumber = FiscalCollection.Where(p => p.PurchaseType == "RoughReceiving").Max(p => p.VoucherNumber) + 1;
                return maxnumber;
            }
            catch (Exception ex)
            {

                return -1;
            }
        }
        public new List<Purchase> GetById(int id)
        {
            var masterdetail = FiscalCollection.FirstOrDefault(p => p.Id == id);
            var data = FiscalCollection.Where(p => p.VoucherNumber == masterdetail.VoucherNumber && p.TransactionType == masterdetail.TransactionType).ToList();
            return data;
        }
        public List<Purchase> GetByVoucherNo(int voucherNo)
        {

            var data = FiscalCollection.Where(p => p.VoucherNumber == voucherNo && p.TransactionType  == VoucherType.Purchase).ToList();
            return data;
        }
        public void Save(PurchaseExtra obj) { }

        public void Update(PurchaseExtra obj) { }

        public List<Purchase> GetPending()
        {
            return FiscalCollection.Where(p => !p.IsReceived.Value).ToList();
        }
    }
}