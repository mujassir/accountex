using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Core.Objects;
using Transaction = AccountEx.CodeFirst.Models.Transaction;
using System.Configuration;
using AccountEx.CodeFirst.Models;
using AccountEx.Repositories.Vehicles;
using AccountEx.DbMapping;

namespace AccountEx.Repositories
{
    public class TransactionRepository : GenericRepository<Transaction>
    {
        public TransactionRepository() : base() { }
        public TransactionRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public override void Delete(int id)
        {


            using (var scope = TransactionScopeBuilder.Create())
            {
                var trans = FiscalCollection.FirstOrDefault(p => p.Id == id);
                if (trans != null)
                {

                    foreach (var item in FiscalCollection.Where(p => p.VoucherNumber == trans.VoucherNumber && p.TransactionType == trans.TransactionType))
                    {
                        Db.Transactions.Remove(item);
                    }
                    Db.Transactions.Remove(trans);
                    Db.SaveChanges();

                }
                scope.Complete();
            }
        }
        public override void Add(List<Transaction> transactions)
        {
            Add(transactions, false);
        }
        public void Add(List<Transaction> transactions, bool skipZeroCheck)
        {
            var error = "";
            var debit = Math.Round(transactions.Sum(p => p.Debit), 2);
            var credit = Math.Round(transactions.Sum(p => p.Credit), 2);
            //Get All acccounts other then 4th level

            if (debit != credit)
            {
                error = "Transaction debit(" + debit + ") & credit(" + credit + ") are not equal.,";
            }
            if (!skipZeroCheck)
            {
                if (debit == 0 && credit == 0)
                {
                    error = "Transaction total amount should be greatet than zero(0).,";
                }
            }
            foreach (var item in transactions.Where(p => p.AccountId == 0))
            {
                var debitCredit = item.Debit > 0 ? "Debit:" + item.Debit : "Credit:" + item.Credit;
                error += "No transaction can be added with invalid Account.(AccountId:0,EnteryType:" + item.EntryType + "," + debitCredit + ",Comment:" + item.Comments + ",";
            }
            var accountIds = transactions.Select(p => p.AccountId).Distinct().ToList();
            var accounts = new AccountRepository().GetOtherThenLeafAccount(accountIds);
            foreach (var item in accounts)
            {
                error = "Account(" + item.Name + ") must be a 4th level account.,";
            }
            if (string.IsNullOrWhiteSpace(error.Trim().Trim(',')))
            {
                base.Add(transactions.Where(p => p.Debit > 0 || p.Credit > 0).ToList());
            }
            else
            {
                throw new OwnException(error);
            }



        }
        public decimal GetBalance(int accountId, DateTime date)
        {
            var query = FiscalCollection.Where(p => p.AccountId == accountId && p.Date <= date);
            if (query.Any()) return query.Sum(p => (p.Debit - p.Credit));
            return 0;
        }
        public List<Transaction> GetLastCGSEntry(List<int> itemIds,DateTime saleReturnDate)
        {

            return FiscalCollection.Where(p => p.Date <= saleReturnDate && p.TransactionType == VoucherType.Sale && p.ProductId > 0 && itemIds.Contains(p.ProductId.Value)).GroupBy(p => p.ProductId)
                  .Select(g => g.OrderByDescending(p => p.Date).FirstOrDefault()).ToList();

        }
        public decimal GetBalanceWithOutTradeIn(int accountId, DateTime date)
        {
            var query = FiscalCollection.Where(p => p.AccountId == accountId && p.Date <= date && p.TransactionType != VoucherType.TradeIn);
            if (query.Any()) return query.Sum(p => (p.Debit - p.Credit));
            return 0;
        }
        public decimal GetChassisAdvanceBalance(int accountId, int vehicleId)
        {
            var query = FiscalCollection.Where(p => p.AccountId == accountId && p.MainEntityId == vehicleId && p.TransactionType == VoucherType.AdvanceReceipts);
            if (query.Any()) return query.Sum(p => (p.Debit - p.Credit));
            return 0;
        }
        public decimal GetOpeningBalance(int accountId, DateTime date)
        {
            return GetOpeningBalance(accountId, date, false);
        }
        public decimal GetVehicleExpenses(int vehicleId)
        {
            var types = new List<VoucherType> { VoucherType.BL, VoucherType.CashPayments, VoucherType.BankPayments, VoucherType.VehiclePayable };
            if (Collection.Any(p => types.Contains(p.TransactionType) && p.MainEntityId.HasValue && p.MainEntityId.Value == vehicleId))
                return Collection.Where(p => types.Contains(p.TransactionType) && p.MainEntityId.HasValue && p.MainEntityId.Value == vehicleId).Sum(p => p.Debit);
            else return 0;
        }
        public decimal GetOpeningBalance(int accountId, DateTime date, bool includeAutoClosingBalance)
        {
            var openingBalancesType = new List<VoucherType> { VoucherType.OpeningBalance, VoucherType.AutoOpeningBalance };
            if (includeAutoClosingBalance) openingBalancesType.Add(VoucherType.AutoClosingBalance);
            var cr = FiscalCollection.Where(p => p.AccountId == accountId && (EntityFunctions.TruncateTime(p.Date) < EntityFunctions.TruncateTime(date) || openingBalancesType.Contains(p.TransactionType))).Sum(p => (decimal?)p.Credit);
            var dr = FiscalCollection.Where(p => p.AccountId == accountId && (EntityFunctions.TruncateTime(p.Date) < EntityFunctions.TruncateTime(date) || openingBalancesType.Contains(p.TransactionType))).Sum(p => (decimal?)p.Debit);
            if (!cr.HasValue) cr = 0;
            if (!dr.HasValue) dr = 0;
            return dr.Value - cr.Value;
        }
        public decimal GetForexOpeningBalance(int accountId, DateTime date, int currencyId)
        {
            var vehicleRepo = new VehicleRepository(this).AsQueryable();
            var vehicleVoucherRepo = new VehicleVoucherRepository(this).AsQueryable();
            var cr = vehicleRepo.Where(p => p.IsForex && p.VendorId == accountId && (EntityFunctions.TruncateTime(p.PurchaseDate) < EntityFunctions.TruncateTime(date)) && p.CurrencyId == currencyId).Sum(p => (decimal?)p.ForexPrice);
            var dr = vehicleVoucherRepo.Where(p => p.AccountId1 == accountId && p.TransactionType == VoucherType.ForexVoucher && (EntityFunctions.TruncateTime(p.Date) < EntityFunctions.TruncateTime(date)) && p.CurrencyId == currencyId).Sum(p => (decimal?)p.ForexPrice);
            if (!cr.HasValue) cr = 0;
            if (!dr.HasValue) dr = 0;
            return dr.Value - cr.Value;
        }

