using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using AccountEx.CodeFirst.Models;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using AccountEx.DbMapping;
using AccountEx.BussinessLogic.Common;

namespace AccountEx.BussinessLogic
{
    public static class ReportManager
    {
        public static DataTable GetReportData(ReportObject ro)
        {
            var report = new ReportRepository().GetReportById(ro.Id);
            return new SqlRepository().GetDataTable(report.Location, GetParameters(ro.Id, ro.Parameters));
        }
        public static DataTable GetReportData(string reportName, List<SqlParameter> parameters)
        {
            return new SqlRepository().GetDataTable(reportName, parameters);
        }

        public static DataSet GetReportData(List<SqlParameter> parameters, string reportName)
        {
            return new SqlRepository().GetDataSet(reportName, parameters);
        }

        private static List<SqlParameter> GetParameters(int reportId, List<ReportParameter> parameters)
        {
            var paramValues = parameters.ToDictionary(p => p.Name, q => q.Value);
            var reportParams = new ReportParameterRepository().GetByReportId(reportId);
            var list = new List<SqlParameter>();
            foreach (var item in reportParams)
            {
                if (item.AdvanceType == (byte)ReportParameterType.Simple)
                {
                    if (!paramValues.ContainsKey(item.Name)) throw new OwnException(string.Format("Parameter: {0} is missing", item.Name));
                    var value = paramValues[item.Name];
                    if (item.Type == SqlDbType.Date.ToString())
                    {
                        DateTime dt;
                        DateTime.TryParseExact(paramValues[item.Name], "yyyy/MM/dd", new CultureInfo("en-US"), DateTimeStyles.None, out dt);
                        list.Add(new SqlParameter(item.Name, dt) { });
                    }
                    else
                        list.Add(new SqlParameter(item.Name, paramValues[item.Name]) { });
                }
                else if (item.AdvanceType == (byte)ReportParameterType.Session)
                {
                    var value = SiteContext.Current.User.CompanyId;
                    list.Add(new SqlParameter(item.Name, value) { });

                }
                else if (item.AdvanceType == (byte)ReportParameterType.FiscalId)
                {
                    var value = SiteContext.Current.Fiscal.Id;
                    list.Add(new SqlParameter(item.Name, value) { });

                }
                else if (item.AdvanceType == (byte)ReportParameterType.DefaultValue)
                {
                    list.Add(new SqlParameter(item.Name, item.Value) { });

                }

                else if (item.AdvanceType == (byte)ReportParameterType.FiscalYearStartDate)
                {
                    var date = FiscalYearManager.GetStartDate();
                    list.Add(new SqlParameter(item.Name, date) { });

                }
                else if (item.AdvanceType == (byte)ReportParameterType.FiscalYearEndDate)
                {
                    var date = FiscalYearManager.GetEndDate();
                    list.Add(new SqlParameter(item.Name, date) { });

                }
                else if (item.AdvanceType == (byte)ReportParameterType.IdType)
                {
                    var idTypeList = item.Value.Split(',').Select(int.Parse).ToList().ToIdTypeDataTable();
                    list.Add(new SqlParameter(item.Name, idTypeList) { SqlDbType = SqlDbType.Structured });

                }

            }
            return list;
        }

        private static List<SqlParameter> GetCustomParameters(List<CustomReportParameter> parameters)
        {
            var paramValues = parameters.ToDictionary(p => p.Name, q => q.Value);
            var list = new List<SqlParameter>();
            foreach (var param in parameters)
            {
                var paramValue = param.Value;
                list.Add(new SqlParameter(param.Name, param.Value) { });
            }
            return list;
        }

        public static List<Report> GetReports(byte module)
        {
            var reports = new List<Report>();
            var rptRepo = new ReportRepository();
            var rptParamRepo = new ReportParameterRepository();
            var rptCollection = new List<Report>();
            if (module == 0)
                rptCollection = rptRepo.GetAll();
            else
                rptCollection = rptRepo.GetReportByModule(module);
            foreach (var report in rptCollection)
            {
                if (!report.IsExternal && !report.IsExecuted)
                {
                    var sqlParams = new SqlRepository().GetParams(report.Location);
                    var sn = 0;
                    foreach (SqlParameter param in sqlParams)
                    {
                        if (param.ParameterName == "@RETURN_VALUE") continue;
                        var rptParam = new ReportParameter()
                        {
                            ReportId = report.Id,
                            Name = param.ParameterName,
                            Type = param.SqlDbType.ToString(),
                            ControlType = "text",
                            IsVisible = GetIsVisible(param.ParameterName),
                            AdvanceType = GetAdvanceType(param.ParameterName),
                            SequenceNumber = ++sn,
                            DisplayLabel = GetDisplayLabel(param.ParameterName),
                            InplutCssClass = GetInplutCssClass(param),
                        };
                        report.ReportParameters.Add(rptParam);
                    }
                    report.IsExecuted = true;
                    rptRepo.Save(report);
                }
                reports.Add(report);
            }
            return reports;
        }
        private static string GetDisplayLabel(string label)
        {
            return System.Text.RegularExpressions.Regex.Replace(label.Replace("@", ""), "(\\B[A-Z])", " $1");
        }
        private static bool GetIsVisible(string name)
        {
            name = name.Replace("@", "");
            var data = new List<string> { "VoucherType", "CompanyId", "UserId", "AccountDetailFormId", "FiscalId" };
            return !data.Contains(name);
        }

