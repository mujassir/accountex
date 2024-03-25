using AccountEx.BussinessLogic;
using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AccountEx.CodeFirst.Models;
using AccountEx.Repositories;
using AccountEx.Web.Controllers.api.Shared;

namespace AccountEx.Web.Controllers.api
{
    public class SalaryController : GenericApiController<Salary>
    {
        public override ApiResponse Post(Salary input)
        {
            ApiResponse response;
            try
            {
                PayrollManager.AddSalary(input);

                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }
        public override ApiResponse Delete(int id)
        {

            ApiResponse response;
            try
            {

                new SalaryRepository().Delete(id);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }

            return response;
        }
        public override ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                //PayrollManager.ProcessSalary(id, DateTime.Now.Year);
                // var salary = new SalaryRepository().GetById(id);

                var repo = new SalaryRepository();
                var accDetailRepo = new AccountDetailRepository(repo);


                var queryString = Request.RequestUri.ParseQueryString();
                var salary = !Numerics.GetBool(queryString["loadwithvoucherno"]) ? repo.GetById(id) : new SalaryRepository().GetByVoucherNumber(id);


                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Salary = salary,
                        Employee = accDetailRepo.GetByAccountId(salary.AccountId)
                    }
                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }
        protected override JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "VoucherNumber", "Code", "Name", "Month", "Paid At", "BasicSalary", "SummaryAllowances", "SummaryOT", "SummaryDeductions", "SummarySalary", "IncomeTax", "NetSalary", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var intsearch = Numerics.GetInt((queryString["sSearch"] + "").Trim());
            var month = Numerics.GetInt(queryString["month"]);
            var year = Numerics.GetInt(queryString["year"]);
            var isProcessed = Numerics.GetBool(queryString["isProcessed"]);
            //var dal = new ProjectRepository();
            var records = Repository.AsQueryable();
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            if (month > 0)
                records = records.Where(p => p.Month == month);
            if (queryString["IsProcessed"] + "" != "")
                records = records.Where(p => p.IsProcessed == isProcessed);
            if (year > 0)
                records = records.Where(p => p.Year == year);
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>

                    p.Code.Contains(search)
                    || p.Name.Contains(search)
                     || p.VoucherNumber == intsearch
                      || p.Month == month
                       || p.BasicSalary == intsearch
                        || p.OTCost == intsearch
                          || p.NetSalary == intsearch
                            || p.OTCost == intsearch
                    );
            var orderedList = filteredList.OrderByDescending(p => p.Id);
            if (colIndex < coloumns.Length && coloumns[colIndex] + "" != "")
            {
                var sortDir = queryString["sSortDir_0"];
                orderedList = sortDir == "asc" ? filteredList.OrderBy(coloumns[colIndex]) :
                    filteredList.OrderByDescending(coloumns[colIndex]);
            }
            var sb = new StringBuilder();
            sb.Clear();
            string[] months = { "January", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            var rs = new JQueryResponse();
            //const string tickIcon = "<i class='fa fa-check'  title='Salary Processed' ></i>";
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                var data = new List<string>
                {
                  
                      item.VoucherNumber+"",
                    item.Code,
                    item.Name,
                    months[item.Month],
                    item.PaymentDate.HasValue ? item.PaymentDate.Value.ToString(AppSetting.DateFormat) : "",
                    Numerics.DecimalToString(item.BasicSalary, 0),
                    Numerics.DecimalToString(item.SummaryAllowances, 0),
                    Numerics.DecimalToString(item.SummaryOT, 0),
                    Numerics.DecimalToString(item.SummaryDeductions, 0),
                    Numerics.DecimalToString(item.IncomeTax, 0),
                    Numerics.DecimalToString(item.NetSalary, 0)
                };
                var editIcon = "<i class='fa fa-edit' onclick=\"Salary.Edit(" + item.Id + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"Salary.Delete(" + item.Id + ")\" title='Delete' ></i>";
                var printIcon = "<i class='fa fa-print' onclick=\"Salary.GetPrintDetail(" + item.Id + ")\" title='Print' ></i>";

                var icons = "<span class='action'>";
                icons += printIcon;
                icons += editIcon;
                icons += deleteIcon;
                //if (item.IsProcessed)
                //    icons += tickIcon;
                icons += "</span>";
                data.Add(icons);
                rs.aaData.Add(data);
            }
            rs.sEcho = echo;
            rs.iTotalRecords = totalRecords;
            rs.iTotalDisplayRecords = totalDisplayRecords;
            return rs;
        }
    }
}
