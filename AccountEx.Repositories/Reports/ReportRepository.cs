using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using AccountEx.CodeFirst.Models;
using AccountEx.Common.RentalAgreement;
using AccountEx.CodeFirst.Models.COA;
using AccountEx.Common.VehicleSystem;
using System.Data.Entity.Core.Objects;
using AccountEx.DbMapping;

namespace AccountEx.Repositories
{
    public class ReportRepository : GlobalGenericRepository<Report>
    {
        public Report GetReportById(int id)
        {
            return Db.Reports.FirstOrDefault(p => p.Id == id);
        }
        public Report GetReportByName(string name)
        {
            name = name.ToLower();
            //return Collection.FirstOrDefault(p => p.ReportName.ToLower().Trim().Contains(name));
            return Collection.FirstOrDefault(p => p.ReportName.ToLower().Trim() == name.ToLower().Trim());
        }
        public List<Report> GetReportByModule(byte module)
        {
            return Collection.Where(p => p.ModuleType == module).ToList();
        }
        public List<StockReport> GetProductLedger(DateTime fromdate, DateTime todate, int accountId)
        {
            var query = string.Format("EXEC [DBO].[GetProductLedger] @COMPANYID = {0},@FISCALID={1}, @FROMDATE = {2}, @TODATE={3},@ACCOUNTID={4}",
                 SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, "'" + fromdate.ToString("yyyy-MM-dd") + "'", "'" + todate.ToString("yyyy-MM-dd") + "'", accountId);
            return Db.Database.SqlQuery<StockReport>(query).ToList();
        }
        public GetVehicleWithSaleInforReports GetVehicleandSaleInfo(int vehicleId, int saleId)
        {
            var query = string.Format("EXEC [DBO].[GetVehicleWithSaleInforReports] @COMPANYID = {0},@VehilceId ={1},@SaleId ={2}", SiteContext.Current.User.CompanyId, vehicleId, saleId);
            return Db.Database.SqlQuery<GetVehicleWithSaleInforReports>(query).FirstOrDefault();
        }
        public List<GetRentAccountStatementV1> GetRentalAccountStatement(int agreementId, DateTime fromdate, DateTime todate)
        {
            var query = string.Format("EXEC [DBO].[GetRentAccountStatement] @COMPANYID = {0},@RentAgreementId ={1}, @FROMDATE = {2}, @TODATE={3}", SiteContext.Current.User.CompanyId, agreementId, "'" + fromdate.ToString("yyyy-MM-dd") + "'", "'" + todate.ToString("yyyy-MM-dd") + "'");
            return Db.Database.SqlQuery<GetRentAccountStatementV1>(query).ToList();
        }
        public List<GetSecuirtyPossessionAccountStatementExtra> GetSecuirtyPossessionAccountStatement(int agreementId, VoucherType transactionType)
        {
            var records = new List<GetSecuirtyPossessionAccountStatementExtra>();
            var ra = new RentAgreementRepository().GetById(agreementId);
            var challans = new ChallanRepository().AsQueryable(true).Where(p => p.TransactionType == transactionType && p.IsReceived).Select(p => new
            {
                p.Id,
                p.NumberOfInstallment,
                p.ReceiveDate,
                p.NetAmount,
                p.RcvNo
            }).ToList();

            var paidChallans = new List<Challan>();
            var j = 1;
            foreach (var challan in challans)
            {
                for (int i = 1; i <= challan.NumberOfInstallment; i++)
                {
                    paidChallans.Add(new Challan()
                    {
                        Id = challan.Id,
                        NumberOfInstallment = j,
                        ReceiveDate = challan.ReceiveDate,
                        RcvNo = challan.RcvNo,
                        NetAmount = challan.NetAmount,

                    });
                    j++;
                }

            }


            var totalInstallment = ra.SecurityInstallment;
            var installmentAmount = ra.SecurityPerInstallment;
            if (transactionType == VoucherType.PossessionCharges)
            {
                totalInstallment = ra.PossessionInstallment;
                installmentAmount = ra.PossessionPerInstallment;
            }
            var startDate = ra.CreatedAt;
            if (ra.RentAgreementSchedules.Any())
                startDate = ra.RentAgreementSchedules.FirstOrDefault().FromDate.Value;
            for (int i = 1; i <= totalInstallment; i++)
            {
                startDate = i == 1 ? startDate : startDate.AddMonths(1);
                var record = new GetSecuirtyPossessionAccountStatementExtra();
                record.NoOfInstallment = i;
                record.DueAmount = installmentAmount;
                record.DueDate = startDate;

                var rcvChallan = paidChallans.FirstOrDefault(p => p.NumberOfInstallment == i);
                if (rcvChallan != null)
                {
                    record.PaidOn = rcvChallan.ReceiveDate;
                    record.BillNo = rcvChallan.Id;
                    record.RcvNo = rcvChallan.RcvNo;
                    //var rvChallanNo = RvNos.FirstOrDefault(p => p.ChallanId == rcvChallan.Id);
                    //if (rvChallanNo != null)
                    //{
                    //    record.RVNo = rcvChallan.VoucherNumber;
                    //}
                }
                else
                    record.OutStanding = installmentAmount;
                records.Add(record);

            }


            return records;

        }
        public List<GetSecuirtyPossessionAccountStatementExtra1> GetSecuirtyPossessionAccountStatement1(int agreementId, VoucherType transactionType, DateTime fromDate, DateTime toDate)
        {
            var records = new List<GetSecuirtyPossessionAccountStatementExtra>();
            var query = new ChallanRepository().AsQueryable(true).
              Where(p => (EntityFunctions.TruncateTime(p.DueDate) >= fromDate && EntityFunctions.TruncateTime(p.DueDate) <= toDate) && p.RentAgreementId == agreementId);


            var challans = query.Where(p => p.TransactionType == transactionType).Select(p => new GetSecuirtyPossessionAccountStatementExtra1()
            {
                DueDate = p.DueDate,
                IsReceived = p.IsReceived,
                PaidOn = p.ReceiveDate,
                NetAmount = p.NetAmount,
                BillNo = p.Id
            }).ToList();
            return challans;

        }
        public List<GetRecoveryOfPossessionCharge> GetRecoveryOfPossessionCharges(int month, int year, int blockId, VoucherType transactionType)
        {
            var query = string.Format("EXEC [DBO].[GetRecoveryOfPossessionCharges] @COMPANYID = {0},@Month ={1},@Year  = {2},@BlockId ={3},@TransactionType ={4}", SiteContext.Current.User.CompanyId, month, year, blockId, (byte)transactionType);
            return Db.Database.SqlQuery<GetRecoveryOfPossessionCharge>(query).ToList();
        }
        public List<GetRecoveryOfPossessionCharge1> GetRecoveryOfPossessionCharges1(DateTime fromdate, DateTime todate, int blockId, VoucherType transactionType)
        {
            var query = string.Format("EXEC [DBO].[GetRecoveryOfPossessionCharges1] @COMPANYID = {0},@FromDate ={1},@ToDate  = {2},@BlockId ={3},@TransactionType ={4}", SiteContext.Current.User.CompanyId, "'" + fromdate.ToString("yyyy-MM-dd") + "'", "'" + todate.ToString("yyyy-MM-dd") + "'", blockId, (byte)transactionType);
            return Db.Database.SqlQuery<GetRecoveryOfPossessionCharge1>(query).ToList();
        }
        public List<GetVehicleSale> GetVehicleSales(DateTime fromdate, DateTime todate, int branchId)
        {
            var query = string.Format("EXEC [DBO].[GetVehicleSales] @COMPANYID = {0} , @FromDate ={1},@ToDate  = {2},@BranchId={3}", SiteContext.Current.User.CompanyId, "'" + fromdate.ToString("yyyy-MM-dd") + "'", "'" + todate.ToString("yyyy-MM-dd") + "'", branchId);
            return Db.Database.SqlQuery<GetVehicleSale>(query).ToList();
        }
        public List<GetCustomerCollections> GetCustomerCollections(DateTime fromdate, DateTime todate, int branchId, int accountId)
        {
            var query = string.Format("EXEC [DBO].[GetCustomerCollections] @COMPANYID = {0},@FISCALID={1} , @FromDate ={2},@ToDate  = {3},@BranchId={4},@AccountId={5}", SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, "'" + fromdate.ToString("yyyy-MM-dd") + "'", "'" + todate.ToString("yyyy-MM-dd") + "'", branchId, accountId);
            return Db.Database.SqlQuery<GetCustomerCollections>(query).ToList();
        }
        public List<GetVehicleCustomerBalances> GetVehicleCustomerBalance(DateTime toDate, int branchId)
        {
            var query = string.Format("EXEC [DBO].[GetCustomerBalanceDebtors] @COMPANYID = {0},@FISCALID={1} , @Date ={2},@BranchId={3}", SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, "'" + toDate.ToString("yyyy-MM-dd") + "'", branchId);
            return Db.Database.SqlQuery<GetVehicleCustomerBalances>(query).ToList();
        }
        public List<AccountSummary> GetCashBankSumamry(DateTime toDate)
        {
            var query = string.Format("EXEC [DBO].[GetCashNBankSummary] @COMPANYID = {0},@FISCALID={1} , @Date ={2}", SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, "'" + toDate.ToString("yyyy-MM-dd") + "'");
            return Db.Database.SqlQuery<AccountSummary>(query).ToList();
        }
        public VehicleIncomeStatment GetVehicleIncomeStatment(DateTime fromdate, DateTime todate, int branchId)
        {
            var query = string.Format("EXEC [DBO].[GetIncomeStatement] @COMPANYID = {0} , @FromDate ={1},@ToDate  = {2},@FiscalId={3},@BranchId={4}", SiteContext.Current.User.CompanyId, "'" + fromdate.ToString("yyyy-MM-dd") + "'", "'" + todate.ToString("yyyy-MM-dd") + "'", SiteContext.Current.Fiscal.Id, branchId);
            return Db.Database.SqlQuery<VehicleIncomeStatment>(query).FirstOrDefault();
        }
        public List<AccountSummary1> GetVehicleExpensesForIncomeStatment(DateTime fromdate, DateTime todate, int branchId)
        {
            var query = string.Format("EXEC [DBO].[GetIndirectExpenseBalances] @COMPANYID = {0} , @FromDate ={1},@ToDate  = {2},@FiscalId={3},@BranchId={4}", SiteContext.Current.User.CompanyId, "'" + fromdate.ToString("yyyy-MM-dd") + "'", "'" + todate.ToString("yyyy-MM-dd") + "'", SiteContext.Current.Fiscal.Id, branchId);
            return Db.Database.SqlQuery<AccountSummary1>(query).ToList();
        }

