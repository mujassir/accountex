using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.Web.Controllers.api.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace AccountEx.Web.Controllers.api.CRM
{
    public class FollowUpController : BaseApiController
    {
        public JQueryResponse Get()
        {
            return GetDataTable();
        }
        protected JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "LeadOwner", "Company", "NoOfMeetings", "Mobile", "NextFollowUp" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var intSearch = Numerics.GetInt((queryString["sSearch"] + "").Trim());
            var type = (queryString["type"] + "").Trim();

            var dal = new FollowUpRepository();
            var records = dal.AsQueryable();
            //var records = Repository.AsQueryable();
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>

                     p.Company.Contains(search) ||
                     p.LeadOwner.Contains(search) ||
                     p.NoOfMeetings == intSearch

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

            var sr = 0;
            var rs = new JQueryResponse();
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                var data = new List<string>();
                //data.Add("<td><input type='checkbox' class='checkboxes' value='1' /></td>");
                //data.Add("<a href='/crm/leadactivities?leadid=" + item.Id + "' target='_blank'>" + item.FirstName + " " + item.LastName + "</a>");
                data.Add("<a href='/crm/leadactivities?leadid=" + item.Id + "' target='_blank'>" + item.LeadOwner + " </a>");
                data.Add(item.Company);
                data.Add(item.NoOfMeetings + "");
                //data.Add(item.MeetingStatus);
                //data.Add(item.InterestLevel);
                data.Add(item.Mobile);
                //data.Add(item.FollowUpStatus);
                data.Add(item.NextFollowUp.HasValue ? item.NextFollowUp.Value.ToString(AppSetting.GridDateFormat) : "No");
                //var editIcon = "<i class='fa fa-edit' onclick=\"Leads.Edit(" + item.Id + ")\" title='Edit' ></i>";
                //var deleteIcon = "<i class='fa fa-trash-o' onclick=\"Leads.Delete(" + item.Id + ")\" title='Delete' ></i>";
                //var icons = "<span class='action'>";
                //icons += editIcon;
                //icons += deleteIcon;
                //icons += "</span>";
                //if (type != "report") data.Add(icons);
                rs.aaData.Add(data);
            }
            rs.sEcho = echo;
            rs.iTotalRecords = totalRecords;
            rs.iTotalDisplayRecords = totalDisplayRecords;
            return rs;
        }

    }
}
