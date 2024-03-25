using AccountEx.BussinessLogic;
using AccountEx.Common;
using AccountEx.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace AccountEx.Web.Controllers.api
{
    public class ParentAccountController : BaseApiController
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
                var result = "";
                var branch = new AccountRepository().GetById(id);
                var allowedChild = new Dictionary<string, bool>();
                allowedChild.Add("BranchCategories", true);
                allowedChild.Add("BranchHalls", true);
                result = JsonConvert.SerializeObject(SetJsonValue(branch, allowedChild, new string[] { "Branch", "BranchHall", "Category", "Tables" }), Formatting.Indented, GetJsonSetting());
                response = new ApiResponse
                {
                    Success = true,
                    Data = result

                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        // POST api/test
        public ApiResponse Post([FromBody]CodeFirst.Models.Account input)
        {
            ApiResponse response;
            try
            {
                input.DisplayName = input.Name;
                input.HasChild = false;
                var err =AccountManager.ServerValidateSave(input);
                if (err == "")
                {

                    new AccountRepository().Save(input);
                    response = new ApiResponse() { Success = true, Data = input.Id };
                }
                else
                {
                    response = new ApiResponse() { Success = false, Error = err };
                }
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }

            return response;

        }



        // DELETE api/test/5
        public ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                new AccountRepository().Delete(id);
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
            var coloumns = new[] { "Id", "Name", "NoOfQuestions", "TotalTime", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var isClientside = Numerics.GetBool(queryString["IsClientSide"]);
            var dal = new AccountRepository();
            new AccountRepository().AsQueryable().Select(p => new { p.Name, p.Id }).ToList();
            var records = dal.AsQueryable();

            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p => p.Name.Contains(search));
            var orderedList = filteredList.OrderByDescending(p => p.Id);
            if (colIndex < coloumns.Length && coloumns[colIndex] + "" != "")
            {
                var sortDir = queryString["sSortDir_0"];
                orderedList = sortDir == "asc" ? filteredList.OrderBy(coloumns[colIndex]) :
                    filteredList.OrderByDescending(coloumns[colIndex]);
            }
            var sb = new StringBuilder();
            sb.Clear();

            var rs = new JQueryResponse();
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                //var data = new List<string>();
                //data.Add(item.Id + "");
                //data.Add(item.Name);
                //var subcats = "";
                //foreach (var subcat in item.BranchCategories.ToList())
                //{
                //    subcats += subcat.Category != null ? "<label class='label label-info'>" + subcat.Category.Name + "</label>&nbsp;&nbsp;" : "";
                //}
                //data.Add(subcats);
                //var halls = "";
                //foreach (var hall in item.BranchHalls.ToList())
                //{
                //    var dbhal = halldata.FirstOrDefault(p => p.Id == hall.HallId);
                //    halls += dbhal != null ? "<label class='label label-important'>" + dbhal.Name + "</label>&nbsp;&nbsp;" : "";
                //}
                //data.Add(halls);

                ////data.Add(item.NoOfQuestions + "");
                ////data.Add(item.TotalTime + "");
                //var editIcon = "<i class='icon-edit' onclick=\"Edit(" + item.Id + ")\" data-original-title='Edit' ></i>";
                //var deleteIcon = "<i class='icon-remove' onclick=\"Delete(" + item.Id + ")\" data-original-title='Delete' ></i>";
                //var icons = "<span class='action'>";
                //icons += editIcon;
                //icons += deleteIcon;
                //icons += "</span>";
                //data.Add(icons);
                //rs.aaData.Add(data);
            }
            rs.sEcho = echo;
            rs.iTotalRecords = totalRecords;
            rs.iTotalDisplayRecords = totalDisplayRecords;
            return rs;
        }
    }
}
