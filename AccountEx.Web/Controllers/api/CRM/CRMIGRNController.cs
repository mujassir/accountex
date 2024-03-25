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
using AccountEx.Common.CRM;

namespace AccountEx.Web.Controllers.api.CRM
{
    public class CRMIGRNController : BaseApiController
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
                case "GetNextInvoiceNo":
                    response = GetNextInvoiceNo();
                    break;


            }
            return response;
        }
        public ApiResponse Get(string piNumber, bool isEdit)
        {
            ApiResponse response;
            try
            {
                var repo = new CRMImportIGRNRepository();
                var records = repo.GetByPINumber(piNumber);
                var products = new List<vw_CRMPRoducts>();
                var salePersons = new List<IdName>();
                if (records.Count() > 0)
                {
                    var prductIds = records.Select(p => p.ProductId).ToList();
                    products = new CRMProductRepository().GetProducts(prductIds);
                    var salePersonIds = records.Select(p => p.SalePersonId).ToList();
                    salePersons = new UserRepository().GetIdNames(salePersonIds);

                }
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Invoices = records,
                        Products = products,
                        SalePersons = salePersons,
                    }
                };

            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        public ApiResponse Post([FromBody]CRMIGRNContainer IGRN)
        {
            ApiResponse response;
            try
            {
                var records = IGRN.Invoices.ToList();
                var err = CRMIGRNManager.ValidateSave(records);
                if (err == "")
                {
                    CRMIGRNManager.Save(records);
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
        public ApiResponse Post([FromBody]CRMIGRNImportContainerExtra input, bool import, bool isValidate)
        {
            ApiResponse response;
            try
            {

                CRMIGRNManager.Import(input, isValidate);
                response = new ApiResponse
                {
                    Success = true,
                    Data = null
                };
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
                var coloumns = new[] { "PINumber", "PIDate", "Customer", "Product", "Quantity", "Currency", "ExcRate", "Price", "NetTotalF", "NetTotal", "SaleType", "Project", "SalePerson", "" };
                var echo = Convert.ToInt32(queryString["sEcho"]);
                var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
                var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
                var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
                var search = (queryString["sSearch"] + "").Trim();
                var intSearch = Numerics.GetInt(search);
                var type = (queryString["type"] + "").Trim();
                var sortDirection = queryString["sSortDir_0"];
                var sortColumn = coloumns[colIndex];
                var filterType = Numerics.GetInt(queryString["FilterType"] + "");
                var productId = Numerics.GetInt(queryString["FilterProduct"] + "");
                var regionId = Numerics.GetInt(queryString["FilterRegion"] + "");
                var customerId = Numerics.GetInt(queryString["FilterCustomer"] + "");
                var invoiceNo = queryString["FilterInvoiceNumber"] + "";
                var salePersonId = Numerics.GetInt(queryString["FilterSalePersonId"] + "");
                var fromdate = DateConverter.ConvertStandardDate(queryString["FromDate"] + "");
                var todate = DateConverter.ConvertStandardDate(queryString["ToDate"] + "");




                var records = new GenericRepository<vw_CRMImportIGRN>().AsQueryable(true);
                if (SiteContext.Current.UserTypeId == CRMUserType.SalesExecutive)
                {
                    records = records.Where(p => p.SalePersonId == SiteContext.Current.User.Id);
                }
                else if (SiteContext.Current.UserTypeId == CRMUserType.RSM)
                {
                    records = records.Where(p => p.RegionId == SiteContext.Current.RegionId);
                }
                else if (SiteContext.Current.UserTypeId == CRMUserType.DivisionalHead)
                {
                    records = records.Where(p => p.DivisionId == SiteContext.Current.DivisionId);
                }

                if (!string.IsNullOrWhiteSpace(invoiceNo))
                    records = records.Where(p => p.InvoiceNo == invoiceNo);
                if (customerId > 0)
                    records = records.Where(p => p.CustomerId == customerId);
                if (productId > 0)
                    records = records.Where(p => p.ProductId == productId);
                if (regionId > 0)
                    records = records.Where(p => p.RegionId == regionId);
                if (salePersonId > 0)
                    records = records.Where(p => p.SalePersonId == salePersonId);

                if (!string.IsNullOrEmpty(queryString["FromDate"] + "") && !string.IsNullOrEmpty(queryString["ToDate"] + ""))
                    records = records.Where(p => p.InvoiceDate >= fromdate && p.InvoiceDate <= todate);
                var totalRecords = records.Count();
                var filteredList = records;
                if (search != "")
                    filteredList = records.Where(p =>

                         p.Customer.Contains(search) ||
                          p.SalePerson.Contains(search) ||
                           p.PINumber.Contains(search) ||
                            p.BLNo.Contains(search) ||
                           p.Product.Contains(search) ||
                             p.SalePerson.Contains(search) ||
                                 p.Currency.Contains(search) ||
                                  p.SaleType.Contains(search) ||
                                    p.DeliveryType.Contains(search) ||
                                      p.Project.Contains(search) ||
                         (intSearch > 0 && p.Quantity == intSearch) ||
                         (intSearch > 0 && p.ExcRate == intSearch) ||
                         (intSearch > 0 && p.NetTotal == intSearch)
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
                    data.Add(item.PINumber);
                    data.Add(item.PIDate.HasValue ? item.PIDate.Value.ToString(AppSetting.GridDateFormat) : "");
                    data.Add(item.Customer);
                    data.Add(item.Product);
                    data.Add(Numerics.IntToString(item.Quantity));
                    data.Add(item.Currency);
                    data.Add(Numerics.DecimalToString(item.ExcRate));
                    data.Add(Numerics.DecimalToString(item.Price));
                    data.Add(Numerics.IntToString(item.NetTotalF));
                    data.Add(Numerics.IntToString(item.NetTotal));
                    data.Add(item.SaleType);
                    data.Add(item.Project);
                    data.Add(item.SalePerson);
                    var editIcon = "<i class='fa fa-edit btn-edit' data-id='" + item.PINumber + "' title='Edit' ></i>";
                    var viewIcon = "<i class='fa fa-eye btn-view' data-id='" + item.PINumber + "' title='View' ></i>";
                    var deleteIcon = "<i class='fa fa-trash-o btn-delete' data-id='" + item.Id + "' title='Delete' ></i>";
                    var icons = "<span class='action'>";
                    icons += viewIcon;
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
                CRMIGRNManager.Delete(id);
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
                var data = new CRMProductRepository().GetOwnProducts();
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

        private ApiResponse GetSalePerson()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var customerId = Numerics.GetInt(queryString["customerId"]);
            var productId = Numerics.GetInt(queryString["productId"]);
            var categoryId = Numerics.GetInt(queryString["categoryId"]);
            ApiResponse response;
            try
            {
                var data = new CRMCustomerSalePersonRepository().GetSalePersonByCatgeoryId(customerId, categoryId).FirstOrDefault();
                response = new ApiResponse { Success = true, Data = data };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
        private ApiResponse GetNextInvoiceNo()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            ApiResponse response;
            try
            {
                var data = new CRMSaleInvoiceRepository().GetNGSTNextBookNumber();
                response = new ApiResponse { Success = true, Data = data };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
    }

    public class CRMIGRNContainer
    {
        public List<CRMImportGRN> Invoices { get; set; }
    }
}