        public decimal GetVehicleOpeningBalance(int vehicleId, DateTime date)
        {
            var transactionTypes = new List<VoucherType> { VoucherType.BL, VoucherType.CashPayments, VoucherType.BankPayments };
            var query = FiscalCollection.Where(p => transactionTypes.Contains(p.TransactionType) && p.MainEntityId == vehicleId && (EntityFunctions.TruncateTime(p.Date) < EntityFunctions.TruncateTime(date) || transactionTypes.Contains(p.TransactionType))).AsQueryable();
            var cr = query.Sum(p => (decimal?)p.Credit);
            var dr = query.Sum(p => (decimal?)p.Debit);
            if (!cr.HasValue) cr = 0;
            if (!dr.HasValue) dr = 0;
            return dr.Value - cr.Value;
        }

        public decimal GetOpeningBalanceForStaticLedger(int accountId, DateTime date)
        {
            var openingBalancesType = new List<VoucherType> { VoucherType.OpeningBalance, VoucherType.AutoOpeningBalance };
            var cr = FiscalCollection.Where(p => p.AccountId == accountId && (openingBalancesType.Contains(p.TransactionType))).Sum(p => (decimal?)p.Credit);
            var dr = FiscalCollection.Where(p => p.AccountId == accountId && (openingBalancesType.Contains(p.TransactionType))).Sum(p => (decimal?)p.Debit);
            if (!cr.HasValue) cr = 0;
            if (!dr.HasValue) dr = 0;
            return dr.Value - cr.Value;
        }
        public bool IsAutoOpeningBalanceExist(int accountId)
        {

            return FiscalCollection.Any(p => p.AccountId == accountId && p.TransactionType == VoucherType.AutoOpeningBalance);

        }
        public List<OpeningBalanceExt> GetOpeningBalances(List<int> accountIds, DateTime date)
        {
            var openingBalancesType = new List<VoucherType> { VoucherType.OpeningBalance, VoucherType.AutoOpeningBalance };
            return FiscalCollection.Where(p => accountIds.Contains(p.AccountId) && (EntityFunctions.TruncateTime(p.Date) < EntityFunctions.TruncateTime(date) || openingBalancesType.Contains(p.TransactionType))).GroupBy(p => p.AccountId).Select(p => new OpeningBalanceExt()
            {
                AccountId = p.Key,
                Balance = p.Sum(q => (decimal?)q.Debit - (decimal?)q.Credit) ?? 0.0M
            }).ToList();

        }
        //public List<OpeningBalance> GetOpeningBalanceByDates(List<int> accountIds, DateTime fromdate, DateTime todate)
        //{
        //    return Collection.Where(p => accountIds.Contains(p.AccountId) && EntityFunctions.TruncateTime(p.Date) >= EntityFunctions.TruncateTime(fromdate) && EntityFunctions.TruncateTime(p.Date) <= EntityFunctions.TruncateTime(todate)).GroupBy(p => p.AccountId).Select(p => new OpeningBalance()
        //    {
        //        AccountId = p.Key,
        //        Balance = p.Sum(q => (decimal?)q.Debit - (decimal?)q.Credit) ?? 0.0M
        //    }).ToList();

        //}
        public decimal GetBalanceByDates(int accountId, DateTime fromdate, DateTime todate)
        {
            var cr = FiscalCollection.Where(p => p.AccountId == accountId && EntityFunctions.TruncateTime(p.Date) >= EntityFunctions.TruncateTime(fromdate) && EntityFunctions.TruncateTime(p.Date) <= EntityFunctions.TruncateTime(todate)).Sum(p => (decimal?)p.Credit);
            var dr = FiscalCollection.Where(p => p.AccountId == accountId && EntityFunctions.TruncateTime(p.Date) >= EntityFunctions.TruncateTime(fromdate) && EntityFunctions.TruncateTime(p.Date) <= EntityFunctions.TruncateTime(todate)).Sum(p => (decimal?)p.Debit);
            if (!cr.HasValue) cr = 0;
            if (!dr.HasValue) dr = 0;
            return dr.Value - cr.Value;
        }
        public decimal GetBalanceByDates(List<int> accountIds, DateTime fromdate, DateTime todate)
        {
            var cr = FiscalCollection.Where(p => accountIds.Contains(p.AccountId) && EntityFunctions.TruncateTime(p.Date) >= EntityFunctions.TruncateTime(fromdate) && EntityFunctions.TruncateTime(p.Date) <= EntityFunctions.TruncateTime(todate)).Sum(p => (decimal?)p.Credit);
            var dr = FiscalCollection.Where(p => accountIds.Contains(p.AccountId) && EntityFunctions.TruncateTime(p.Date) >= EntityFunctions.TruncateTime(fromdate) && EntityFunctions.TruncateTime(p.Date) <= EntityFunctions.TruncateTime(todate)).Sum(p => (decimal?)p.Debit);
            if (!cr.HasValue) cr = 0;
            if (!dr.HasValue) dr = 0;
            return dr.Value - cr.Value;
        }
        public List<Transaction> GetOpeningStartBalance(List<int> parties)
        {
            return FiscalCollection.Where(p => parties.Contains(p.AccountId) && p.TransactionType == VoucherType.OpeningBalance).ToList();
        }