        public List<VehiclePeriodicalActivity> GetVehiclePeriodicActivity(DateTime fromdate, DateTime todate, int headId, int branchId)
        {
            var query = string.Format("EXEC [DBO].[PeriodicalActivity] @COMPANYID = {0} , @FromDate ={1},@ToDate  = {2},@FiscalId={3},@HeadAccountId={4},@BranchId={5}", SiteContext.Current.User.CompanyId, "'" + fromdate.ToString("yyyy-MM-dd") + "'", "'" + todate.ToString("yyyy-MM-dd") + "'", SiteContext.Current.Fiscal.Id, headId, branchId);
            return Db.Database.SqlQuery<VehiclePeriodicalActivity>(query).ToList();
        }
        public List<VehicleMonthlyCredit> GetVehicleMonthlyCredits(int month, int year, int branchId, bool isBadDebit)
        {
            var query = string.Format("EXEC [DBO].[GetMonthlyCredit] @COMPANYID = {0} ,@FiscalId={1},@Month={2},@Year={3} , @BranchId ={4}, @IsBadDebit ={5}", SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, month, year, branchId, isBadDebit);
            return Db.Database.SqlQuery<VehicleMonthlyCredit>(query).ToList();
        }
        public List<VehicleMonthlyCredit> GetVehicleOverDueAmounts(DateTime fromdate, DateTime todate, int branchId, bool isBadDebit)
        {
            var query = string.Format("EXEC [DBO].[VehicleOverdueAmounts] @COMPANYID = {0} , @FromDate ={1},@ToDate  = {2},@FiscalId={3},@BranchId={4},@IsBadDebit ={5}", SiteContext.Current.User.CompanyId, "'" + fromdate.ToString("yyyy-MM-dd") + "'", "'" + todate.ToString("yyyy-MM-dd") + "'", SiteContext.Current.Fiscal.Id, branchId, isBadDebit);
            return Db.Database.SqlQuery<VehicleMonthlyCredit>(query).ToList();
        }
        public List<VehicleRepossessions> GetVehicleRepossessions(int branchId, int recoveryStatus)
        {
            var query = string.Format("EXEC [DBO].[VehicleRepossessions] @COMPANYID = {0},@BranchId={1},@StatusId={2}", SiteContext.Current.User.CompanyId, branchId, recoveryStatus);
            return Db.Database.SqlQuery<VehicleRepossessions>(query).ToList();
        }
        public List<VehicleStockExtra> GetVehicleActiveStocks(int branchId)
        {
            var query = string.Format("EXEC [DBO].[GetVehicleActiveStock] @COMPANYID = {0},@BranchId = {1}", SiteContext.Current.User.CompanyId, branchId);
            return Db.Database.SqlQuery<VehicleStockExtra>(query).ToList();

        }
        public decimal GetVehicleActiveStockValue(int? branchId, DateTime date)
        {
            var query = string.Format("EXEC [DBO].[GetVehicleActiveStockValue] @COMPANYID = {0},@Date = '{1}'", SiteContext.Current.User.CompanyId, date.ToString("yyyy-MM-dd"));
            if (branchId.HasValue)
                query = string.Format("EXEC [DBO].[GetVehicleActiveStockValue] @COMPANYID = {0},@BranchId = {1},@Date = '{2}'", SiteContext.Current.User.CompanyId, branchId, date.ToString("yyyy-MM-dd"));
            List<VehicleStockExtra> result = Db.Database.SqlQuery<VehicleStockExtra>(query).ToList();
            if (result == null || result.Count == 0)
                return 0;
            return result.FirstOrDefault().TotalCost;

        }
        public List<VehicleStockExtra> GetVehicleTransfeerStock(int branchId)
        {
            var query = string.Format("EXEC [DBO].[GetVehicleTransfeerStock] @COMPANYID = {0},@BranchId = {1}", SiteContext.Current.User.CompanyId, branchId);
            return Db.Database.SqlQuery<VehicleStockExtra>(query).ToList();

        }
        public List<VehicleStockExtra> GetVehicleRepossessedStock(int branchId)
        {
            var query = string.Format("EXEC [DBO].[GetVehicleRepossessedStock] @COMPANYID = {0},@BranchId={1}", SiteContext.Current.User.CompanyId, branchId);
            return Db.Database.SqlQuery<VehicleStockExtra>(query).ToList();

        }
        public List<VehicleStockExtra> GetVehicleDeliveredStock(int branchId)
        {
            var query = string.Format("EXEC [DBO].[GetVehicleDeliveredStock] @COMPANYID = {0} ,@BranchId={1}", SiteContext.Current.User.CompanyId, branchId);
            return Db.Database.SqlQuery<VehicleStockExtra>(query).ToList();

        }
        public List<VehicleStockExtra> GetVehicleSoldStockAnalysis(int branchId)
        {
            var query = string.Format("EXEC [DBO].[GetVehicleSoldStock] @COMPANYID = {0},@BranchId={1}", SiteContext.Current.User.CompanyId, branchId);
            return Db.Database.SqlQuery<VehicleStockExtra>(query).ToList();
        }
        public List<VehicleStockExtra> GetVehicleSoldStockAnalysisByDates(DateTime fromdate, DateTime todate, int branchId)
        {
            var query = string.Format("EXEC [DBO].[GetVehicleSoldStockByDate] @COMPANYID = {0}, @FromDate ={1},@ToDate  = {2},@BranchId={3}", SiteContext.Current.User.CompanyId, "'" + fromdate.ToString("yyyy-MM-dd") + "'", "'" + todate.ToString("yyyy-MM-dd") + "'", branchId);
            return Db.Database.SqlQuery<VehicleStockExtra>(query).ToList();
        }

