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
using System.Web;
using AccountEx.BussinessLogic;

namespace AccountEx.Web.Controllers.api.Manage
{
    public class LessAssignmentToCustomerController : BaseApiController
    {
        public JQueryResponse Get()
        {

            return GetDataTable();

        }
        public ApiResponse Get(int groupId, int itemgroupId, string key)
        {
            ApiResponse response;
            try
            {
                var query = new LessAssignmentToCustomerRepository().AsQueryable().Where(q => q.CustomerGroupId == groupId && q.ItemGroupId == itemgroupId).ToList();
                var group = new ItemGroupRepository().GetById(groupId);
                var records = query;
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Items = records,
                        Group = group
                    }
                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }


        public ApiResponse Post([FromBody]List<GroupItemCustomerLess> Items)
        {
            ApiResponse response;
            try
            {
                new LessAssignmentToCustomerRepository().Save(Items);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }

        public ApiResponse Delete(int groupId, int itemgroupId, string key)
        {
            ApiResponse response;
            try
            {
                new LessAssignmentToCustomerRepository().DeleteByCustomerGroupId(groupId, itemgroupId);
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

        protected JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "Name", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var type = (queryString["type"] + "").Trim();
            //var dal = new ProjectRepository();
            var records = new LessAssignmentToCustomerRepository().GetCustomerLess();
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>

                     p.Group.Contains(search) ||
                      p.ItemGroup.Contains(search)

                   );


            var orderedList = filteredList.OrderByDescending(p => p.Group);
            if (colIndex < coloumns.Length && coloumns[colIndex] + "" != "")
            {
                var sortDir = queryString["sSortDir_0"];
                orderedList = sortDir == "asc" ? filteredList.OrderBy(coloumns[colIndex]) :
                    filteredList.OrderByDescending(coloumns[colIndex]);
            }
            var sb = new StringBuilder();
            sb.Clear();

            var sr = 0;
            var rs = new JQueryResponse();
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                var data = new List<string>();
                //data.Add("<td><input type='checkbox' class='checkboxes' value='1' /></td>");
                data.Add(item.Group);
                data.Add(item.ItemGroup);
                var editIcon = "<i class='fa fa-edit' onclick=\"LessAssignmentToCustomers.Edit(" + item.GroupId + "," + item.ItemGroupId + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"LessAssignmentToCustomers.Delete(" + item.GroupId + "," + item.ItemGroupId + ")\" title='Delete' ></i>";
                var icons = "<span class='action'>";
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
