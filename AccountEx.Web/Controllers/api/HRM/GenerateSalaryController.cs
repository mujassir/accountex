using AccountEx.BussinessLogic;
using AccountEx.CodeFirst.Context;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.Web.Controllers.api.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace AccountEx.Web.Controllers.api.Manage
{
    public class GenerateSalaryController : BaseApiController
    {
        public ApiResponse Post([FromBody]ESalary input)
        {
            ApiResponse response;
            try
            {
                var err = ServerValidateSave(input);
                if (err == "")
                {
                    //input.FiscalId = SiteContext.Current.Fiscal.Id;
                    new ESalaryRepository().Save(input);
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
            catch (Exception ex)
            {
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
                var salaryConfigItems = repo.GetSalaryConfigItems(date);
                response = new ApiResponse
              {
                  Success = true,
                  Data = new
                  {
                      Salaries = esalaries,
                      SalaryConfigItems = salaryConfigItems,
                      VoucherNumber = voucherNumber,
                  }
              };

            }
            catch (Exception ex)
            {
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
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
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
    }
}
