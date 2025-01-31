using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AccountEx.BussinessLogic;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.CodeFirst.Models;
using AccountEx.Web.Controllers.api.Shared;

namespace AccountEx.Web.Controllers.api.Manage
{
    public class DoctorActivityController: GenericApiController<DoctorActivity>
    {
        public DoctorActivityController()
        {
        }
        public override JQueryResponse Get()
        {
                return GetDataTable();
        }

        protected override JQueryResponse GetDataTable()
        {
    
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "Code", "Tag", "Date", "Cattle Status", "Preg Status", "Breeding Status", "Days In Milk", "Bull Name", "Days Preg", "Month Preg", "Milk", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var numericsearch = Numerics.GetInt((queryString["sSearch"] + "").Trim());
            var repo = new GenericRepository<DoctorActivity>();
            var records = repo.AsQueryable();
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                                p.Code.Contains(search)
                            || p.Tag.Contains(search)
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
            var rs = new JQueryResponse();
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                var data = new List<string>();
                data.Add(item.Code);
                data.Add(item.Tag);
                data.Add(item.Date ?? "");
                data.Add(item.CattleStatus ?? "");
                data.Add(item.PregnanceyStatus ?? "");
                data.Add(item.BreedingStatus ?? "");
                data.Add(item.DaysInMilk ?? "");
                data.Add(item.BullName ?? "");
                data.Add(item.DaysPreg ?? "");
                data.Add(item.MonthPreg ?? "");
                data.Add(item.Milk ?? "");
                var editIcon = "<i class='fa fa-edit' onclick=\"Products.Edit(" + item.Id + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"Products.Delete(" + item.Id + ")\" title='Delete' ></i>";
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
