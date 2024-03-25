using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace AccountEx.Web.Controllers.api
{
    public class AccountBalancesController : BaseApiController
    {
        // GET api/test
        public ApiResponse Get()
        {
            return GetAccountBalances();
        }
        private ApiResponse GetAccountBalances()
        {
            ApiResponse response;
            var setting = new FormSettingRepository().GetFormSettingByVoucherType("Recovery");
            var queryString = Request.RequestUri.ParseQueryString();
            var date = DateTime.Parse(queryString["date"]);
            var accountId = Numerics.GetInt(queryString["accountId"]);
            var accounts = new AccountRepository().GetChildren(accountId);
            var result = new List<AccountBalance>();
            foreach (var item in accounts)
            {
                var transRepo = new TransactionRepository();
                var transactions = transRepo.GetTrailBalance(item.Id, date);
                var list = transactions.Select(p => new TrialBalanceLine
                {
                    AccountTitle = p.AccountTitle,
                    Debit = Numerics.DecimalToString(p.Debit, ""),
                    Credit = Numerics.DecimalToString(p.Credit, ""),
                }).ToList();
                var sumDebit = Numerics.GetDecimal(transactions.Where(p => p.Debit != p.Credit).Sum(p => (decimal?)p.Debit));
                var sumCredit = Numerics.GetDecimal(transactions.Where(p => p.Debit != p.Credit).Sum(p => (decimal?)p.Credit));
                var totalDebit = Numerics.DecimalToString(sumDebit, "");
                var totalCredit = Numerics.DecimalToString(sumCredit, "");
                var difference = sumDebit - sumCredit;
                result.Add(new AccountBalance
                    {
                        AccountTitle = item.Name,
                        TotalDebit = totalDebit,
                        TotalCredit = totalCredit,
                        Difference = Numerics.DecimalToString(difference),
                        Records = list,
                    });

            }
            response = new ApiResponse
            {
                Success = true,
                Data = result,
            };
            return response;
        }
    }
}