        public List<GetVehicleFollowUpReport> GetVehicleFollowups(DateTime fromdate, int branchId, string type)
        {
            var query = string.Format("EXEC [DBO].[GetVehicleFollowUpReport] @COMPANYID ={0}, @FiscalId ={1},@Date ={2},@BranchId={3},@Type ='{4}'", SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, "'" + fromdate.ToString("yyyy-MM-dd") + "'", branchId, type);
            return Db.Database.SqlQuery<GetVehicleFollowUpReport>(query).ToList();
        }
        public List<GetVehicleSale> GetCustomerColection(DateTime fromdate, DateTime todate)
        {
            var query = string.Format("EXEC [DBO].[GetVehicleSales] @COMPANYID = {0} , @FromDate ={1},@ToDate  = {2}", SiteContext.Current.User.CompanyId, "'" + fromdate.ToString("yyyy-MM-dd") + "'", "'" + todate.ToString("yyyy-MM-dd") + "'");
            return Db.Database.SqlQuery<GetVehicleSale>(query).ToList();
        }
        public List<BillsIssuesToTenants> GetDetailOfOverallBillsIssueToTenants(int month, int year, int blockId)
        {
            var query = string.Format("EXEC [DBO].[GetBillsIssuesToTenants] @COMPANYID = {0},@Month ={1},@Year  = {2},@BlockId ={3}", SiteContext.Current.User.CompanyId, month, year, blockId);
            return Db.Database.SqlQuery<BillsIssuesToTenants>(query).ToList();
        }
        public List<GetRentRecoveryDetail> GetRecoveryOfRent(int month, int year, int blockid)
        {
            var query = string.Format("EXEC [DBO].[GetRentRecoveryDetail] @COMPANYID = {0},@Month ={1},@Year  = {2},@blockid  = {3}", SiteContext.Current.User.CompanyId, month, year, blockid);
            return Db.Database.SqlQuery<GetRentRecoveryDetail>(query).ToList();
        }
        public List<GetOverallRecoveryDetail> GetOverallRecoveryReport(int month, int year, int blockid)
        {
            var query = string.Format("EXEC [DBO].[GetOverallRecoveryDetail] @COMPANYID = {0},@Month ={1},@Year  = {2},@blockid  = {3}", SiteContext.Current.User.CompanyId, month, year, blockid);
            return Db.Database.SqlQuery<GetOverallRecoveryDetail>(query).ToList();
        }


