using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System.Web.Http;
using System.Globalization;
using AccountEx.BussinessLogic;
using AccountEx.DbMapping;

namespace AccountEx.Web.Controllers.api.Manage
{
    public class EmployeeLeaveController : BaseApiController
    {
        public JQueryResponse Get()
        {
            return GetDataTable();
        }
        public ApiResponse Get(int id) 
        {
            ApiResponse response;
            try
            {
                response = new ApiResponse
                {
                    Success = true,
                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }

            return response;
        }

        public ApiResponse Get(string key)
        {
            ApiResponse response;
            try
            {
                response = GetMonthLeaves();
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }

        public ApiResponse Post([FromBody]EmployeeLeaveExtra input)
        {
            ApiResponse response;
            try
            {
                var repo = new EmployeeLeaveRepository();
                //var tranrRepo = new TransactionRepository(repo);

                var empleave = input.EmployeeLeaveList.FirstOrDefault();
                if (input.RecordType == "Save")
                {
                    var err = ServerValidateSave(empleave);
                    if (err == "")
                    {
                        repo.Save(input.EmployeeLeaveList);
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

                    var month = Numerics.GetInt(empleave.Month);
                    var year = Numerics.GetInt(empleave.Year);
                    var name = empleave.Name;
                    using (var scope = TransactionScopeBuilder.Create())
                    {
                        repo.DeleteByMonth(month, year, name);
                        repo.Save(input.EmployeeLeaveList);
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
                var fromDate = Convert.ToDateTime(Request.GetQueryString("fromdate"));
                var toDate = Convert.ToDateTime(Request.GetQueryString("todate"));
                var name =Request.GetQueryString("Name").ToString();
                new EmployeeLeaveRepository().DeleteByDateRange(fromDate, toDate,name);
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
            var dal = new EmployeeLeaveRepository();
            var records = dal.AsQueryable();
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                    p.Name.Contains(search)
                    );
            var groupData = filteredList.GroupBy(p => p.Name).Select(p => new EmployeeleavesExtra()
            {
                Name = p.Key,
                FromDate = p.FirstOrDefault().FromDate,
                ToDate = p.FirstOrDefault().ToDate,
                Month=p.FirstOrDefault().Month,
                Year=p.FirstOrDefault().Year
            }).ToList();
            var orderedList = groupData.OrderByDescending(p => p.ToDate);
            var sb = new StringBuilder();
            sb.Clear();

            var rs = new JQueryResponse();
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                var data = new List<string>();
                data.Add(item.Name);
                String month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Month);
                data.Add(month);
                data.Add(item.Year+"");
                //data.Add(item.FromDate.Value.ToString(AppSetting.DateFormat));
                //data.Add(item.ToDate.Value.ToString(AppSetting.DateFormat));

                var editIcon = "<i class='fa fa-edit' onclick=\"EmployeeLeave.Edit(" + item.Month + "," + item.Year + ",'" + item.Name + "')\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"EmployeeLeave.Delete(" + item.Month + "," + item.Year + ",'" + item.Name + "')\" title='Delete' ></i>";
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

        private string ServerValidateSave(EmployeeLeave input)
        {
            var err = ",";
            try
            {
                //if (new EmployeeLeaveRepository().GetByName(input.Name) != null)
                //{
                //    err += "This User name has already been taken.";
                //}
                if (new EmployeeLeaveRepository().CheckEmployeeleaveByMonth(input.Month, input.Year))
                {
                    err += input.Name+" has already been taken.";
                }
                
            }
            catch (Exception)
            {
                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;
        }

        private ApiResponse GetMonthLeaves()
        {
            var month = Numerics.GetInt(Request.GetQueryString("month"));
            var year = Numerics.GetInt(Request.GetQueryString("year"));

            var repo = new EmployeeLeaveRepository();
            var accRepo = new AccountRepository(repo);

            var employeeleave = repo.GetEmployeeleaveByMonth(month, year);
            var response = new ApiResponse
            {
                Success = true,
                Data = new
                {
                    EmployeeLeave = employeeleave,
                    Employee = accRepo.GetByIds(employeeleave.Select(p => p.AccountId).Distinct().ToList())
                }
            };
            return response;
        }
    }
}
