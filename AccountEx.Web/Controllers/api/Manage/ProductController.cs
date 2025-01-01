using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AccountEx.BussinessLogic;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.CodeFirst;
using AccountEx.CodeFirst.Models;
using System.Web.Http;
using System.Web;
using AccountEx.CodeFirst.Mapping;

namespace AccountEx.Web.Controllers.api.Manage
{
    public class ProductController : AccountDetailController
    {
        public ProductController()
        {
            AccountDetailFormId = (int)AccountDetailFormType.Products;
            HeadAccountId = SettingManager.ProductHeadId;
        }
        public override JQueryResponse Get()
        {
            var type = (QueryString["type"] + "").Trim();
            if (type == "report")
                return GetDataTableForReport();
            if (type == "DairyProducts")
                return GetDataTableForDiaryProducts();
            else
                return GetDataTable();
        }
        public ApiResponse Get(bool loadBarcodeData)
        {

            return GetDataForBarcodePrinting();
        }

        public ApiResponse Post([FromBody]List<IdsExtra> records, bool printBarcode)
        {
            ApiResponse response;
            try
            {

                string baseUrl = Request.RequestUri.Scheme + "://" + Request.RequestUri.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/";
                var filePath = BarCodeManager.PrintBarcode(records,baseUrl);
                response = new ApiResponse { Success = true, Data = filePath };
            }
            catch (Exception ex)
            {
                 ;
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }

            return response;

        }

        protected override JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "Code", "Name", "PackingPerCarton", "PurchasePrice", "SalePrice", "Manufacturer", "Others", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var numericsearch = Numerics.GetInt((queryString["sSearch"] + "").Trim());
            var records = Repository.AsQueryable().Where(p => p.AccountDetailFormId == AccountDetailFormId && p.Route != "#DairyProducts#");
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                                p.Code.Contains(search)
                            || p.Name.Contains(search)
                            || (numericsearch != 0 && p.SalePrice == numericsearch)
                            || (numericsearch != 0 && p.PurchasePrice == numericsearch)
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
            var rs = new JQueryResponse();
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                var data = new List<string>();
                data.Add(item.Code);
                data.Add(item.Name);
                data.Add(item.PackingPerCarton + "");
                data.Add(item.PurchasePrice + "");
                data.Add(item.SalePrice + "");
                data.Add(item.Manufacturer);
                data.Add(item.Others);
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
        protected JQueryResponse GetDataTableForDiaryProducts()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var type = (QueryString["type"] + "").Trim();
            var coloumns = new[] { "Code", "BarCode", "Name", "Company", "Readings", "Weight", "UnitType", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var numericsearch = Numerics.GetInt((queryString["sSearch"] + "").Trim());
            var records = Repository.AsQueryable().Where(p => p.AccountDetailFormId == AccountDetailFormId && p.Route == "#" + type + "#");
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                                p.Code.Contains(search)
                            || p.Name.Contains(search)
                            || (numericsearch != 0 && p.SalePrice == numericsearch)
                            || (numericsearch != 0 && p.PurchasePrice == numericsearch)
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
            var rs = new JQueryResponse();
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                var data = new List<string>();
                data.Add(item.Code);
                data.Add(item.BarCode);
                data.Add(item.Name);
                data.Add(item.Company);
                data.Add(item.Readings);
                data.Add(item.Weight.ToString());
                data.Add(item.UnitType);
                data.Add(item.PurchasePrice.ToString());
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
        protected JQueryResponse GetDataTableForReport()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "Code", "Name", "PackingPerCarton", "Unit", "PurchasePrice", "SalePrice" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var numericsearch = Numerics.GetInt((queryString["sSearch"] + "").Trim());
            var groups = new GenericRepository<ProductGroup>().AsQueryable().ToList();
            var records = Repository.AsQueryable().Where(p => p.AccountDetailFormId == AccountDetailFormId);
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                    p.Code.Contains(search)
                    || p.Name.Contains(search)
                    || p.SalePrice == numericsearch
                    || p.PurchasePrice == numericsearch
                    );
            var totalRecords = filteredList.Count();
            var totalDisplayRecords = totalRecords;
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
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength).OrderBy(p => p.GroupId))
            {
                var group = groups.FirstOrDefault(p => p.Id == item.GroupId);
                var data = new List<string>
                {
                    (group!=null ?group.Name:""),
                    item.Code,
                    item.Name,
                    item.PackingPerCarton + "",
                    item.Unit + "",
                    item.PurchasePrice + "",
                    item.SalePrice + ""
                };
                rs.aaData.Add(data);
            }
            rs.sEcho = echo;
            rs.iTotalRecords = totalRecords;
            rs.iTotalDisplayRecords = totalDisplayRecords;
            return rs;
        }
        private ApiResponse GetDataForBarcodePrinting()
        {
            ApiResponse response;
            try
            {
                var groups = new GenericRepository<ProductGroup>().AsQueryable().ToList();
                var records = Repository.AsQueryable().Where(p => p.AccountDetailFormId == AccountDetailFormId).Select(p => new
                {
                    p.Id,
                    p.AccountId,
                    p.Code,
                    p.Name,
                    p.PurchasePrice,
                    p.SalePrice,
                    p.Manufacturer,
                    p.ArticleNo
                });
                response = new ApiResponse { Success = true, Data = records };
            }
            catch (Exception ex)
            {

                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
                 ;
            }
            return response;


        }
    }
}