        public List<ActivityEntry> GetDailyActivity(DateTime fromdate, DateTime todate)
        {
            var query = string.Format("EXEC [DBO].[GetDailyActivity] @COMPANYID = {0},@FISCALID={1}, @FROMDATE = {2}, @TODATE={3}",
                SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, "'" + fromdate.ToString("yyyy-MM-dd") + "'", "'" + todate.ToString("yyyy-MM-dd") + "'");
            return Db.Database.SqlQuery<ActivityEntry>(query).ToList();
        }
        public List<CustomerExtra> GetCustomerBalances(DateTime fromdate, DateTime todate)
        {
            var query = string.Format("EXEC [DBO].[GetCustomerBalances] @COMPANYID = {0},@FISCALID={1}, @FROMDATE = {2}, @TODATE={3}",
                 SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, "'" + fromdate.ToString("yyyy-MM-dd") + "'", "'" + todate.ToString("yyyy-MM-dd") + "'");
            return Db.Database.SqlQuery<CustomerExtra>(query).ToList();
        }
        public List<CustomerExtra> GetSupplierBalances(DateTime fromdate, DateTime todate)
        {
            var query = string.Format("EXEC [DBO].[GetSupplierBalances] @COMPANYID = {0},@FISCALID={1}, @FROMDATE = {2}, @TODATE={3}",
                 SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, "'" + fromdate.ToString("yyyy-MM-dd") + "'", "'" + todate.ToString("yyyy-MM-dd") + "'");
            return Db.Database.SqlQuery<CustomerExtra>(query).ToList();
        }

