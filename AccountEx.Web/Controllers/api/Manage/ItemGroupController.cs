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
    public class ItemGroupController : GenericApiController<ItemGroup>
    {
        public ApiResponse Get(int customergroupid, string key)
        {
            ApiResponse response;
            try
            {

                var ItemGroupdIds = new LessAssignmentToCustomerRepository().AsQueryable().Where(p => p.CustomerGroupId == customergroupid).Select(p => p.ItemGroupId).Distinct().ToList();
                var itemgroups = new ItemGroupRepository().AsQueryable().Where(p => !ItemGroupdIds.Contains(p.Id) && p.GroupType == (byte)GroupType.ItemGroup && p.GroupSubType == (byte)GroupSubType.Less).ToList();
                var customergroups = new ItemGroupRepository().GetById(customergroupid);
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        ItemGroups = itemgroups,
                        CustomerGroups = customergroups,
                    },

                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;

        }
        public override ApiResponse Post([FromBody]ItemGroup input)
        {
            ApiResponse response;
            try
            {
                var repo = new ItemGroupRepository();
                if (input.Id == 0)
                {
                    repo.Add(input);
                }
                else
                {
                    repo.Update(input);
                }
                response = new ApiResponse() { Success = true };
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
            var coloumns = new[] { "Name", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var type = Convert.ToByte((queryString["type"] + "").Trim());
            var grouptype = Convert.ToByte((queryString["grouptype"] + "").Trim());
            var records = Enumerable.Empty<ItemGroup>().AsQueryable();
            if (grouptype == (byte)GroupType.ItemGroup)
                records = Repository.AsQueryable().Where(p => p.GroupType == (byte)GroupType.ItemGroup);
            if (grouptype == (byte)GroupType.CustomerIncentiveGroup)
                records = Repository.AsQueryable().Where(p => p.GroupType == (byte)GroupType.CustomerIncentiveGroup);

            records = records.Where(p => p.GroupSubType == type);

            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
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
            var sb = new StringBuilder();
            sb.Clear();

            var sr = 0;
            var rs = new JQueryResponse();
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                var data = new List<string>();
                //data.Add("<td><input type='checkbox' class='checkboxes' value='1' /></td>");
                data.Add(item.Name);
                if (grouptype == (byte)GroupType.ItemGroup)
                {
                    var editIcon = "<i class='fa fa-edit' onclick=\"ItemGroups.Edit(" + item.Id + ")\" title='Edit' ></i>";
                    var deleteIcon = "<i class='fa fa-trash-o' onclick=\"ItemGroups.Delete(" + item.Id + ")\" title='Delete' ></i>";
                    var icons = "<span class='action'>";
                    icons += editIcon;
                    icons += deleteIcon;
                    icons += "</span>";
                    data.Add(icons);
                }
                if (grouptype == (byte)GroupType.CustomerIncentiveGroup)
                {
                    var editIcon = "<i class='fa fa-edit' onclick=\"CustomerIncentiveGroups.Edit(" + item.Id + ")\" title='Edit' ></i>";
                    var deleteIcon = "<i class='fa fa-trash-o' onclick=\"CustomerIncentiveGroups.Delete(" + item.Id + ")\" title='Delete' ></i>";
                    var icons = "<span class='action'>";
                    icons += editIcon;
                    icons += deleteIcon;
                    icons += "</span>";
                    data.Add(icons);
                }
                rs.aaData.Add(data);
            }
            rs.sEcho = echo;
            rs.iTotalRecords = totalRecords;
            rs.iTotalDisplayRecords = totalDisplayRecords;
            return rs;
        }
    }
}
