using AccountEx.Common;
using System;
using System.Linq;
using AccountEx.CodeFirst.Models;
using AccountEx.DbMapping;

namespace AccountEx.Repositories
{
    public class RecoveryRepository : GenericRepository<Transaction>
    {
        public decimal GetOpeningBalance(int accountId, DateTime date)
        {
            var cr = Db.Transactions.Where(p => p.AccountId == accountId && p.Date < date).Sum(p => (decimal?)p.Credit);
            var dr = Db.Transactions.Where(p => p.AccountId == accountId && p.Date < date).Sum(p => (decimal?)p.Debit);
            if (!cr.HasValue) cr = 0;
            if (!dr.HasValue) dr = 0;
            return dr.Value - cr.Value;
        }

        public void Save(SaleExtra obj)
        {
            var settings = new FormSettingRepository().GetFormSettingByVoucherType("Recovery");
            var setting = settings.FirstOrDefault(p => p.KeyName == "CashAccountId");
            if (setting != null)
            {
                var cashAccountId = Numerics.GetInt(setting.Value);
                setting = settings.FirstOrDefault(p => p.KeyName == "CashAccountName");
                if (setting != null)
                {
                    var cashAccountName = setting.Value;

                    var ids = obj.Items.Where(q => q.Id > 0).Select(q => q.Id).ToList();
                    var oldrecord = Collection.Where(p => ids.Contains(p.Id)).ToList();
                    foreach (var item in obj.Items)
                    {

                        if (item.Id == 0)
                        {
                            item.Date = item.Date;
                            item.CreatedDate = DateTime.Now;
                            Db.Transactions.Add(item);
                            var transaction = new Transaction();
                            transaction.Date = item.Date;
                            transaction.VoucherNumber = item.Id;
                            transaction.Debit = item.Credit;
                            transaction.AccountId = cashAccountId;
                            transaction.AccountTitle = cashAccountName;
                            transaction.TransactionType = VoucherType.Recovery;
                            transaction.EntryType = (byte)EntryType.Item;
                            Db.Transactions.Add(transaction);
                        }
                        else
                        {
                            var currentrecord = oldrecord.FirstOrDefault(p => p.Id == item.Id);
                            var childrecord = Db.Transactions.FirstOrDefault(p => p.EntryType == (byte)EntryType.Item && p.TransactionType  == VoucherType.Recovery && p.VoucherNumber == item.Id);
                            if (currentrecord != null)
                            {
                                currentrecord.Credit = item.Credit;
                                currentrecord.Debit = item.Debit;

                            }
                            if (childrecord == null) continue;
                            childrecord.Debit = item.Credit;
                            childrecord.Credit = 0;
                        }


                    }
                }
            }

            SaveChanges();

        }



    }
}