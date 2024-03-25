using AccountEx.CodeFirst.Models;
using System.Collections.Generic;
using System.Linq;
using AccountEx.Common;
using System;
using AccountEx.DbMapping;

namespace AccountEx.Repositories
{
    public class OpeningBalancesRepository : GenericRepository<OpeningBalance>
    {

        public void Save(List<OpeningBalance> records, int stockValueAccountId, bool allowCGS)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {

                var dbItems = FiscalCollection.ToDictionary(p => p.AccountId, q => q);
                foreach (var item in records)
                {
                    if (dbItems.ContainsKey(item.AccountId))
                    {
                        var dbItem = dbItems[item.AccountId];
                        dbItem.Quantity = item.Quantity;
                        dbItem.UnitPrice = item.UnitPrice;
                        if (dbItem.Quantity == 0 && dbItem.UnitPrice == 0)
                            Db.OpeningBalances.Remove(dbItem);
                    }
                    else
                    {
                        if (item.Quantity != 0 || item.Debit != 0 || item.Credit != 0 || item.UnitPrice != 0)
                        {
                            item.FiscalId = SiteContext.Current.Fiscal.Id;
                            item.OpeningBalanceTypeId = (byte)OpeningBalances.Stock;
                            Db.OpeningBalances.Add(item);
                        }

                    }
                }
                if (allowCGS)
                {
                    ///Transaciont realtd processing to Inventory Account
                    var amounts = records.Sum(p => (p.Quantity * p.UnitPrice));
                    var ids = new List<int>() { stockValueAccountId };
                    var openingBalancesType = new List<VoucherType> { VoucherType.OpeningBalance, VoucherType.AutoOpeningBalance };
                    var tranRepo = new TransactionRepository(this);
                    var oldrecord = tranRepo.AsQueryable(true).FirstOrDefault(p => openingBalancesType.Contains(p.TransactionType) && ids.Contains(p.AccountId));
                    if (oldrecord != null)
                    {
                        if (oldrecord.TransactionType != VoucherType.AutoOpeningBalance)
                        {
                            oldrecord.Debit = amounts;
                        }
                    }
                    else
                    {
                        var item = new Transaction();
                        var dt = SiteContext.Current.Fiscal.FromDate;
                        item.AccountId = stockValueAccountId;
                        item.Debit = amounts;
                        item.Date = dt;
                        item.CreatedDate = DateTime.Now;
                        item.TransactionType = VoucherType.OpeningBalance;
                        item.CompanyId = SiteContext.Current.User.CompanyId;
                        item.FiscalId = SiteContext.Current.Fiscal.Id;
                        Db.Transactions.Add(item);
                    }
                    tranRepo.SaveChanges();
                }
                SaveChanges();
                scope.Complete();
            }
        }






        public List<StockOpeningBalanceExt> Get()
        {
            var accounts = AsQueryable<AccountDetail>().Where(p => p.AccountDetailFormId == (byte)AccountDetailFormType.Products).Select(p => new
            {
                p.AccountId,
                p.Code,
                p.Name
            }).ToList();
            var openingBalances = FiscalCollection.ToList();
            var records = new List<StockOpeningBalanceExt>();

            foreach (var item in accounts)
            {
                var ob = openingBalances.Any(p => p.AccountId == item.AccountId) ? openingBalances.FirstOrDefault(p => p.AccountId == item.AccountId) : new OpeningBalance();
                records.Add(new StockOpeningBalanceExt()
                                {
                                    Id = ob.Id,
                                    AccountId = item.AccountId,
                                    AccountCode = item.Code,
                                    AccountName = item.Name,
                                    Quantity = ob.Quantity,
                                    UnitPrice = ob.UnitPrice,
                                    OpeningBalanceTypeId = ob.OpeningBalanceTypeId
                                });
            }
            return records;

        }
    }
}
