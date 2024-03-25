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

namespace AccountEx.Web.Controllers.api.Manage
{
    public class SalaryBankReconciliationController : BaseApiController
    {
        public ApiResponse Post([FromBody]ESalary input)
        {
            ApiResponse response;
            try
            {
                var accountDetailrepo = new AccountDetailRepository();
                List<int> salaryItemIds = input.SalaryItems.Select(p => p.Id).ToList();
                List<int> accountIds = input.SalaryItems.Select(p => p.AccountId).ToList();
                var employees = accountDetailrepo.GetByAccountIds(accountIds);
                var queryString = Request.RequestUri.ParseQueryString();
                var key = queryString["key"].ToLower();
                if (key == "approve")
                {
                    var err = ServerValidateSave(input);
                    if (err == "")
                    {
                        
                      
                        //ESalaryRepository esalaryrepo = new ESalaryRepository();
                        //esalaryrepo.ProsessedEmailBySalaryItemIds(salaryItemIds, accountIds,esalaryrepo);

                        PayrollManager.ProsessedEmailBySalaryItemIds(salaryItemIds, accountIds);

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
                DateTime date = new DateTime(year, month, 01);
                var repo = new ESalaryRepository();
                if (voucherNumber == 0)
                    voucherNumber = repo.GetNextVoucherNumber();

                var esalaries = repo.GetByDate(date);
                var mailsalaries = esalaries.Where(p => p.SalaryItems.Any(s => s.Status == (byte)SalaryStatus.EmailToBank)).ToList();
                var mailprocessed = esalaries.Where(p => p.SalaryItems.Any(s => s.Status == (byte)SalaryStatus.BankProcessed)).ToList();

                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        MailSalaries = mailsalaries,
                        MailProcessed=mailprocessed,
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
                new SalaryRepository().DeleteByVoucherNumber(id);
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
                else if (SettingManager.SalaryExpenseId == 0 )
                {
                    err += "Salary expense account is missing.,";
                }
                else if (SettingManager.CashAccountId == 0 )
                {
                    err += "Cash account is missing.,";
                }
                else if (SettingManager.PFAccountId == 0 )
                {
                    err += "Provident fund account is missing.,";
                }
                else if (SettingManager.EOBIId == 0 )
                {
                    err += "EOBI account is missing.,";
                }
                else if (SettingManager.IncomeTaxId == 0 )
                {
                    err += "Income tax account is missing.";
                }
                else if (SettingManager.SSTId == 0 )
                {
                    err += "Social Security account is missing.,";
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
