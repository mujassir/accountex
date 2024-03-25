using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Web.Controllers.api.Shared;
using System.Web.Http;
using AccountEx.Repositories;
using AccountEx.Repositories.Config;
using AccountEx.BussinessLogic;
using AccountEx.CodeFirst.Models.CRM;
using AccountEx.BussinessLogic.CRM;

namespace AccountEx.Web.Controllers.api.CRM
{
    public class CRMSaleForecastRSMController : BaseApiController
    {

        public ApiResponse Get(string dataKey)
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var response = new ApiResponse();
            var key = queryString["dataKey"];
            switch (key)
            {
                case "GetProducts":
                    response = GetProducts();
                    break;
                case "GetSalePerson":
                    response = GetSalePerson();
                    break;
                case "GetProjects":
                    response = GetProjects();
                    break;
                case "GetSale":
                    response = GetSale();
                    break;
            }
            return response;
        }
        public ApiResponse Get(int id)
        {

            var queryString = Request.RequestUri.ParseQueryString();
            var month = Numerics.GetInt(queryString["month"]);
            var year = Numerics.GetInt(queryString["year"]);
            var type = Numerics.GetByte(queryString["type"]);
            var productId = Numerics.GetInt(queryString["productId"]);
            ApiResponse response;
            try
            {
                var data = new CRMSaleInvoiceRepository().GetSaleForRSM(SiteContext.Current.User.Id, month, year, productId);
                response = new ApiResponse { Success = true, Data = data };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }

        public ApiResponse Post([FromBody]List<CRMSaleForecast> records)
        {
            ApiResponse response;
            try
            {
                var err = CRMSaleForecastManager.ValidateSave(records);
                if (err == "")
                {
                    CRMSaleForecastManager.SaveRSM(records);
                    response = new ApiResponse
                    {
                        Success = true,
                    };
                }
                else
                {
                    response = new ApiResponse()
                    {
                        Success = false,
                        Error = err
                    };
                }
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;

        }
        public JQueryResponse GetDataTable()
        {
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                var coloumns = new[] { "VoucherNumber", "Date", "Customer", "SalePerson", "" };
                var echo = Convert.ToInt32(queryString["sEcho"]);
                var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
                var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
                var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
                var search = (queryString["sSearch"] + "").Trim();
                var intSearch = Numerics.GetInt(search);
                var type = (queryString["type"] + "").Trim();
                //var dal = new ProjectRepository();
                var records = new GenericRepository<vw_CRMPMCS>().AsQueryable();
                var totalRecords = records.Count();

                var filteredList = records;
                if (search != "")
                    filteredList = records.Where(p =>

                         p.Customer.Contains(search) ||
                          p.SalePerson.Contains(search) ||
                         (intSearch > 0 && p.VoucherNumber == intSearch)

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
                var rs = new JQueryResponse();
                foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
                {
                    var data = new List<string>();
                    data.Add(item.VoucherNumber + "");
                    data.Add(item.Date.ToString(AppSetting.GridDateFormat));
                    data.Add(item.Customer);
                    data.Add(item.SalePerson);
                    var editIcon = "<i class='fa fa-edit btn-edit' data-id='" + item.VoucherNumber + "' title='Edit' ></i>";
                    var deleteIcon = "<i class='fa fa-trash-o btn-delete' data-id='" + item.VoucherNumber + "' title='Delete' ></i>";
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
            catch (Exception ex)
            {
                ErrorManager.Log(ex);
                throw;
            }


        }

        public ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                CRMSaleInvoiceManager.Delete(id);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }

            return response;
        }

        private ApiResponse GetProducts()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            ApiResponse response;
            try
            {
                var data = new GenericRepository<vw_CRMPRoducts>().GetAll();
                response = new ApiResponse { Success = true, Data = data };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
        private ApiResponse GetProjects()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var customerId = Numerics.GetInt(queryString["customerId"]);
            var productId = Numerics.GetInt(queryString["productId"]);
            var divisionId = Numerics.GetInt(queryString["divisionId"]);
            ApiResponse response;
            try
            {
                var data = new CRMProjectRepository().GetProjects(customerId, productId).FirstOrDefault();
                response = new ApiResponse { Success = true, Data = data };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
        private ApiResponse GetSale()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var month = Numerics.GetInt(queryString["month"]);
            var year = Numerics.GetInt(queryString["year"]);
            var salePersonId = Numerics.GetInt(queryString["salePersonId"]);
            var productId = Numerics.GetInt(queryString["productId"]);
            var type = Numerics.GetByte(queryString["type"]);
            ApiResponse response;
            try
            {
                var data = new CRMSaleInvoiceRepository().GetSaleByProductIds(salePersonId, month, year, productId, type);
                response = new ApiResponse { Success = true, Data = data };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }

        private ApiResponse GetSalePerson()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var customerId = Numerics.GetInt(queryString["customerId"]);
            var productId = Numerics.GetInt(queryString["productId"]);
            var divisionId = Numerics.GetInt(queryString["divisionId"]);
            ApiResponse response;
            try
            {
                var data = new CRMCustomerSalePersonRepository().GetSalePersonByCatgeoryId(customerId, divisionId).FirstOrDefault();
                response = new ApiResponse { Success = true, Data = data };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
    }
}