        private static byte GetAdvanceType(string name)
        {
            name = name.Replace("@", "");
            var defaultParameters = new List<string> { "VoucherType", "AccountDetailFormId" };
            var sessionParameters = new List<string> { "CompanyId", "UserId", "FiscalId" };
            if (defaultParameters.Contains(name))
                return (byte)ReportParameterType.DefaultValue;
            else if (sessionParameters.Contains(name))
                return (byte)ReportParameterType.Session;
            else
                return (byte)ReportParameterType.Simple;
        }
        private static string GetInplutCssClass(SqlParameter param)
        {
            var InplutCssClass = "";
            switch (param.SqlDbType)
            {
                case SqlDbType.Date:
                case SqlDbType.DateTime:
                case SqlDbType.DateTime2:
                    InplutCssClass = "date-picker";
                    break;
                case SqlDbType.BigInt:
                case SqlDbType.Int:
                case SqlDbType.SmallInt:
                case SqlDbType.TinyInt:
                    InplutCssClass = "number";
                    break;
            }
            return InplutCssClass;
        }
        public static DataTable GetJobInvoiceRegister(DateTime d1, DateTime d2)
        {
            var query = "GetJobInvoiceSummary";
            var sqlParams = new List<SqlParameter> { };

            sqlParams.Add(new SqlParameter("FromDate", SqlDbType.Date) { Value = d1 });
            sqlParams.Add(new SqlParameter("ToDate", SqlDbType.Date) { Value = d2 });
            return new SqlRepository().GetDataTable(query, sqlParams);
        }
        /// <summary>
        /// Retrieves all reports for a program
        /// </summary>
        /// <param name="programId">The program for which reports should be retrieved</param>
        /// <returns>An IEnumerable of reports, or an empty IEnumerable if none are found</returns>
        //public static IEnumerable<Report> GetReportsByProgramId(int programId)
        //{
        //    var dbCollection = DBProviderManager<DBReportsProvider>.Provider.GetReportsByProgramId(programId);
        //    return dbCollection.Select(r => new Report(r)).ToList();
        //}

        /// <summary>
        /// Returns an object representing a specific report
        /// </summary>
        /// <param name="reportId">The id of the report to retrieve</param>
        /// <returns>The report</returns>
        public static Report GetReportById(int reportId)
        {
            var dbReport = new ReportRepository().GetReportById(reportId);
            return dbReport;
            //if (dbReport != null)
            //{
            //    return new Report(dbReport);
            //}
            //return null;
        }

        /// <summary>
        /// Retrieves a report by its name, pulling the program specific version if one exists
        /// </summary>
        /// <param name="reportName">The name of the report</param>
        /// <param name="programId">The program for which the program specific one should be retrieved, if present</param>
        /// <returns>A single report (non program specific if the program specific one for the specified program could not be found)</returns>
        //public static Report GetReportByNameAndProgram(string reportName, int programId)
        //{
        //    var dbReport = DBProviderManager<DBReportsProvider>.Provider.GetReportByNameAndProgram(reportName, programId);
        //    if (dbReport != null)
        //    {
        //        return new Report(dbReport);
        //    }
        //    return null;
        //}

