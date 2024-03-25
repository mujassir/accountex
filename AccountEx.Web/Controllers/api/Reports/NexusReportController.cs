using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Net.Http;
using AccountEx.BussinessLogic;
using AccountEx.Common;
using AccountEx.Repositories;
using Newtonsoft.Json;
using System.Text;
using SelectPdf;
using System.Web;
using System.IO;
using System.Drawing;
using AccountEx.CodeFirst;
using AccountEx.CodeFirst.Models;
using BussinessLogic;
using AccountEx.Repositories.Config;
using AccountEx.DbMapping;
using AccountEx.Repositories.Nexus;

namespace AccountEx.Web.Controllers.api.Reports
{


    public class NexusReportController : BaseApiController
    {

        public virtual ApiResponse Get(string key)
        {
            // var key = Request.GetQueryString("key");
            var response = new ApiResponse();
            try
            {
                switch (key)
                {

                    case "SummaryOfDepartmentBilling":
                        response = SummaryOfDepartmentBilling();
                        break;
                    case "PendingSummary":
                        response = PendingSummary();
                        break;
                    case "SummaryOfDepartmentBillingByPatient":
                        response = SummaryOfDepartmentBillingByPatient();
                        break;
                    case "BillingByPatient":
                        response = BillingByPatient();
                        break;
                    case "ReferralSummary":
                        response = ReferralSummary();
                        break;
                    case "ReceivablesSummary":
                        response = ReceivablesSummary();
                        break;

                        
                    case "DetailBillPayment":
                        response = DetailBillPayment();
                        break;
                    case "MonthlyReceiptSummary":
                        response = MonthlyReceiptSummary();
                        break;
                    case "GetVehicleCustomerBalances":
                        response = GetVehicleCustomerBalances();
                        break;
                    case "GetVehicleIncomeStatment":
                        response = GetVehicleIncomeStatment();
                        break;
                    case "GetVehicleMonthlyCredits":
                        response = GetVehicleMonthlyCredits();
                        break;
                    case "GetVehicleOverDueAmounts":
                        response = GetVehicleOverDueAmounts();
                        break;
                    case "GetVehiclePeriodicActivity":
                        response = GetVehiclePeriodicActivity();
                        break;
                    case "GetVehicleRepossessions":
                        response = GetVehicleRepossessions();
                        break;
                    case "GetVehicleSales":
                        response = GetVehicleSales();
                        break;
                    case "GetCustomerCollections":
                        response = GetCustomerCollections();
                        break;
                    case "GetProfitLoss":
                        response = GetProfitLoss();
                        break;
                    case "GetGeneralLedger":
                        response = GetGeneralLedger();
                        break;


                }

            }
            catch (Exception ex)
            {

                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };


            }
            return response;
        }

