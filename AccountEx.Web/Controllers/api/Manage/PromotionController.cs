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
    public class PromotionController : GenericApiController<Promotion>
    {
        public override ApiResponse Post([FromBody]Promotion input)
        {
            ApiResponse response;
            try
            {
                string configName;
                var repo = new PromotionRepository();
                if (repo.IsAlreadyExistInDates(input.Id, input.FromDate, input.ToDate, out configName) == false)
                {
                    input.FiscalId = SiteContext.Current.Fiscal.Id;
                    if (input.Id == 0)
                    {
                        repo.Add(input);
                    }
                    else
                    {
                        repo.Update(input);
                    }
                    response = new ApiResponse { Success = true };
                }
                else
                {
                    response = new ApiResponse
                    {
                        Success = false,
                        Error = "Promtions already exist in applied dates with Name: " + configName,
                    };
                }

            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }


        public ApiResponse Get(int groupId, string key)
        {
            ApiResponse response;
            try
            {
                var query = new PromotionRepository().AsQueryable(true).Where(p => !p.PromotionItems.Any(q => q.CustomerGroupId.HasValue && q.CustomerGroupId == groupId));
                var group = new ItemGroupRepository().GetById(groupId);
                var records = query.ToList();
                records.ForEach(p => { p.PromotionItems = p.PromotionItems.Where(q => !q.CustomerGroupId.HasValue).ToList(); });
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Promotions = records,
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
        protected override JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "Name", "GroupId", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var type = (queryString["type"] + "").Trim();
            //var dal = new ProjectRepository();
            var records = Repository.AsQueryable();
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
                var itempromotiongroup=new ItemGroupRepository().GetById(item.GroupId);
                data.Add(itempromotiongroup.Name);
                var editIcon = "<i class='fa fa-edit' onclick=\"Promotions.Edit(" + item.Id + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"Promotions.Delete(" + item.Id + ")\" title='Delete' ></i>";
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