        /// <summary>
        /// Runs the specified report
        /// </summary>
        /// <param name="report">The report to be run</param>
        /// <param name="reportParameters">The parameters to be passed to the report stored procedure</param>
        /// <returns>The data generated by the report</returns>
        public static IEnumerable<IReportData> LoadReport(Report report, List<ReportParameterExtra> reportParameters)
        {
            if (reportParameters == null)
            {
                reportParameters = new List<ReportParameterExtra>();
            }

            IEnumerable<IReportData> reportResults = new List<IReportData>();

            //Set up and invoke the reporting method
            var reportManager = typeof(ReportManager);
            if (reportManager != null)
            {
                var reportMethod = reportManager.GetMethod(report.Location);
                var parameterList = reportMethod.GetParameters();
                //need to make sure report parameters matches exactly what we want to be passing to the method
                var orderedParameters = new List<ReportParameterExtra>();
                foreach (var methodParameter in parameterList)
                {
                    var matchingParameter = reportParameters.FirstOrDefault(rp => rp.ParameterName.ToLower() == methodParameter.Name.ToLower());
                    if (matchingParameter == null)
                    {
                        //no matching parameter found, for now use null, but I'm going to need to determine behaviour for non nullables
                        orderedParameters.Add(new ReportParameterExtra { ParameterName = methodParameter.Name, ParameterValue = null });
                    }
                    else
                    {
                        //we have a match, cast the match to the right type
                        //Don't seem to be able to do an arbitrary typecast, so doing this type by type
                        if (methodParameter.ParameterType == typeof(int))
                        {
                            matchingParameter.ParameterValue = int.Parse(matchingParameter.ParameterValue.ToString());
                        }
                        else if (methodParameter.ParameterType == typeof(DateTime))
                        {
                            matchingParameter.ParameterValue = DateTime.Parse(matchingParameter.ParameterValue.ToString());
                        }
                        else if (methodParameter.ParameterType == typeof(bool))
                        {
                            matchingParameter.ParameterValue = matchingParameter.ParameterValue.ToString().ToLower() == "on" ? true : false;
                        }
                        else if (methodParameter.ParameterType.IsGenericType && methodParameter.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            //we are dealing with a nullable parameter, so get the underlying type
                            if (Nullable.GetUnderlyingType(methodParameter.ParameterType) == typeof(int))
                            {
                                int? val = null;
                                int temp;
                                if (int.TryParse(matchingParameter.ParameterValue.ToString(), out temp) && temp > 0)
                                {
                                    val = temp;
                                }
                                matchingParameter.ParameterValue = val;
                            }
                            else if (Nullable.GetUnderlyingType(methodParameter.ParameterType) == typeof(DateTime))
                            {
                                DateTime? val = null;
                                DateTime temp;
                                if (DateTime.TryParse(matchingParameter.ParameterValue.ToString(), out temp))
                                {
                                    val = temp;
                                }
                                matchingParameter.ParameterValue = val;
                            }
                            else if (Nullable.GetUnderlyingType(methodParameter.ParameterType) == typeof(bool))
                            {
                                matchingParameter.ParameterValue = matchingParameter.ParameterValue.ToString().ToLower() == "on" ? true : false;
                            }
                        }
                        orderedParameters.Add(matchingParameter);
                    }

                }
                var key = string.Format("Report_{0}", reportMethod.Name);
                foreach (var param in orderedParameters)
                {
                    key += "_" + param.ParameterValue;
                }
                //reportResults = SiteCache.Get(key, () =>
                //{
                return (IEnumerable<IReportData>)reportMethod.Invoke(null, orderedParameters.Select(rp => rp.ParameterValue).ToArray());
                //}, DateTime.Now.AddMinutes(5));
            }

            return reportResults;
        }

        /// <summary>
        /// Returns a collection of key-value pairs to be used to create a drop down for reporting.
        /// </summary>
        /// <param name="parameterName">The name of the parameter the drop down represents. Generally the primary key of some table</param>
        /// <param name="programID">The program in which the report is being run</param>
        /// <returns>Set of tuples containing the id of the row and the name of the entry</returns>
        public static IEnumerable<Tuple<int, string>> GetGenericReportDropDown(string parameterName, int programID)
        {
            var resultSet = new List<Tuple<int, string>>();
            //resultSet.Add(new Tuple<int, string>(0, LocalizationManager.GetLocaleResourceString("Admin.Common.All", SiteContext.Current.AdminCurrentProgramID)));

            //if (parameterName.ToLower() == "supplierid")
            //{
            //    //resultSet.AddRange(SupplierManager.GetAllSuppliers(ProgramManager.GetProgramById(programID).BranchId).Select(s => new Tuple<int, string>(s.SupplierId, s.Name)));
            //    //change supplier dropdown box from supplier list to product supplier list                
            //    resultSet.AddRange(ProductSupplierManager.GetProductSuppliersByProgramID(programID).Select(s => new Tuple<int, string>(s.ProductSupplierID, s.Name)));
            //}
            //else if (parameterName.ToLower() == "quizid")
            //{
            //    resultSet.AddRange(QuestionSetManager.GetAllQuizzesByProgram(programID).Select(q => new Tuple<int, string>(q.QuizId, q.Name)));
            //}
            //else if (parameterName.ToLower() == "accountid" || parameterName.ToLower() == "subaccountid")
            //{
            //    resultSet.AddRange(CustomerManager.GetCustomerAccounts(SiteContext.Current.User.CustomerId, programID).Select(a => new Tuple<int, string>(a.CustomerAccountId, a.AccountNumber)));
            //}
            //else if (parameterName.ToLower() == "quarterid")
            //{
            //    resultSet = new int[] { 1, 2, 3, 4 }.Select(q => new Tuple<int, string>(q, string.Format("Quarter {0}", q))).ToList();
            //}

            return resultSet;
        }