        public List<CustomerRecoveryEntry> GetCustomerRecovery(DateTime fromdate, DateTime todate)
        {
            var query = string.Format("EXEC [DBO].[GetCustomerRecovery] @COMPANYID = {0},@FISCALID={1}, @FROMDATE = {2}, @TODATE={3}",
                 SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, "'" + fromdate.ToString("yyyy-MM-dd") + "'", "'" + todate.ToString("yyyy-MM-dd") + "'");
            return Db.Database.SqlQuery<CustomerRecoveryEntry>(query).ToList();
        }

        public List<StockReport> GetSalePurchaseReport(DateTime fromdate, DateTime todate, int accountId, int voutyp)
        {
            var query = string.Format("EXEC [DBO].[GetSalePurchaseReport] @COMPANYID = {0},@FISCALID={1}, @FROMDATE = {2}, @TODATE={3},@ACCOUNTID={4},@VoucherType={5}",
                 SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, "'" + fromdate.ToString("yyyy-MM-dd") + "'", "'" + todate.ToString("yyyy-MM-dd") + "'", accountId, voutyp);
            return Db.Database.SqlQuery<StockReport>(query).ToList();
        }

        public List<YearlySalePurchaseSummary> GetYearlySalePurchaseSummaryByVoucherTypes(List<VoucherType> VoucherTypeIds)
        {
            var sqlquery = string.Format("EXEC [dbo].[GetSalePurchaseYearlySummaryByVoucherType] @CompanyId = {0},@VoucherTypeIds = '{1}'", SiteContext.Current.User.CompanyId, string.Join(",", VoucherTypeIds.Select(p => Numerics.GetByte(p))));
            return Db.Database.SqlQuery<YearlySalePurchaseSummary>(sqlquery).ToList();
        }

