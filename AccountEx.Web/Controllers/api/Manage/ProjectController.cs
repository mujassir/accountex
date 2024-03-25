using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.BussinessLogic;
using AccountEx.CodeFirst.Models;

namespace AccountEx.Web.Controllers.api.Manage
{
    public class ProjectController : BaseApiController
    {
        // GET api/test
        public JQueryResponse Get()
        {
            return GetDataTable();
        }


        // GET api/test/5
        public ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                var voucherIds = new ProjectReceiptRepository().GetVoucherIds(id);
                var records = new VoucherTransRepository().AsQueryable().Where(p => voucherIds.Contains(p.Id)).ToList();
                response = new ApiResponse
                {
                    Success = true,
                    Data = new {
                        data = new ProjectRepository().GetById(id),
                        Vouchers = records,
                    }

                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        // POST api/test
        public ApiResponse Post([FromBody]Project input)
        {
            ApiResponse response;
            try
            {
                input.FiscalId = SiteContext.Current.Fiscal.Id;
                ProjectManager.Save(input);
                response = new ApiResponse { Success = true, Data = input.Id };
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                response = new ApiResponse { Success = false, Error = "" };
            }
            //catch (Exception ex)
            //{
            //     response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            //}

            return response;

        }



        // DELETE api/test/5
        public ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                new ProjectRepository().Delete(id);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }

            return response;
        }

        private JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "Id", "Number", "Title", "PONumber", "POIssueDate", "AccountTitle", "StartDate", "EndDate", "GrossCost", "WorkScope" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var type = (queryString["type"] + "").Trim();
            var intsearch = Numerics.GetInt((queryString["sSearch"] + "").Trim());
            var dal = new ProjectRepository();
            var records = dal.AsQueryable(true);
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                    p.Number == intsearch
                    || p.Title.Contains(search)
                    || p.PONumber.Contains(search)
                    || p.AccountTitle.Contains(search)
                    || p.WorkScope.Contains(search)
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
            var projects = orderedList.Skip(displayStart).Take(displayLength).ToList();
            //var projectIds = projects.Select(p => p.Id).ToList();
            //var receipts = new ProjectReceiptRepository().AsQueryable().Where(p => projectIds.Contains(p.ProjectId)).ToList();
            var prReceiptRepo = new ProjectReceiptRepository();
            var rs = new JQueryResponse();
            var sr = 0;
            foreach (var item in projects)
            {
                var data = new List<string>();
                if (type == "report") data.Add((++sr) + "");
                else data.Add("<td><input type='checkbox' class='checkboxes' value='1' /></td>");
                data.Add(item.Number + "");
                data.Add(item.Title);
                data.Add(item.AccountTitle);
                data.Add(item.PONumber);
                //data.Add(Numerics.DateToString(item.POIssueDate));
                data.Add(Numerics.DateToString(item.StartDate));
                data.Add(Numerics.DateToString(item.EndDate));
                data.Add(Numerics.DecimalToString(item.GrossCost));

                var receipts = prReceiptRepo.GetPayments1(item.Id);
                var balance = item.GrossCost - receipts;
                data.Add(Numerics.DecimalToString(receipts));
                data.Add(Numerics.DecimalToString(balance));
                
                //data.Add(item.WorkScope);
                var printIcon = "<i class='fa fa-print' onclick=\"Project.Edit(" + item.Id + ",this,true)\" title='Edit' ></i>";
                var editIcon = "<i class='fa fa-edit' onclick=\"Project.Edit(" + item.Id + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"Project.Delete(" + item.Id + ")\" title='Delete' ></i>";
                var icons = "<span class='action'>";
                icons += printIcon;
                icons += editIcon;
                icons += deleteIcon;
                icons += "</span>";
                if (type != "report") data.Add(icons);
                rs.aaData.Add(data);
            }
            rs.sEcho = echo;
            rs.iTotalRecords = totalRecords;
            rs.iTotalDisplayRecords = totalDisplayRecords;
            return rs;
        }
    }
}
