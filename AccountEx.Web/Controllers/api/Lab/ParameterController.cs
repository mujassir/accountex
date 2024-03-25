
using AccountEx.CodeFirst.Models.Lab;
using AccountEx.Common;
using AccountEx.Repositories.Lab;
using AccountEx.Web.Controllers.api.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;

namespace AccountEx.Web.Controllers.api.Lab
{
    public class ParameterController : GenericApiController<Parameter>
    {
        public override ApiResponse Post([FromBody]Parameter input)
        {
            ApiResponse response;
            try
            {
                input.Type = (byte)TestType.Pathology;
                base.Post(input);
                //var test = new Test();

                //var dbTest = new TestRepository().GetByName(input.Name);
                //if (dbTest == null)
                //{
                //    if (input.Id == 0)
                //        dbTest = new Test();
                //    else
                //    {
                //        var dbparameter = new ParameterRepository().GetById(input.Id);
                //        dbTest = new TestRepository().GetByName(dbparameter.Name);
                //        if (dbTest == null)
                //            dbTest = new Test();

                //    }
                //}

                //test = new Test()
                //{
                //    Name = input.Name,
                //    Id = dbTest.Id,
                //    Type = input.Type,
                //    TestGroupId = input.TestGroupId,
                //    CreatedAt = dbTest.CreatedAt,
                //    CreatedBy = dbTest.CreatedBy,
                //    ModifiedAt = dbTest.ModifiedAt,
                //    ModifiedBy = dbTest.ModifiedBy
                //};
                //test.TestParameters.Add(new TestParameter() { ParameterId = input.Id, TestId = test.Id });
                //new TestRepository().Save(test);
                response = new ApiResponse()
                {
                    Success = true,

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
            var coloumns = new[] { "Name", "Unit", "MinValue", "MaxValue", "", };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var type = (byte)TestType.Pathology;
            var records = Repository.AsQueryable().Where(p => p.Type == type);
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                    p.Name.Contains(search) ||
                     p.MinValue.Contains(search) ||
                     p.MaxValue.Contains(search)
                    );
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
                data.Add(item.Unit);
                data.Add(item.MinValue);
                data.Add(item.MaxValue);
                var editIcon = "<i class='fa fa-edit' onclick=\"Parameter.Edit(" + item.Id + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"Parameter.Delete(" + item.Id + ")\" title='Delete' ></i>";
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
