using System.Linq;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;

namespace AccountEx.BussinessLogic
{
    public static class FiscalYearManager
    {

        public static DateTime GetStartDate()
        {
            return SiteContext.Current.Fiscal.FromDate;
        }
        public static DateTime GetEndDate()
        {
            return SiteContext.Current.Fiscal.ToDate;
        }
        public static DateTime GetCurrentMonthStartDate()
        {
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            return startDate;
        }
        public static string CreateMonthYearDropDownOptions(bool includeEmptyOption = true)
        {
            var fromDate = SiteContext.Current.Fiscal.FromDate;
            var toDate = SiteContext.Current.Fiscal.ToDate;
            var html = "";
            if (includeEmptyOption)
                html = "<option></option>";
            while (fromDate < toDate)
            {
                var strMonth = fromDate.ToString("MMM") + " " + fromDate.Year;
                html += "<option data-custom='" + fromDate.Year + "' value='" + fromDate.Month + "'>" + strMonth + "</option>";
                fromDate = fromDate.AddMonths(1);
            }
            return html;

        }
        public static string GetFiscalYearsDropDownOptions(bool includeEmptyOption = false)
        {
            var years = new FiscalRepository().GetShortNames();
            var html = "";
            if (includeEmptyOption)
                html = "<option></option>";
            var selected = "selected='selected'";
            foreach (var year in years)
            {
                selected = "";
                if (SiteContext.Current.Fiscal.Id == year.Id)
                    selected = "selected='selected'";
                html += "<option " + selected + " value='" + year.Id + "'>" + year.Name + "</option>";
            }
            return html;

        }
        public static DateTime GetCurrentMonthEndDate()
        {
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            return endDate;
        }
        public static bool IsValidFiscalDate(DateTime date)
        {
            return SiteContext.Current.Fiscal.FromDate <= date && SiteContext.Current.Fiscal.ToDate >= date;
        }

