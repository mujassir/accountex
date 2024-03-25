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
    public class PromotionAssignmentController : BaseApiController
    {

        public JQueryResponse Get()
        {

            return GetDataTable();

        }
        public ApiResponse Get(int groupId, int promotionId, string key)
        {
            ApiResponse response;
            try
            {
                var query = new PromotionRepository().AsQueryable(true).FirstOrDefault(p => p.PromotionItems.Any(q => q.CustomerGroupId == groupId && q.PromotionId == promotionId));
                var group = new ItemGroupRepository().GetById(groupId);
                var records = query;
                records.PromotionItems = records.PromotionItems.Where(q => q.CustomerGroupId.HasValue).ToList();
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
        public ApiResponse Delete(int groupId, int promotionId, string key)
        {
            ApiResponse response;
            try
            {
                new PromotionItemRepository().DeleteByCustomerGroupId(groupId,promotionId);
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
        public ApiResponse Post([FromBody]List<PromotionItem> Items)
        {
            ApiResponse response;
            try
            {
                new PromotionItemRepository().Save(Items);
                response = new ApiResponse { Success = true };
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
            var records = new PromotionRepository().GetCustomerPromotions();
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>

                     p.Group.Contains(search) ||
                      p.Promotion.Contains(search)

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
                data.Add(item.Promotion);
                data.Add(item.FromDate.ToString("dd/MM/yyy"));
                data.Add(item.ToDate.ToString("dd/MM/yyy"));
                var editIcon = "<i class='fa fa-edit' onclick=\"promotionassignment.Edit(" + item.GroupId + "," + item.PromotionId + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"promotionassignment.Delete(" + item.GroupId + "," + item.PromotionId + ")\" title='Delete' ></i>";
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
