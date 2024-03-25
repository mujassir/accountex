using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using AccountEx.CodeFirst.Models.Lab;
using AccountEx.Web.Controllers.api.Shared;
using AccountEx.Repositories.Lab;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.DbMapping.Lab;
using AccountEx.BussinessLogic;

namespace AccountEx.Web.Controllers.api.Lab
{
    public class TestController : GenericApiController<Test>
    {
        // GET: /Investigation/
        public override ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {


                response = TestManager.GetTest(id);
                //var queryString = Request.RequestUri.ParseQueryString();
                //var repo = new TestRepository();
                //var data = repo.GetById(id);
                //var pr = new ParameterRepository(repo).GetByParameterIds(data.TestParameters.Select(p => p.ParameterId).ToList());
                //response = new ApiResponse()
                //{
                //    Success = true,
                //    Data = new
                //    {
                //        Test = data,
                //        Parameters = pr

                //    }
                //};
            }
            catch (Exception ex)
            {
                response = new ApiResponse() { Success = false, Error = ex.Message };
            }
            return response;
        }

        public ApiResponse Post([FromBody]TestSaveExtra input, bool withRateList)
        {
            ApiResponse response;
            try
            {
                response = TestManager.Save(input);

            }

            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ex.Message };
            }

            return response;

        }

        public override ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                //var type = (queryString["type"]);
                //var vouchertype =(VoucherType)Convert.ToByte(type);
                new TestRepository().Delete(id);
                new TestParameterRepository().HardDelete(id);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ex.Message };
            }

            return response;
        }

        protected override JQueryResponse GetDataTable()
        {
            //  var queryString = Request.RequestUri.ParseQueryString();
            var queryString = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var coloumns = new[] { "Name", "GroupName", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var records = new GenericRepository<vw_Investigations>().AsQueryable();
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                    p.Name.Contains(search)
                    );
            var totalRecords = filteredList.Count();
            var totalDisplayRecords = totalRecords;
            var orderedList = filteredList.OrderByDescending(p => p.Id);
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

                data.Add("<td><input type='checkbox' class='checkboxes' value='1' /></td>");
                data.Add(item.Name);
                data.Add(item.GroupName);
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