        public static void ReopenFiscalYear()
        {
            var fiscalRepo = new FiscalRepository();
            var currentFiscal = SiteContext.Current.Fiscal;
            var nextFiscal = fiscalRepo.GetNextFiscal(currentFiscal.ToDate, SiteContext.Current.User.CompanyId, true);
            var fromDate = SiteContext.Current.Fiscal.FromDate;
            var toDate = SiteContext.Current.Fiscal.ToDate;
            if (!currentFiscal.IsClosed) throw new OwnException("Fiscal year already opened");
            if (nextFiscal.IsClosed)
                throw new OwnException("Can't reopen current fiscal year until next fiscal year is closed");

            new TransactionRepository().ReopenFiscalYear(currentFiscal.Id, nextFiscal.Id);

        }
        public static void CloseFiscalYear()
        {
            var fiscalRepo = new FiscalRepository();

            var currentFiscal = SiteContext.Current.Fiscal;
            var nextFiscal = fiscalRepo.GetNextFiscal(currentFiscal.ToDate, SiteContext.Current.User.CompanyId, true);
            var previousFiscal = fiscalRepo.GetPreviousFiscal(currentFiscal.FromDate, SiteContext.Current.User.CompanyId);
            var fromDate = SiteContext.Current.Fiscal.FromDate;
            var toDate = SiteContext.Current.Fiscal.ToDate;

            #region Validation
            if (currentFiscal.IsClosed) throw new OwnException("Fiscal year already closed");
            if (previousFiscal != null && previousFiscal.IsClosed == false)
                throw new OwnException("Can't close current fiscal year until previous fiscal year is opened");

            if (SettingManager.PLSAccountId == 0) throw new OwnException("PLS account is not set in setting");
            if (SettingManager.StockValueAccountId == 0) throw new OwnException("StockValue account is not set in setting");
            if (SettingManager.UnappropriatedProfitAccountId == 0) throw new OwnException("Unappropriated Profit account is not set in setting");
            var accountRepo = new AccountRepository();

            if (accountRepo.AsQueryable().FirstOrDefault(p => p.Id == SettingManager.PLSAccountId).Level != 4)
                throw new OwnException("PLS account is not selected as 4th level account");
            if (accountRepo.AsQueryable().FirstOrDefault(p => p.Id == SettingManager.StockValueAccountId).Level != 4)
                throw new OwnException("StockValue account is not selected as 4th level account");
            if (accountRepo.AsQueryable().FirstOrDefault(p => p.Id == SettingManager.UnappropriatedProfitAccountId).Level != 4)
                throw new OwnException("Unappropriated Profit account is not selected as 4th level account");
            #endregion

            var transRepo = new TransactionRepository();
            decimal nextFiscalDifference = 0;
            var accounts = new Dictionary<int, string>();
            var expenseAccounts = new AccountRepository().GetLeafAccounts(SettingManager.ExpensesHeadId);
            var revenuAccounts = new AccountRepository().GetLeafAccounts(SettingManager.RevenuHeadId);
            var ids = revenuAccounts.Select(p => p.Id).ToList();
            revenuAccounts.AddRange(expenseAccounts.Where(p => !ids.Contains(p.Id)));

            accounts = revenuAccounts.ToDictionary(p => p.Id, q => q.Name);
            //if (!accounts.ContainsKey(SettingManager.PLSAccountId)) accounts.Add(SettingManager.PLSAccountId, SettingManager.PLSAc);
            if (!accounts.ContainsKey(SettingManager.ServicesAccountId)) accounts.Add(SettingManager.ServicesAccountId, SettingManager.ServicesAc);

            if (!accounts.ContainsKey(SettingManager.SaleAccountHeadId)) accounts.Add(SettingManager.SaleAccountHeadId, SettingManager.SaleAc);
            if (!accounts.ContainsKey(SettingManager.SaleReturnAccountHeadId)) accounts.Add(SettingManager.SaleReturnAccountHeadId, SettingManager.SaleReturnAc);
            if (!accounts.ContainsKey(SettingManager.PurchaseAccountHeadId)) accounts.Add(SettingManager.PurchaseAccountHeadId, SettingManager.PurchaseAc);
            if (!accounts.ContainsKey(SettingManager.PurchaseReturnAccountHeadId)) accounts.Add(SettingManager.PurchaseReturnAccountHeadId, SettingManager.PurchaseReturnAc);

            if (!accounts.ContainsKey(SettingManager.GstSaleAccountHeadId)) accounts.Add(SettingManager.GstSaleAccountHeadId, SettingManager.GstSaleAc);
            if (!accounts.ContainsKey(SettingManager.GstSaleReturnAccountHeadId)) accounts.Add(SettingManager.GstSaleReturnAccountHeadId, SettingManager.GstSaleReturnAc);
            if (!accounts.ContainsKey(SettingManager.GstPurchaseAccountHeadId)) accounts.Add(SettingManager.GstPurchaseAccountHeadId, SettingManager.GstPurchaseAc);
            if (!accounts.ContainsKey(SettingManager.GstPurchaseReturnAccountHeadId)) accounts.Add(SettingManager.GstPurchaseReturnAccountHeadId, SettingManager.GstPurchaseReturnAc);



            if (SettingManager.AllowCGS && accounts.ContainsKey(SettingManager.StockValueAccountId)) accounts.Remove(SettingManager.StockValueAccountId);
            decimal debit = 0, credit = 0;

            var dt = DateTime.Now;
            List<Transaction> transactions = new List<Transaction>();
            var trialEntries = transRepo.GetFiscalTrial();

            trialEntries = trialEntries.Where(p => p.Value != 0).ToDictionary(p => p.Key, q => q.Value);

            var diff = trialEntries.Sum(p => p.Value);
            foreach (var item in accounts)
            {
                if (!trialEntries.ContainsKey(item.Key) || trialEntries[item.Key] == 0) continue;

                // Balance = debit - credit
                var balance = trialEntries[item.Key];

                debit = credit = 0;

                if (balance > 0) debit = balance;
                else credit = balance * -1;

                // Reverse transaction to Nill account balances, change debit to credit and credit to debit
                transactions.Add(new Transaction()
                {
                    AccountId = item.Key,
                    Debit = credit,
                    Credit = debit,
                    CompanyId = SiteContext.Current.User.CompanyId,
                    Date = currentFiscal.ToDate,
                    TransactionType = VoucherType.AutoClosingBalance,
                    FiscalId = SiteContext.Current.Fiscal.Id,
                    CreatedDate = dt,
                    Comments = "Fiscal year closing reverse transaction for Account: " + item.Value,
                });

                // Closing account balances moved to PLS account
                transactions.Add(new Transaction()
                {
                    AccountId = SettingManager.PLSAccountId,
                    Debit = debit,
                    Credit = credit,
                    CompanyId = SiteContext.Current.User.CompanyId,
                    Date = currentFiscal.ToDate,
                    TransactionType = VoucherType.AutoClosingBalance,
                    FiscalId = SiteContext.Current.Fiscal.Id,
                    CreatedDate = dt,
                    Comments = "Fiscal year closing transaction for Account: " + item.Value,
                });

                trialEntries.Remove(item.Key);
            }
            nextFiscalDifference = transactions.Where(p => p.FiscalId == nextFiscal.Id).Sum(p => p.Debit - p.Credit);
            var openingStockValue = transRepo.GetOpeningBalance(SettingManager.StockValueAccountId, fromDate);
            debit = credit = 0;
            if (openingStockValue > 0) debit = openingStockValue;
            else credit = openingStockValue * -1;
            if (SettingManager.AllowCGS == false && openingStockValue != 0)
            {
                // Reverse transaction to Nill openingStockValue, change debit to credit and credit to debit
                transactions.Add(new Transaction()
                {
                    AccountId = SettingManager.StockValueAccountId,
                    Debit = credit,
                    Credit = debit,
                    CompanyId = SiteContext.Current.User.CompanyId,
                    Date = currentFiscal.ToDate,
                    TransactionType = VoucherType.AutoClosingBalance,
                    FiscalId = SiteContext.Current.Fiscal.Id,
                    CreatedDate = dt,
                    Comments = "Fiscal year closing reverse transaction for Account: " + SettingManager.StockValueAc,
                });

                // Closing account balances moved to PLS account
                transactions.Add(new Transaction()
                {
                    AccountId = SettingManager.PLSAccountId,
                    Debit = debit,
                    Credit = credit,
                    CompanyId = SiteContext.Current.User.CompanyId,
                    Date = currentFiscal.ToDate,
                    TransactionType = VoucherType.AutoClosingBalance,
                    FiscalId = SiteContext.Current.Fiscal.Id,
                    CreatedDate = dt,
                    Comments = "Fiscal year closing stock value transaction for Account: " + SettingManager.StockValueAc,
                });
            }
            nextFiscalDifference = transactions.Where(p => p.FiscalId == nextFiscal.Id).Sum(p => p.Debit - p.Credit);
            var currentStockValue = transRepo.GetStockValue(fromDate, toDate);
            debit = credit = 0;
            if (currentStockValue > 0) debit = currentStockValue;
            else credit = currentStockValue * -1;
            if (SettingManager.AllowCGS == false && currentStockValue != 0)
            {

                // Reverse transaction to move calculated stock value to PLS, change debit to credit and credit to debit
                transactions.Add(new Transaction()
                {
                    AccountId = SettingManager.PLSAccountId,
                    Debit = credit,
                    Credit = debit,
                    CompanyId = SiteContext.Current.User.CompanyId,
                    Date = currentFiscal.ToDate,
                    TransactionType = VoucherType.AutoClosingBalance,
                    FiscalId = SiteContext.Current.Fiscal.Id,
                    CreatedDate = dt,
                    Comments = "Fiscal year closing transaction for Calculated stock value to PLS A/c",
                });


                // Closing transaction to move calculated stock value to Stock Value A/c
                transactions.Add(new Transaction()
                {
                    AccountId = SettingManager.StockValueAccountId,
                    Debit = debit,
                    Credit = credit,
                    CompanyId = SiteContext.Current.User.CompanyId,
                    Date = currentFiscal.ToDate,
                    TransactionType = VoucherType.AutoClosingBalance,
                    FiscalId = SiteContext.Current.Fiscal.Id,
                    CreatedDate = dt,
                    Comments = "Fiscal year closing reverse transaction for Calculated stock value",
                });

                //Fiscal year opening balance

                //debit = credit = 0;
                //var nextYearOpeningStockValue = currentStockValue + openingStockValue;
                //if (currentStockValue > 0) debit = nextYearOpeningStockValue;
                //else credit = nextYearOpeningStockValue * -1;

                transactions.Add(new Transaction()
                {
                    AccountId = SettingManager.StockValueAccountId,
                    Debit = debit,
                    Credit = credit,
                    CompanyId = SiteContext.Current.User.CompanyId,
                    Date = nextFiscal.FromDate,
                    TransactionType = VoucherType.AutoOpeningBalance,
                    FiscalId = nextFiscal.Id,
                    CreatedDate = dt,
                    Comments = "Fiscal year opening balance for Stock A/c",
                });
            }
            nextFiscalDifference = transactions.Where(p => p.FiscalId == nextFiscal.Id).Sum(p => p.Debit - p.Credit);

            foreach (var item in trialEntries)
            {
                if (item.Value == 0) continue;
                debit = credit = 0;

                if (item.Value > 0) debit = item.Value;
                else credit = item.Value * -1;

                // Stock a/c handled seperately
                if (item.Key == SettingManager.UnappropriatedProfitAccountId) continue;
                if (SettingManager.AllowCGS == false && item.Key == SettingManager.StockValueAccountId) continue;

                transactions.Add(new Transaction()
                {
                    AccountId = item.Key,
                    Debit = debit,
                    Credit = credit,
                    CompanyId = SiteContext.Current.User.CompanyId,
                    Date = nextFiscal.FromDate,
                    TransactionType = VoucherType.AutoOpeningBalance,
                    FiscalId = nextFiscal.Id,
                    CreatedDate = dt,
                    Comments = "Fiscal year opening balance for Account: " + item.Value,
                });
            }
            nextFiscalDifference = transactions.Where(p => p.FiscalId == nextFiscal.Id).Sum(p => p.Debit - p.Credit);
            var unappropriatedProfit = transactions.Where(p => p.AccountId == SettingManager.PLSAccountId).Sum(p => p.Debit - p.Credit);
            debit = credit = 0;
            if (unappropriatedProfit > 0) debit = unappropriatedProfit;
            else credit = unappropriatedProfit * -1;
            if (unappropriatedProfit != 0)
            {


                // Reverse transaction to Nill PLS account, change debit to credit and credit to debit
                transactions.Add(new Transaction()
                {
                    AccountId = SettingManager.PLSAccountId,
                    Debit = credit,
                    Credit = debit,
                    CompanyId = SiteContext.Current.User.CompanyId,
                    Date = currentFiscal.ToDate,
                    TransactionType = VoucherType.AutoClosingBalance,
                    FiscalId = SiteContext.Current.Fiscal.Id,
                    CreatedDate = dt,
                    Comments = "Fiscal year closing transaction to Nill PLS A/c",
                });

                // Closing PLS account balances moved to Unappropriated Profit account
                transactions.Add(new Transaction()
                {
                    AccountId = SettingManager.UnappropriatedProfitAccountId,
                    Debit = debit,
                    Credit = credit,
                    CompanyId = SiteContext.Current.User.CompanyId,
                    Date = currentFiscal.ToDate,
                    TransactionType = VoucherType.AutoClosingBalance,
                    FiscalId = SiteContext.Current.Fiscal.Id,
                    CreatedDate = dt,
                    Comments = "Fiscal year closing transaction for PLS A/c to Unappropriated Profit A/c",
                });

                // Opening balance transaction for Unappropriated Profit A/c in next fiscal year
                if (trialEntries.ContainsKey(SettingManager.UnappropriatedProfitAccountId))
                    credit += trialEntries[SettingManager.UnappropriatedProfitAccountId] * -1;
                transactions.Add(new Transaction()
                {
                    AccountId = SettingManager.UnappropriatedProfitAccountId,
                    Debit = debit,
                    Credit = credit,
                    CompanyId = SiteContext.Current.User.CompanyId,
                    Date = nextFiscal.FromDate,
                    TransactionType = VoucherType.AutoOpeningBalance,
                    FiscalId = nextFiscal.Id,
                    CreatedDate = dt,
                    Comments = "Fiscal year opening balance for Unappropriated Profit A/c",
                });
            }


            var currentFiscalDifference = transactions.Where(p => p.FiscalId == currentFiscal.Id).Sum(p => p.Debit - p.Credit);
            if (currentFiscalDifference != 0)
            {
                throw new OwnException(string.Format("Closing Debit/Credit are not equeal in current fiscal year by amount {0} that may cause difference in trial balance", currentFiscalDifference));
            }
            //string json = Newtonsoft.Json.JsonConvert.SerializeObject(transactions);

            nextFiscalDifference = transactions.Where(p => p.FiscalId == nextFiscal.Id).Sum(p => p.Debit - p.Credit);
            if (nextFiscalDifference != 0)
            {
                throw new OwnException(string.Format("Opening balance Debit/Credit are not equeal for next fiscal year by amount {0} that may cause difference in trial balance", nextFiscalDifference));
            }

            var stockOpeningBalances = transRepo.GetStock(currentFiscal.FromDate, currentFiscal.ToDate, 0)
                .Select(p => new OpeningBalance()
                {
                    AccountId = p.AccountId,
                    CompanyId = SiteContext.Current.User.CompanyId,
                    CreatedBy = SiteContext.Current.User.Id,
                    FiscalId = nextFiscal.Id,
                    Quantity = p.Balance,
                    OpeningBalanceTypeId = (byte)OpeningBalances.Stock,
                    UnitPrice = p.AvgRate,
                }).ToList();

            transRepo.CloseFiscalYear(currentFiscal.Id, transactions, stockOpeningBalances, nextFiscal.Id);

        }



    }
}
