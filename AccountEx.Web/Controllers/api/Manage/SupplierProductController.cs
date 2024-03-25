using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AccountEx.BussinessLogic;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.Repositories.COA;
using AccountEx.CodeFirst.Models.COA;

namespace AccountEx.Web.Controllers.api.Manage
{
    public class SupplierProductController : BaseApiController
    {
        public JQueryResponse Get()
        {
            return GetDataTable();
        }

        public ApiResponse Get(int supplierId, string key, int genericId, int brandId)
        {
            ApiResponse response;
            try
            {
                var products = new AccountDetailRepository().AsQueryable().Where(p => p.AccountDetailFormId == (int)AccountDetailFormType.Products && p.BrandId == brandId && p.GenericId == genericId).Select(p => new {Id=p.AccountId,Code=p.Code,Name=p.Name }).ToList();
                var alreadyUsedBySupplier = new SupplierProductRepository().AsQueryable().Where(p => p.SupplierId == supplierId).Select(p => p.ProductId).ToList();
                products = products.Where(p => !alreadyUsedBySupplier.Contains(p.Id)).Take(200).ToList();
                response = new ApiResponse
                {
                    Success = true,
                    Data = products
                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;

        }

        public ApiResponse Post(List<SupplierProduct> records)
        {
            ApiResponse response;
            try
            {
                new SupplierProductRepository().Save(true, records);
                response = new ApiResponse { Success = true };
            }

            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;

        }

        public ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                new SupplierProductRepository().Delete(id);
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
            var coloumns = new[] { "SupplierName", "ProductName", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var type = (queryString["type"] + "").Trim();
            var supplierId = Numerics.GetInt(queryString["supplierId"]);
            var dal = new vw_SupplierProductsRepository();
            var records = dal.AsQueryable().Where(p => p.SupplierId == supplierId);
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            //if (search != "")
            //    filteredList = records.Where(p =>
            //         p.Name.Contains(search)
            //       );
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
                data.Add(item.SupplierName);
                data.Add(item.ProductName);
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"SupplierProducts.Delete(" + item.Id + ")\" title='Delete' ></i>";
                var icons = "<span class='action'>";
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