        public List<YearlyComparison> GetYearlyComparison(int expenseheadid)
        {
            var sqlquery = string.Format("EXEC [dbo].[GetYearlyComparison] @CompanyId = {0},@ExpenseheadId = '{1}'", SiteContext.Current.User.CompanyId, expenseheadid);
            return Db.Database.SqlQuery<YearlyComparison>(sqlquery).ToList();
        }

        //        public List<StockReport> GetPartyTrans(DateTime fromdate, DateTime todate, int accountId, int voutyp)
        //        {
        //            var leafaccounts = new AccountRepository().GetLeafAccounts(accountId);
        //            var ids = String.Join(",", leafaccounts.Select(p => p.Id).ToList());
        //            if (string.IsNullOrWhiteSpace(ids))
        //                ids = "0";
        //            var query = @"
        //                        DECLARE @FROMDATE DATETIME = '{FROMDATE}'
        //                        DECLARE @TODATE DATETIME = '{TODATE}'
        //                        DECLARE @ACCOUNTID INT = {ACCOUNTID}
        //                        DECLARE @VoucherType INT={VOUCHERTYPE}
        //                        DECLARE @COMPANYID INT = {COMPANYID}
        //                        DECLARE @FISCALID INT = {FISCALID}
        //
        //                         SELECT 
        //                              S.[Date],
        //                              S.VoucherNumber,
        //                              S.AccountCode+'-'+S.AccountName as Name,
        //                              S.AccountCode,
        //                              S.AccountName,
        //                              S.AccountName,
        //                              S.TransactionType,
        //                              SI.ItemCode ,
        //                              SI.ItemName,
        //                              ISNULL(CONVERT(DECIMAL(16,4),SI.Quantity),0.0) as Quantity,
        //                              ISNULL(SI.Rate,0.0) as Rate,
        //                              ISNULL(SI.Amount,0.0) as Amount,
        //                              ISNULL(SI.DiscountAmount,0.0) as DiscountAmount ,
        //                              ISNULL( SI.NetAmount,0.0) AS NetAmount,
        //                              S.Comments
        //  
        //                         FROM SALES AS S INNER JOIN SALEITEMS AS SI ON S.Id = SI.SaleId 
        //                         WHERE S.ISDELETED = 0 AND S.CompanyId = @COMPANYID AND S.FiscalId = @FISCALID AND cast(S.[DATE] as Date) BETWEEN @FROMDATE AND @TODATE And s.TransactionType=@VoucherType
        //                        ";

        //            if (accountId > 0)
        //                query = query + " AND S.AccountId = @ACCOUNTID  order by s.[Date]";
        //            else
        //                query = query + " order by s.[Date]";


        //            query = query.Replace("{FROMDATE}", fromdate.ToString("yyyy-MM-dd"));
        //            query = query.Replace("{TODATE}", todate.ToString("yyyy-MM-dd"));
        //            query = query.Replace("{ACCOUNTID}", accountId + "");
        //            query = query.Replace("{VOUCHERTYPE}", voutyp + "");
        //            query = query.Replace("{COMPANYID}", SiteContext.Current.User.CompanyId + "");
        //            query = query.Replace("{FISCALID}", SiteContext.Current.Fiscal.Id + "");

        //            return Db.Database.SqlQuery<StockReport>(query).ToList();
        //        }

        public List<BalanceSheet> GetBalanceSheet(DateTime fromdate, DateTime todate, int expensesHeadId)
        {


            //var query = string.Format("EXEC dbo.GetBalanceSheet @FromDate = {0}, @ToDate = {1}, @CompanyId = {2},@FiscalId={3},@ExpenseHeadId={4}",
            //                         "'" + fromdate.ToString("yyyy-MM-dd") + "'", "'" + todate.ToString("yyyy-MM-dd") + "'", SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, expensesHeadId);

            var query = string.Format("EXEC dbo.GetBalanceSheet @FromDate = {0}, @ToDate = {1}, @CompanyId = {2},@FiscalId={3}",
                                     "'" + fromdate.ToString("yyyy-MM-dd") + "'", "'" + todate.ToString("yyyy-MM-dd") + "'", SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id);
            return Db.Database.SqlQuery<BalanceSheet>(query).ToList();
        }

