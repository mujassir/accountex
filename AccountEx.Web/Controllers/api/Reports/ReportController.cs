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
using AccountEx.CodeFirst.Models.Vehicles;
using AccountEx.DbMapping;
using System.Data.Entity.Core.Objects;
using System.Linq.Expressions;
using AccountEx.CodeFirst.Models.Production;

namespace AccountEx.Web.Controllers.api.Reports
{


    public class ReportController : BaseApiController
    {
        // GET api/test
        public JQueryResponse Get()
        {
            return GetDataTable();
        }
        public virtual ApiResponse Get(string key)
        {
            // var key = Request.GetQueryString("key");
            var response = new ApiResponse();
            try
            {
                switch (key)
                {
                    case "JobRegisterSummary":
                    case "JobInvoiceSummary":
                        var FromDate = DateConverter.ConvertFromDmy(QueryString["FromDate"]);
                        var ToDate = DateConverter.ConvertFromDmy(QueryString["ToDate"]);
                        var dt = ReportManager.GetJobInvoiceRegister(FromDate, ToDate);
                        response = new ApiResponse
                        {
                            Success = true,
                            Data = JsonConvert.SerializeObject(dt)

                        };
                        break;
                    case "GetDailyActivity":
                        response = GetDailyActivity();
                        break;
                    case "GetGeneralLedger":
                        response = GetGeneralLedger();
                        break;
                    case "GetVehicleLedger":
                        response = GetVehicleLedger();
                        break;
                    case "GetVehicleProfile":
                        response = GetVehicleProfile();
                        break;
                    case "GetDetailedGeneralLedger":
                        response = GetDetailedGeneralLedger();
                        break;
                    case "GetNTDetailedGeneralLedger":
                        response = GetNTDetailedGeneralLedger();
                        break;
                    case "GetProductLedger":
                        response = GetProductLedger();
                        break;
                    case "GetVoucherList":
                        response = GetVoucherList();
                        break;
                    case "GetOrderListing":
                        response = GetOrderListing();
                        break;
                    case "GetOrderByStatus":
                        response = GetOrderByStatus();
                        break;
                    case "GetProductSalePurchaseReport":
                        response = GetSalePurchaseReport();
                        break;
                    case "GetPartyTrans":
                        response = GetPartyTrans();
                        break;
                    case "GetLabourReport":
                        response = GetLabourReport();
                        break;
                    case "GetVatRegister":
                        response = GetVatRegister();
                        break;


                    case "GetProfitLoss":
                        response = GetProfitLoss();
                        break;
                    case "GetProfitLossFormat1":
                        response = GetProfitLossFormat1();
                        break;


                    case "GetBalanceSheet":
                        response = GetBalanceSheet();
                        break;
                    case "GetPeriodicBalances":
                        response = GetPeriodicBalances();
                        break;
                    case "GetCustomerAging":
                        response = GetCustomerAging();
                        break;
                    case "GetPharmacyStock":
                        response = GetPharmacyStock();
                        break;
                    case "GetPharmacyStockWithNote":
                        response = GetPharmacyStockWithNote();
                        break;
                    case "AccountBalances":
                        response = GetAccountBalances();
                        break;
                    case "GetDayBook":
                        response = GetDayBook();
                        break;
                    case "GetYearlySalePurchaseSummary":
                        response = GetYearlySalePurchaseSummary();
                        break;
                    case "GetYearlyComparison":
                        response = GetYearlyComparisonReport();
                        break;
                    case "GetAreaWiseSaleReport":
                        response = GetAreaWiseSaleReport();
                        break;
                    case "SaleReportByAreaDateRange":
                        response = SaleReportByAreaDateRange();
                        break;
                    case "GetStaticLedger":
                        response = GetStaticLedger();
                        break;
                    case "GetLessDetail":
                        response = GetLessDetail();
                        break;
                    case "GetUsmanBrosCustomers":
                        response = GetUsmanBrosCustomers();
                        break;
                    case "GetRegisterSummary":
                        response = GetRegisterSummary();
                        break;
                    case "GetNTVehicleDetail":
                        response = GetNTVehicleDetail();
                        break;
                    case "GetNTDataExport":
                        response = GetNTDataExport();
                        break;
                    case "GetDailyProfitLoss":
                        response = GetDailyProfitLoss();
                        break;
                    case "GetVehicleInstallmentDetail":
                        response = GetVehicleInstallmentDetail();
                        break;
                    case "GetRentalAccountStatement":
                        response = GetRentalAccountStatement();
                        break;
                    case "GetRecoveryOfPossessionCharges":
                        response = GetRecoveryOfPossessionCharges();
                        break;
                    case "GetRecoveryOfPossessionCharges1":
                        response = GetRecoveryOfPossessionCharges1();
                        break;
                    case "GetDetailOfOverallBillsIssueToTenants":
                        response = GetDetailOfOverallBillsIssueToTenants();
                        break;
                    case "GetRecoveryOfRent":
                        response = GetRecoveryOfRent();
                        break;
                    case "GetOverallRecoveryReport":
                        response = GetOverallRecoveryReport();
                        break;
                    case "GetSecuirtyPossessionAccountStatement":
                        response = GetSecuirtyPossessionAccountStatement();
                        break;
                    case "GetSecuirtyPossessionAccountStatement1":
                        response = GetSecuirtyPossessionAccountStatement1();
                        break;
                    case "GetGeneralLedgerWithMultiAccouts":
                        response = GetGeneralLedgerWithMultiAccouts();
                        break;

                    case "GetCustomerColection":
                        response = GetCustomerColection();
                        break;
                    case "GetVehiclePostDatedCheque":
                        response = GetVehiclePostDatedCheque();
                        break;
                    case "GetProductionDetail":
                        response = GetProductionDetail();
                        break;




                        //case "GetActiveStocks":
                        //    response = GetActiveStocks();
                        //    break;







                }

            }
            catch (Exception ex)
            {

                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };


            }
            return response;
        }
        public virtual ApiResponse Get(string key, string name)
        {
            // var key = Request.GetQueryString("key");
            var response = new ApiResponse();
            try
            {
                response = new ApiResponse
                {
                    Success = true,
                    Data = new ReportRepository().GetReportByName(name)
                };

            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
        public virtual ApiResponse Get(int Id, string key)
        {
            // var key = Request.GetQueryString("key");
            var response = new ApiResponse();
            try
            {
                response = new ApiResponse
                {
                    Success = true,
                    Data = new ReportRepository().GetById(Id)
                };

            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
        public virtual ApiResponse Post(string key, [FromBody]PrintExtra input)
        {
            // var key = Request.GetQueryString("key");
            var response = new ApiResponse();
            try
            {

                response = new ApiResponse
                {
                    Success = true,
                    Data = ConvertToPDF(input)
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
        private string ConvertToPDF(PrintExtra Report)
        {
            var queryString = Request.RequestUri.ParseQueryString();
            //SelectPdf.GlobalProperties.LicenseKey = "jkfdjjdfkjfdk";
            // var file = Server.MapPath("~/Upload/ReportHTML.htm");

            // read parameters from the webpage
            string htmlString = Report.Data;
            string baseUrl = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/";
            // htmlString = htmlString.Replace("/Content/", "http://localhost:27099/Content/");
            htmlString = htmlString.Replace("ReportPrint1234", "ReportPrint1").Replace("visible-print", "");




            string pdf_page_size = "A4";
            PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize),
                pdf_page_size, true);

            string pdf_orientation = "Portrait";
            PdfPageOrientation pdfOrientation =
                (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation),
                pdf_orientation, true);

            int webPageWidth = 1024;
            //try
            //{
            //    webPageWidth = Convert.ToInt32(TxtWidth.Text);
            //}
            //catch { }

            int webPageHeight = 0;
            //try
            //{
            //    webPageHeight = Convert.ToInt32(TxtHeight.Text);
            //}
            //catch { }

            // instantiate a html to pdf converter object
            HtmlToPdf converter = new HtmlToPdf();
            // set converter options
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = pdfOrientation;
            converter.Options.WebPageWidth = webPageWidth;
            converter.Options.WebPageHeight = webPageHeight;
            converter.Options.InternalLinksEnabled = false;
            converter.Options.ExternalLinksEnabled = false;
            converter.Options.MarginTop = 5;
            //converter.Options.MarginLeft = 5;
            //converter.Options.MarginRight = 5;
            converter.Options.MarginBottom = 5;
            // converter.Options.CssMediaType = HtmlToPdfCssMediaType.Print;
            // create a new pdf document converting an url
            SelectPdf.PdfDocument doc = converter.ConvertHtmlString(htmlString, baseUrl);
            doc.DocumentInformation.CreationDate = DateTime.Now;
            doc.DocumentInformation.Title = Report.ReportName;
            doc.DocumentInformation.Author = SettingManager.ApplicationTitle;
            doc.DocumentInformation.Subject = Report.ReportName;

            // doc.Margins = new PdfMargins(10, 10, 0, 0);
            // create a new pdf font
            PdfFont font = doc.AddFont(PdfStandardFont.Helvetica);
            font.Size = 100;

            // add a new page to the document
            //PdfPage page = doc.AddPage();

            // footer template (100 points in height) with text element
            // doc.Footer = doc.AddTemplate(doc.Pages[0].ClientRectangle.Width, 100);
            //PdfTextElement text1 = new PdfTextElement(0, 50,
            //    "Page: {page_number} of {total_pages}.", font);
            //text1.ForeColor = System.Drawing.Color.Blue;
            //doc.Footer.Add(text1);


            var dirPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Upload/PrintFiles/");
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            var filename = Report.ReportName.Replace(" ", "-").Replace("|", "-") + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            while (filename.Contains("--"))
                filename = filename.Replace("--", "-");
            doc.Save(dirPath + filename + ".pdf");

            // close pdf document
            doc.Close();
            return "../Upload/PrintFiles/" + filename + ".pdf";
        }
        private string ConvertToExcel(PrintExtra Report)
        {

            var dirPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Upload/Temp/");

            //if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            //var file = new Random().Next();
            // ExcelFileManager.ListToExcelUsineEpPlus();

            return "../Upload/PrintFiles/" + dirPath + ".pdf";
        }
        private ApiResponse GetDayBook()
        {
            var date1 = Convert.ToDateTime(Request.GetQueryString("date1"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("date2"));
            var accountId = SettingManager.CashAccountId;
            var repo = new TransactionRepository();
            var openingBalance = repo.GetOpeningBalance(accountId, date1);
            var vouchertypelist = new List<VoucherType>();
            vouchertypelist.Add(VoucherType.CashReceipts);
            vouchertypelist.Add(VoucherType.CashPayments);
            var records = new VoucherTransRepository().GetByDates(date1, date2, vouchertypelist);
            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    OpeningBalance = openingBalance,
                    Records = records
                }
            };
            return response;

        }

        private ApiResponse GetGeneralLedgerWithMultiAccouts()
        {
            var date1 = Convert.ToDateTime(Request.GetQueryString("date1"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("date2"));
            var accountIds = Request.GetQueryString("accountIds");
            var repo = new TransactionRepository();
            var accountRepo = new AccountRepository();
            var Alltransactions = new List<GeneralLedgerMultiAccount>();
            var accounts = accountRepo.GetByIds(accountIds.Split(',').Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => Numerics.GetInt(p)).ToList());

            foreach (var account in accounts)
            {
                var accountId = account.Id;

                var openingBalance = repo.GetOpeningBalance(accountId, date1);
                var transactions = repo.GetTransactions(accountId, date1, date2, true).Where(p => p.Debit + p.Credit > 0).OrderBy(p => p.Date).ThenBy(p => p.TransactionType).ThenBy(p => p.VoucherNumber).ToList();
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

                Alltransactions.Add(new GeneralLedgerMultiAccount()
                {
                    Account = account.Name,
                    OpeningBalances = openingBalance,
                    Records = records, //transactions.Select(p => new GeneralLedgerEntry(p)).ToList(),
                    TotalDebit = totalDebit,
                    TotalCredit = totalCredit,
                    TotalBalance = totalDebit - totalCredit + openingBalance,
                });
            }






            var response = new ApiResponse
            {
                Success = true,
                Data = Alltransactions
            };
            return response;

        }
        private ApiResponse GetYearlyComparisonReport()
        {
            var expensesheadid = SettingManager.ExpensesHeadId;

            var data = new ReportRepository().GetYearlyComparison(expensesheadid);

            var response = new ApiResponse
            {
                Success = true,
                Data = data
            };
            return response;

        }

        private ApiResponse GetYearlySalePurchaseSummary()
        {
            var ReportType = Request.GetQueryString("ReportType");

            var voucherTypeIds = new List<VoucherType>();
            if (ReportType == "Sale")
            {
                voucherTypeIds.Add(VoucherType.Sale);
                voucherTypeIds.Add(VoucherType.GstSale);
            }
            else
            {
                voucherTypeIds.Add(VoucherType.Purchase);
                voucherTypeIds.Add(VoucherType.GstPurchase);
            }

            var data = new ReportRepository().GetYearlySalePurchaseSummaryByVoucherTypes(voucherTypeIds);

            var response = new ApiResponse
            {
                Success = true,
                Data = data
            };
            return response;

        }
        private ApiResponse GetNTVehicleDetail()
        {
            var date1 = Convert.ToDateTime(Request.GetQueryString("date1"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("date2"));

            var vehicalDetail = new SaleRepository().GetNTVehicleDetail(date1, date2);
            var dirPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Upload/Temp/");

            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            var file = "Vehicle_Detail_" + new Random().Next() + ".xlsx";
            var finalPath = dirPath + file;
            ExcelFileManager.ListToExcelUsineEpPlus(vehicalDetail, finalPath);
            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    Vehicles = vehicalDetail,
                    FilePath = "../Upload/Temp/" + file
                }
            };
            return response;
        }
        private ApiResponse GetNTDataExport()
        {
            ApiResponse response = new ApiResponse();
            var date1 = Convert.ToDateTime(Request.GetQueryString("date1"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("date2"));
            var dataExportType = (VoucherType)Convert.ToByte(Request.GetQueryString("DataExportType"));
            if (dataExportType == VoucherType.Sale)
            {
                var saleDataExport = new ReportRepository().GetNTSaleDataExport(date1, date2);
                var dirPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Upload/Temp/");
                if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
                var file = "NT_Data_Export_" + new Random().Next() + ".xlsx";
                var finalPath = dirPath + file;
                ExcelFileManager.ListToExcelUsineEpPlus(saleDataExport, finalPath);
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        SaleDataExport = saleDataExport,
                        FilePath = "../Upload/Temp/" + file
                    }
                };
            }
            else
            {
                var purchaseDataExport = new ReportRepository().GetNTPurchaseDataExport(date1, date2);
                var dirPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Upload/Temp/");
                if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
                var file = "NT_Data_Export_" + new Random().Next() + ".xlsx";
                var finalPath = dirPath + file;
                ExcelFileManager.ListToExcelUsineEpPlus(purchaseDataExport, finalPath);
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        PurchaseDataExport = purchaseDataExport,
                        FilePath = "../Upload/Temp/" + file
                    }
                };
            }
            return response;
        }
        private ApiResponse GetDailyProfitLoss()
        {
            ApiResponse response = new ApiResponse();
            var date1 = Convert.ToDateTime(Request.GetQueryString("date1"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("date2"));
            var data = new ReportRepository().GetDailyProfitLoss(date1, date2);
            var accounts = new AccountRepository().GetChildrenToNLevel(SettingManager.DailyExpenseACHeadId);
            var expensesIds = accounts.Select(p => p.Id).ToList();
            var expenses = new TransactionRepository().GetBalanceByDates(expensesIds, date1, date2);
            response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    Transaction = data,
                    Expenses = expenses
                }
            };
            return response;
        }
        private ApiResponse GetVehiclePostDatedCheque()
        {

            var vehicleSaleId = Convert.ToInt32(Request.GetQueryString("vehicleSaleId"));


            var status = Convert.ToByte(Request.GetQueryString("status"));
            var chequeNo = Convert.ToString(Request.GetQueryString("chequeNo"));
            var bankId = Convert.ToInt32(Request.GetQueryString("bankId"));

            var data = new ReportRepository().GetVehiclePostDatedCheque(vehicleSaleId, status, bankId, chequeNo);

            var response = new ApiResponse
            {
                Success = true,
                Data = data
            };
            return response;
        }
        private ApiResponse GetProductionDetail()
        {

            var date1 = Convert.ToDateTime(Request.GetQueryString("date1"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("date2"));

            var data = new ReportRepository().GetProductionDetail(date1, date2);
            var response = new ApiResponse
            {
                Success = true,
                Data = data
            };
            return response;
        }


        private ApiResponse GetVehicleInstallmentDetail()
        {
            ApiResponse response = new ApiResponse();
            var date1 = Convert.ToDateTime(Request.GetQueryString("date1"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("date2"));
            var data = new ReportRepository().GetVehicleInstallmentDetail(date1, date2);

            response = new ApiResponse
            {
                Success = true,
                Data = data
            };
            return response;
        }
        private ApiResponse GetRegisterSummary()
        {
            var date1 = Convert.ToDateTime(Request.GetQueryString("date1"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("date2"));
            //var voutype = Numerics.GetInt(Request.GetQueryString("voutype"));
            var voutype = VoucherType.Sale;
            var repo = new ReportRepository();

            var openingBalance = 0M;

            var transactions = new SaleRepository().GetSalePurchaseDateWiseSummary(date1, date2, voutype);

            transactions = transactions.OrderBy(p => p.Date).ToList();

            var totalamount = transactions.Sum(p => p.GrossTotal);
            var totaldiscountamount = transactions.Sum(p => p.Discount);
            var totalnetamount = transactions.Sum(p => p.NetTotal);

            var runningTotal = openingBalance;
            foreach (var item in transactions)
            {
                runningTotal += item.NetTotal;
                //item.Balance1 = Numerics.DecimalToString(runningTotal);
                //item.Date1 = item.Date.ToString(AppSetting.DateFormat);

            }
            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    OpeningBalance = Numerics.DecimalToString(openingBalance),
                    Records = transactions.OrderByDescending(p => p.Date), //transactions.Select(p => new GeneralLedgerEntry(p)).ToList(),
                    TotalAmount = Numerics.DecimalToString(totalamount),
                    TotalDiscount = Numerics.DecimalToString(totaldiscountamount),
                    TotalNetAmount = Numerics.DecimalToString(totalnetamount),
                    TotalBalance = Numerics.DecimalToString(totalnetamount),
                }
            };
            return response;
        }
        private ApiResponse GetDailyActivity()
        {
            var date1 = Convert.ToDateTime(Request.GetQueryString("date1"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("date2"));
            var accountId = Numerics.GetInt(Request.GetQueryString("accountId"));
            var repo = new ReportRepository();
            var transactions = repo.GetDailyActivity(date1, date2).OrderBy(p => p.Date);
            var totalDebit = transactions.Sum(p => p.Debit);
            var totalCredit = transactions.Sum(p => p.Credit);
            var records = new List<GeneralLedgerEntry>();
            decimal runningTotal = 0;
            foreach (var item in transactions)
            {
                runningTotal += item.Debit - item.Credit;
                var entry = new GeneralLedgerEntry(item);
                entry.Balance = Numerics.DecimalToString(runningTotal, 0);
                records.Add(entry);
            }
            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    OpeningBalance = Numerics.DecimalToString(0),
                    Records = records, //transactions.Select(p => new GeneralLedgerEntry(p)).ToList(),
                    TotalDebit = Numerics.DecimalToString(totalDebit, 0),
                    TotalCredit = Numerics.DecimalToString(totalCredit, 0),
                    TotalBalance = Numerics.DecimalToString(totalDebit - totalCredit, 0),
                }
            };
            return response;

        }
        private ApiResponse GetRentalAccountStatement()
        {

            var agreemnetId = Numerics.GetInt(Request.GetQueryString("agreemnetId"));
            var fromMonth = Numerics.GetInt(Request.GetQueryString("month"));
            var fromYear = Numerics.GetInt(Request.GetQueryString("year"));
            var toMonth = Numerics.GetInt(Request.GetQueryString("toMonth"));
            var toYear = Numerics.GetInt(Request.GetQueryString("toYear"));


            var fromDate = UtilityFunctionManager.GetFromDate(fromMonth, fromYear);
            var ToDate = UtilityFunctionManager.GetToDate(toMonth, toYear);

            var repo = new ReportRepository();
            var records = repo.GetRentalAccountStatement(agreemnetId, fromDate, ToDate);
            var tenant = new vw_RentAgreementsRepository().GetById(agreemnetId);
            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    Rents = records,
                    Tenant = tenant
                }
            };
            return response;

        }
        private ApiResponse GetSecuirtyPossessionAccountStatement()
        {

            var agreemnetId = Numerics.GetInt(Request.GetQueryString("agreemnetId"));
            var transactionType = (VoucherType)Numerics.GetByte(Request.GetQueryString("transactionType"));
            var repo = new ReportRepository();
            var records = repo.GetSecuirtyPossessionAccountStatement(agreemnetId, transactionType);
            var tenant = new vw_RentAgreementsRepository().GetById(agreemnetId);

            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    Rents = records,
                    Tenant = tenant
                }
            };
            return response;

        }
        private ApiResponse GetSecuirtyPossessionAccountStatement1()
        {

            var agreemnetId = Numerics.GetInt(Request.GetQueryString("agreemnetId"));
            var transactionType = (VoucherType)Numerics.GetByte(Request.GetQueryString("transactionType"));
            var fromMonth = Numerics.GetInt(Request.GetQueryString("month"));
            var fromYear = Numerics.GetInt(Request.GetQueryString("year"));
            var toMonth = Numerics.GetInt(Request.GetQueryString("toMonth"));
            var toYear = Numerics.GetInt(Request.GetQueryString("toYear"));
            var fromDate = new DateTime(fromYear, fromMonth, 1);
            var toDate = new DateTime(toYear, toMonth, DateTime.DaysInMonth(toYear, toMonth));

            var repo = new ReportRepository();
            var records = repo.GetSecuirtyPossessionAccountStatement1(agreemnetId, transactionType, fromDate, toDate);
            var tenant = new vw_RentAgreementsRepository().GetById(agreemnetId);
            var agreement = new RentAgreementRepository().GetById(agreemnetId);

            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    Challans = records,
                    Tenant = tenant,
                    Agreement = agreement
                }
            };
            return response;

        }












        //private ApiResponse GetActiveStocks()
        //{

        //    var fromdate = Convert.ToDateTime(Request.GetQueryString("date1"));
        //    var todate = Convert.ToDateTime(Request.GetQueryString("date2"));
        //    var accountId = Numerics.GetInt(Request.GetQueryString("accountId"));
        //    var repo = new ReportRepository();
        //    var records = repo.GetVehicleActiveStocks();



        //    var response = new ApiResponse
        //    {
        //        Success = true,
        //        Data = records
        //    };
        //    return response;

        //}

        private ApiResponse GetCustomerColection()
        {

            var fromdate = Convert.ToDateTime(Request.GetQueryString("date1"));
            var todate = Convert.ToDateTime(Request.GetQueryString("date2"));
            var repo = new ReportRepository();
            var records = repo.GetCustomerColection(fromdate, todate);
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
        private ApiResponse GetRecoveryOfPossessionCharges()
        {

            var month = Numerics.GetInt(Request.GetQueryString("month"));
            var year = Numerics.GetInt(Request.GetQueryString("year"));
            var blockId = Numerics.GetInt(Request.GetQueryString("blockId"));
            var transactionType = (VoucherType)(VoucherType)Numerics.GetByte(Request.GetQueryString("transactionType"));
            var repo = new ReportRepository();
            var records = repo.GetRecoveryOfPossessionCharges(month, year, blockId, transactionType);


            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    Charges = records,



                    // Tenant = tenant

                    // Tenant = tenant

                }
            };
            return response;

        }

        private ApiResponse GetRecoveryOfPossessionCharges1()
        {

            var fromMonth = Numerics.GetInt(Request.GetQueryString("month"));
            var fromYear = Numerics.GetInt(Request.GetQueryString("year"));
            var toMonth = Numerics.GetInt(Request.GetQueryString("toMonth"));
            var toYear = Numerics.GetInt(Request.GetQueryString("toYear"));
            var fromDate = new DateTime(fromYear, fromMonth, 1);
            var toDate = new DateTime(toYear, toMonth, DateTime.DaysInMonth(toYear, toMonth));
            var blockId = Numerics.GetInt(Request.GetQueryString("blockId"));
            var transactionType = (VoucherType)(VoucherType)Numerics.GetByte(Request.GetQueryString("transactionType"));
            var repo = new ReportRepository();
            var records = repo.GetRecoveryOfPossessionCharges1(fromDate, toDate, blockId, transactionType);
            var response = new ApiResponse
            {
                Success = true,
                Data = records

            };
            return response;

        }
        private ApiResponse GetDetailOfOverallBillsIssueToTenants()
        {

            var month = Numerics.GetInt(Request.GetQueryString("month"));
            var year = Numerics.GetInt(Request.GetQueryString("year"));
            var blockId = Numerics.GetInt(Request.GetQueryString("blockId"));

            var repo = new ReportRepository();
            var records = repo.GetDetailOfOverallBillsIssueToTenants(month, year, blockId);


            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    Charges = records,
                    // Tenant = tenant
                }
            };
            return response;

        }
        private ApiResponse GetRecoveryOfRent()
        {

            var month = Numerics.GetInt(Request.GetQueryString("month"));
            var year = Numerics.GetInt(Request.GetQueryString("year"));
            var blockid = Numerics.GetInt(Request.GetQueryString("blockid"));
            var repo = new ReportRepository();
            var records = repo.GetRecoveryOfRent(month, year, blockid);


            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    Charges = records,
                    // Tenant = tenant
                }
            };
            return response;

        }
        private ApiResponse GetOverallRecoveryReport()
        {

            var month = Numerics.GetInt(Request.GetQueryString("month"));
            var year = Numerics.GetInt(Request.GetQueryString("year"));
            var blockid = Numerics.GetInt(Request.GetQueryString("blockid"));
            var repo = new ReportRepository();
            var records = repo.GetOverallRecoveryReport(month, year, blockid);

            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    Charges = records,
                    // Tenant = tenant
                }
            };
            return response;

        }
        private ApiResponse GetGeneralLedger()
        {
            var date1 = Convert.ToDateTime(Request.GetQueryString("date1"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("date2"));
            var accountId = Numerics.GetInt(Request.GetQueryString("accountId"));
            var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
            var repo = new TransactionRepository();
            var openingBalance = repo.GetOpeningBalance(accountId, date1);
            var recieptTypes = new List<VoucherType> { VoucherType.VCR, VoucherType.VBR, VoucherType.VSD, VoucherType.AdvanceReceipts, VoucherType.AuctionnerPayments, VoucherType.PenaltyPayments };
            var transactions = repo.GetTransactions(accountId, date1, date2, true, branchId).Where(p => p.Debit + p.Credit > 0).OrderBy(p => p.Date).ThenByDescending(p => recieptTypes.Contains(p.TransactionType)).ThenBy(p => p.TransactionType).ThenBy(p => p.VoucherNumber).ToList();
            //transactions = transactions.OrderByDescending(p => p.TransactionType == 45).ThenByDescending(p => p.TransactionType == 46).ThenByDescending(p => p.TransactionType == 57)
            //.ThenBy(p => p.VoucherNumber).ToList();
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


        private ApiResponse GetVehicleProfile()
        {
            var date1 = Convert.ToDateTime(Request.GetQueryString("date1"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("date2"));
            var vehicleId = Numerics.GetInt(Request.GetQueryString("vehicleId"));
            var repo = new TransactionRepository();
            var reportRepo = new ReportRepository();
            var openingBalance = repo.GetVehicleOpeningBalance(vehicleId, date1);
            var transactions = repo.GetVehicleTransactions(vehicleId, date1, date2).Where(p => p.Debit + p.Credit > 0).OrderBy(p => p.Date).ThenBy(p => p.TransactionType).ThenBy(p => p.VoucherNumber).ToList();
            var info = reportRepo.GetVehicleandSaleInfo(vehicleId, 0);
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
                    Info = info
                }
            };
            return response;

        }
        private ApiResponse GetVehicleLedger()
        {
            var date1 = Convert.ToDateTime(Request.GetQueryString("date1"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("date2"));
            var saleId = Numerics.GetInt(Request.GetQueryString("saleId"));
            var repo = new VehicleSaleRepository();
            var reportRepo = new ReportRepository();
            var vehicleRepo = new vw_VehiclesRepository();
            var paymentRepo = new VehicleInstallmentPaymentRepository();
            var transRepo = new TransactionRepository();
            var sale = repo.GetById(saleId);
            var vehicleId = sale.VehicleId;
            var vehicle = vehicleRepo.GetById(vehicleId);
            var Info = reportRepo.GetVehicleandSaleInfo(vehicleId, saleId);
            var transactions = transRepo.GetVehicleAuctionnerPenaltyTransactions(vehicleId, sale.AccountId).OrderBy(p => p.Date).ThenBy(p => p.TransactionType).ThenBy(p => p.VoucherNumber).ToList();

            var payments = new List<VehicleInstallmentPayment>();
            if (sale != null)
                payments = paymentRepo.GetBySaleId(sale.Id);

            var totalDebit = transactions.Sum(p => p.Debit);
            var totalCredit = transactions.Sum(p => p.Credit);
            var records = new List<GeneralLedgerEntry>();
            var runningTotal = 0.0M;
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

                    Sale = sale,
                    Info = Info,
                    Payments = payments,
                    Records = records,
                    TotalDebit = Numerics.DecimalToString(totalDebit),
                    TotalCredit = Numerics.DecimalToString(totalCredit),
                    TotalBalance = totalDebit - totalCredit,
                }
            };
            return response;

        }
        private ApiResponse GetDetailedGeneralLedger()
        {
            var date1 = Convert.ToDateTime(Request.GetQueryString("date1"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("date2"));
            var accountId = Numerics.GetInt(Request.GetQueryString("accountId"));
            var repo = new TransactionRepository();
            var openingBalance = repo.GetOpeningBalance(accountId, date1);
            var transactions = repo.GetTransactions(accountId, date1, date2, true).Where(p => p.Debit + p.Credit > 0).OrderBy(p => p.Date).ThenBy(p => p.TransactionType).ThenBy(p => p.VoucherNumber).ToList();
            var voucherTypes = new List<int> { 1, 2, 3, 4, 25, 26, 27, 28 };
            var sales = new SaleRepository().GetSaleDetailForDetailedLedger(date1, date2, voucherTypes);
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
                    SalesDetail = sales,
                    TotalDebit = Numerics.DecimalToString(totalDebit),
                    TotalCredit = Numerics.DecimalToString(totalCredit),
                    TotalBalance = Numerics.DecimalToString(totalDebit - totalCredit + openingBalance),
                }
            };
            return response;

        }
        private ApiResponse GetNTDetailedGeneralLedger()
        {
            var date1 = Convert.ToDateTime(Request.GetQueryString("date1"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("date2"));
            var accountId = Numerics.GetInt(Request.GetQueryString("accountId"));
            var repo = new TransactionRepository();
            var openingBalance = repo.GetOpeningBalance(accountId, date1);
            var transactions = repo.GetTransactions(accountId, date1, date2, true).Where(p => p.Debit + p.Credit > 0).OrderBy(p => p.Date).ThenBy(p => p.TransactionType).ThenBy(p => p.VoucherNumber).ToList();
            var voucherTypes = new List<VoucherType> { VoucherType.GoodIssue, VoucherType.GoodReceive };
            var deliveryChallans = new SaleRepository().GetDCDetailForNTDetailedLedger(date1, date2, voucherTypes);
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
                    DCDetail = deliveryChallans,
                    TotalDebit = Numerics.DecimalToString(totalDebit),
                    TotalCredit = Numerics.DecimalToString(totalCredit),
                    TotalBalance = Numerics.DecimalToString(totalDebit - totalCredit + openingBalance),
                }
            };
            return response;

        }
        private ApiResponse GetStaticLedger()
        {
            var date1 = Convert.ToDateTime(Request.GetQueryString("date1"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("date2"));
            var accountId = Numerics.GetInt(Request.GetQueryString("accountId"));
            var transactiontype = Request.GetQueryString("transactiontype");
            var salepostedtype = Request.GetQueryString("salepostedtype");
            var voucherpostedtype = Request.GetQueryString("voucherpostedtype");
            var partnerId = Numerics.GetInt(Request.GetQueryString("partnerId"));
            var clearingType = Request.GetQueryString("clearingType");
            var sales = new SaleRepository().AsQueryable(true);
            var challans = new DeliveryChallanRepository().AsQueryable(true);

            var vouchers = new VoucherTransRepository().AsQueryable(true).Where(p => p.AccountId == accountId || p.VoucherItems.Any(q => q.AccountId == accountId));
            if (partnerId > 0)
            {

                vouchers = vouchers.Where(p => p.CompanyPartnerId == partnerId);
            }
            var bankTypes = new List<VoucherType> { VoucherType.CashPayments, VoucherType.BankPayments };
            switch (voucherpostedtype)
            {
                case "posted":
                    vouchers = vouchers.Where(p => !bankTypes.Contains(p.TransactionType) || (p.IsFinal && bankTypes.Contains(p.TransactionType)));
                    break;
                case "unposted":
                    vouchers = vouchers.Where(p => !bankTypes.Contains(p.TransactionType) || (!p.IsFinal && bankTypes.Contains(p.TransactionType)));
                    break;
                default:
                    break;
            }
            var openingBalance = new TransactionRepository().GetOpeningBalanceForStaticLedger(accountId, date1);
            //var transactions = repo.GetTransactions(accountId, date1, date2, true).Where(p => p.Debit + p.Credit > 0).OrderBy(p => p.Date).ToList();

            // var openingBalance = 25565M;
            var transactions = new List<AccountEx.CodeFirst.Models.Transaction>();
            var trans = new List<AccountEx.CodeFirst.Models.Transaction>();

            foreach (var v in vouchers)
            {

                if (v.AccountId == accountId)
                {
                    trans.AddRange(

             v.VoucherItems.Select(item => new AccountEx.CodeFirst.Models.Transaction
             {
                 AccountId = Numerics.GetInt(v.AccountId),
                 InvoiceNumber = v.InvoiceNumber,
                 VoucherNumber = v.VoucherNumber,
                 TransactionType = v.TransactionType,
                 EntryType = (byte)EntryType.MasterDetail,
                 Comments = item.Description,
                 Date = v.Date,
                 Debit = item.Debit.HasValue ? item.Debit.Value : 0,
                 Credit = item.Credit.HasValue ? item.Credit.Value : 0,

             }).ToList());
                }
                else
                {
                    var accountItems = v.VoucherItems.Where(p => p.AccountId == accountId).ToList();
                    trans.AddRange(
                        v.VoucherItems.Select(item => new AccountEx.CodeFirst.Models.Transaction
                        {
                            AccountId = item.AccountId,
                            InvoiceNumber = v.InvoiceNumber,
                            VoucherNumber = v.VoucherNumber,
                            TransactionType = v.TransactionType,
                            EntryType = (byte)EntryType.Item,
                            Comments = item.Description,
                            Date = v.Date,
                            Debit = item.Debit.HasValue ? item.Debit.Value : 0,
                            Credit = item.Credit.HasValue ? item.Credit.Value : 0,
                        }).ToList()
                        );
                }


            }
            if (salepostedtype == "withinvoice")
            {
                sales = sales.Where(p => p.AccountId == accountId);
                if (partnerId > 0)
                {

                    sales = sales.Where(p => p.CompanyPartnerId == partnerId);
                }

                var commercialVoucherTyes = new List<VoucherType>() { VoucherType.Sale, VoucherType.SaleReturn };
                if (transactiontype == "purchase")
                    commercialVoucherTyes = new List<VoucherType>() { VoucherType.Purchase, VoucherType.PurchaseReturn };
                else if (!string.IsNullOrWhiteSpace(clearingType))
                {
                    switch (clearingType)
                    {
                        case "cleared":
                            sales = sales.Where(p => p.IsCleared);
                            break;
                        case "uncleared":
                            sales = sales.Where(p => !p.IsCleared);
                            break;
                        default:
                            break;
                    }

                }

                var commercialTransactions = sales.Where(p => commercialVoucherTyes.Contains(p.TransactionType)).ToList();
                foreach (var s in commercialTransactions)
                {

                    var dt = DateTime.Now;
                    var saleTrans = new List<AccountEx.CodeFirst.Models.Transaction>
            {
                new AccountEx.CodeFirst.Models.Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = s.AccountId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte) EntryType.MasterDetail,
                    Quantity = 1,
                     Date=s.Date,
                    Debit =
                        s.TransactionType == VoucherType.Sale ||
                        s.TransactionType == VoucherType.PurchaseReturn
                            ? Numerics.GetInt(s.NetTotal)
                            : 0,
                    Credit =
                        s.TransactionType == VoucherType.SaleReturn ||
                        s.TransactionType == VoucherType.Purchase
                            ? Numerics.GetInt(s.NetTotal)
                            : 0,
                },
                new AccountEx.CodeFirst.Models.Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId =
                        s.TransactionType == VoucherType.Sale
                            ? SettingManager.SaleAccountHeadId
                            : s.TransactionType == VoucherType.Purchase
                                ? SettingManager.PurchaseAccountHeadId
                                : s.TransactionType == VoucherType.SaleReturn
                                    ? SettingManager.SaleReturnAccountHeadId
                                    : SettingManager.PurchaseReturnAccountHeadId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte) EntryType.HeadAccount,
                    Quantity = 1,
                      Date=s.Date,
                    Credit =
                        s.TransactionType == VoucherType.Sale ||
                        s.TransactionType == VoucherType.PurchaseReturn
                            ? Numerics.GetInt(s.NetTotal + s.Discount)
                            : 0,
                    Debit =
                        s.TransactionType == VoucherType.SaleReturn ||
                        s.TransactionType == VoucherType.Purchase
                            ? Numerics.GetInt(s.NetTotal + s.Discount)
                            : 0
                }
            };

                    if (Numerics.GetInt(s.Discount) > 0)
                    {
                        saleTrans.Add(new AccountEx.CodeFirst.Models.Transaction
                        {
                            InvoiceNumber = s.InvoiceNumber,
                            VoucherNumber = s.VoucherNumber,
                            AccountId = SettingManager.DiscountAccountId,
                            TransactionType = s.TransactionType,
                            Date = s.Date,
                            EntryType = (byte)EntryType.Discount,
                            Debit = s.TransactionType == VoucherType.Sale || s.TransactionType == VoucherType.PurchaseReturn
                                    ? Numerics.GetInt(s.Discount)
                                    : 0,
                            Credit =
                                s.TransactionType == VoucherType.SaleReturn || s.TransactionType == VoucherType.Purchase
                                    ? Numerics.GetInt(s.Discount)
                                    : 0
                        });


                    }
                    if (Numerics.GetInt(s.TotalFreight) > 0)
                    {
                        saleTrans.Add(new AccountEx.CodeFirst.Models.Transaction
                        {
                            InvoiceNumber = s.InvoiceNumber,
                            VoucherNumber = s.VoucherNumber,
                            AccountId = s.VehicleId,
                            TransactionType = s.TransactionType,
                            EntryType = (byte)EntryType.Discount,
                            Date = s.Date,
                            Credit = s.TransactionType == VoucherType.Sale || s.TransactionType == VoucherType.PurchaseReturn
                                    ? Numerics.GetInt(s.TotalFreight)
                                    : 0,
                            Debit =
                                s.TransactionType == VoucherType.SaleReturn || s.TransactionType == VoucherType.Purchase
                                    ? Numerics.GetInt(s.TotalFreight)
                                    : 0
                        });
                        saleTrans.Add(new AccountEx.CodeFirst.Models.Transaction
                        {
                            InvoiceNumber = s.InvoiceNumber,
                            VoucherNumber = s.VoucherNumber,
                            AccountId = SettingManager.FreightHeadId,
                            TransactionType = s.TransactionType,
                            EntryType = (byte)EntryType.Discount,
                            Date = s.Date,
                            Debit = s.TransactionType == VoucherType.Sale || s.TransactionType == VoucherType.PurchaseReturn
                                    ? Numerics.GetInt(s.TotalFreight)
                                    : 0,
                            Credit =
                                s.TransactionType == VoucherType.SaleReturn || s.TransactionType == VoucherType.Purchase
                                    ? Numerics.GetInt(s.TotalFreight)
                                    : 0
                        });
                    }
                    trans.AddRange(saleTrans);



                }

                var gstVoucherTyes = new List<VoucherType>() { VoucherType.GstSale, VoucherType.GstSaleReturn };
                var gstTransactions = sales.Where(p => gstVoucherTyes.Contains(p.TransactionType)).ToList();

                if (transactiontype == "purchase")
                    commercialVoucherTyes = new List<VoucherType>() { VoucherType.GstPurchase, VoucherType.GstPurchaseReturn };
                foreach (var s in gstTransactions)
                {

                    if (s.TransactionType == VoucherType.GstSale || s.TransactionType == VoucherType.GstSaleReturn)
                    {

                        var saleTrans = new List<AccountEx.CodeFirst.Models.Transaction>
            {
                new AccountEx.CodeFirst.Models.Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = s.AccountId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte) EntryType.MasterDetail,
                    Quantity = 1,
                    Date=s.Date,
                    Debit =
                        s.TransactionType == VoucherType.GstSale ||
                        s.TransactionType == VoucherType.GstPurchaseReturn
                            ? Numerics.GetInt(s.NetTotal)
                            : 0,
                    Credit =
                        s.TransactionType == VoucherType.GstSaleReturn ||
                        s.TransactionType == VoucherType.GstPurchase
                            ? Numerics.GetInt(s.NetTotal)
                            : 0,
                },
                new AccountEx.CodeFirst.Models.Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId =
                        s.TransactionType == VoucherType.GstSale
                            ? SettingManager.GstSaleAccountHeadId
                            : s.TransactionType == VoucherType.GstPurchase
                                ? SettingManager.GstPurchaseAccountHeadId
                                : s.TransactionType == VoucherType.GstSaleReturn
                                    ? SettingManager.GstSaleReturnAccountHeadId
                                    : SettingManager.GstPurchaseReturnAccountHeadId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte) EntryType.HeadAccount,
                    Quantity = 1,
                    Date=s.Date,
                    Credit =
                        s.TransactionType == VoucherType.GstSale ||
                        s.TransactionType == VoucherType.GstPurchaseReturn
                            ? Numerics.GetInt(s.NetTotal - s.GstAmountTotal)
                            : 0,
                    Debit =
                        s.TransactionType == VoucherType.GstSaleReturn ||
                        s.TransactionType == VoucherType.GstPurchase
                            ? Numerics.GetInt(s.NetTotal - s.GstAmountTotal)
                            : 0
                }
            };
                        if (Numerics.GetInt(s.GstAmountTotal) > 0)
                        {
                            trans.Add(new AccountEx.CodeFirst.Models.Transaction
                            {
                                InvoiceNumber = s.InvoiceNumber,
                                VoucherNumber = s.VoucherNumber,
                                AccountId = SettingManager.GstHeadId,
                                TransactionType = s.TransactionType,
                                EntryType = (byte)EntryType.Gst,
                                Date = s.Date,
                                Credit = s.TransactionType == VoucherType.GstSale || s.TransactionType == VoucherType.GstPurchaseReturn
                              ? Numerics.GetInt(s.GstAmountTotal)
                              : 0,
                                Debit =
                                    s.TransactionType == VoucherType.GstSaleReturn || s.TransactionType == VoucherType.GstPurchase
                                        ? Numerics.GetInt(s.GstAmountTotal)
                                        : 0

                            });

                        }
                        trans.AddRange(saleTrans);
                    }
                    else
                    {

                        var saleTrans = s.SaleItems.Select(item => new AccountEx.CodeFirst.Models.Transaction
                        {
                            AccountId = s.AccountId,
                            Quantity = 1,
                            Price = item.Amount,
                            InvoiceNumber = item.InvoiceNumber,
                            VoucherNumber = item.VoucherNumber,
                            TransactionType = s.TransactionType,
                            Date = s.Date,
                            EntryType = (byte)EntryType.MasterDetail,
                            Credit = s.TransactionType == VoucherType.GstPurchase ? item.Amount : 0,
                            Debit = s.TransactionType == VoucherType.GstPurchaseReturn ? item.Amount : 0,

                        }).ToList();


                        //Credit  Amount to Duties Exp 
                        trans.AddRange(s.SaleItems.Select(item => new AccountEx.CodeFirst.Models.Transaction
                        {
                            AccountId = SettingManager.DutiesExpenseHeadId,
                            Quantity = 1,
                            Price = Numerics.GetDecimal(item.AIT + item.CD + item.RD + item.SED + item.Freight + item.FWD + item.GSTAmount + item.Others),
                            InvoiceNumber = item.InvoiceNumber,
                            VoucherNumber = item.VoucherNumber,
                            TransactionType = s.TransactionType,
                            EntryType = (byte)EntryType.HeadAccount,
                            Date = s.Date,
                            Credit = s.TransactionType == VoucherType.GstPurchase ? Numerics.GetDecimal(item.AIT + item.CD + item.RD + item.SED + item.Freight + item.FWD + item.GSTAmount + item.Others) : 0,
                            Debit = s.TransactionType == VoucherType.GstPurchaseReturn ? Numerics.GetDecimal(item.AIT + item.CD + item.RD + item.SED + item.Freight + item.FWD + item.GSTAmount + item.Others) : 0
                        }).ToList());


                        //Debit  to Gst
                        trans.AddRange(s.SaleItems.Where(p => p.GSTAmount > 0).Select(item => new AccountEx.CodeFirst.Models.Transaction
                        {
                            AccountId = SettingManager.GstHeadId,
                            Quantity = 1,
                            Price = item.GSTAmount,
                            InvoiceNumber = item.InvoiceNumber,
                            VoucherNumber = item.VoucherNumber,
                            TransactionType = s.TransactionType,
                            EntryType = (byte)EntryType.Item,
                            Date = s.Date,
                            Debit = s.TransactionType == VoucherType.GstPurchase ? item.GSTAmount : 0,
                            Credit = s.TransactionType == VoucherType.GstPurchaseReturn ? item.GSTAmount : 0,


                        }).ToList());


                        //Debit  NetTotal to Purchase Account
                        trans.AddRange(s.SaleItems.Select(item => new AccountEx.CodeFirst.Models.Transaction
                        {
                            AccountId = s.TransactionType == VoucherType.GstPurchase ? SettingManager.GstPurchaseAccountHeadId
                            : SettingManager.GstPurchaseReturnAccountHeadId,
                            Quantity = 1,
                            Price = item.NetAmount + item.GSTAmount,
                            InvoiceNumber = item.InvoiceNumber,
                            VoucherNumber = item.VoucherNumber,
                            TransactionType = s.TransactionType,
                            EntryType = (byte)EntryType.Item,
                            Date = s.Date,
                            Debit = s.TransactionType == VoucherType.GstPurchase ? item.NetAmount : 0,
                            Credit = s.TransactionType == VoucherType.GstPurchaseReturn ? item.NetAmount : 0,

                        }).ToList());
                        trans.AddRange(saleTrans);
                    }


                }
            }
            else
            {

                challans = new DeliveryChallanRepository().AsQueryable(true).Where(p => p.AccountId == accountId);

                if (partnerId > 0)
                {

                    challans = challans.Where(p => p.CompanyPartnerId == partnerId);
                }
                var commercialVoucherTyes = new List<VoucherType>() { VoucherType.Sale, VoucherType.SaleReturn };
                if (transactiontype == "purchase")
                {
                    challans = challans.Where(p => p.TransactionType == VoucherType.GoodReceive);
                    commercialVoucherTyes = new List<VoucherType>() { VoucherType.Purchase, VoucherType.PurchaseReturn };
                }
                else
                    challans = challans.Where(p => p.TransactionType == VoucherType.GoodIssue);
                var commercialChallans = challans.Where(p => commercialVoucherTyes.Contains(p.InvoiceTransactionType)).ToList();
                foreach (var s in commercialChallans)
                {

                    var dt = DateTime.Now;
                    var saleTrans = new List<AccountEx.CodeFirst.Models.Transaction>
            {
                new AccountEx.CodeFirst.Models.Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId =Numerics.GetInt(s.AccountId),
                    TransactionType = s.TransactionType,
                    EntryType = (byte) EntryType.MasterDetail,
                    Quantity = 1,
                     Date=s.Date,
                    Debit =
                        s.InvoiceTransactionType == VoucherType.Sale ||
                        s.InvoiceTransactionType == VoucherType.PurchaseReturn
                            ? Numerics.GetInt(s.NetTotal)
                            : 0,
                    Credit =
                        s.InvoiceTransactionType == VoucherType.SaleReturn ||
                        s.InvoiceTransactionType == VoucherType.Purchase
                            ? Numerics.GetInt(s.NetTotal)
                            : 0,
                },
                new AccountEx.CodeFirst.Models.Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId =
                        s.InvoiceTransactionType == VoucherType.Sale
                            ? SettingManager.SaleAccountHeadId
                            : s.InvoiceTransactionType == VoucherType.Purchase
                                ? SettingManager.PurchaseAccountHeadId
                                : s.InvoiceTransactionType == VoucherType.SaleReturn
                                    ? SettingManager.SaleReturnAccountHeadId
                                    : SettingManager.PurchaseReturnAccountHeadId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte) EntryType.HeadAccount,
                    Quantity = 1,
                      Date=s.Date,
                    Credit =
                        s.InvoiceTransactionType == VoucherType.Sale ||
                        s.InvoiceTransactionType == VoucherType.PurchaseReturn
                            ? Numerics.GetInt(s.NetTotal + s.Discount)
                            : 0,
                    Debit =
                        s.InvoiceTransactionType == VoucherType.SaleReturn ||
                        s.InvoiceTransactionType == VoucherType.Purchase
                            ? Numerics.GetInt(s.NetTotal + s.Discount)
                            : 0
                }
            };

                    if (Numerics.GetInt(s.Discount) > 0)
                    {
                        saleTrans.Add(new AccountEx.CodeFirst.Models.Transaction
                        {
                            InvoiceNumber = s.InvoiceNumber,
                            VoucherNumber = s.VoucherNumber,
                            AccountId = SettingManager.DiscountAccountId,
                            TransactionType = s.TransactionType,
                            Date = s.Date,
                            EntryType = (byte)EntryType.Discount,
                            Debit = s.InvoiceTransactionType == VoucherType.Sale || s.InvoiceTransactionType == VoucherType.PurchaseReturn
                                    ? Numerics.GetInt(s.Discount)
                                    : 0,
                            Credit =
                                s.InvoiceTransactionType == VoucherType.SaleReturn || s.InvoiceTransactionType == VoucherType.Purchase
                                    ? Numerics.GetInt(s.Discount)
                                    : 0
                        });


                    }
                    if (Numerics.GetInt(s.TotalFreight) > 0)
                    {
                        saleTrans.Add(new AccountEx.CodeFirst.Models.Transaction
                        {
                            InvoiceNumber = s.InvoiceNumber,
                            VoucherNumber = s.VoucherNumber,
                            AccountId = s.VehicleId,
                            TransactionType = s.TransactionType,
                            EntryType = (byte)EntryType.Discount,
                            Date = s.Date,
                            Credit = s.InvoiceTransactionType == VoucherType.Sale || s.InvoiceTransactionType == VoucherType.PurchaseReturn
                                    ? Numerics.GetInt(s.TotalFreight)
                                    : 0,
                            Debit =
                                s.InvoiceTransactionType == VoucherType.SaleReturn || s.InvoiceTransactionType == VoucherType.Purchase
                                    ? Numerics.GetInt(s.TotalFreight)
                                    : 0
                        });
                        saleTrans.Add(new AccountEx.CodeFirst.Models.Transaction
                        {
                            InvoiceNumber = s.InvoiceNumber,
                            VoucherNumber = s.VoucherNumber,
                            AccountId = SettingManager.FreightHeadId,
                            TransactionType = s.TransactionType,
                            EntryType = (byte)EntryType.Discount,
                            Date = s.Date,
                            Debit = s.InvoiceTransactionType == VoucherType.Sale || s.InvoiceTransactionType == VoucherType.PurchaseReturn
                                    ? Numerics.GetInt(s.TotalFreight)
                                    : 0,
                            Credit =
                                s.InvoiceTransactionType == VoucherType.SaleReturn || s.InvoiceTransactionType == VoucherType.Purchase
                                    ? Numerics.GetInt(s.TotalFreight)
                                    : 0
                        });
                    }
                    trans.AddRange(saleTrans);



                }

                var gstVoucherTyes = new List<VoucherType>() { VoucherType.GstSale, VoucherType.GstSaleReturn };


                if (transactiontype == "purchase")
                    commercialVoucherTyes = new List<VoucherType>() { VoucherType.GstPurchase, VoucherType.GstPurchaseReturn };
                var gstChallans = challans.Where(p => gstVoucherTyes.Contains(p.InvoiceTransactionType)).ToList();
                foreach (var s in gstChallans)
                {

                    if (s.InvoiceTransactionType == VoucherType.GstSale || s.InvoiceTransactionType == VoucherType.GstSaleReturn)
                    {

                        var saleTrans = new List<AccountEx.CodeFirst.Models.Transaction>
            {
                new AccountEx.CodeFirst.Models.Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = Numerics.GetInt(s.AccountId),
                    TransactionType = s.TransactionType,
                    EntryType = (byte) EntryType.MasterDetail,
                    Quantity = 1,
                    Date=s.Date,
                    Debit =
                        s.InvoiceTransactionType == VoucherType.GstSale ||
                        s.InvoiceTransactionType == VoucherType.GstPurchaseReturn
                            ? Numerics.GetInt(s.NetTotal)
                            : 0,
                    Credit =
                        s.InvoiceTransactionType == VoucherType.GstSaleReturn ||
                        s.InvoiceTransactionType == VoucherType.GstPurchase
                            ? Numerics.GetInt(s.NetTotal)
                            : 0,
                },
                new AccountEx.CodeFirst.Models.Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId =
                        s.TransactionType == VoucherType.GstSale
                            ? SettingManager.GstSaleAccountHeadId
                            : s.InvoiceTransactionType == VoucherType.GstPurchase
                                ? SettingManager.GstPurchaseAccountHeadId
                                : s.InvoiceTransactionType == VoucherType.GstSaleReturn
                                    ? SettingManager.GstSaleReturnAccountHeadId
                                    : SettingManager.GstPurchaseReturnAccountHeadId,
                    TransactionType = s.TransactionType,
                    EntryType = (byte) EntryType.HeadAccount,
                    Quantity = 1,
                    Date=s.Date,
                    Credit =
                        s.InvoiceTransactionType == VoucherType.GstSale ||
                        s.InvoiceTransactionType == VoucherType.GstPurchaseReturn
                            ? Numerics.GetInt(s.NetTotal - s.GstAmountTotal)
                            : 0,
                    Debit =
                        s.InvoiceTransactionType == VoucherType.GstSaleReturn ||
                        s.InvoiceTransactionType == VoucherType.GstPurchase
                            ? Numerics.GetInt(s.NetTotal - s.GstAmountTotal)
                            : 0
                }
            };
                        if (Numerics.GetInt(s.GstAmountTotal) > 0)
                        {
                            trans.Add(new AccountEx.CodeFirst.Models.Transaction
                            {
                                InvoiceNumber = s.InvoiceNumber,
                                VoucherNumber = s.VoucherNumber,
                                AccountId = SettingManager.GstHeadId,
                                TransactionType = s.TransactionType,
                                EntryType = (byte)EntryType.Gst,
                                Date = s.Date,
                                Credit = s.InvoiceTransactionType == VoucherType.GstSale || s.InvoiceTransactionType == VoucherType.GstPurchaseReturn
                              ? Numerics.GetInt(s.GstAmountTotal)
                              : 0,
                                Debit =
                                    s.InvoiceTransactionType == VoucherType.GstSaleReturn || s.InvoiceTransactionType == VoucherType.GstPurchase
                                        ? Numerics.GetInt(s.GstAmountTotal)
                                        : 0

                            });

                        }
                        trans.AddRange(saleTrans);
                    }
                    else
                    {

                        var saleTrans = s.DCItems.Select(item => new AccountEx.CodeFirst.Models.Transaction
                        {
                            AccountId = Numerics.GetInt(s.AccountId),
                            Quantity = 1,
                            Price = item.Amount,
                            InvoiceNumber = item.InvoiceNumber,
                            VoucherNumber = item.VoucherNumber,
                            TransactionType = s.TransactionType,
                            Date = s.Date,
                            EntryType = (byte)EntryType.MasterDetail,
                            Credit = s.InvoiceTransactionType == VoucherType.GstPurchase ? item.Amount : 0,
                            Debit = s.InvoiceTransactionType == VoucherType.GstPurchaseReturn ? item.Amount : 0,

                        }).ToList();


                        //Credit  Amount to Duties Exp 
                        trans.AddRange(s.DCItems.Select(item => new AccountEx.CodeFirst.Models.Transaction
                        {
                            AccountId = SettingManager.DutiesExpenseHeadId,
                            Quantity = 1,
                            Price = Numerics.GetDecimal(item.GSTAmount),
                            InvoiceNumber = item.InvoiceNumber,
                            VoucherNumber = item.VoucherNumber,
                            TransactionType = s.TransactionType,
                            EntryType = (byte)EntryType.HeadAccount,
                            Date = s.Date,
                            Credit = s.InvoiceTransactionType == VoucherType.GstPurchase ? Numerics.GetDecimal(item.GSTAmount) : 0,
                            Debit = s.InvoiceTransactionType == VoucherType.GstPurchaseReturn ? Numerics.GetDecimal(item.GSTAmount) : 0
                        }).ToList());


                        //Debit  to Gst
                        trans.AddRange(s.DCItems.Where(p => p.GSTAmount > 0).Select(item => new AccountEx.CodeFirst.Models.Transaction
                        {
                            AccountId = SettingManager.GstHeadId,
                            Quantity = 1,
                            Price = item.GSTAmount,
                            InvoiceNumber = item.InvoiceNumber,
                            VoucherNumber = item.VoucherNumber,
                            TransactionType = s.TransactionType,
                            EntryType = (byte)EntryType.Item,
                            Date = s.Date,
                            Debit = s.InvoiceTransactionType == VoucherType.GstPurchase ? item.GSTAmount : 0,
                            Credit = s.InvoiceTransactionType == VoucherType.GstPurchaseReturn ? item.GSTAmount : 0,


                        }).ToList());


                        //Debit  NetTotal to Purchase Account
                        trans.AddRange(s.DCItems.Select(item => new AccountEx.CodeFirst.Models.Transaction
                        {
                            AccountId = s.TransactionType == VoucherType.GstPurchase ? SettingManager.GstPurchaseAccountHeadId
                            : SettingManager.GstPurchaseReturnAccountHeadId,
                            Quantity = 1,
                            Price = item.NetAmount + item.GSTAmount,
                            InvoiceNumber = item.InvoiceNumber,
                            VoucherNumber = item.VoucherNumber,
                            TransactionType = s.TransactionType,
                            EntryType = (byte)EntryType.Item,
                            Date = s.Date,
                            Debit = s.InvoiceTransactionType == VoucherType.GstPurchase ? item.NetAmount : 0,
                            Credit = s.InvoiceTransactionType == VoucherType.GstPurchaseReturn ? item.NetAmount : 0,

                        }).ToList());
                        trans.AddRange(saleTrans);
                    }


                }



            }
            var records = new List<GeneralLedgerEntry>();
            trans = trans.Where(p => p.AccountId == accountId).ToList();
            var cr = trans.Where(p => p.Date < date1).Sum(p => (decimal?)p.Credit);
            var dr = trans.Where(p => p.Date < date1).Sum(p => (decimal?)p.Debit);
            if (!cr.HasValue) cr = 0;
            if (!dr.HasValue) dr = 0;
            var balance = dr.Value - cr.Value;
            openingBalance += balance;
            var runningTotal = openingBalance;
            trans = trans.Where(p => p.Date >= date1 && p.Date <= date2).OrderBy(p => p.Date).ThenBy(p => p.TransactionType).ThenBy(p => p.VoucherNumber).ToList();
            foreach (var item in trans)
            {
                runningTotal += item.Debit - item.Credit;
                var entry = new GeneralLedgerEntry(item);
                entry.Balance = Numerics.DecimalToString(runningTotal);
                records.Add(entry);
            }

            var totalDebit = trans.Sum(p => p.Debit);
            var totalCredit = trans.Sum(p => p.Credit);

            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    OpeningBalance = Numerics.DecimalToString(openingBalance),
                    Records = records, //transactions.Select(p => new GeneralLedgerEntry(p)).ToList(),
                    Sales = salepostedtype == "withinvoice" ? sales.ToList() : null,
                    Challans = salepostedtype != "withinvoice" ? challans.ToList() : null,
                    TotalDebit = Numerics.DecimalToString(totalDebit),
                    TotalCredit = Numerics.DecimalToString(totalCredit),
                    TotalBalance = Numerics.DecimalToString(totalDebit - totalCredit + openingBalance),
                }
            };
            return response;

        }
        private ApiResponse GetVoucherList()
        {
            var date1 = Convert.ToDateTime(Request.GetQueryString("date1"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("date2"));
            var accountId = Numerics.GetInt(Request.GetQueryString("accountId"));
            var voucherslist = !string.IsNullOrWhiteSpace(Request.GetQueryString("vouchertype"))
                ? Request.GetQueryString("vouchertype").Split(',').ToList()
                : null;

            var vouchers = new List<VoucherType>() { VoucherType.PurchaseReturn, VoucherType.CashReceipts, VoucherType.CashPayments, VoucherType.BankReceipts, VoucherType.BankPayments, VoucherType.TransferVoucher };
            if (voucherslist != null) vouchers = voucherslist.Select(p => (VoucherType)Numerics.GetByte(p)).ToList();


            var data = ReportManager.GetVoucherList(date1, date2, vouchers).OrderBy(p => p.Date);

            var response = new ApiResponse
            {
                Success = true,
                Data = data
            };
            return response;
        }
        private ApiResponse GetOrderListing()
        {
            var data = OrderManager.GetOrderListing();
            var response = new ApiResponse
            {
                Success = true,
                Data = data
            };
            return response;
        }
        private ApiResponse GetOrderByStatus()
        {
            var date1 = Convert.ToDateTime(Request.GetQueryString("date1"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("date2"));
            //var accountId = Numerics.GetInt(Request.GetQueryString("accountId"));
            var vouchertype = Numerics.GetInt(Request.GetQueryString("voucherType"));
            var data = new OrderBookingRepository().GetOrderByStatusesSP(date1, date2, vouchertype);
            var response = new ApiResponse
            {
                Success = true,
                Data = data
            };
            return response;
        }
        private ApiResponse GetProductLedger()
        {
            var date1 = Convert.ToDateTime(Request.GetQueryString("date1"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("date2"));
            var accountId = Numerics.GetInt(Request.GetQueryString("accountId"));
            var repo = new ReportRepository();
            var product = new AccountDetailRepository().GetByAccountId(accountId);
            var openingBalance = Numerics.GetDecimal(product.Quantity);
            var transactions = repo.GetProductLedger(date1, date2, accountId);
            var totalstockin = transactions.Sum(p => p.StockIn);
            var totalstockout = transactions.Sum(p => p.StockOut);

            var runningTotal = openingBalance;
            foreach (var item in transactions)
            {
                runningTotal += item.StockIn - item.StockOut;
                item.Balance1 = Numerics.DecimalToString(runningTotal);
                item.Date1 = item.Date.ToString(AppSetting.DateFormat);

            }
            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    OpeningBalance = Numerics.DecimalToString(openingBalance),
                    Records = transactions.OrderBy(p => p.Date), //transactions.Select(p => new GeneralLedgerEntry(p)).ToList(),
                    TotalStockIn = Numerics.DecimalToString(totalstockin),
                    TotalStockOut = Numerics.DecimalToString(totalstockout),
                    TotalBalance = Numerics.DecimalToString((openingBalance + totalstockin) - totalstockout),
                }
            };
            return response;

        }
        private ApiResponse GetAccountBalances()
        {
            ApiResponse response;
            var setting = new FormSettingRepository().GetFormSettingByVoucherType("Recovery");
            var queryString = Request.RequestUri.ParseQueryString();
            var date1 = DateTime.Parse(queryString["date1"]);
            var date2 = DateTime.Parse(queryString["date2"]);
            var accountId = Numerics.GetInt(queryString["accountId"]);
            var accounts = new AccountRepository().GetChildren(accountId);
            var accountIds = new AccountRepository().GetLeafAccounts(accountId).Select(p => p.Id).ToList();
            var openingBalances = new TransactionRepository().GetOpeningBalances(accountIds, date1);
            var result = new List<AccountBalance>();
            foreach (var item in accounts)
            {
                var transRepo = new TransactionRepository();
                var transactions = transRepo.GetTrailBalance(item.Id, date1, date2, true);
                var list = transactions.Select(p => new TrialBalanceLine()
                {
                    AccountTitle = p.AccountTitle,
                    OpeningBalance = openingBalances.Any(q => q.AccountId == p.AccountId) ? openingBalances.FirstOrDefault(q => q.AccountId == p.AccountId).Balance : 0.0M,
                    Debit = Numerics.DecimalToString(p.Debit, ""),
                    Credit = Numerics.DecimalToString(p.Credit, ""),
                    Balance = Numerics.Sum((openingBalances.Any(q => q.AccountId == p.AccountId) ? openingBalances.FirstOrDefault(q => q.AccountId == p.AccountId).Balance : 0.0M), p.Debit, p.Credit)
                    //Balance = Numerics.GetDecimal(p.Debit - p.Credit)

                }).ToList();

                list = list.Where(p => p.Debit != "0" || p.Credit != "0" || p.OpeningBalance != 0 || p.Balance != 0).ToList();

                var sumDebit = Numerics.GetDecimal(transactions.Sum(p => (decimal?)p.Debit));
                var sumCredit = Numerics.GetDecimal(transactions.Sum(p => (decimal?)p.Credit));
                var totalDebit = Numerics.DecimalToString(sumDebit, "");
                var totalCredit = Numerics.DecimalToString(sumCredit, "");
                //var difference = sumDebit + sumCredit;
                var difference = list.Sum(p => (decimal?)p.Balance);
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
        private ApiResponse GetSalePurchaseReport()
        {
            var date1 = Convert.ToDateTime(Request.GetQueryString("date1"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("date2"));
            var accountId = Numerics.GetInt(Request.GetQueryString("accountId"));
            var voutype = Numerics.GetInt(Request.GetQueryString("voutype"));
            var repo = new ReportRepository();
            //var product = new AccountDetailRepository().GetByAccountId(accountId);
            var openingBalance = 0M;
            var transactions = repo.GetSalePurchaseReport(date1, date2, accountId, voutype);
            transactions = transactions.OrderBy(p => p.ItemName).ThenBy(p => p.Date).ToList();

            var totalamount = transactions.Sum(p => p.Amount);
            var totaldiscountamount = transactions.Sum(p => p.DiscountAmount);
            var totalgstamount = transactions.Sum(p => p.GSTAmount);
            var totalnetamount = transactions.Sum(p => p.NetAmount);
            var totalquantity = transactions.Sum(p => p.Quantity);

            var runningTotal = openingBalance;
            foreach (var item in transactions)
            {
                runningTotal += item.NetAmount;
                item.Balance1 = Numerics.DecimalToString(runningTotal);
                item.Date1 = item.Date.ToString(AppSetting.DateFormat);

            }
            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    OpeningBalance = Numerics.DecimalToString(openingBalance),
                    Records = transactions, //transactions.Select(p => new GeneralLedgerEntry(p)).ToList(),
                    TotalAmount = Numerics.DecimalToString(totalamount),
                    TotalDiscount = Numerics.DecimalToString(totaldiscountamount),
                    TotalGSTAmount = Numerics.DecimalToString(totalgstamount),
                    TotalNetAmount = Numerics.DecimalToString(totalnetamount),
                    TotalBalance = Numerics.DecimalToString(totalnetamount),
                    TotalQuantity = Numerics.DecimalToString(totalquantity),
                }
            };
            return response;

        }
        private ApiResponse GetPartyTrans()
        {
            var date1 = Convert.ToDateTime(Request.GetQueryString("date1"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("date2"));
            var accountId = Numerics.GetInt(Request.GetQueryString("accountId"));
            var salesmanId = Numerics.GetInt(Request.GetQueryString("SalesmanId"));
            var areaId = Numerics.GetInt(Request.GetQueryString("areaId"));
            var groupId = Numerics.GetInt(Request.GetQueryString("groupId"));
            var voutype = (VoucherType)Numerics.GetInt(Request.GetQueryString("voutype"));
            var repo = new ReportRepository();
            //var product = new AccountDetailRepository().GetByAccountId(accountId);
            var openingBalance = 0M;
            // Fourth parameter is used for Area in Usman Bros
            var transactions = new SaleRepository().GetSales(date1, date2, accountId, voutype, areaId, groupId, salesmanId);
            //transactions = transactions.OrderBy(p => p.AccountCode).ThenBy(p => p.Date).ToList();
            var totalamount = transactions.Sum(p => p.GrossTotal);
            var totaldiscountamount = transactions.Sum(p => p.Discount);
            var totalnetamount = transactions.Sum(p => p.NetTotal);

            var runningTotal = openingBalance;
            foreach (var item in transactions)
            {
                runningTotal += item.NetTotal;
                //item.Balance1 = Numerics.DecimalToString(runningTotal);
                //item.Date1 = item.Date.ToString(AppSetting.DateFormat);

            }
            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    OpeningBalance = Numerics.DecimalToString(openingBalance),
                    Records = transactions.OrderBy(p => p.Date), //transactions.Select(p => new GeneralLedgerEntry(p)).ToList(),
                    TotalAmount = Numerics.DecimalToString(totalamount),
                    TotalDiscount = Numerics.DecimalToString(totaldiscountamount),
                    TotalNetAmount = Numerics.DecimalToString(totalnetamount),
                    TotalBalance = Numerics.DecimalToString(totalnetamount),
                }
            };
            return response;

        }//GetLessDetail
        private ApiResponse GetLabourReport()
        {
            var date1 = Convert.ToDateTime(Request.GetQueryString("date1"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("date2"));
            var employeeId = Numerics.GetInt(Request.GetQueryString("employeeId"));

            // Define the base predicate without employeeId condition
            Expression<Func<WPItemWithParentDetail, bool>> basePredicate = p =>
                EntityFunctions.TruncateTime(p.Date) >= EntityFunctions.TruncateTime(date1) &&
                EntityFunctions.TruncateTime(p.Date) <= EntityFunctions.TruncateTime(date2);

            // Add employeeId condition only if it's not null or 0
            if (employeeId != null && employeeId != 0)
            {
                basePredicate = Expression.Lambda<Func<WPItemWithParentDetail, bool>>(
                    Expression.AndAlso(
                        basePredicate.Body,
                        Expression.Equal(
                            Expression.PropertyOrField(basePredicate.Parameters[0], "ItemId"),
                            Expression.Constant(employeeId)
                        )
                    ),
                    basePredicate.Parameters
                );
            }

            var transactions = LaboursManager.GetWithParentDetail(basePredicate);
            var totalamount = transactions.Sum(p => p.Amount);
            var totalqty = transactions.Sum(p => p.Quantity);

            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    Records = transactions.OrderBy(p => p.Date),
                    TotalAmount = Numerics.DecimalToString(totalamount),
                    TotalQuantity = Numerics.DecimalToString(totalqty),
                }
            };
            return response;

        }//GetLessDetail


        private ApiResponse GetVatRegister()
        {
            var date1 = Convert.ToDateTime(Request.GetQueryString("date1"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("date2"));
            var salesmanIds = Request.GetQueryString("salesmanIds");
            var includeStockTransfer = Numerics.GetInt(Request.GetQueryString("includeStockTransfer"));
            var repo = new ReportRepository();
            //var product = new AccountDetailRepository().GetByAccountId(accountId);
            // Fourth parameter is used for Area in Usman Bros
            var transactions = ReportManager.GetVatRegister(date1, date2, salesmanIds,includeStockTransfer);

            var response = new ApiResponse
            {
                Success = true,
                Data = transactions
            };
            return response;

        }//GetLessDetail

        private ApiResponse GetLessDetail()
        {
            var fromDate = Convert.ToDateTime(Request.GetQueryString("FromDate"));
            var toDate = Convert.ToDateTime(Request.GetQueryString("ToDate"));
            var reporttype = Numerics.GetInt(Request.GetQueryString("ReportType"));
            var parentAccountId = Numerics.GetInt(Request.GetQueryString("ParentAccountId"));
            var data = new TransactionRepository().GetLeadAndSampling(fromDate, toDate, reporttype, parentAccountId);

            var response = new ApiResponse
            {
                Success = true,
                Data = data
            };
            return response;
        }
        private ApiResponse SaleReportByAreaDateRange()
        {
            var fromDate = Convert.ToDateTime(Request.GetQueryString("FromDate"));
            var toDate = Convert.ToDateTime(Request.GetQueryString("ToDate"));
            var accountId = Numerics.GetInt(Request.GetQueryString("AccountId"));
            var data = new TransactionRepository().GetSaleReportByAreaDateRange(fromDate, toDate, accountId);

            var response = new ApiResponse
            {
                Success = true,
                Data = data
            };
            return response;
        }
        private ApiResponse GetAreaWiseSaleReport()
        {
            var fromDate = Convert.ToDateTime(Request.GetQueryString("FromDate"));
            var toDate = Convert.ToDateTime(Request.GetQueryString("ToDate"));
            var accountId = Numerics.GetInt(Request.GetQueryString("AccountId"));
            var data = new TransactionRepository().GetSaleReportByArea(fromDate, toDate, accountId);

            var response = new ApiResponse
            {
                Success = true,
                Data = data
            };
            return response;
        }

        public ApiResponse GetUsmanBrosCustomers()
        {
            var accountId = Numerics.GetInt(Request.GetQueryString("AccountId"));
            var data = new TransactionRepository().GetUsmanBrosCustomers(accountId);
            var response = new ApiResponse
            {
                Success = true,
                Data = data
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
        private ApiResponse GetProfitLossFormat1()
        {

            var date1 = Convert.ToDateTime(Request.GetQueryString("date1"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("date2"));
            var isBeforeClosing = Convert.ToBoolean(QueryString["isBeforeClosing"]);
            // var openingstock = Numerics.GetInt(Request.GetQueryString("OpeningStock"));
            // var voutype = Numerics.GetInt(Request.GetQueryString("voutype"));
            var repo = new ReportRepository();
            var profitLoss = ReportManager.GetProfitLossFormat1(date1, date2, isBeforeClosing);
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

        private ApiResponse GetPeriodicBalances()
        {
            var date1 = Convert.ToDateTime(Request.GetQueryString("date1"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("date2"));
            var openingstock = Numerics.GetInt(Request.GetQueryString("OpeningStock"));
            var voutype = Numerics.GetInt(Request.GetQueryString("voutype"));
            var repo = new ReportRepository();
            //var product = new AccountDetailRepository().GetByAccountId(accountId);

            var transactions = repo.GetPeriodicBalancesSP(date1, date2);

            //var groupdata = transactions.GroupBy(p => p.Head).Select(p => new
            //{
            //    Head = p.Key,
            //    HeadTotal = p.Sum(q => q.Amount),
            //    ControlHeads = p.ToList().GroupBy(q => q.ControlHead).Select(q => new
            //    {
            //        ControlHead = q.Key,
            //        ControlHeadTotal = q.Sum(r => r.Amount),
            //        SubHeads = q.ToList()
            //    }).ToList()
            //}).ToList();


            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    Records = transactions
                }
            };
            return response;

        }
        private ApiResponse GetBalanceSheet()
        {
            var date1 = Convert.ToDateTime(Request.GetQueryString("date1"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("date2"));
            var openingstock = Numerics.GetInt(Request.GetQueryString("OpeningStock"));
            var voutype = Numerics.GetInt(Request.GetQueryString("voutype"));
            var repo = new ReportRepository();
            //var product = new AccountDetailRepository().GetByAccountId(accountId);
            //var accounts = new AccountRepository().GetLeafAccounts(SettingManager.ExpensesHeadId).ToDictionary(p => p.Id, q => q.Name);
            var transactions = repo.GetBalanceSheet(date1, date2, SettingManager.ExpensesHeadId);
            var groupdata = transactions.GroupBy(p => p.Head).Select(p => new
            {
                Head = p.Key,
                HeadTotal = p.Sum(q => q.Amount),
                ControlHeads = p.ToList().GroupBy(q => q.ControlHead).Select(q => new
                {
                    ControlHead = q.Key,
                    ControlHeadTotal = q.Sum(r => r.Amount),
                    SubHeads = q.ToList()
                }).ToList()
            }).ToList();


            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    Records = groupdata



                }
            };
            return response;

        }
        private ApiResponse GetCustomerAging()
        {
            var date1 = Convert.ToDateTime(Request.GetQueryString("date1"));

            var accountId = Numerics.GetInt(Request.GetQueryString("accountId"));
            var repo = new ReportRepository();
            //var product = new AccountDetailRepository().GetByAccountId(accountId);

            var transactions = repo.GetCustomerAging(date1, accountId);

            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    Records = transactions,
                    CurrentTotal = Numerics.DecimalToString(transactions.Sum(p => p.Current)),
                    Day7Total = Numerics.DecimalToString(transactions.Sum(p => p.Day7)),
                    Day15Total = Numerics.DecimalToString(transactions.Sum(p => p.Day15)),
                    Day30Total = Numerics.DecimalToString(transactions.Sum(p => p.Day30)),
                    Day60Total = Numerics.DecimalToString(transactions.Sum(p => p.Day60)),
                    Day90Total = Numerics.DecimalToString(transactions.Sum(p => p.Day90)),
                    Day120Total = Numerics.DecimalToString(transactions.Sum(p => p.Day120)),



                }
            };
            return response;

        }

        private ApiResponse GetPharmacyStockWithNote()
        {
            var date1 = Convert.ToDateTime(Request.GetQueryString("fromdate"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("todate"));
            var accountId = Numerics.GetInt(Request.GetQueryString("accountId"));
            var transactions = new TransactionRepository().GetStock(date1, date2, accountId);
            var medicineIds = transactions.Select(p => p.AccountId).Distinct().ToList();
            var medicines = new AccountDetailRepository().AsQueryable().Where(p => medicineIds.Contains(p.AccountId)).Select(p => new
            {
                p.AccountId,
                p.Generic,
                p.Brand,
                p.MedicineNote
            }).ToList();
            //.Where(p => !string.IsNullOrWhiteSpace(p.GroupName)).ToList();

            //foreach (var item in transactions)
            //{
            //    item.GroupName = item.GroupName.Trim();
            //}
            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    Stocks = transactions,
                    Medicines = medicines
                }
            };
            return response;
        }
        private ApiResponse GetPharmacyStock()
        {
            var date1 = Convert.ToDateTime(Request.GetQueryString("fromdate"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("todate"));
            var accountId = Numerics.GetInt(Request.GetQueryString("accountId"));
            var transactions = new TransactionRepository().GetStock(date1, date2, accountId);
            //.Where(p => !string.IsNullOrWhiteSpace(p.GroupName)).ToList();

            //foreach (var item in transactions)
            //{
            //    item.GroupName = item.GroupName.Trim();
            //}
            var response = new ApiResponse
            {
                Success = true,
                Data = transactions
            };
            return response;
        }
        private JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "Id", "Name", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            search = search.ToLower();
            var module = Numerics.GetByte(queryString["module"]);
            var records = ReportManager.GetReports(module);
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                    p.Name.ToLower().Contains(search)
                    ).ToList();
            var orderedList = filteredList.OrderBy(p => p.SequenceNumber);
            //if (colIndex < coloumns.Length && coloumns[colIndex] + "" != "")
            //{
            //    var sortDir = queryString["sSortDir_0"];
            //    orderedList = sortDir == "asc" ? filteredList.OrderBy(coloumns[colIndex]) :
            //        filteredList.OrderByDescending(coloumns[colIndex]);
            //}
            var sb = new StringBuilder();
            sb.Clear();

            var rs = new JQueryResponse();
            var sn = 0;
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                var data = new List<string>();
                data.Add(++sn + "");
                //var hdParams = "<span class='hide report-object'>" + JsonConvert.SerializeObject(item) + "</span>";
                var reportname = string.Format("<a class='report-name' href='javascript:;' onclick='Report.Get({0})'>{1}</a>", item.Id, item.Name);
                data.Add(reportname);
                //var icons = "<span class='action'>";
                //icons += string.Format("<a href='javascript:;' onclick='Report.RunReport(this,{0})'>View</a>", item.Id);
                //icons += "</span>";
                //data.Add(icons);
                rs.aaData.Add(data);
            }
            rs.sEcho = echo;
            rs.iTotalRecords = totalRecords;
            rs.iTotalDisplayRecords = totalDisplayRecords;
            return rs;
        }
        public ApiResponse Post([FromBody]ReportObject input)
        {
            ApiResponse response;
            try
            {
                var dt = ReportManager.GetReportData(input);
                response = new ApiResponse() { Success = true, Data = dt };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }


    }
}