        public static IEnumerable<IReportData> GetTrialBalanceData(DateTime fromDate)
        {


            var rep = new AccountRepository();
            var setting = new FormSettingRepository().GetFormSettingByVoucherType("Recovery");
            var customer = 0;
            var customersetting = setting.FirstOrDefault(p => p.KeyName == "MasterAccountId");
            if (customersetting != null)
                customer = Numerics.GetInt(customersetting.Value);
            var account = rep.GetLeafAccount(customer);


            var list = new List<TrailBalanceExtra>();
            var data = new TrailBalanceExtra();
            var balances = new TransactionRepository().GetOpeningStartBalance(account.Select(p => p.Id).ToList());
            var i = 1;
            foreach (var item in account)
            {
                data = new TrailBalanceExtra();
                var balance = new TransactionRepository().GetOpeningBalance(item.Id, fromDate);
                data.Debit = balance > 0 ? balance : 0;
                data.Credit = balance < 0 ? balance : 0;
                data.CustomerName = item.Name;
                data.Sr = i;
                if (data.Debit == 0 && data.Credit == 0)
                    continue;
                list.Add(data);
                i++;

            }
            return list;
        }
        public static IEnumerable<IReportData> GetGeneralLedgerData(DateTime fromDate, DateTime toDate, int accountId)
        {
            var repo = new TransactionRepository();

            //var data = new
            //{
            //    OpeningBalance = repo.GetOpeningBalance(accountId, fromDate),
            //    Transactions = repo.GetTransactions(accountId, fromDate, toDate)

            //};
            var data = repo.GetTransactions(accountId, fromDate, toDate);
            var list = new List<DetailedLedgerReport>();
            var obj = new DetailedLedgerReport();
            //var lastTotal = data.OpeningBalance;
            decimal lastTotal = 0;
            foreach (var item in data)
            {
                obj = new DetailedLedgerReport();
                lastTotal = lastTotal - item.Credit + item.Debit;
                obj.Date = item.Date.ToString(AppSetting.DateFormat);
                obj.VoucherNumber = item.VoucherNumber + "";
                obj.VoucherType = GetVoucherShortType(item.TransactionType + "") + "";
                obj.CustomerName = item.AccountTitle;
                obj.Description = (item.Quantity + " * " + item.Price) + "";
                obj.Debit = item.Debit;
                obj.Credit = item.Credit;
                obj.Balance = lastTotal;
                list.Add(obj);
            }
            return list;

        }
        public static IEnumerable<IReportData> GetSaleData(DateTime fromDate, DateTime toDate)
        {

            var repo = new TransactionRepository();
            var data = repo.AsQueryable().Where(p => p.Date >= fromDate && p.Date <= toDate).ToList();
            var list = new List<SaleReport>();
            var obj = new SaleReport();
            foreach (var item in data)
            {
                obj = new SaleReport();

                obj.Date = item.Date.ToString(AppSetting.DateFormat);
                obj.VoucherNumber = item.VoucherNumber + "";
                obj.CustomerName = item.AccountTitle;
                obj.BillNumber = item.InvoiceNumber + "";
                obj.Amount = item.Debit;
                list.Add(obj);

            }
            return list;

        }
        public static List<Voucher> GetVoucherList(DateTime fromDate, DateTime toDate, List<VoucherType> transactionTypes)
        {



            return new VoucherTransRepository().GetByDates(fromDate, toDate, transactionTypes);

        }
        public static List<Voucher> GetUnpostedVoucherList(DateTime fromDate, DateTime toDate, List<VoucherType> transactionTypes)
        {



            return new VoucherTransRepository().GetUnPostedByDates(fromDate, toDate, transactionTypes);

        }


        //public static List<Order> GetPendingOrder()
        //{

        //    var repo = new OrderBookingRepository();

        //    var data = repo.AsQueryable().Where(p => p.Status == (byte)TransactionStatus.PendingOrder || p.Status == (byte)TransactionStatus.PendingOrder).ToList();

        //    return data;

        //}

        //public static List<DeliveryChallan> GetPendingDC()
        //{

        //    var repo = new DeliveryChallanRepository();

        //    var data = repo.AsQueryable().Where(p => p.Status == (byte)TransactionStatus.PendingDC).ToList();