        public List<TrialBalanceExtra> GetPeriodicBalancesSP(DateTime fromdate, DateTime todate)
        {
            var query = string.Format("EXEC [DBO].[GetTrialBalance] @FROMDATE = {0}, @TODATE = {1}, @COMPANYID={2},@FiscalId={3}", "'" + fromdate.ToString("yyyy-MM-dd") + "'", "'" + todate.ToString("yyyy-MM-dd") + "'", SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id);
            return Db.Database.SqlQuery<TrialBalanceExtra>(query).ToList();
        }
        public List<StockReport> GetIrisStock(DateTime fromdate, DateTime todate)
        {
            var query = string.Format("EXEC [DBO].[GetIRISStock] @FROMDATE = {0}, @TODATE = {1}, @COMPANYID={2},@FiscalId={3}", "'" + fromdate.ToString("yyyy-MM-dd") + "'", "'" + todate.ToString("yyyy-MM-dd") + "'", SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id);
            return Db.Database.SqlQuery<StockReport>(query).ToList();
        }

        public List<BalanceSheet> GetPeriodicBalances(DateTime fromdate, DateTime todate)
        {
            var query = string.Format("EXEC [DBO].[GetSalePurchaseReport] @COMPANYID = {0},@FISCALID={1} @FROMDATE = {2}, @TODATE={3}",
                 SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, "'" + fromdate.ToString("yyyy-MM-dd") + "'", "'" + todate.ToString("yyyy-MM-dd") + "'");
            return Db.Database.SqlQuery<BalanceSheet>(query).ToList();
        }
        public List<CustomerAging> GetCustomerAging(DateTime fromdate, int? accountid)
        {
            var AccountId = accountid == 0 ? "NULL" : accountid.ToString();
            var query = string.Format("EXEC [DBO].[GetCustomerAging] @AccountId = {0}, @DATE = {1}, @COMPANYID={2}, @FiscalId={3}", AccountId, "'" + fromdate.ToString("yyyy-MM-dd") + "'", SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id);
            return Db.Database.SqlQuery<CustomerAging>(query).ToList();
        }

        public List<GetNTPurchaseDataExport> GetNTPurchaseDataExport(DateTime fromDate, DateTime toDate)
        {
            string query = string.Format("EXEC [DBO].[GetNTPurchaseDataExport] @FromDate='{0}',@ToDate='{1}',@CompanyId={2},@FiscalId={3}", fromDate.ToString("yyyy-MM-dd"), toDate.ToString("yyyy-MM-dd"),
                SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id);
            return Db.Database.SqlQuery<GetNTPurchaseDataExport>(query).ToList();
        }
        public List<GetNTSaleDataExport> GetNTSaleDataExport(DateTime fromDate, DateTime toDate)
        {
            string query = string.Format("EXEC [DBO].[GetNTSaleDataExport] @FromDate='{0}',@ToDate='{1}',@CompanyId={2},@FiscalId={3}", fromDate.ToString("yyyy-MM-dd"), toDate.ToString("yyyy-MM-dd"),
                SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id);
            return Db.Database.SqlQuery<GetNTSaleDataExport>(query).ToList();
        }
        public List<GetDailyProfitLoss> GetDailyProfitLoss(DateTime fromDate, DateTime toDate)
        {
            string query = string.Format("EXEC [DBO].[GetProfitLossByDates] @FROMDATE='{0}',@TODATE='{1}',@COMPANYID={2}", fromDate.ToString("yyyy-MM-dd"), toDate.ToString("yyyy-MM-dd"),
                SiteContext.Current.User.CompanyId);
            return Db.Database.SqlQuery<GetDailyProfitLoss>(query).ToList();
        }
        public List<GetVehiclePostDatedCheque> GetVehiclePostDatedCheque(int? vehicleSaleId, byte? status, int? bankId, string chequeNo)
        {

            string query = string.Format("EXEC [DBO].[GetVehiclePostDatedCheques] @ChqNo={0},@VehicleSaleId={1},@BankId={2},@StatusId={3},@COMPANYID={4}",
                (string.IsNullOrWhiteSpace(chequeNo) ? "NULL" : "'" + chequeNo + "'"), (vehicleSaleId == 0 ? "NULL" : vehicleSaleId + ""), (bankId == 0 ? "NULL" : bankId + ""), (status == 0 ? "NULL" : status + ""), SiteContext.Current.User.CompanyId);
            return Db.Database.SqlQuery<GetVehiclePostDatedCheque>(query).ToList();
        }
        public List<ProductionDetail> GetProductionDetail(DateTime fromDate, DateTime toDate)
        {
            string query = string.Format("EXEC [DBO].[GetProductionDetail] @FROMDATE='{0}',@TODATE='{1}',@COMPANYID={2},@FiscalId={3}", fromDate.ToString("yyyy-MM-dd"), toDate.ToString("yyyy-MM-dd"),
                           SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id);
            return Db.Database.SqlQuery<ProductionDetail>(query).ToList();
        }
        public List<InternalStockTransferDetail> GetInternalStockTransferDetail(DateTime fromDate, DateTime toDate)
        {
            string query = string.Format("EXEC [DBO].[GetInternalStockTransferDetail] @FROMDATE='{0}',@TODATE='{1}',@COMPANYID={2},@FiscalId={3}", fromDate.ToString("yyyy-MM-dd"), toDate.ToString("yyyy-MM-dd"),
                           SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id);
            return Db.Database.SqlQuery<InternalStockTransferDetail>(query).ToList();
        }
        public List<VehicleInstallmentDetailExtra> GetVehicleInstallmentDetail(DateTime fromDate, DateTime toDate)
        {
            string query = string.Format("EXEC [DBO].[VehicleInstallmentDetail] @FROMDATE='{0}',@TODATE='{1}'", fromDate.ToString("yyyy-MM-dd"), toDate.ToString("yyyy-MM-dd"));
            return Db.Database.SqlQuery<VehicleInstallmentDetailExtra>(query).ToList();
        }
        
