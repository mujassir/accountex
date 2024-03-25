using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AccountEx.BussinessLogic;
using AccountEx.Common;

namespace AccountEx.Web.Controllers.api.Manage
{
    public class EquipmentController : AccountDetailController
    {
        public EquipmentController()
        {
            AccountDetailFormId = (int)AccountDetailFormType.Equipments;
            HeadAccountId = SettingManager.EquipmentHeadId;
        }
        public override JQueryResponse Get()
        {
            var type = (QueryString["type"] + "").Trim();
            if (type == "report")
                return GetDataTableForReport();
            else
                return GetDataTable();
        }

        protected override JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "Id", "Code", "Name", "PurchasePrice", "SalePrice", "Manufacturer", "Others", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var numericsearch = Numerics.GetInt((queryString["sSearch"] + "").Trim());
            var type = (queryString["type"] + "").Trim();
            //var dal = new ProjectRepository();
            var records = Repository.AsQueryable().Where(p => p.AccountDetailFormId == AccountDetailFormId);
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                    p.Code.Contains(search)
                    || p.Name.Contains(search)
                      || p.SalePrice == numericsearch
                       || p.PurchasePrice == numericsearch
                        || p.Name.Contains(search)
                    || p.Manufacturer.Contains(search)
                    || p.Others.Contains(search)
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
                if (type == "report")
                {
                    //data.Add((++sr) + "");
                    data.Add(item.GroupId + "");
                    //data.Add(item.GroupName);
                }
                //else data.Add("<td><input type='checkbox' class='checkboxes' value='1' /></td>");
                data.Add(item.Code);
                data.Add(item.Name);
                data.Add(item.PackingPerCarton + "");
                data.Add(item.PurchasePrice + "");
                data.Add(item.SalePrice + "");
                if (type != "report")
                {
                    data.Add(item.Manufacturer);
                    data.Add(item.Others);
                }
                var editIcon = "<i class='fa fa-edit' onclick=\"Equipments.Edit(" + item.Id + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"Equipments.Delete(" + item.Id + ")\" title='Delete' ></i>";
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
        protected JQueryResponse GetDataTableForReport()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "Id", "Code", "Name", "PurchasePrice", "SalePrice", "Manufacturer", "Others", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var numericsearch = Numerics.GetInt((queryString["sSearch"] + "").Trim());
            var type = (queryString["type"] + "").Trim();
            //var dal = new ProjectRepository();
            var records = Repository.AsQueryable().Where(p => p.AccountDetailFormId == AccountDetailFormId);
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                    p.Code.Contains(search)
                    || p.Name.Contains(search)
                      || p.SalePrice == numericsearch
                       || p.PurchasePrice == numericsearch
                        || p.Name.Contains(search)
                    || p.Manufacturer.Contains(search)
                    || p.Others.Contains(search)
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
            foreach (var item in orderedList.Where(p => p.GroupName != null).OrderBy(p => p.GroupName).Skip(displayStart).Take(displayLength))
            {
                var data = new List<string>
                {
                    type == "report"
                        ? item.GroupName
                        : "<td><input type='checkbox' class='checkboxes' value='1' /></td>",
                    item.Code,
                    item.Name,
                    item.PackingPerCarton + "",
                    item.PurchasePrice + "",
                    item.SalePrice + ""
                };
                if (type != "report")
                {
                    data.Add(item.Manufacturer);
                    data.Add(item.Others);
                }
                var editIcon = "<i class='fa fa-edit' onclick=\"Equipments.Edit(" + item.AccountId + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"Equipments.Delete(" + item.Id + ")\" title='Delete' ></i>";
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