        //    return data;

        //}

        public static string GetVoucherShortType(string vouchertype)
        {
            var shorttype = "";
            var dictionary = new Dictionary<string, string>();
            dictionary.Add("1", "SV");
            dictionary.Add("2", "PV");
            dictionary.Add("3", "SRV");
            dictionary.Add("4", "PRV");
            dictionary.Add("5", "CR");
            dictionary.Add("6", "CP");
            dictionary.Add("7", "BR");
            dictionary.Add("8", "BP");
            dictionary.Add("9", "SV");
            dictionary.Add("10", "SV");
            if (dictionary.ContainsKey(vouchertype))
            {
                shorttype = dictionary[vouchertype];
            }
            return shorttype;


        }

        public static List<ProfitLoss> GetProfitLoss(DateTime fromDate, DateTime toDate, bool isBeforeClosing)
        {
            var repo = new ReportRepository();
            var transRepo = new TransactionRepository();
            var accountRepo = new AccountRepository();

            var trialEntries = transRepo.GetFiscalTrial(fromDate, toDate, isBeforeClosing);

            var accounts = new Dictionary<int, string>();
            var plsAccounts = new List<ProfitLossAccount>();
            var expenseAccounts = new List<ProfitLossAccount>();
            var saleAccountIds = new List<IdName>();

            if (SettingManager.SaleAccountHeadId != 0)
            {
                var saleAccount = accountRepo.GetById(SettingManager.SaleAccountHeadId, false);
                saleAccountIds = accountRepo.GetLeafAccount(saleAccount.ParentId.Value, false);
            }
            else if (SettingManager.GstSaleAccountHeadId != 0)
            {
                var gstSaleAccount = accountRepo.GetById(SettingManager.GstSaleAccountHeadId, false);
                saleAccountIds = accountRepo.GetLeafAccount(gstSaleAccount.ParentId.Value, false);
            }



            //if (!accounts.ContainsKey(SettingManager.SaleAccountHeadId)) accounts.Add(SettingManager.SaleAccountHeadId, SettingManager.SaleAc);
            //if (!accounts.ContainsKey(SettingManager.SaleReturnAccountHeadId)) accounts.Add(SettingManager.SaleReturnAccountHeadId, SettingManager.SaleReturnAc);

            //foreach (var item in accounts)
            //{
            //    if (trialEntries.ContainsKey(item.Key))
            //    {
            //        plsAccounts.Add(new ProfitLossAccount() { AccountName = item.Value, Amount = trialEntries[item.Key], SubType = "Sales" });
            //    }
            //}
            //accounts = new Dictionary<int, string>();
            //if (!accounts.ContainsKey(SettingManager.GstSaleAccountHeadId)) accounts.Add(SettingManager.GstSaleAccountHeadId, SettingManager.GstSaleAc);
            //if (!accounts.ContainsKey(SettingManager.GstSaleReturnAccountHeadId)) accounts.Add(SettingManager.GstSaleReturnAccountHeadId, SettingManager.GstSaleReturnAc);
            //foreach (var item in accounts)
            //{
            //    if (trialEntries.ContainsKey(item.Key))
            //    {
            //        plsAccounts.Add(new ProfitLossAccount() { AccountName = item.Value, Amount = trialEntries[item.Key], SubType = "GSTSales" });
            //    }
            //}

            //accounts = new Dictionary<int, string>();
            //if (!accounts.ContainsKey(SettingManager.ServicesAccountId)) accounts.Add(SettingManager.ServicesAccountId, SettingManager.ServicesAc);

            //foreach (var item in accounts)
            //{
            //    if (trialEntries.ContainsKey(item.Key))
            //    {
            //        plsAccounts.Add(new ProfitLossAccount() { AccountName = item.Value, Amount = trialEntries[item.Key], SubType = "Services" });
            //    }
            //}

            foreach (var item in saleAccountIds)
            {
                if (trialEntries.ContainsKey(item.Id))
                {
                    plsAccounts.Add(new ProfitLossAccount() { AccountName = item.Name, Amount = trialEntries[item.Id], SubType = "Sales" });
                }
            }
            accounts = new Dictionary<int, string>();
            if (!accounts.ContainsKey(SettingManager.PurchaseAccountHeadId)) accounts.Add(SettingManager.PurchaseAccountHeadId, SettingManager.PurchaseAc);
            if (!accounts.ContainsKey(SettingManager.PurchaseReturnAccountHeadId)) accounts.Add(SettingManager.PurchaseReturnAccountHeadId, SettingManager.PurchaseReturnAc);
            if (!accounts.ContainsKey(SettingManager.GstPurchaseAccountHeadId)) accounts.Add(SettingManager.GstPurchaseAccountHeadId, SettingManager.GstPurchaseAc);
            if (!accounts.ContainsKey(SettingManager.GstPurchaseReturnAccountHeadId)) accounts.Add(SettingManager.GstPurchaseReturnAccountHeadId, SettingManager.GstPurchaseReturnAc);


            foreach (var item in accounts)
            {
                if (trialEntries.ContainsKey(item.Key))
                {
                    plsAccounts.Add(new ProfitLossAccount() { AccountName = item.Value, Amount = trialEntries[item.Key], SubType = "Purchase" });
                }
            }

            if (SettingManager.DirectExpensesHeadId != 0)
            {
                var directExpAccountIds = accountRepo.GetLeafAccount(SettingManager.DirectExpensesHeadId, false);
                foreach (var item in directExpAccountIds)
                {

                    if (trialEntries.ContainsKey(item.Id))
                        plsAccounts.Add(new ProfitLossAccount() { AccountName = item.Name, Amount = trialEntries[item.Id], SubType = "DirectExp" });

                }

            }
            //accounts = new Dictionary<int, string>();
            //if (!accounts.ContainsKey(SettingManager.PurchaseAccountHeadId)) accounts.Add(SettingManager.PurchaseAccountHeadId, SettingManager.PurchaseAc);


            if (trialEntries.ContainsKey(SettingManager.OtherIncomeAccountId))
            {
                plsAccounts.Add(new ProfitLossAccount() { AccountName = SettingManager.OtherIncome, Amount = trialEntries[SettingManager.OtherIncomeAccountId], SubType = "OtherIncome" });
            }

            if (trialEntries.ContainsKey(SettingManager.WorkInProcessHeadId))
            {
                plsAccounts.Add(new ProfitLossAccount() { AccountName = SettingManager.WorkInProcess, Amount = trialEntries[SettingManager.WorkInProcessHeadId], SubType = "RevenueFromLots" });
            }



            //var purchaseValue = transRepo.GetOpeningBalance(SettingManager.PurchaseAccountHeadId, fromDate, !isBeforeClosing);
            //var gstPurchaseValue = transRepo.GetOpeningBalance(SettingManager.GstPurchaseAccountHeadId, fromDate, !isBeforeClosing);
            //var openingStockValue = transRepo.GetOpeningBalance(SettingManager.StockValueAccountId, fromDate, !isBeforeClosing);
            decimal openingStockValue = 0;
            decimal closingStockValue = transRepo.GetStockValue(SiteContext.Current.Fiscal.FromDate, toDate);
            if (SettingManager.StockValueType == VoucherType.OpeningBalance.ToString())
            {
                openingStockValue = transRepo.GetOpeningBalance(SettingManager.StockValueAccountId, fromDate, !isBeforeClosing);
            }
            else if (SettingManager.StockValueType == VoucherType.ActiveStock.ToString())
            {
                openingStockValue = repo.GetVehicleActiveStockValue(null, fromDate);
                closingStockValue = repo.GetVehicleActiveStockValue(null, toDate);
            }
            else
            {
                openingStockValue = transRepo.GetStockValue(SiteContext.Current.Fiscal.FromDate, fromDate.AddDays(-1));
            }


            var currentStockValue = transRepo.GetStockValue(fromDate, toDate);
            plsAccounts.Add(new ProfitLossAccount() { AccountName = "Opening Stock Value", Amount = openingStockValue, SubType = "OpeningStock" });
            plsAccounts.Add(new ProfitLossAccount() { AccountName = "Closing Stock Value", Amount = closingStockValue, SubType = "ClosingStock" });
            plsAccounts.Add(new ProfitLossAccount() { AccountName = "Current Stock Value", Amount = currentStockValue, SubType = "CurrentStock" });


            accounts = new AccountRepository().GetLeafAccounts(SettingManager.ExpensesHeadId, false).ToDictionary(p => p.Id, q => q.Name);

            foreach (var item in accounts)
            {
                if (trialEntries.ContainsKey(item.Key))
                {
                    expenseAccounts.Add(new ProfitLossAccount() { AccountId = item.Key, AccountName = item.Value, Amount = trialEntries[item.Key] });
                }
            }
            var profitLoss = new List<ProfitLoss>();
            profitLoss.Add(new ProfitLoss() { AccountType = "Profit", Accounts = plsAccounts });
            profitLoss.Add(new ProfitLoss() { AccountType = "Expense", Accounts = expenseAccounts });
            return profitLoss;
        }