        public List<ProfitLoss> GetProfitLoss(DateTime fromdate, DateTime todate, int openingStock)
        {
            var acrepo = new AccountRepository();
            var salerepo = new SaleRepository();
            var transrepo = new TransactionRepository();

            //var ProfitAccount = new AccountRepository().GetLeafAccountsDetail(ProfitaccountId);
            var profitLoss = new List<ProfitLoss>();
            var profitlossAccounts = new List<ProfitLossAccount>();
            //var placc = new ProfitLossAccount
            //{
            //    AccountName = "Sale",
            //    Amount = salerepo.GetSumByVoucherType(fromdate, todate, VoucherType.Sale)
            //};
            //profitlossAccounts.Add(placc);
            //placc = new ProfitLossAccount
            //{
            //    AccountName = "Sale Returns",
            //    Amount = -(salerepo.GetSumByVoucherType(fromdate, todate, VoucherType.SaleReturn))
            //};
            //profitlossAccounts.Add(placc);
            //placc = new ProfitLossAccount
            //{
            //    AccountName = "Purchase",
            //    Amount = -(salerepo.GetSumByVoucherType(fromdate, todate, VoucherType.Purchase))
            //};
            //profitlossAccounts.Add(placc);
            //placc = new ProfitLossAccount
            //{
            //    AccountName = "Purchase Returns",
            //    Amount = salerepo.GetSumByVoucherType(fromdate, todate, VoucherType.Purchase)
            //};
            //profitlossAccounts.Add(placc);
            //placc = new ProfitLossAccount
            //{
            //    AccountName = "Opening Stock",
            //    Amount = -(openingStock)
            //};
            //profitlossAccounts.Add(placc);
            //placc = new ProfitLossAccount
            //{
            //    AccountName = "Closing Stock",
            //    Amount = transrepo.GetAllStock().Sum(p => p.BookValue)
            //};
            //profitlossAccounts.Add(placc);

            //profitLoss.Add(new ProfitLoss()
            //{
            //    AccountType = "Profit",
            //    Accounts = profitlossAccounts
            //});
            var expenseaccountId = acrepo.GetIdByName("Expenses");
            var expenseAccounts = new AccountRepository().GetLeafAccountsDetail(expenseaccountId);

            profitlossAccounts = expenseAccounts.Select(item => new ProfitLossAccount()
            {
                AccountName = item.DisplayName,
                Amount = transrepo.GetBalanceByDates(item.Id, fromdate, todate),
            }).ToList();
            profitLoss.Add(new ProfitLoss()
            {
                AccountType = "Expense",
                Accounts = profitlossAccounts
            });
            return profitLoss;
        }
    }
}