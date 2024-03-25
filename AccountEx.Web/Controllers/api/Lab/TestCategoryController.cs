
using AccountEx.CodeFirst.Models.Lab;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.Repositories.Lab;
using AccountEx.Web.Controllers.api.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
namespace AccountEx.Web.Controllers.api.Lab
{
    public class TestCategoryController : GenericApiController<TestCategory>
    {
        public ApiResponse Get(string Key)
        {
            ApiResponse response;
            try
            {
                response = new ApiResponse()
                {
                    Success = true,
                    Data = new GenericRepository<TestCategory>().GetAll()
                };

            }
            catch (Exception ex)
            {
                response = new ApiResponse() { Success = false, Error = ex.Message };
            }
            return response;
        }

        //Remove the controller after testing
        public ApiResponse Get(int PatientId, string Key)
        {
            ApiResponse response;
            try
            {

                response = new ApiResponse()
                {
                    Success = true,
                    Data = new TestCategoryRepository().GetGroup()
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse() { Success = false, Error = ex.Message };
            }
            return response;
        }

        protected override JQueryResponse GetDataTable()
        {
            //  var queryString = Request.RequestUri.ParseQueryString();
            var queryString = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var coloumns = new[] { "Name", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var records = Repository.AsQueryable();
            var totalRecords = records.Count();

            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                    p.Name.Contains(search)
                    );
            var orderedList = filteredList.OrderByDescending(p => p.Id);
            if (colIndex < coloumns.Length && coloumns[colIndex] + "" != "")
            {
                var sortDir = queryString["sSortDir_0"];
                orderedList = sortDir == "asc" ? filteredList.OrderBy(coloumns[colIndex]) :
                    filteredList.OrderByDescending(coloumns[colIndex]);
            }
            var totalDisplayRecords = filteredList.Count();
            var sb = new StringBuilder();
            sb.Clear();

            var rs = new JQueryResponse();
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                var data = new List<string>();

                data.Add("<td><input type='checkbox' class='checkboxes' value='1' /></td>");
                data.Add(item.Name);

                var editIcon = GetEditIcon(item.Id, this.ControllerContext.ControllerDescriptor.ControllerName);
                var deleteIcon = GetDeleteIcon(item.Id, this.ControllerContext.ControllerDescriptor.ControllerName);
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
    }
}