        public static List<ProfitLoss> GetProfitLossFormat1(DateTime fromDate, DateTime toDate, bool isBeforeClosing)
        {
            var repo = new ReportRepository();
            var transRepo = new TransactionRepository();
            var accountRepo = new AccountRepository();

            var trialEntries = transRepo.GetFiscalTrial(fromDate, toDate, isBeforeClosing);

            var accounts = new Dictionary<int, string>();
            var plsAccounts = new List<ProfitLossAccount>();
            var expenseAccounts = new List<ProfitLossAccount>();
            var saleAccountIds = new List<IdName>();

            if (SettingManager.SaleAccountHeadId != 0)
            {
                var saleAccount = accountRepo.GetById(SettingManager.SaleAccountHeadId, false);
                saleAccountIds = accountRepo.GetLeafAccount(saleAccount.ParentId.Value, false);
            }
            else if (SettingManager.GstSaleAccountHeadId != 0)
            {
                var gstSaleAccount = accountRepo.GetById(SettingManager.GstSaleAccountHeadId, false);
                saleAccountIds = accountRepo.GetLeafAccount(gstSaleAccount.ParentId.Value, false);
            }



            foreach (var item in saleAccountIds)
            {
                if (trialEntries.ContainsKey(item.Id))
                {
                    plsAccounts.Add(new ProfitLossAccount() { AccountName = item.Name, Amount = trialEntries[item.Id], SubType = "Sales" });
                }
            }
            accounts = new Dictionary<int, string>();
            if (!accounts.ContainsKey(SettingManager.CGSAccountId)) accounts.Add(SettingManager.CGSAccountId, SettingManager.CGSAc);
            


            foreach (var item in accounts)
            {
                if (trialEntries.ContainsKey(item.Key))
                {
                    plsAccounts.Add(new ProfitLossAccount() { AccountName = item.Value, Amount = trialEntries[item.Key], SubType = "CGS" });
                }
            }

            if (SettingManager.DirectExpensesHeadId != 0)
            {
                var directExpAccountIds = accountRepo.GetLeafAccount(SettingManager.DirectExpensesHeadId, false);
                foreach (var item in directExpAccountIds)
                {

                    if (trialEntries.ContainsKey(item.Id))
                        plsAccounts.Add(new ProfitLossAccount() { AccountName = item.Name, Amount = trialEntries[item.Id], SubType = "DirectExp" });

                }

            }
            //accounts = new Dictionary<int, string>();
            //if (!accounts.ContainsKey(SettingManager.PurchaseAccountHeadId)) accounts.Add(SettingManager.PurchaseAccountHeadId, SettingManager.PurchaseAc);


            if (trialEntries.ContainsKey(SettingManager.OtherIncomeAccountId))
            {
                plsAccounts.Add(new ProfitLossAccount() { AccountName = SettingManager.OtherIncome, Amount = trialEntries[SettingManager.OtherIncomeAccountId], SubType = "OtherIncome" });
            }

            if (trialEntries.ContainsKey(SettingManager.WorkInProcessHeadId))
            {
                plsAccounts.Add(new ProfitLossAccount() { AccountName = SettingManager.WorkInProcess, Amount = trialEntries[SettingManager.WorkInProcessHeadId], SubType = "RevenueFromLots" });
            }



            accounts = new AccountRepository().GetLeafAccounts(SettingManager.ExpensesHeadId, false).ToDictionary(p => p.Id, q => q.Name);

            foreach (var item in accounts)
            {
                if (trialEntries.ContainsKey(item.Key))
                {
                    expenseAccounts.Add(new ProfitLossAccount() { AccountId = item.Key, AccountName = item.Value, Amount = trialEntries[item.Key] });
                }
            }
            var profitLoss = new List<ProfitLoss>();
            profitLoss.Add(new ProfitLoss() { AccountType = "Profit", Accounts = plsAccounts });
            profitLoss.Add(new ProfitLoss() { AccountType = "Expense", Accounts = expenseAccounts });
            return profitLoss;
        }

