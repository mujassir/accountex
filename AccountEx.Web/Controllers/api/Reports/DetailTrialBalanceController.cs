using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace AccountEx.Web.Controllers.api
{
    public class DetailTrialBalanceController : BaseApiController
    {
        // GET api/test
        public ApiResponse Get()
        {
            return GetTrialBalance();
        }
        private ApiResponse GetTrialBalance()
        {
            new FormSettingRepository().GetFormSettingByVoucherType("Recovery");
            var queryString = Request.RequestUri.ParseQueryString();
            var date2 = DateTime.Parse(queryString["date2"]);
            var isfilterbyzero = queryString["IsFilter"];
            List<TrialBalanceEntry> transactions;
            if (string.IsNullOrWhiteSpace(queryString["date1"]))
            {
                transactions = new TransactionRepository().GetTrailBalance(date2);
            }
            else
            {
                var date1 = DateTime.Parse(queryString["date1"]);
                transactions = new TransactionRepository().GetTrailBalance(date1, date2);

            }
            IEnumerable<TrialBalanceEntry> query;
            if (isfilterbyzero != null && isfilterbyzero == "true")
            {
                query = transactions.Where(p => p.AccountId > 0 && !string.IsNullOrWhiteSpace(p.AccountTitle) && p.Balance != 0).ToList();
            }
            else
            {
                query = transactions.Where(p => p.AccountId > 0 && !string.IsNullOrWhiteSpace(p.AccountTitle)).ToList();
            }
            var list = query.Select(p => new
            {
                p.Code,
                p.AccountTitle,
                p.AccountId,
                OpeningBalance = Numerics.DecimalToString(p.OpeningBalance, ""),
                Debit = Numerics.DecimalToString(p.Debit, ""),
                Credit = Numerics.DecimalToString(p.Credit, ""),
                Balance = Numerics.DecimalToString(p.Balance, ""),
            }).ToList();
            var sumDebit = Numerics.GetDecimal(transactions.Sum(p => (decimal?)p.Debit));
            var sumCredit = Numerics.GetDecimal(transactions.Sum(p => (decimal?)p.Credit));
            var sumBalance = Numerics.GetDecimal(transactions.Sum(p => (decimal?)p.Balance));
            var sumOpeningBalance = Numerics.GetDecimal(transactions.Sum(p => p.OpeningBalance));
            var totalDebit = Numerics.DecimalToString(sumDebit, "");
            var totalCredit = Numerics.DecimalToString(sumCredit, "");
            var totalBalance = Numerics.DecimalToString(sumBalance, "");
            var totalOpeningBalance = Numerics.DecimalToString(sumOpeningBalance, "0");
            //var difference = sumDebit + sumCredit;
            var difference = transactions.Sum(p => (decimal?)p.Balance);

            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    TotalDebit = totalDebit,
                    TotalCredit = totalCredit,
                    Difference = Numerics.DecimalToString(difference),
                    TotalBalance = totalBalance,
                    TotalOpeningBalance = totalOpeningBalance,
                    Records = list,
                }
            };
            return response;
        }
    }
}
