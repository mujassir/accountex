using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AccountEx.BussinessLogic;
using AccountEx.Common;
using AccountEx.Repositories;

namespace AccountEx.Web.Controllers.api.Manage
{
    public class CustomerController : AccountDetailController
    {

        public CustomerController()
        {
            AccountDetailFormId = (int)AccountDetailFormType.Customers;
            HeadAccountId = SettingManager.CustomerHeadId;
        }
        public override JQueryResponse Get()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var type = (queryString["type"] + "").Trim();
            var extraKey = (queryString["extraKey"] + "").Trim();
            if (type == "report" && extraKey == "group")
                return GetDataTableForReportWithGroup();
            else if (type == "report")
                return GetDataTableForReport();
            else
                return GetDataTable();
        }
        protected override JQueryResponse GetDataTable()
        {

            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "Id", "Code", "Name", "BankName", "NTN", "GST", "ContactNumber", "ContactPerson", "Email", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
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
                    || p.BankName.Contains(search)
                    || p.NTN.Contains(search)
                    || p.ContactNumber.Contains(search)
                    || p.ContactPerson.Contains(search)
                    || p.GST.Contains(search)
                    || p.Email.Contains(search)
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
                if (type == "report") data.Add((++sr) + "");
                //else data.Add("<td><input type='checkbox' class='checkboxes' value='1' /></td>");
                data.Add(item.Code);
                data.Add(item.Name);
                if (type == "report") data.Add(item.GroupName);
                data.Add(item.BankName);
                data.Add(item.NTN);
                data.Add(item.GST);
                data.Add(item.ContactNumber);
                data.Add(item.ContactPerson);
                data.Add(item.Email);
                var editIcon = "<i class='fa fa-edit' onclick=\"Customers.Edit(" + item.Id + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"Customers.Delete(" + item.Id + ")\" title='Delete' ></i>";
                //var icons = "<span class='action'><a class='btn default blue-stripe btn-xs' href='../reports/generalledger?accountId=" + item.AccountId + "'>Ledger</a>";
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
            var coloumns = new[] { "Id", "Code", "Name", "GroupName", "CityName", "BankName", "NTN", "GST", "ContactNumber", "ContactPerson", "Email", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var type = (queryString["type"] + "").Trim();
            //var dal = new ProjectRepository();
            var records = Repository.AsQueryable().Where(p => p.AccountDetailFormId == AccountDetailFormId);
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            var sr = 0;
            if (search != "")
                filteredList = records.Where(p =>
                    p.Code.Contains(search)
                    || p.Name.Contains(search)
                    || p.BankName.Contains(search)
                    || p.NTN.Contains(search)
                    || p.ContactNumber.Contains(search)
                    || p.ContactPerson.Contains(search)
                    || p.GST.Contains(search)
                    || p.Email.Contains(search)
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
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength).ToList())
            {
                var data = new List<string>();
                data.Add((++sr) + "");


                data.Add(item.Code);
                data.Add(item.Name);
                data.Add(item.GroupName);
                data.Add(item.CityName);
                data.Add(item.BankName);
                data.Add(item.NTN);
                data.Add(item.GST);
                data.Add(item.ContactNumber);
                data.Add(item.ContactPerson);
                data.Add(item.Email);
                rs.aaData.Add(data);
            }
            rs.sEcho = echo;
            rs.iTotalRecords = totalRecords;
            rs.iTotalDisplayRecords = totalDisplayRecords;
            return rs;
        }
        protected JQueryResponse GetDataTableForReportWithGroup()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "Id", "Code", "Name", "BankName", "NTN", "GST", "ContactNumber", "ContactPerson", "Email", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var type = (queryString["type"] + "").Trim();
            //var dal = new ProjectRepository();
            var records = Repository.AsQueryable().Where(p => p.AccountDetailFormId == AccountDetailFormId);
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            var sr = 0;
            if (search != "")
                filteredList = records.Where(p =>
                    p.Code.Contains(search)
                    || p.Name.Contains(search)
                    || p.BankName.Contains(search)
                    || p.NTN.Contains(search)
                    || p.ContactNumber.Contains(search)
                    || p.ContactPerson.Contains(search)
                    || p.GST.Contains(search)
                    || p.Email.Contains(search)
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
            var recordListing = orderedList.OrderBy(p => p.GroupName).ThenBy(p => p.CityName).Skip(displayStart).Take(displayLength).ToList();
            //var recordListing = orderedList.Skip(displayStart).Take(displayLength).ToList();
            var accountIds = recordListing.Select(p => p.AccountId).ToList();
            var orderTakerIds = recordListing.Where(p => p.OrderTakerId.HasValue).Select(p => p.OrderTakerId.Value).ToList();

            var accountsWithParents = new AccountRepository().AsQueryable().Where(p => accountIds.Contains(p.Id)).Select(p => new
            {
                p.Id,
                p.AccountCode,
                p.ParentId,
                p.Name
            }).ToList();

            var parentIds = accountsWithParents.Where(p => p.ParentId.HasValue).Select(p => p.ParentId.Value).ToList();
            parentIds.AddRange(orderTakerIds);
            var accounts = new AccountRepository().AsQueryable().Where(p => parentIds.Contains(p.Id)).Select(p => new
           {
               p.Id,
               p.AccountCode,
               p.Name
           }).ToList();

            var rs = new JQueryResponse();
            foreach (var item in recordListing)
            {
                var data = new List<string>();
                data.Add((++sr) + "");
                var parentId = accountsWithParents.FirstOrDefault(p => p.Id == item.AccountId).ParentId;
                var parentName = "";
                if (accounts.Any(p => p.Id == item.ParentId))
                    parentName = accounts.FirstOrDefault(p => p.Id == item.ParentId).Name;
                var orderTakerName = "";
                if (accounts.Any(p => p.Id == item.OrderTakerId))
                    orderTakerName = accounts.FirstOrDefault(p => p.Id == item.OrderTakerId).Name;
                data.Add(parentName);
                data.Add(item.ParentVendorAccountId > 0 ? "Child" : "Parent");
                data.Add(item.Code);
                data.Add(item.Name);
                data.Add(item.GroupName);
                data.Add(orderTakerName);
                data.Add(item.SalemanName);
                rs.aaData.Add(data);
            }
            rs.sEcho = echo;
            rs.iTotalRecords = totalRecords;
            rs.iTotalDisplayRecords = totalDisplayRecords;
            return rs;
        }
    }
}