        public static List<ProfitLoss> GetProfitLossBeforeClosing(DateTime fromDate, DateTime toDate)
        {
            var repo = new ReportRepository();
            var transRepo = new TransactionRepository();
            var trialEntries = transRepo.GetFiscalTrial(fromDate, toDate, true);
            var accounts = new Dictionary<int, string>();

            if (!accounts.ContainsKey(SettingManager.SaleAccountHeadId)) accounts.Add(SettingManager.SaleAccountHeadId, SettingManager.SaleAc);
            if (!accounts.ContainsKey(SettingManager.SaleReturnAccountHeadId)) accounts.Add(SettingManager.SaleReturnAccountHeadId, SettingManager.SaleReturnAc);







            var plsAccounts = new List<ProfitLossAccount>();
            var expenseAccounts = new List<ProfitLossAccount>();

            foreach (var item in accounts)
            {
                if (trialEntries.ContainsKey(item.Key))
                {
                    plsAccounts.Add(new ProfitLossAccount() { AccountName = item.Value, Amount = trialEntries[item.Key], SubType = "Sales" });
                }
            }
            accounts = new Dictionary<int, string>();
            if (!accounts.ContainsKey(SettingManager.GstSaleAccountHeadId)) accounts.Add(SettingManager.GstSaleAccountHeadId, SettingManager.GstSaleAc);
            if (!accounts.ContainsKey(SettingManager.GstSaleReturnAccountHeadId)) accounts.Add(SettingManager.GstSaleReturnAccountHeadId, SettingManager.GstSaleReturnAc);
            foreach (var item in accounts)
            {
                if (trialEntries.ContainsKey(item.Key))
                {
                    plsAccounts.Add(new ProfitLossAccount() { AccountName = item.Value, Amount = trialEntries[item.Key], SubType = "GSTSales" });
                }
            }

            accounts = new Dictionary<int, string>();
            if (!accounts.ContainsKey(SettingManager.ServicesAccountId)) accounts.Add(SettingManager.ServicesAccountId, SettingManager.ServicesAc);

            foreach (var item in accounts)
            {
                if (trialEntries.ContainsKey(item.Key))
                {
                    plsAccounts.Add(new ProfitLossAccount() { AccountName = item.Value, Amount = trialEntries[item.Key], SubType = "Services" });
                }
            }

            accounts = new Dictionary<int, string>();
            if (!accounts.ContainsKey(SettingManager.PurchaseAccountHeadId)) accounts.Add(SettingManager.PurchaseAccountHeadId, SettingManager.PurchaseAc);
            if (!accounts.ContainsKey(SettingManager.PurchaseReturnAccountHeadId)) accounts.Add(SettingManager.PurchaseReturnAccountHeadId, SettingManager.PurchaseReturnAc);
            if (!accounts.ContainsKey(SettingManager.GstPurchaseAccountHeadId)) accounts.Add(SettingManager.GstPurchaseAccountHeadId, SettingManager.GstPurchaseAc);
            if (!accounts.ContainsKey(SettingManager.GstPurchaseReturnAccountHeadId)) accounts.Add(SettingManager.GstPurchaseReturnAccountHeadId, SettingManager.GstPurchaseReturnAc);


            foreach (var item in accounts)
            {
                if (trialEntries.ContainsKey(item.Key))
                {
                    plsAccounts.Add(new ProfitLossAccount() { AccountName = item.Value, Amount = trialEntries[item.Key], SubType = "Purchase" });
                }
            }

            var openingStockValue = transRepo.GetOpeningBalance(SettingManager.StockValueAccountId, fromDate, true);
            var currentStockValue = transRepo.GetStockValue(fromDate, toDate);
            plsAccounts.Add(new ProfitLossAccount()
            {
                AccountName = "Opening Stock Value",
                Amount = openingStockValue,
                SubType = "OpeningStock"
            });
            plsAccounts.Add(new ProfitLossAccount() { AccountName = "Closing Stock Value", Amount = currentStockValue, SubType = "ClosingStock" });


            accounts = new AccountRepository().GetLeafAccounts(SettingManager.ExpensesHeadId, false).ToDictionary(p => p.Id, q => q.Name);

            foreach (var item in accounts)
            {
                if (trialEntries.ContainsKey(item.Key))
                {
                    expenseAccounts.Add(new ProfitLossAccount() { AccountId = item.Key, AccountName = item.Value, Amount = trialEntries[item.Key] });
                }
            }
            var profitLoss = new List<ProfitLoss>();
            profitLoss.Add(new ProfitLoss() { AccountType = "Profit", Accounts = plsAccounts });
            profitLoss.Add(new ProfitLoss() { AccountType = "Expense", Accounts = expenseAccounts });
            return profitLoss;
        }


        public static DataSet GetVatRegister(DateTime fromDate, DateTime toDate,string salesmanIds,int includeStockTransfer)
        {

            var parameters = new List<SqlParameter>();
            parameters.AddRange(ReportParametrManager.GetFiscalANDCompanyIdParametrs());
            parameters.Add(ReportParametrManager.GetFromDate(fromDate));
            parameters.Add(ReportParametrManager.GetToDate(toDate));
            parameters.Add(ReportParametrManager.GetSalePersonIds(salesmanIds));
            parameters.Add(ReportParametrManager.GetIncludeStockTransfer(includeStockTransfer));
            return GetReportData(parameters, "[DBO].[GetVatRegister]");
        }

    }

}
