using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AccountEx.BussinessLogic;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.CodeFirst.Models;

namespace AccountEx.Web.Controllers.api.Manage
{
    public class MedicineItemController : AccountDetailController
    {
        public MedicineItemController()
        {
            AccountDetailFormId = (int)AccountDetailFormType.Products;
            HeadAccountId = SettingManager.ProductHeadId;
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
            var coloumns = new[] { "Code", "Name", "Brand", "Generic", "PackagingType", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var type = (queryString["type"] + "").Trim();
            //var dal = new ProjectRepository();
            var records = new GenericRepository<vw_PharmacyProduct>().AsQueryable();
            var totalRecords = records.Count();
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>

                     p.Name.Contains(search) ||
                      p.Code.Contains(search) ||
                        p.Generic.Contains(search) ||
                          p.Brand.Contains(search) ||
                            p.PackagingType.Contains(search)


                   );

            var totalDisplayRecords = filteredList.Count();
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
                if (type != "report")
                    data.Add("<td><input type='checkbox' class='checkboxes' value='1' /></td>");
                data.Add(item.Code);
                data.Add(item.Name);
                data.Add(item.Brand);
                data.Add(item.Generic);
                data.Add(item.PackagingType + "");
                data.Add(item.Location);


                var editIcon = "<i class='fa fa-edit' onclick=\"MedicineItem.Edit(" + item.Id + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"MedicineItem.Delete(" + item.Id + ")\" title='Delete' ></i>";
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
            var coloumns = new[] { "Code", "Name", "Brand", "Generic", "PackagingType", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var type = (queryString["type"] + "").Trim();
            //var dal = new ProjectRepository();
            var records = Repository.AsQueryable();
            records = Repository.AsQueryable().Where(p => p.AccountDetailFormId == AccountDetailFormId);
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>

                     p.Name.Contains(search) ||
                      p.Code.Contains(search) ||
                        p.Generic.Contains(search) ||
                          p.Brand.Contains(search) ||
                            p.PackagingType.Contains(search)


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
            var medicinedata = orderedList.Skip(displayStart).Take(displayLength).OrderBy(p => p.Code);
            foreach (var item in medicinedata)
            {
                var data = new List<string>();

                data.Add(item.Code);
                data.Add(item.Name);
                data.Add(item.Brand);
                data.Add(item.Generic);
                data.Add(item.PackagingType + "");
                rs.aaData.Add(data);
            }
            rs.sEcho = echo;
            rs.iTotalRecords = totalRecords;
            rs.iTotalDisplayRecords = totalDisplayRecords;
            return rs;
        }


    }
}
