using AccountEx.Common;
using System;
using System.Linq;
using AccountEx.CodeFirst.Models;
using System.Collections.Generic;
using AccountEx.DbMapping;

namespace AccountEx.Repositories
{
    public class OpeningBalanceRepository : GenericRepository<Transaction>
    {
        public decimal GetOpeningBalance(int accountId, DateTime date)
        {
            var cr = FiscalCollection.Where(p => p.AccountId == accountId && p.Date < date).Sum(p => (decimal?)p.Credit);
            var dr = FiscalCollection.Where(p => p.AccountId == accountId && p.Date < date).Sum(p => (decimal?)p.Debit);
            if (!cr.HasValue) cr = 0;
            if (!dr.HasValue) dr = 0;
            return dr.Value - cr.Value;
        }
        public decimal GetOpeningBalance(string type, DateTime date, byte entryType)
        {
            var vouchertype = ((VoucherType)Enum.Parse(typeof(VoucherType), type));
            var cr = FiscalCollection.Where(p => p.TransactionType == vouchertype && p.Date < date && p.EntryType == entryType).Sum(p => (decimal?)p.Credit);
            var dr = FiscalCollection.Where(p => p.TransactionType == vouchertype && p.Date < date && p.EntryType == entryType).Sum(p => (decimal?)p.Debit);
            if (!cr.HasValue) cr = 0;
            if (!dr.HasValue) dr = 0;
            return dr.Value - cr.Value;
        }

        public decimal GetOpeningBalance(int accountId, string type, DateTime date)
        {
            var vouchertype = ((VoucherType)Enum.Parse(typeof(VoucherType), type));
            var accountIds = FiscalCollection.Where(p => p.AccountId == accountId).Select(p => p.AccountId).ToList();
            var cr = FiscalCollection.Where(p => p.TransactionType == vouchertype && accountIds.Contains(accountId) && p.Date < date).Sum(p => (decimal?)p.Credit);
            var dr = FiscalCollection.Where(p => p.TransactionType == vouchertype && accountIds.Contains(accountId) && p.Date < date).Sum(p => (decimal?)p.Debit);
            if (!cr.HasValue) cr = 0;
            if (!dr.HasValue) dr = 0;
            return dr.Value - cr.Value;
        }

        public decimal GetOpeningBalance(DateTime date, int accountId)
        {
            var accountIds = AsQueryable<Account>().Where(p => p.ParentId == accountId).Select(p => p.Id).ToList();
            var cr = FiscalCollection.Where(p => accountIds.Contains(p.Id) && p.Date < date).Sum(p => (decimal?)p.Credit);
            var dr = FiscalCollection.Where(p => accountIds.Contains(p.Id) && p.Date < date).Sum(p => (decimal?)p.Debit);
            if (!cr.HasValue) cr = 0;
            if (!dr.HasValue) dr = 0;
            return dr.Value - cr.Value;
        }
        public decimal GetSumOnly(string type, DateTime date, byte entryType)
        {
            var vouchertype = ((VoucherType)Enum.Parse(typeof(VoucherType), type));
            var cr = FiscalCollection.Where(p => p.TransactionType == vouchertype && p.Date < date && p.EntryType == entryType).Sum(p => (decimal?)p.Credit);

            if (!cr.HasValue) cr = 0;

            return cr.Value;
        }

        public void Save(SaleExtra obj)
        {
            var ids = obj.Items.Select(q => q.AccountId).ToList();
            var openingBalancesType = new List<VoucherType> { VoucherType.OpeningBalance, VoucherType.AutoOpeningBalance };
            var oldrecord = FiscalCollection.Where(p => openingBalancesType.Contains(p.TransactionType) && ids.Contains(p.AccountId)).ToList();
            foreach (var item in obj.Items)
            {
                var currentrecord = oldrecord.FirstOrDefault(p => p.AccountId == item.AccountId);
                if (currentrecord != null)
                {
                    if (item.Debit > 0 || item.Credit > 0)
                    {
                        currentrecord.Credit = item.Credit;
                        currentrecord.Debit = item.Debit;
                        var dt = SiteContext.Current.Fiscal.FromDate;
                        currentrecord.Date = dt;
                    }

                    else if (item.TransactionType != VoucherType.AutoOpeningBalance)
                        Db.Transactions.Remove(currentrecord);
                }
                else if (item.Debit > 0 || item.Credit > 0)
                {
                    //var dt = new DateTime(DateTime.Now.Year, 07, 01);
                    //if (dt > DateTime.Now) dt = dt.AddYears(-1);
                    var dt = SiteContext.Current.Fiscal.FromDate;
                    item.Date = dt;
                    item.CreatedDate = DateTime.Now;
                    item.TransactionType = VoucherType.OpeningBalance;
                    item.CompanyId = SiteContext.Current.User.CompanyId;
                    item.FiscalId = SiteContext.Current.Fiscal.Id;
                    Db.Transactions.Add(item);
                }
            }
            SaveChanges();
        }
    }
}