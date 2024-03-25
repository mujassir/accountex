using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.Web.Controllers.api.Shared;
using System.Web.Http;
using AccountEx.CodeFirst;
using AccountEx.BussinessLogic;
using SelectPdf;
using System.IO;
using System.Web;
using BussinessLogic;
using System.Net.Mail;

namespace AccountEx.Web.Controllers.api.Manage
{
    public class ApproveSalaryController : BaseApiController
    {
        public ApiResponse Post([FromBody]ESalary input)
        {
            ApiResponse response;
            try
            {
                List<int> salaryItemIds = input.SalaryItems.Select(p => p.Id).ToList();
                List<int> inputaccountIds = input.SalaryItems.Select(p => p.AccountId).ToList();
                var queryString = Request.RequestUri.ParseQueryString();
                var key = queryString["key"].ToLower();
                
                if (key == "approve")
                {
                    var err = ServerValidateSave(input);
                    if (err == "")
                    {
                        PayrollManager.ApproveSalary(salaryItemIds);
                        
                        // input.FiscalId = SiteContext.Current.Fiscal.Id;
                        //PayrollManager.ApproveSalary(salaryItemIds, esalRepo);
                        //esalRepo.ApproveBySalaryItemIds(salaryItemIds, SettingManager.SalaryExpenseId, SettingManager.CashAccountId,
                        //SettingManager.PFAccountId, SettingManager.EOBIId, SettingManager.SSTId, SettingManager.IncomeTaxId, esalRepo);

                        response = new ApiResponse()
                        {
                            Success = true,
                            Data = input
                        };
                    }
                    else
                    {
                        response = new ApiResponse()
                        {
                            Success = false,
                            Error = err
                        };
                    }
                }
                else if (key == "email")
                {
                    var month = Numerics.GetInt(queryString["month"].ToLower());
                    var year = Numerics.GetInt(queryString["year"].ToLower());
                    DateTime date = new DateTime(year, month, 01);

                    var esalaryrepo = new ESalaryRepository();
                    var accountdetailRepo = new AccountDetailRepository(esalaryrepo);
                    var employees = accountdetailRepo.GetByAccountIds(inputaccountIds);

                    var err = ServerEmailValidate(employees);//BankId is required validations
                    if (err == "")
                    {
                       

                        //Send Email to the Banks
                        //  esalRepo.UpdateSalaryStatus(esalaries);
                        var salerItems = esalaryrepo.UpdateSalaryStatus(salaryItemIds);
                        var salary = esalaryrepo.GetESalaryByMonthYear(date.Month, date.Year);
                        var accounIds = salary.SelectMany(p => p.SalaryItems).Select(p => p.AccountId).Distinct().ToList();
                        employees = accountdetailRepo.GetByAccountIds(accounIds);
                        var bankIds = employees.Select(p => p.BankId).Distinct().ToList();
                        var banks = accountdetailRepo.GetByAccountIds(bankIds);
                        SendEmail(employees, banks, salerItems, date);



                        response = new ApiResponse()
                        {
                            Success = true,
                            Data = input
                        };
                    }
                    else
                    {
                        response = new ApiResponse()
                        {
                            Success = false,
                            Error = err
                        };
                    }

                }
                else
                {
                    new ESalaryRepository().DeleteSalaryItems(salaryItemIds);
                    response = new ApiResponse() { Success = true };
                }
            }
            catch (Exception ex)
            {
                 ;
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }
        private void SendEmail(List<AccountDetail> employees, List<AccountDetail> banks, List<SalaryItem> salaries, DateTime date)
        {
            var queryString = Request.RequestUri.ParseQueryString();

            // var file = Server.MapPath("~/Upload/ReportHTML.htm");

            // read parameters from the webpage
         




            string pdf_page_size = "A4";
            PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize),
                pdf_page_size, true);

            string pdf_orientation = "Portrait";
            PdfPageOrientation pdfOrientation =
                (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation),
                pdf_orientation, true);

            int webPageWidth = 1024;


            int webPageHeight = 0;

