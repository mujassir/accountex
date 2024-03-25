using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AccountEx.BussinessLogic;
using AccountEx.Common;

namespace AccountEx.Web.Controllers.api.Manage
{
    public class SalesmanController : AccountDetailController
    {
        public SalesmanController()
        {
            AccountDetailFormId = (int)AccountDetailFormType.Salesman;
            HeadAccountId = SettingManager.SalemanHeadId;
        }

        protected override JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "Id", "Code", "Name", "ContactNumber", "Email", "Address", "" };
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
                data.Add(item.ContactNumber);
                data.Add(item.Email);
                data.Add(item.Address);

                var editIcon = "<i class='fa fa-edit' onclick=\"Salesman.Edit(" + item.Id + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"Salesman.Delete(" + item.Id + ")\" title='Delete' ></i>";
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