        public List<Transaction> GetByVoucherNumber(VoucherType voucherType, int voucherNumber)
        {
            return FiscalCollection.Where(p => p.TransactionType == voucherType && p.VoucherNumber == voucherNumber).ToList();
        }
        public int GetNextVoucherNumber(VoucherType vouchertype= VoucherType.Production)
        {
            var maxnumber = ConfigurationReader.GetConfigKeyValue<int>("VoucherStartNumber", 1001);
            if (!FiscalCollection.Any(p => p.TransactionType == vouchertype))
                return maxnumber;
            return FiscalCollection.Where(p => p.TransactionType == vouchertype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
        }
        //over load for adjustment
        public int GetNextVoucherNumber(List<VoucherType> vouchertype)
        {
            var maxnumber = 1;
            if (!FiscalCollection.Any(p => vouchertype.Contains(p.TransactionType)))
                return maxnumber;
            return FiscalCollection.Where(p => vouchertype.Contains(p.TransactionType)).OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
        }

        public int GetNextInvoiceNumber(VoucherType vouchertype)
        {

            var maxnumber = ConfigurationReader.GetConfigKeyValue<int>("InvoiceStartNumber", 1);
            if (!FiscalCollection.Any(p => p.TransactionType == vouchertype))
                return maxnumber;
            return FiscalCollection.Where(p => p.TransactionType == vouchertype).OrderByDescending(p => p.InvoiceNumber).FirstOrDefault().InvoiceNumber + 1;
        }
        public int GetLastInvoiceNumber(VoucherType vouchertype)
        {

            var maxnumber = ConfigurationReader.GetConfigKeyValue<int>("InvoiceStartNumber", 1);
            if (!FiscalCollection.Any(p => p.TransactionType == vouchertype))
                return maxnumber;
            return FiscalCollection.Where(p => p.TransactionType == vouchertype).OrderByDescending(p => p.Id).FirstOrDefault().InvoiceNumber + 1;
        }
        public new List<Transaction> GetById(int id)
        {
            var masterdetail = FiscalCollection.FirstOrDefault(p => p.Id == id);
            var data = FiscalCollection.Where(p => p.VoucherNumber == masterdetail.VoucherNumber && p.TransactionType == masterdetail.TransactionType).ToList();
            return data;
        }
        public void Save(SaleExtra obj)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var vou = obj.VoucherNumber;
                if (obj.Id == 0)
                    vou = GetNextVoucherNumber(obj.TransactionType);

                var data = new Transaction
                {
                    TransactionType = obj.TransactionType,
                    EntryType = obj.EntryType,
                    AccountTitle = obj.AccountTitle,
                    AccountId = obj.AccountId,
                    InvoiceNumber = obj.InvoiceNumber,
                    VoucherNumber = vou,
                    Debit = obj.Debit,
                    Credit = obj.Credit,
                    Date = obj.Date,
                    CreatedDate = DateTime.Now,
                    Comments = obj.Comments,
                    PartyAddress = obj.PartyAddress,
                    CompanyId = SiteContext.Current.User.CompanyId,
                    FiscalId = SiteContext.Current.Fiscal.Id

                };
                Db.Transactions.Add(data);
                foreach (var item in obj.Items)
                {
                    item.VoucherNumber = vou;
                    item.Date = obj.Date;
                    item.Comments = obj.Comments;
                    item.CreatedDate = DateTime.Now;
                    item.CompanyId = SiteContext.Current.User.CompanyId;
                    item.FiscalId = SiteContext.Current.Fiscal.Id;
                    Db.Transactions.Add(item);
                }
                SaveChanges();
                scope.Complete();
            }

        }
        public Transaction GetRecovery(int accountId, DateTime date)
        {
            return FiscalCollection.FirstOrDefault(p => p.AccountId == accountId && p.Date == date.Date
                    && p.TransactionType == VoucherType.Recovery && p.EntryType == (byte)EntryType.MasterDetail);
        }
        public void Update(SaleExtra obj)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                foreach (var item in FiscalCollection.Where(p => p.VoucherNumber == obj.VoucherNumber && p.TransactionType == obj.TransactionType))
                {
                    Db.Transactions.Remove(item);
                }
                Save(obj);
                scope.Complete();
            }
        }
        public void Update2(SaleExtra obj)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var masterdetail = FiscalCollection.FirstOrDefault(p => p.Id == obj.Id);
                var data = FiscalCollection.Where(p => p.VoucherNumber == masterdetail.VoucherNumber && p.TransactionType == masterdetail.TransactionType).ToList();
                var preitems = data.Where(p => p.EntryType == (byte)EntryType.Item).ToList();
                var prediscount = data.FirstOrDefault(p => p.EntryType == (byte)EntryType.Discount);
                var currentitems = obj.Items.Where(p => p.EntryType == (byte)EntryType.Item).ToList();
                var currentdiscount = obj.Items.FirstOrDefault(p => p.EntryType == (byte)EntryType.Discount);
                var currenttransactionIds = obj.Items.Select(p => p.Id).ToList();
                var deletedTransaction = preitems.Where(p => !currenttransactionIds.Contains(p.Id)).ToList();
                //transactionIds.Add(obj.Id);
                //var dbtransaction = Collection.Where(p => transactionIds.Contains(p.Id));

                if (masterdetail != null)
                {
                    masterdetail.TransactionType = obj.TransactionType;
                    masterdetail.EntryType = obj.EntryType;
                    masterdetail.AccountId = obj.AccountId;
                    masterdetail.AccountTitle = obj.AccountTitle;
                    masterdetail.InvoiceNumber = obj.InvoiceNumber;
                    masterdetail.VoucherNumber = obj.InvoiceNumber;
                    masterdetail.Comments = obj.Comments;
                    masterdetail.PartyAddress = obj.PartyAddress;
                    if (masterdetail.Debit > 0)
                        masterdetail.Debit = obj.Debit;
                    else
                        masterdetail.Credit = obj.Credit;
                    masterdetail.Date = obj.Date;
                }
                if (currentdiscount != null)
                {
                    if (prediscount != null)
                    {
                        prediscount.Debit = currentdiscount.Price * currentdiscount.Quantity;
                        prediscount.Price = currentdiscount.Price;
                        prediscount.Quantity = currentdiscount.Quantity;
                        prediscount.InvoiceNumber = currentdiscount.InvoiceNumber;
                    }
                    else
                        Db.Transactions.Add(currentdiscount);
                }
                else
                {
                    if (prediscount != null)
                        Db.Transactions.Remove(prediscount);
                }


                foreach (var item in obj.Items.Where(p => p.EntryType == (byte)EntryType.Item))
                {

                    if (item.Id > 0)
                    {
                        var preitem = preitems.FirstOrDefault(p => p.Id == item.Id);
                        if (preitem != null)
                        {

                            if (preitem.Credit > 0)
                                preitem.Credit = item.Credit;
                            else preitem.Debit = item.Debit;
                            preitem.Price = item.Price;
                            preitem.Quantity = item.Quantity;
                            preitem.InvoiceNumber = item.InvoiceNumber;
                            preitem.Discount = item.Discount;
                            preitem.Comments = obj.Comments;
                        }
                    }
                    else if (item.Id == 0)
                    {
                        item.Date = obj.Date;
                        item.CreatedDate = DateTime.Now;
                        item.Comments = obj.Comments;
                        Db.Transactions.Add(item);
                    }


                }
                foreach (var deltedtransaction in deletedTransaction)
                {
                    Db.Transactions.Remove(deltedtransaction);
                }

                SaveChanges();
                scope.Complete();
            }

        }
        public double? GetTodaySale(int accountId, DateTime date)
        {
            return FiscalCollection.Where(p => p.AccountId == accountId && EntityFunctions.TruncateTime(p.Date) == EntityFunctions.TruncateTime(date.Date) && p.TransactionType == VoucherType.Sale).Sum(p => (double?)p.Debit);
        }
        public List<Transaction> GetTransactions(int accountId, DateTime date1, DateTime date2)
        {
            return GetTransactions(accountId, date1, date2, false);
        }
        public List<Transaction> GetTransactions(int accountId, DateTime date1, DateTime date2, bool exludeOpening)
        {

            return GetTransactions(accountId, date1, date2, exludeOpening, 0);
        }
        public List<Transaction> GetTransactions(int accountId, DateTime date1, DateTime date2, bool exludeOpening, int branchId)
        {
            var openingBalancesType = new List<VoucherType> { VoucherType.OpeningBalance, VoucherType.AutoOpeningBalance };
            var query = FiscalCollection.Where(p => p.AccountId == accountId && p.Date >= date1 && p.Date <= date2).AsQueryable();
            if (branchId > 0)
                query = query.Where(p => p.BranchId == branchId);
            if (exludeOpening)
                return query.Where(p => !openingBalancesType.Contains(p.TransactionType)).ToList();
            else
                return query.ToList();

        }
        public List<Transaction> GetForexTransactions(int accountId, DateTime date1, DateTime date2, int currencyId)
        {

            var vehicleRepo = new VehicleRepository(this).AsQueryable();
            var vehicleVoucherRepo = new VehicleVoucherRepository(this).AsQueryable();
            var credits = vehicleRepo.Where(p => p.IsForex && p.VendorId == accountId && (EntityFunctions.TruncateTime(p.PurchaseDate) >= EntityFunctions.TruncateTime(date1) && EntityFunctions.TruncateTime(p.PurchaseDate) <= EntityFunctions.TruncateTime(date2)) && p.CurrencyId == currencyId).Select(p => new
            {
                p.PurchaseDate,
                p.ForexPrice,
                p.Id
            }).ToList();
            var debits = vehicleVoucherRepo.Where(p => p.TransactionType == VoucherType.ForexVoucher && p.AccountId1 == accountId && (EntityFunctions.TruncateTime(p.Date) >= EntityFunctions.TruncateTime(date1) && EntityFunctions.TruncateTime(p.Date) <= EntityFunctions.TruncateTime(date2)) && p.CurrencyId == currencyId).Select(p => new
            {
                p.AccountId1,
                p.ForexPrice,
                p.VoucherNumber,
                p.Date,
                p.Id,
                p.Comments
            }).ToList();
            var vehicleIds = credits.Select(p => p.Id).ToList();
            var creditVoucherTypes = new List<VoucherType>() { VoucherType.LocalPurchase, VoucherType.BLPurchase };
            var trans = FiscalCollection.Where(p => creditVoucherTypes.Contains(p.TransactionType) && vehicleIds.Contains(p.ReferenceId.Value) && p.AccountId == accountId).ToList();
            foreach (var item in trans)
            {
                var vehicleDetail = credits.FirstOrDefault(p => p.Id == item.ReferenceId.Value);
                if (vehicleDetail != null)
                {
                    item.Credit = vehicleDetail.ForexPrice;
                }
            }
            var debitTrans = debits.Select(p => new Transaction()
            {
                AccountId = p.AccountId1,
                Debit = p.ForexPrice,
                VoucherNumber = p.VoucherNumber,
                InvoiceNumber = p.VoucherNumber,
                Date = p.Date,
                Comments = p.Comments
            }).ToList();
            trans.AddRange(debitTrans);

            return trans;



        }


        public List<Transaction> GetVehicleTransactions(int vehicleId, DateTime date1, DateTime date2)
        {

            var transactionTypes = new List<VoucherType> { VoucherType.BL, VoucherType.CashPayments, VoucherType.BankPayments, VoucherType.VehiclePayable };
            return FiscalCollection.Where(p => p.MainEntityId == vehicleId && p.Date >= date1 && p.Date <= date2 && transactionTypes.Contains(p.TransactionType)).ToList();


        }
        public List<Transaction> GetVehicleAuctionnerPenaltyTransactions(int vehicleId, int accountId)
        {
            var transactionTypes = new List<VoucherType> { VoucherType.AuctionnerCharges, VoucherType.Penalty, VoucherType.AuctionnerPayments, VoucherType.PenaltyPayments };
            return FiscalCollection.Where(p => p.MainEntityId == vehicleId && transactionTypes.Contains(p.TransactionType) && p.AccountId == accountId).ToList();
        }
        public decimal GetVehicleAuctionnerPenaltyTotal(int vehicleId, int accountId)
        {
            var transactionTypes = new List<VoucherType> { VoucherType.AuctionnerCharges, VoucherType.Penalty, VoucherType.AuctionnerPayments, VoucherType.PenaltyPayments };
            var query = FiscalCollection.Where(p => p.MainEntityId == vehicleId && transactionTypes.Contains(p.TransactionType) && p.AccountId == accountId);

            if (query.Any())
                return query.Sum(p => (p.Debit - p.Credit));
            return 0;
        }



        public List<Transaction> GetTransactions(int accountId, int count)
        {
            return FiscalCollection.Where(p => p.AccountId == accountId).OrderByDescending(p => p.Id).Take(count).ToList();
        }


        public List<TransactionExtra> GetDetailedTransactions(int accountId, DateTime date1, DateTime date2)
        {
            return FiscalCollection.Where(p => p.AccountId == accountId && p.Date >= date1 && p.Date <= date2).Join(
                AsQueryable<SaleItem>(), a => a.AccountId, b => b.ItemId, (a, b) => new { Transaction = a, Item = b }).Select(p => new TransactionExtra
                {

                    AccountId = p.Transaction.AccountId,
                    CreatedDate = p.Transaction.CreatedDate,
                    Credit = p.Transaction.Credit,
                    Date = p.Transaction.Date,
                    Debit = p.Transaction.Debit,
                    EntryType = p.Transaction.EntryType,
                    InvoiceNumber = p.Transaction.InvoiceNumber,
                    Quantity = p.Item.Quantity,
                    VoucherNumber = p.Transaction.VoucherNumber,
                }).ToList();
        }

        public List<TransactionExtra> GetOpeningBalances(int parentId)
        {
            var query = string.Format("EXEC [DBO].[GetOpeningBalances] @CompanyId = {0}, @TransactionType = {1}, @Level2ParentId={2}, @FiscalId={3}", SiteContext.Current.User.CompanyId, (byte)VoucherType.OpeningBalance, parentId, SiteContext.Current.Fiscal.Id);
            return Db.Database.SqlQuery<OpeningBalanceExt>(query).Select(p => new TransactionExtra(p)).ToList();
        }

        public List<TrialBalanceEntry> GetTrailBalance(DateTime date)
        {
            return FiscalCollection.Where(p => p.Date < date).GroupBy(p => p.AccountId).Select(p => new
            {
                AccountId = p.Key,
                Credit = p.Sum(q => (decimal?)q.Credit),
                Debit = p.Sum(q => (decimal?)q.Debit)
            }).Where(p => p.Credit.HasValue || p.Debit.HasValue).Join(AsQueryable<Account>(),
             t => t.AccountId, a => a.Id, (t, a) => new { Transaction = t, Account = a }).Select(p => new
             {
                 AccountId = p.Account.Id,
                 Code = p.Account.AccountCode,
                 AccountTitle = p.Account.DisplayName,
                 Debit = p.Transaction.Debit ?? 0,
                 Credit = p.Transaction.Credit ?? 0,
             }).ToList().Select(p => new TrialBalanceEntry
             {
                 AccountId = p.AccountId,
                 Code = p.Code,
                 AccountTitle = p.AccountTitle,
                 Debit = p.Debit - p.Credit > 0 ? p.Debit - p.Credit : 0,
                 Credit = p.Debit - p.Credit < 0 ? p.Debit - p.Credit : 0,
             }).ToList();
        }
        //public List<TrialBalanceEntry> GetTrailBalance(DateTime date1, DateTime date2)
        //{
        //    return Collection.Where(p => p.Date >= date1 && p.Date <= date2).GroupBy(p => p.AccountId).Select(p => new
        //    {
        //        AccountId = p.Key,
        //        Credit = p.Sum(q => (decimal?)q.Credit),
        //        Debit = p.Sum(q => (decimal?)q.Debit)
        //    }).Where(p => p.Credit.HasValue || p.Debit.HasValue).Join(AsQueryable<Account>().Where(p => !p.IsDeleted),
        //     t => t.AccountId, a => a.Id, (t, a) => new { Transaction = t, Account = a }).Select(p => new
        //     {
        //         Code = p.Account.AccountCode,
        //         AccountId = p.Account.Id,
        //         AccountTitle = p.Account.DisplayName,
        //         Debit = p.Transaction.Debit ?? 0,
        //         Credit = p.Transaction.Credit ?? 0,
        //     }).ToList().Select(p => new TrialBalanceEntry
        //     {
        //         AccountId = p.AccountId,
        //         Code = p.Code,
        //         AccountTitle = p.AccountTitle,
        //         Debit = p.Debit - p.Credit > 0 ? p.Debit - p.Credit : 0,
        //         Credit = p.Debit - p.Credit < 0 ? p.Debit - p.Credit : 0,
        //     }).ToList();
        //}

        public List<TrialBalanceEntry> GetTrailBalance(DateTime fromDate, DateTime toDate)
        {
            var sqlquery = string.Format("EXEC [dbo].[GetDetailTrialBalances] @FromDate = '{0}', @ToDate = '{1}', @CompanyId = {2},@FiscalId = {3}", fromDate.ToString("yyyy-MM-dd"), toDate.ToString("yyyy-MM-dd"), SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id);
            return Db.Database.SqlQuery<TrialBalanceEntry>(sqlquery).ToList();
        }

        public List<TrialBalanceEntry> GetTrailBalance(int parentId, DateTime date)
        {
            return FiscalCollection.Where(p => p.Date <= date).GroupBy(p => p.AccountId).Select(p => new
            {
                AccountId = p.Key,
                Credit = p.Sum(q => (decimal?)q.Credit),
                Debit = p.Sum(q => (decimal?)q.Debit)
            }).Where(p => p.Credit.HasValue || p.Debit.HasValue)
            .Join(AsQueryable<Account>().Where(p => p.ParentId == parentId),
            t => t.AccountId, a => a.Id, (t, a) => new { Transaction = t, Account = a }).Select(p => new
            {
                Code = p.Account.AccountCode,
                AccountId = p.Account.Id,
                AccountTitle = p.Account.DisplayName,
                Debit = p.Transaction.Debit ?? 0,
                Credit = p.Transaction.Credit ?? 0,
            }).ToList().Select(p => new TrialBalanceEntry
            {
                AccountId = p.AccountId,
                Code = p.Code,
                AccountTitle = p.AccountTitle,
                Debit = p.Debit,
                Credit = p.Credit,
                Balance = p.Debit - p.Credit
            }).ToList();
        }
        public List<TrialBalanceEntry> GetTrailBalance(int parentId, DateTime date1, DateTime date2)
        {
            return GetTrailBalance(parentId, date1, date2, false);
        }
        public List<TrialBalanceEntry> GetTrailBalance(int parentId, DateTime date1, DateTime date2, bool excludeOpeningBalance)
        {

            var sqlquery = string.Format("EXEC dbo.GetChildBalances @ParentId={0}, @FromDate='{1}', @ToDate='{2}', @ExcludeOpeningBalance={3}",
                parentId, date1.ToString("yyyy-MM-dd"), date2.ToString("yyyy-MM-dd"), excludeOpeningBalance ? 1 : 0);

            return Db.Database.SqlQuery<TrialBalanceEntry>(sqlquery).ToList();


        }

        public Dictionary<int, decimal> GetFiscalTrial()
        {
            return FiscalCollection.GroupBy(p => p.AccountId).ToDictionary(p => p.Key, q => q.Sum(a => a.Debit - a.Credit));
        }
        public Dictionary<int, decimal> GetFiscalTrial(DateTime fromDate, DateTime toDate)
        {
            return GetFiscalTrial(fromDate, toDate, false);
        }
        public Dictionary<int, decimal> GetFiscalTrial(DateTime fromDate, DateTime toDate, bool isBeforeClosing)
        {
            var openingBalancesType = new List<VoucherType> { VoucherType.AutoClosingBalance };
            if (isBeforeClosing)
                return FiscalCollection.Where(p => p.Date >= fromDate && p.Date <= toDate && !openingBalancesType.Contains(p.TransactionType)).GroupBy(p => p.AccountId).ToDictionary(p => p.Key, q => q.Sum(a => a.Debit - a.Credit));
            else
                return FiscalCollection.Where(p => p.Date >= fromDate && p.Date <= toDate).GroupBy(p => p.AccountId).ToDictionary(p => p.Key, q => q.Sum(a => a.Debit - a.Credit));

        }

        public decimal GetStockValue(DateTime fromDate, DateTime toDate)
        {
            var sqlquery = string.Format("EXEC [dbo].[GetStockValue] @FromDate = '{0}', @ToDate = '{1}', @CompanyId = {2}, @FiscalId = {3}", fromDate.ToString("yyyy-MM-dd"), toDate.ToString("yyyy-MM-dd"), SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id);
            if (SiteContext.Current.User.CompanyId == 28)
                sqlquery = string.Format("EXEC [dbo].[GetIRISStockValue] @FromDate = '{0}', @ToDate = '{1}', @CompanyId = {2}, @FiscalId = {3}", fromDate.ToString("yyyy-MM-dd"), toDate.ToString("yyyy-MM-dd"), SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id);
            return Db.Database.SqlQuery<decimal>(sqlquery).FirstOrDefault();
        }

        public void ReopenFiscalYear(int currentFiscalYearId, int nextFiscalYearId)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var fiscal = Db.Fiscals.FirstOrDefault(p => p.Id == currentFiscalYearId);
                fiscal.IsClosed = false;
                var closingTransactions = FiscalCollection.Where(p => p.TransactionType == VoucherType.AutoClosingBalance).ToList();
                foreach (var item in closingTransactions)
                {
                    Db.Transactions.Remove(item);
                }
                var openingTransactions = Db.Transactions.Where(p => p.CompanyId == SiteContext.Current.User.CompanyId
                                        && p.FiscalId == nextFiscalYearId && p.TransactionType == VoucherType.AutoOpeningBalance).ToList();
                foreach (var item in openingTransactions)
                {
                    Db.Transactions.Remove(item);
                }
                var stockOpeningBalances = AsQueryable<OpeningBalance>().Where(p => p.FiscalId == nextFiscalYearId && p.OpeningBalanceTypeId == (byte)OpeningBalances.Stock).ToList();
                foreach (var item in stockOpeningBalances)
                {
                    Db.OpeningBalances.Remove(item);
                }
                Db.SaveChanges();
                scope.Complete();
            }
        }

        public void CloseFiscalYear(int fiscalYearId, List<Transaction> transactions, List<OpeningBalance> stockOpeningBalances, int nextFiscalId)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                Db.Configuration.AutoDetectChangesEnabled = false;
                var fiscal = Db.Fiscals.FirstOrDefault(p => p.Id == fiscalYearId);
                fiscal.IsClosed = true;
                //Delete existing enteries
                var closingTransactions = FiscalCollection.Where(p => p.TransactionType == VoucherType.AutoClosingBalance).ToList();
                foreach (var item in closingTransactions)
                {
                    Db.Transactions.Remove(item);
                }
                var openingTransactions = Db.Transactions.Where(p => p.CompanyId == SiteContext.Current.User.CompanyId
                                        && p.FiscalId == nextFiscalId && p.TransactionType == VoucherType.AutoOpeningBalance).ToList();
                foreach (var item in openingTransactions)
                {
                    Db.Transactions.Remove(item);
                }
                var dbStockOpeningBalances = AsQueryable<OpeningBalance>().Where(p => p.FiscalId == nextFiscalId && p.OpeningBalanceTypeId == (byte)OpeningBalances.Stock).ToList();
                foreach (var item in dbStockOpeningBalances)
                {
                    Db.OpeningBalances.Remove(item);
                }
                foreach (var item in transactions)
                {
                    Db.Transactions.Add(item);
                }
                foreach (var item in stockOpeningBalances)
                {
                    Db.OpeningBalances.Add(item);
                }
                Db.Configuration.AutoDetectChangesEnabled = true;
                Db.SaveChanges();
                scope.Complete();
            }
        }

        public List<StockReport> GetStock(DateTime fromdate, DateTime todate, int accountId)
        {


            var sqlquery = string.Format("EXEC [dbo].[GetStock] @FromDate = '{0}', @ToDate = '{1}', @CompanyId = {2},@FiscalId = {3}", fromdate.ToString("yyyy-MM-dd"), todate.ToString("yyyy-MM-dd"), SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id);

            return Db.Database.SqlQuery<StockReport>(sqlquery).ToList();
        }

        public List<GetDairyProfileReport> GetDairyProfile(DateTime fromdate, DateTime todate)
        {
            var sqlquery = string.Format("EXEC [dbo].[GetDairyAdjustments] @FromDate = '{0}', @ToDate = '{1}', @CompanyId = {2}", fromdate.ToString("yyyy-MM-dd"), todate.ToString("yyyy-MM-dd"), SiteContext.Current.User.CompanyId);
            return Db.Database.SqlQuery<GetDairyProfileReport>(sqlquery).ToList();
        }
        public List<StockReport> GetStockByQuantity(DateTime fromdate, DateTime todate, int accountId)
        {
            var sqlquery = string.Format("EXEC [dbo].[GetStockByQuantitySize] @FromDate = '{0}', @ToDate = '{1}', @CompanyId = {2},@FiscalId = {3}", fromdate.ToString("yyyy-MM-dd"), todate.ToString("yyyy-MM-dd"), SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id);
            return Db.Database.SqlQuery<StockReport>(sqlquery).ToList();
        }
        public List<StockReport> GetStockByWeight(DateTime fromdate, DateTime todate, int accountId)
        {
            var sqlquery = string.Format("EXEC [dbo].[GetStockByWeight] @FromDate = '{0}', @ToDate = '{1}', @CompanyId = {2},@FiscalId = {3}", fromdate.ToString("yyyy-MM-dd"), todate.ToString("yyyy-MM-dd"), SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id);
            return Db.Database.SqlQuery<StockReport>(sqlquery).ToList();
        }
        public List<StockReport> GetAllStock()
        {


            var sqlquery = string.Format("EXEC [dbo].[GetAllStock] @CompanyId = {0},@FiscalId = {1}", SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id);
            return Db.Database.SqlQuery<StockReport>(sqlquery).ToList();
        }
        public decimal GetStockOpeningBalance(int itemId)
        {
            var sqlquery = string.Format("EXEC [dbo].[GetStockOpeningBalance] @ItemId = {0}, @CompanyId = {1},@FiscalId = {2}", itemId, SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id);
            return Db.Database.SqlQuery<decimal>(sqlquery).ToList().FirstOrDefault();
        }
        public List<StockReport> GetStockOpeningBalance(List<int> itemIds)
        {
            var sqlquery = string.Format("EXEC [dbo].[GetStockOpeningBalanceByItemIds] @CompanyId = {0},@FiscalId = {1},@ItemIds = '{2}'", SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, string.Join(",", itemIds));
            return Db.Database.SqlQuery<StockReport>(sqlquery).ToList();
        }
        public List<ItemAvgRates> GetStockAvgRates(List<int> itemIds, DateTime toDate)
        {
            var sqlquery = string.Format("EXEC [dbo].[GetItemAveragePurchasePrice] @CompanyId = {0},@FiscalId = {1},@ItemIds = '{2}',@ToDate='{3}'", SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, string.Join(",", itemIds), toDate.ToString("yyyy-MM-dd"));
            return Db.Database.SqlQuery<ItemAvgRates>(sqlquery).ToList();
        }

        public decimal GetSummary(VoucherType type1, VoucherType type2)
        {
            var query = FiscalCollection.Where(p => p.TransactionType == type1 || p.TransactionType == type2);
            return query.Any() ? query.Sum(p => p.Debit - p.Credit) : 0;
        }
        public decimal GetSummary(VoucherType type)
        {
            var query = FiscalCollection.Where(p => p.TransactionType == type);
            return query.Any() ? query.Sum(p => p.Debit - p.Credit) : 0;
        }
        public decimal GetSummary(int accountId)
        {
            var query = FiscalCollection.Where(p => p.AccountId == accountId);
            return query.Any() ? query.Sum(p => p.Debit - p.Credit) : 0;
        }
        public decimal GetSummary(params int[] accountIds)
        {
            var query = FiscalCollection.Where(p => accountIds.Contains(p.AccountId));
            return query.Any() ? query.Sum(p => p.Debit - p.Credit) : 0;
        }
        public decimal GetSummary(VoucherType type, DateTime d1, DateTime d2)
        {
            var query = FiscalCollection.Where(p => p.TransactionType == type && p.Date >= d1 && p.Date <= d2);
            if (query.Any())
                return query.Sum(p => p.Debit - p.Credit);
            return 0;
        }
        public bool CheckIfTransactionExist(int accountId)
        {
            return Collection.Any(p => p.AccountId == accountId);
        }
        public bool CheckIfTransactionExist(List<int> accounts)
        {
            return Collection.Any(p => accounts.Contains(p.AccountId));
        }

        public void HardDelete(int voucherNumber, VoucherType type)
        {
            HardDelete(voucherNumber, new List<VoucherType>() { type });
        }
        public void HardDelete(int voucherNumber, List<VoucherType> types)
        {
            HardDelete(new List<int> { voucherNumber }, types);
        }
        public void HardDelete(List<int> voucherNumbers, VoucherType type)
        {
            HardDelete(voucherNumbers, new List<VoucherType> { type });
        }
        public void HardDelete(List<int> voucherNumber, List<VoucherType> types)
        {

            if (voucherNumber.Count() > 0 && types.Count() > 0)
            {

                var query = string.Format("DELETE FROM dbo.Transactions WHERE TransactionType IN({0}) AND VoucherNumber IN({1})  AND FiscalId={2} AND CompanyId={3}", string.Join(",", types.Select(p => Numerics.GetByte(p))), string.Join(",", voucherNumber), SiteContext.Current.Fiscal.Id, SiteContext.Current.User.CompanyId);
                Db.Database.ExecuteSqlCommand(query);
            }
        }
        public void HardDeleteByReferenceId(int referenceId)
        {
            var query = string.Format("DELETE FROM dbo.Transactions WHERE ReferenceId={0} AND FiscalId={1} AND CompanyId={2}", referenceId, SiteContext.Current.Fiscal.Id, SiteContext.Current.User.CompanyId);
            Db.Database.ExecuteSqlCommand(query);
        }
        public void HardDeleteByReferenceIdTransactionType(int referenceId, VoucherType type)
        {
            HardDeleteByReferenceIdTransactionType(referenceId, new List<VoucherType> { type });
        }

        public void HardDeleteByReferenceIdTransactionType(int referenceId, List<VoucherType> types)
        {

            if (types.Count() > 0)
            {

                var query = string.Format("DELETE FROM dbo.Transactions WHERE TransactionType IN({0}) AND ReferenceId IN({1})  AND FiscalId={2} AND CompanyId={3}", string.Join(",", types.Select(p => Numerics.GetByte(p))), string.Join(",", referenceId), SiteContext.Current.Fiscal.Id, SiteContext.Current.User.CompanyId);
                Db.Database.ExecuteSqlCommand(query);
            }
        }
        public Transaction GetLastByAccountId(int accountid)
        {
            return Collection.Where(p => p.AccountId == accountid).OrderByDescending(p => p.Id).FirstOrDefault();
        }
        public Transaction GetByAccountId(int accountid)
        {
            return Collection.Where(p => p.AccountId == accountid).FirstOrDefault();
        }

        public List<SaleReportByArea> GetSaleReportByAreaDateRange(DateTime fromdate, DateTime todate, int accountId)
        {
            var sqlquery = "";
            if (accountId == 0)
                sqlquery = string.Format("EXEC [dbo].[GetAreaWiseSaleReportByDateRange] @FromDate = '{0}', @ToDate = '{1}', @CompanyId = {2},@FiscalId = {3}", fromdate.ToString("yyyy-MM-dd"), todate.ToString("yyyy-MM-dd"), SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id);
            else
                sqlquery = string.Format("EXEC [dbo].[GetAreaWiseSaleReportByDateRange] @FromDate = '{0}', @ToDate = '{1}', @CompanyId = {2},@FiscalId = {3},@ParentAccountId={4}", fromdate.ToString("yyyy-MM-dd"), todate.ToString("yyyy-MM-dd"), SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, accountId);

            //var sqlquery = string.Format("EXEC [dbo].[GetAreaWiseSaleReport] @FromDate = {0}, @ToDate = {1}, @CompanyId = {2},@FiscalId = {3},@ParentAccountId={4}", "2015-07-01", "2016-06-30", 40, 22, 31914);
            //   var sqlquery = string.Format("EXEC [dbo].[GetAreaWiseSaleReport] @FromDate = '{0}', @ToDate = '{1}', @CompanyId = {2},@FiscalId = {3},@ParentAccountId={4}", fromdate.ToString("yyyy-MM-dd"), todate.ToString("yyyy-MM-dd"), SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, "NULL");
            return Db.Database.SqlQuery<SaleReportByArea>(sqlquery).ToList();
        }
        public List<LessAndSampling> GetLeadAndSampling(DateTime fromdate, DateTime todate, int reporttype, int parentAccountId)
        {
            var sqlquery = "";

            var parentAccountId_N = parentAccountId + "";
            var saleType_N = "NULL";
            if (parentAccountId == 0) parentAccountId_N = "NULL";
            if (reporttype == (byte)SaleTypeEnum.Sampling) saleType_N = Convert.ToString((byte)SaleTypeEnum.Sampling);


            //if (parentAccountId == 0 && reporttype == 3)
            //    sqlquery = string.Format("EXEC [dbo].[GetLessAndSamplingByDates] @FromDate = '{0}', @ToDate = '{1}', @CompanyId = {2},@FiscalId = {3},@ReportType={4},@SaleType={5},@ParentAccountId={6}", fromdate.ToString("yyyy-MM-dd"), todate.ToString("yyyy-MM-dd"), SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, reporttype, reporttype, "NULL");
            //else
            //    sqlquery = string.Format("EXEC [dbo].[GetLessAndSamplingByDates] @FromDate = '{0}', @ToDate = '{1}', @CompanyId = {2},@FiscalId = {3},@ReportType={4},@ParentAccountId={5}", fromdate.ToString("yyyy-MM-dd"), todate.ToString("yyyy-MM-dd"), SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, reporttype, parentAccountId);

            //if (reporttype == 3)
            //    sqlquery = string.Format("EXEC [dbo].[GetLessAndSamplingByDates] @FromDate = '{0}', @ToDate = '{1}', @CompanyId = {2},@FiscalId = {3},@ReportType={4},@SaleType={5},@ParentAccountId={6}", fromdate.ToString("yyyy-MM-dd"), todate.ToString("yyyy-MM-dd"), SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, reporttype, 3, "NULL");
            //else
            //    sqlquery = string.Format("EXEC [dbo].[GetLessAndSamplingByDates] @FromDate = '{0}', @ToDate = '{1}', @CompanyId = {2},@FiscalId = {3},@ReportType={4},@ParentAccountId={5}", fromdate.ToString("yyyy-MM-dd"), todate.ToString("yyyy-MM-dd"), SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, reporttype, parentAccountId);

            sqlquery = string.Format("EXEC [dbo].[GetLessAndSamplingByDates] @FromDate = '{0}', @ToDate = '{1}', @CompanyId = {2},@FiscalId = {3},@ReportType={4},@SaleType={5},@ParentAccountId={6}",
                fromdate.ToString("yyyy-MM-dd"),
                todate.ToString("yyyy-MM-dd"),
                SiteContext.Current.User.CompanyId,
                SiteContext.Current.Fiscal.Id,
                reporttype,
                saleType_N,
                parentAccountId_N);

            //var sqlquery = string.Format("EXEC [dbo].[GetAreaWiseSaleReport] @FromDate = {0}, @ToDate = {1}, @CompanyId = {2},@FiscalId = {3},@ParentAccountId={4}", "2015-07-01", "2016-06-30", 40, 22, 31914);
            //   var sqlquery = string.Format("EXEC [dbo].[GetAreaWiseSaleReport] @FromDate = '{0}', @ToDate = '{1}', @CompanyId = {2},@FiscalId = {3},@ParentAccountId={4}", fromdate.ToString("yyyy-MM-dd"), todate.ToString("yyyy-MM-dd"), SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, "NULL");
            return Db.Database.SqlQuery<LessAndSampling>(sqlquery).ToList();
        }
        public List<SaleReportByArea> GetSaleReportByArea(DateTime fromdate, DateTime todate, int accountId)
        {
            var sqlquery = "";
            if (accountId == 0)
                sqlquery = string.Format("EXEC [dbo].[GetAreaWiseSaleReport] @FromDate = '{0}', @ToDate = '{1}', @CompanyId = {2},@FiscalId = {3}", fromdate.ToString("yyyy-MM-dd"), todate.ToString("yyyy-MM-dd"), SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id);
            else
                sqlquery = string.Format("EXEC [dbo].[GetAreaWiseSaleReport] @FromDate = '{0}', @ToDate = '{1}', @CompanyId = {2},@FiscalId = {3},@ParentAccountId={4}", fromdate.ToString("yyyy-MM-dd"), todate.ToString("yyyy-MM-dd"), SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, accountId);

            //var sqlquery = string.Format("EXEC [dbo].[GetAreaWiseSaleReport] @FromDate = {0}, @ToDate = {1}, @CompanyId = {2},@FiscalId = {3},@ParentAccountId={4}", "2015-07-01", "2016-06-30", 40, 22, 31914);
            //   var sqlquery = string.Format("EXEC [dbo].[GetAreaWiseSaleReport] @FromDate = '{0}', @ToDate = '{1}', @CompanyId = {2},@FiscalId = {3},@ParentAccountId={4}", fromdate.ToString("yyyy-MM-dd"), todate.ToString("yyyy-MM-dd"), SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, "NULL");
            return Db.Database.SqlQuery<SaleReportByArea>(sqlquery).ToList();
        }

        public List<UsmanBrosCustomer> GetUsmanBrosCustomers(int accountId)
        {
            var query = "";
            if (accountId == 0)
                query = string.Format("EXEC [dbo].[GetUsmanBrosCutomers] @CompanyId={0}", SiteContext.Current.User.CompanyId);
            else
                query = string.Format("EXEC [dbo].[GetUsmanBrosCutomers] @CompanyId={0}, @AreaId={1}", SiteContext.Current.User.CompanyId, accountId);
            return Db.Database.SqlQuery<UsmanBrosCustomer>(query).ToList();
        }

    }
}