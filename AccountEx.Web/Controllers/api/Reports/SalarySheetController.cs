using AccountEx.BussinessLogic;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Linq;
using System.Net.Http;

namespace AccountEx.Web.Controllers.api
{
    public class SalarySheetController : BaseApiController
    {
        // GET api/test
        public ApiResponse Get()
        {
            return GetSalarySheet();
        }
        private ApiResponse GetSalarySheet()
        {
            ApiResponse response;
            try
            {
                var employees = new AccountDetailRepository().AsQueryable().Where(p => p.AccountDetailFormId == (int)AccountDetailFormType.Employees)
                    .Select(p => new { p.AccountId, p.Designation }).ToDictionary(p => p.AccountId, q => q.Designation);

                var queryString = Request.RequestUri.ParseQueryString();
                var month = Numerics.GetInt(queryString["month"]);
                var salaryItems = new SalaryRepository().GetByMonthYear(true, month, DateTime.Now.Year).Select(p => new
                {
                    p.Code,
                    p.Name,
                   // Designation = employees.ContainsKey(p.AccountId) ? employees[p.AccountId] : "",
                    PaymentDate = p.PaymentDate.HasValue ? p.PaymentDate.Value.ToString(AppSetting.DateFormat) : "",
                    BasicSalary = Numerics.DecimalToString(p.BasicSalary, 0),
                    Absents = Numerics.DecimalToString(p.AbsentsCost, 0),
                    Allowances = Numerics.DecimalToString(p.SummaryAllowances, 0),
                    OverTime = Numerics.DecimalToString(p.SummaryOT, 0),
                    Deductions = Numerics.DecimalToString(p.SummaryDeductions, 0),
                    IncomeTax = Numerics.DecimalToString(p.IncomeTax, 0),
                    NetSalary = Numerics.DecimalToString(p.NetSalary, 0)
                });
                response = new ApiResponse
                {
                    Success = true,
                    Data = salaryItems

                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
    }
}
