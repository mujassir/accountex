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
using System.Globalization;
using AccountEx.BussinessLogic;
using AccountEx.DbMapping;

namespace AccountEx.Web.Controllers.api.Manage
{
    public class EmployeeOTHourController : BaseApiController
    {
        public JQueryResponse Get()
        {
            return GetDataTable();
        }

        public ApiResponse Get(int id)
        {
            ApiResponse ApiResponse;
            ApiResponse = new ApiResponse
            {
                Success = true,
            };
            return ApiResponse;
        }

        public ApiResponse Get(string key)
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                var recordType = queryString["key"].ToLower();
                if (recordType == "getotstandardrate")
                {
                    var repo = new SalaryConfigRepository();
                    var salaryCongifRepo = new SalaryConfigItemRepository(repo);

                    var fromdate = Convert.ToDateTime(queryString["fromdate"].ToLower());
                    var todate = Convert.ToDateTime(queryString["todate"].ToLower());
                    var accountId = Numerics.GetInt(queryString["accountId"].ToLower());
                    var salaryconfig = repo.GetSalaryConfigBydate(fromdate, todate);
                    var data = salaryCongifRepo.GetSalaryConfigBydate(accountId, salaryconfig.Id);
                    response = new ApiResponse()
                    {
                        Success = true,
                        Data = data
                    };
                }
                else
                    response = GetMonthlOT();
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }

        public ApiResponse Post([FromBody]EmployeeOTHourExtra input)
        {
            ApiResponse response;
            var repo = new EmployeeOTHourRepository();
            try
            {
                if (input.RecordType == "Save")
                {
                    
                    var err = ServerValidateSave(input.EmployeeOTHourList[0]);
                    if (err == "")
                    {
                        repo.Save(input.EmployeeOTHourList);
                        repo.SaveChanges();
                    }
                    else
                    {
                        response = new ApiResponse()
                        {
                            Success = false,
                            Error = err
                        };
                        return response;
                    }
                }
                else if (input.RecordType == "Edit")
                {
                    //var fromDate = Convert.ToDateTime(input.EmployeeOTHourList[0].FromDate.Value.Date);
                    //var todate = Convert.ToDateTime(input.EmployeeOTHourList[0].ToDate.Value.Date);
                    var month = Numerics.GetInt(input.EmployeeOTHourList[0].Month);
                    var year = Numerics.GetInt(input.EmployeeOTHourList[0].Year);
                    var name = input.EmployeeOTHourList[0].Name;
                    using (var scope = TransactionScopeBuilder.Create())
                    {
                        repo.DeleteByMonth(month, year, name);
                        repo.Save(input.EmployeeOTHourList);
                        repo.SaveChanges();
                        scope.Complete();
                    }
                }
                response = new ApiResponse()
                {
                    Success = true,
                    Data = ""
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
                //var fromDate = Convert.ToDateTime(Request.GetQueryString("fromdate"));
                //var toDate = Convert.ToDateTime(Request.GetQueryString("todate"));
                var month = Numerics.GetInt(Request.GetQueryString("month"));
                var year = Numerics.GetInt(Request.GetQueryString("year"));
                var name = Request.GetQueryString("Name").ToString();
                new EmployeeOTHourRepository().DeleteByMonth(month, year, name);
                response = new ApiResponse() { Success = true };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }

        private JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "Name", "Month", "Year", };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();

            var dal = new EmployeeOTHourRepository();
            var records = dal.AsQueryable();
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                    p.Name.Contains(search)
                    );
            var groupData = filteredList.GroupBy(p => p.Name).Select(p => new EmployeeOverTimeHourExtra()
            {
                Name = p.Key,
                FromDate = p.FirstOrDefault().FromDate,
                ToDate = p.FirstOrDefault().ToDate,
                Month = p.FirstOrDefault().Month,
                Year = p.FirstOrDefault().Year
            }).ToList();
            var orderedList = groupData.OrderByDescending(p => p.ToDate);
            //orderedList.Where(p=>p.Name.Contains(ab))
            //if (colIndex < coloumns.Length && coloumns[colIndex] + "" != "")
            //{
            //    var sortDir = queryString["sSortDir_0"];
            //    orderedList = sortDir == "asc" ? filteredList.OrderBy(coloumns[colIndex]) :
            //        filteredList.OrderByDescending(coloumns[colIndex]);
            //}
            var sb = new StringBuilder();
            sb.Clear();

            var rs = new JQueryResponse();
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                var data = new List<string>();
                data.Add(item.Name);
                String month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Month);
                data.Add(month);
                data.Add(item.Year + "");
                //data.Add(item.FromDate.Value.ToString(AppSetting.DateFormat));
                //data.Add(item.ToDate.Value.ToString(AppSetting.DateFormat));

                var editIcon = "<i class='fa fa-edit' onclick=\"EmployeeOTHour.Edit(" + item.Month + "," + item.Year + ",'" + item.Name + "')\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"EmployeeOTHour.Delete(" + item.Month + "," + item.Year + ",'" + item.Name + "')\" title='Delete' ></i>";
                var icons = "<span class='action'>";
                icons += editIcon;
                icons += deleteIcon;
                icons += "</span>";
                data.Add(icons);
                rs.aaData.Add(data);
            }
            rs.sEcho = echo;
            rs.iTotalRecords = totalRecords;
            rs.iTotalDisplayRecords = totalDisplayRecords;
            return rs;
        }

        private string ServerValidateSave(EmployeeOTHour input)
        {
            var err = ",";
            try
            {
                //if (new EmployeeOTHourRepository().GetByName(input.Name) != null)
                //{
                //    err += "This User name has already been taken.";
                //}
                if (new EmployeeOTHourRepository().CheckEmployeeOTByMonth(input.Month, input.Year))
                {
                    err += "Duplicate Over time " +input.Name +" sheet not allowed.";
                }
            }
            catch (Exception)
            {
                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;
        }

        private ApiResponse GetMonthlOT()
        {
            //var fromDate = Convert.ToDateTime(Request.GetQueryString("fromdate"));
            //var toDate = Convert.ToDateTime(Request.GetQueryString("todate"));

            var repo = new EmployeeOTHourRepository();
            var accountRepo = new AccountRepository(repo);


            var month = Numerics.GetInt(Request.GetQueryString("month"));
            var year = Numerics.GetInt(Request.GetQueryString("year"));
            var employeeovertimehr = repo.GetEmployeeOTByMonth(month, year);
            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    EmployeeOverTimeHours = employeeovertimehr,
                    Employee = accountRepo.GetByIds(employeeovertimehr.Select(p => p.AccountId).Distinct().ToList())
                }
            };
            return response;
        }
    }
}