        private ApiResponse SummaryOfDepartmentBilling()
        {
            var fromMonth = Numerics.GetInt(Request.GetQueryString("FromMonth"));
            var fromYear = Numerics.GetInt(Request.GetQueryString("FromYear"));
            var toMonth = Numerics.GetInt(Request.GetQueryString("ToMonth"));
            var toYear = Numerics.GetInt(Request.GetQueryString("ToYear"));
            var fromDate = UtilityFunctionManager.GetFromDate(fromMonth, fromYear);
            var ToDate = UtilityFunctionManager.GetToDate(toMonth, toYear);
            var repo = new NexusReportRepository();
            var records = repo.GetSummaryOfDepartmentBilling(fromDate, ToDate);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse PendingSummary()
        {
            var fromMonth = Numerics.GetInt(Request.GetQueryString("FromMonth"));
            var fromYear = Numerics.GetInt(Request.GetQueryString("FromYear"));
            var toMonth = Numerics.GetInt(Request.GetQueryString("ToMonth"));
            var toYear = Numerics.GetInt(Request.GetQueryString("ToYear"));
            var fromDate = UtilityFunctionManager.GetFromDate(fromMonth, fromYear);
            var ToDate = UtilityFunctionManager.GetToDate(toMonth, toYear);
            var repo = new NexusReportRepository();
            var records = repo.GetPendingSummary(fromDate, ToDate);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse SummaryOfDepartmentBillingByPatient()
        {
            var fromMonth = Numerics.GetInt(Request.GetQueryString("FromMonth"));
            var fromYear = Numerics.GetInt(Request.GetQueryString("FromYear"));
            var toMonth = Numerics.GetInt(Request.GetQueryString("ToMonth"));
            var toYear = Numerics.GetInt(Request.GetQueryString("ToYear"));
            var fromDate = UtilityFunctionManager.GetFromDate(fromMonth, fromYear);
            var ToDate = UtilityFunctionManager.GetToDate(toMonth, toYear);
            var repo = new NexusReportRepository();
            var records = repo.GetSummaryOfDepartmentBillingByPatient(fromDate, ToDate);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse BillingByPatient()
        {
            var fromMonth = Numerics.GetInt(Request.GetQueryString("FromMonth"));
            var fromYear = Numerics.GetInt(Request.GetQueryString("FromYear"));
            var toMonth = Numerics.GetInt(Request.GetQueryString("ToMonth"));
            var toYear = Numerics.GetInt(Request.GetQueryString("ToYear"));
            var departmentId = Numerics.GetInt(Request.GetQueryString("DepartmentId"));
            var fromDate = UtilityFunctionManager.GetFromDate(fromMonth, fromYear);
            var ToDate = UtilityFunctionManager.GetToDate(toMonth, toYear);
            var repo = new NexusReportRepository();
            var records = repo.GetBillingByPatient(fromDate, ToDate, departmentId);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse ReferralSummary()
        {
            var fromMonth = Numerics.GetInt(Request.GetQueryString("FromMonth"));
            var fromYear = Numerics.GetInt(Request.GetQueryString("FromYear"));
            var fromDate = UtilityFunctionManager.GetFromDate(fromMonth, fromYear);
            var ToDate = UtilityFunctionManager.GetToDate(fromMonth, fromYear);
            var repo = new NexusReportRepository();
            var records = repo.GetReferralSummary(fromDate, ToDate);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse ReceivablesSummary()
        {
            var fromMonth = Numerics.GetInt(Request.GetQueryString("FromMonth"));
            var fromYear = Numerics.GetInt(Request.GetQueryString("FromYear"));
            var toMonth = Numerics.GetInt(Request.GetQueryString("ToMonth"));
            var toYear = Numerics.GetInt(Request.GetQueryString("ToYear"));
            var fromDate = UtilityFunctionManager.GetFromDate(fromMonth, fromYear);
            var ToDate = UtilityFunctionManager.GetToDate(fromMonth, fromYear);
            if (toMonth > 0 && toYear > 0)
                ToDate = UtilityFunctionManager.GetToDate(toMonth, toYear);

            var repo = new NexusReportRepository();
            var records = repo.GetReceivablesSummary(fromDate, ToDate);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse DetailBillPayment()
        {
            var fromMonth = Numerics.GetInt(Request.GetQueryString("FromMonth"));
            var fromYear = Numerics.GetInt(Request.GetQueryString("FromYear"));
            var toMonth = Numerics.GetInt(Request.GetQueryString("ToMonth"));
            var toYear = Numerics.GetInt(Request.GetQueryString("ToYear"));
            var departmentId = Numerics.GetInt(Request.GetQueryString("DepartmentId"));
            var fromDate = UtilityFunctionManager.GetFromDate(fromMonth, fromYear);
            var ToDate = UtilityFunctionManager.GetToDate(fromMonth, fromYear);
            if (toMonth > 0 && toYear > 0)
                ToDate = UtilityFunctionManager.GetToDate(toMonth, toYear);

            var repo = new NexusReportRepository();
            var records = repo.GetReceivablesSummaryByDepartment(fromDate, ToDate, departmentId);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;
        }
        private ApiResponse MonthlyReceiptSummary()
        {
            var fromMonth = Numerics.GetInt(Request.GetQueryString("FromMonth"));
            var fromYear = Numerics.GetInt(Request.GetQueryString("FromYear"));
            var toMonth = Numerics.GetInt(Request.GetQueryString("ToMonth"));
            var toYear = Numerics.GetInt(Request.GetQueryString("ToYear"));
            var fromDate = UtilityFunctionManager.GetFromDate(fromMonth, fromYear);
            var ToDate = UtilityFunctionManager.GetToDate(fromMonth, fromYear);
            var repo = new NexusReportRepository();
            var records = repo.GetMonthlyReceiptSummary(fromDate, ToDate);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;
        }

       

        private ApiResponse GetRepossessedStocks()
        {


            var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
            // var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
            var repo = new ReportRepository();
            var records = repo.GetVehicleRepossessedStock(branchId);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse GetDeliveredStock()
        {

            var repo = new ReportRepository();
            var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
            var records = repo.GetVehicleDeliveredStock(branchId);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse GetGeneralLedger()
        {
            var date1 = Convert.ToDateTime(Request.GetQueryString("date1"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("date2"));
            var accountId = Numerics.GetInt(Request.GetQueryString("accountId"));
            var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
            var currencyId = Numerics.GetInt(Request.GetQueryString("currencyId"));
            var repo = new TransactionRepository();
            var openingBalance = repo.GetForexOpeningBalance(accountId, date1, 0);
            var recieptTypes = new List<VoucherType> { VoucherType.VCR, VoucherType.VBR, VoucherType.VSD, VoucherType.AdvanceReceipts, VoucherType.AuctionnerPayments, VoucherType.PenaltyPayments };
            var transactions = repo.GetForexTransactions(accountId, date1, date2, currencyId).Where(p => p.Debit + p.Credit > 0).OrderBy(p => p.Date).ThenBy(p => p.VoucherNumber).ToList();
            var totalDebit = transactions.Sum(p => p.Debit);
            var totalCredit = transactions.Sum(p => p.Credit);
            var records = new List<GeneralLedgerEntry>();
            var runningTotal = openingBalance;
            foreach (var item in transactions)
            {
                runningTotal += item.Debit - item.Credit;
                var entry = new GeneralLedgerEntry(item);
                entry.Balance = Numerics.DecimalToString(runningTotal);
                records.Add(entry);
            }

            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    OpeningBalance = Numerics.DecimalToString(openingBalance),
                    Records = records, //transactions.Select(p => new GeneralLedgerEntry(p)).ToList(),
                    TotalDebit = Numerics.DecimalToString(totalDebit),
                    TotalCredit = Numerics.DecimalToString(totalCredit),
                    TotalBalance = Numerics.DecimalToString(totalDebit - totalCredit + openingBalance),
                }
            };
            return response;

        }

        private ApiResponse GetSoldStockAnalysis()
        {

            var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
            var repo = new ReportRepository();
            var records = repo.GetVehicleSoldStockAnalysis(branchId);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse GetCashBankSummary()
        {
            var todate = Convert.ToDateTime(Request.GetQueryString("date1"));

            var repo = new ReportRepository();
            var records = repo.GetCashBankSumamry(todate);


            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    Records = records,
                    // Tenant = tenant
                }
            };
            return response;

        }
        private ApiResponse GetProfitLoss()
        {

            var date1 = Convert.ToDateTime(Request.GetQueryString("date1"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("date2"));
            var isBeforeClosing = Convert.ToBoolean(QueryString["isBeforeClosing"]);
            // var openingstock = Numerics.GetInt(Request.GetQueryString("OpeningStock"));
            // var voutype = Numerics.GetInt(Request.GetQueryString("voutype"));
            var repo = new ReportRepository();
            var profitLoss = ReportManager.GetProfitLoss(date1, date2, isBeforeClosing);
            var totalprofit = profitLoss.Where(p => p.AccountType == "Profit").Sum(p => p.Accounts.Sum(q => q.Amount));
            var totalexpense = profitLoss.Where(p => p.AccountType == "Expense").Sum(p => p.Accounts.Sum(q => q.Amount));
            var totalnetamount = totalprofit - totalexpense;
            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {

                    Profits = profitLoss.Where(p => p.AccountType == "Profit").Select(q => q.Accounts).ToList(),
                    Expenses = profitLoss.Where(p => p.AccountType == "Expense").Select(q => q.Accounts).ToList(),
                    TotalProfit = totalprofit,
                    TotalExpense = totalexpense,
                    TotalNetAmount = totalnetamount,
                }
            };
            return response;

        }
        private ApiResponse GetVehicleCustomerBalances()
        {

            var todate = Convert.ToDateTime(Request.GetQueryString("date1"));
            var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
            var repo = new ReportRepository();
            var records = repo.GetVehicleCustomerBalance(todate, branchId);


            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    Records = records,
                    // Tenant = tenant
                }
            };
            return response;

        }
        private ApiResponse GetVehicleIncomeStatment()
        {

            var fromdate = Convert.ToDateTime(Request.GetQueryString("date1"));
            var todate = Convert.ToDateTime(Request.GetQueryString("date2"));
            var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
            var repo = new ReportRepository();
            var record = repo.GetVehicleIncomeStatment(fromdate, todate, branchId);
            var expenses = repo.GetVehicleExpensesForIncomeStatment(fromdate, todate, branchId);





            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    Records = record,
                    Expenses = expenses
                }
            };
            return response;

        }
        private ApiResponse GetVehicleMonthlyCredits()
        {
            var month = Numerics.GetInt(Request.GetQueryString("month"));
            var year = Numerics.GetInt(Request.GetQueryString("year"));
            var isBadDebit = Numerics.GetBool(Request.GetQueryString("isBadDebit"));
            var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
            var repo = new ReportRepository();
            var records = repo.GetVehicleMonthlyCredits(month, year, branchId, isBadDebit);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse GetVehiclePeriodicActivity()
        {

            var fromdate = Convert.ToDateTime(Request.GetQueryString("date1"));
            var todate = Convert.ToDateTime(Request.GetQueryString("date2"));
            var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
            var accountId = Numerics.GetInt(Request.GetQueryString("accountId"));
            var repo = new ReportRepository();
            var records = repo.GetVehiclePeriodicActivity(fromdate, todate, accountId, branchId);



            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse GetVehicleRepossessions()
        {
            var fromdate = Convert.ToDateTime(Request.GetQueryString("date1"));
            var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
            var recoveryStatus = Numerics.GetInt(Request.GetQueryString("recoveryStatus"));
            var repo = new ReportRepository();
            var records = repo.GetVehicleRepossessions(branchId, recoveryStatus);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse GetVehicleSales()
        {

            var fromdate = Convert.ToDateTime(Request.GetQueryString("date1"));
            var todate = Convert.ToDateTime(Request.GetQueryString("date2"));
            var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));

            var repo = new ReportRepository();
            var records = repo.GetVehicleSales(fromdate, todate, branchId);


            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    Records = records,
                    // Tenant = tenant
                }
            };
            return response;

        }
        private ApiResponse GetCustomerCollections()
        {

            var fromdate = Convert.ToDateTime(Request.GetQueryString("date1"));
            var todate = Convert.ToDateTime(Request.GetQueryString("date2"));
            var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
            var accountId = Numerics.GetInt(Request.GetQueryString("accountId"));

            var repo = new ReportRepository();
            var records = repo.GetCustomerCollections(fromdate, todate, branchId, accountId);


            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    Records = records,
                    // Tenant = tenant
                }
            };
            return response;

        }
        private ApiResponse GetVehicleOverDueAmounts()
        {
            var fromdate = Convert.ToDateTime(Request.GetQueryString("date1"));
            var todate = Convert.ToDateTime(Request.GetQueryString("date2"));
            var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
            var isBadDebit = Numerics.GetBool(Request.GetQueryString("isBadDebit"));
            var repo = new ReportRepository();
            var records = repo.GetVehicleOverDueAmounts(fromdate, todate, branchId, isBadDebit);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse GetVehicleFollowups()
        {

            var fromdate = DateConverter.ConvertFromDmy(Request.GetQueryString("date1"));

            var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
            var type = String.Format(Request.GetQueryString("type"));
            var repo = new ReportRepository();
            var records = repo.GetVehicleFollowups(fromdate, branchId, type);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        private ApiResponse GetSoldStockAnalysisByDate()
        {

            var fromdate = DateConverter.ConvertFromDmy(Request.GetQueryString("date1"));
            var todate = DateConverter.ConvertFromDmy(Request.GetQueryString("date2"));
            var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
            var repo = new ReportRepository();
            var records = repo.GetVehicleSoldStockAnalysisByDates(fromdate, todate, branchId);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;
        }





    }
}