            foreach (var bank in banks)
            {
                var bankEmployees = employees.Where(p => p.BankId == bank.AccountId).ToList();

                string htmlString = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/Views/HRM/Template/Email.html"));
                string baseUrl = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/";
                // htmlString = htmlString.Replace("/Content/", "http://localhost:27099/Content/");
                htmlString = htmlString.Replace("ReportPrint1234", "ReportPrint1").Replace("visible-print", "");

                var html = "";
                var i = 1;
                var total = 0.0M;
                foreach (var employee in bankEmployees)
                {
                    
                    var salaryamount = 0.0M;
                    if (salaries.Any(p => p.AccountId == employee.AccountId))
                        salaryamount = salaries.FirstOrDefault(p => p.AccountId == employee.AccountId).NetSalary;
                    html += "<tr><td>" + i + "</td>";
                    html += "<td>" + employee.Name + "</td>";
                    html += "<td>" + employee.AccountId + "</td>";
                    html += "<td>" + salaryamount + "</td></tr>";
                    total += salaryamount;
                }
                html += "<tr><td colspan='3'>Total</td><td>" + total + "</td></tr>";
                htmlString = htmlString.Replace("{tableBody}", html);
                htmlString = htmlString.Replace("{Date}", DateTime.Now.ToString("dd-MM-yyyy"));
                htmlString = htmlString.Replace("{BankName}", bank.Name);
                htmlString = htmlString.Replace("{Month}", date.Month + "");
                htmlString = htmlString.Replace("{Year}", date.Year + "");
                htmlString = htmlString.Replace("{AccountNo}", bank.AccountNumber);
                htmlString = htmlString.Replace("{BankAddress}", bank.Address);
                HtmlToPdf converter = new HtmlToPdf();

                // set converter options
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;
                converter.Options.InternalLinksEnabled = false;
                converter.Options.ExternalLinksEnabled = false;
                converter.Options.MarginTop = 5;
                converter.Options.MarginBottom = 5;
                SelectPdf.PdfDocument doc = converter.ConvertHtmlString(htmlString, baseUrl);
                doc.DocumentInformation.CreationDate = DateTime.Now;
                doc.DocumentInformation.Title = "Salary Process Request";
                doc.DocumentInformation.Author = SettingManager.ApplicationTitle;
                doc.DocumentInformation.Subject = "Salary Process Request";

                PdfFont font = doc.AddFont(PdfStandardFont.Helvetica);
                font.Size = 100;

                var dirPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Upload/PrintFiles/");
                if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
                var filename = "Salary Process Request".Replace(" ", "-").Replace("|", "-") + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                while (filename.Contains("--"))
                    filename = filename.Replace("--", "-");
                var path = dirPath + filename + ".pdf";
                doc.Save(path);

                // close pdf document
                doc.Close();
                SendEmail(bank.Email, "Salary Process Request", "please find the attachment.", path);
            }
        }
        private void SendEmail(string to, string subject, string body, string attachment)
        {


            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            SmtpServer.Port = 587;
            SmtpServer.EnableSsl = true;
            SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
            SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Credentials =
            new System.Net.NetworkCredential("xameer.co@gmail.com", "@123X@m33r");

            var message = new System.Net.Mail.MailMessage()
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            message.From = new MailAddress("xameer.co@gmail.com");
            message.To.Add(to);
            if (!string.IsNullOrWhiteSpace(attachment))
                message.Attachments.Add(new Attachment(attachment));
            SmtpServer.Send(message);
            //using (var smtp = new SmtpClient())
            //{

            //    smtp.Send(message);
            //}
        }
        public ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                var voucherNumber = id;
                var data = new object();
                var queryString = Request.RequestUri.ParseQueryString();
                var month = Numerics.GetInt(queryString["month"].ToLower());
                var year = Numerics.GetInt(queryString["year"].ToLower());
                //var email = queryString["Email"].ToLower();
                DateTime date = new DateTime(year, month, 01);
                var repo = new ESalaryRepository();
                if (voucherNumber == 0)
                    voucherNumber = repo.GetNextVoucherNumber();

                var esalaries = repo.GetByDate(date);

                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Salaries = esalaries,
                        VoucherNumber = voucherNumber,
                    }
                };
            }
            catch (Exception ex)
            {
                 ;
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }

        public ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                

                PayrollManager.DisapproveSalary(id);

                response = new ApiResponse() { Success = true };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }

        private string ServerValidateSave(ESalary input)
        {
            var err = ",";
            try
            {
                //var record = new SaleRepository().GetByVoucherNo(input.VoucherNumber, input.Id, input.TransactionType);
                //if (record != null)
                //{
                //    err += "<li>Voucher no already exist.</li>";
                //}
                if (SiteContext.Current.Fiscal == null)
                {
                    err += "No Fiscal year found";
                }
                else if (SettingManager.SalaryExpenseId == 0)
                {
                    err += "Salary expense account is missing.,";
                }
                else if (SettingManager.CashAccountId == 0)
                {
                    err += "Cash account is missing.,";
                }
                else if (SettingManager.PFAccountId == 0)
                {
                    err += "Provident fund account is missing.,";
                }
                else if (SettingManager.EOBIId == 0)
                {
                    err += "EOBI account is missing.,";
                }
                else if (SettingManager.IncomeTaxId == 0)
                {
                    err += "Income tax account is missing.";
                }
                else if (SettingManager.SSTId == 0)
                {
                    err += "Social Security account is missing.,";
                }
                if (!FiscalYearManager.IsValidFiscalDate(Convert.ToDateTime(input.PaymentDate)))
                {
                    err += "Date should be within current fiscal year.,";
                }
            }
            catch (Exception)
            {
                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;
        }

        private string ServerEmailValidate(List<AccountDetail> accounts)
        {
            var err = ",";
            try
            {
                foreach (var item in accounts)
                {
                    if (item.BankId == 0)
                        err += "Bank is Required for " + item.Name + " .,";
                }

            }
            catch (Exception)
            {
                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;
        }
    }
}
