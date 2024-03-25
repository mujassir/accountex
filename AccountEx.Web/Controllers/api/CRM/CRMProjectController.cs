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
    public class CRMProjectController : BaseApiController
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
            }
            return response;
        }
        public ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                var voucherNumber = id;
                var invoiceNumber = id;
                var queryString = Request.RequestUri.ParseQueryString();
                var type = queryString["type"];
                var key = queryString["key"].ToLower();
                var vouchertype = (VoucherType)Convert.ToByte(type);
                //var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
                bool next, previous;
                var projectRepo = new CRMProjectRepository();

                if (voucherNumber == 0) key = "nextvouchernumber";
                if (key == "nextvouchernumber")
                {
                    invoiceNumber = voucherNumber = projectRepo.GetNextVoucherNumber();
                }
                var data = projectRepo.GetById(id);
                var pmcItem = new vw_CRMPMCItems();
                if (data != null && data.PMCItemId > 0)
                {

                    pmcItem = new PMCItemRepository().GetByIdFromView(Numerics.GetInt(data.PMCItemId));
                }
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Project = data,
                        PMC = pmcItem,
                        Next = false,
                        Previous = false,
                        VoucherNumber = voucherNumber,
                        InvoiceNumber = invoiceNumber,
                    }
                };

            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        public ApiResponse Post([FromBody]CRMProject input)
        {
            ApiResponse response;
            try
            {
                var err = CRMProjectManager.ValidateSave(input);
                if (err == "")
                {
                    input.FiscalId = SiteContext.Current.Fiscal.Id;
                    CRMProjectManager.Save(input);
                    response = new ApiResponse
                    {
                        Success = true,
                        Data = input.Id
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
        public ApiResponse Post(bool changeSaleType, int customerId, int productId)
        {
            ApiResponse response;
            try
            {
                var err = CRMProjectManager.ValidateSaleTypeChange(customerId, productId);
                if (err == "")
                {
                    CRMProjectManager.ChangeSaleType(customerId, productId);
                    response = new ApiResponse
                    {
                        Success = true,
                        Data = null
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
                var coloumns = new[] { "Customer", "ActualProduct", "ActualProductVendor", "Product", "ActualCurrency", "Currency", "ActualPrice", "Price", "ActualQuantity", "Quantity", "ActualPotential", "Potential", "PotentialPercent", "Status", "" };
                var echo = Convert.ToInt32(queryString["sEcho"]);
                var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
                var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
                var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
                var search = (queryString["sSearch"] + "").Trim();
                var intSearch = Numerics.GetInt(search);
                var type = (queryString["type"] + "").Trim();

                var fiscalId = Numerics.GetInt(queryString["fiscalId"] + "");
                if (fiscalId == 0)
                    fiscalId = SiteContext.Current.Fiscal.Id;

                var filterType = Numerics.GetInt(queryString["FilterType"] + "");
                var productId = Numerics.GetInt(queryString["FilterProduct"] + "");
                var divisionId = Numerics.GetInt(queryString["FilterDivision"] + "");
                var customerId = Numerics.GetInt(queryString["FilterCustomer"] + "");
                var salePersonId = Numerics.GetInt(queryString["FilterSalePersonId"] + "");


                //var dal = new ProjectRepository();
                var records = new GenericRepository<vw_CRMProjects>().AsQueryable();
                records = records.Where(p => p.FiscalId == fiscalId);


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
                if (customerId > 0)
                    records = records.Where(p => p.CustomerId == customerId);
                if (productId > 0)
                    records = records.Where(p => p.ProductId == productId);
                if (divisionId > 0)
                    records = records.Where(p => p.DivisionId == divisionId);
                if (salePersonId > 0)
                    records = records.Where(p => p.SalePersonId == salePersonId);

                if (filterType > 0)
                {
                    if (filterType == 1)
                        records = records.Where(p => p.PMCItemId == 0);
                    else if (filterType == 2)
                        records = records.Where(p => p.PMCItemId > 0);
                }

                var totalRecords = records.Count();
                var filteredList = records;
                if (search != "")
                    filteredList = records.Where(p =>

                         p.Customer.Contains(search) ||
                         p.ActualProduct.Contains(search) ||
                          p.ActualProductVendor.Contains(search) ||
                         p.Product.Contains(search) ||
                          p.Currency.Contains(search) ||
                            p.ActualCurrency.Contains(search) ||
                          p.Status.Contains(search) ||
                         (intSearch > 0 && p.VoucherNumber == intSearch) ||
                         (intSearch > 0 && p.ActualPrice == intSearch) ||
                         (intSearch > 0 && p.Price == intSearch) ||
                         (intSearch > 0 && p.ActualQuantity == intSearch) ||
                         (intSearch > 0 && p.Quantity == intSearch) ||
                         (intSearch > 0 && p.ActualPotential == intSearch) ||
                         (intSearch > 0 && p.Potential == intSearch) ||
                         (intSearch > 0 && p.PotentialPercent == intSearch)
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
                    data.Add(item.Customer);
                    data.Add(item.ActualProduct);
                    data.Add(item.ActualProductVendor);
                    data.Add(item.Product);
                    data.Add(item.ActualCurrency);
                    data.Add(item.Currency);
                    data.Add(Numerics.DecimalToString(item.ActualPrice));
                    data.Add(Numerics.DecimalToString(item.Price));
                    data.Add(Numerics.IntToString(item.ActualQuantity));
                    data.Add(Numerics.IntToString(item.Quantity));
                    data.Add(Numerics.IntToString(item.ActualPotential));
                    data.Add(Numerics.IntToString(item.Potential));
                    data.Add(Numerics.IntToString(item.PotentialPercent));
                    data.Add(item.Status);
                    var editIcon = "<i class='fa fa-edit btn-edit' data-id='" + item.Id + "' title='Edit' ></i>";
                    var viewIcon = "<i class='fa fa-eye btn-view' data-id='" + item.Id + "' title='View' ></i>";
                    var deleteIcon = "<i class='fa fa-trash-o btn-delete' data-id='" + item.Id + "' title='Delete' ></i>";
                    var icons = "<span class='action'>";

                    icons += viewIcon;
                    if (SiteContext.Current.Fiscal.Id == item.FiscalId && ((SiteContext.Current.User.IsAdmin || SiteContext.Current.UserTypeId == CRMUserType.Admin) || (!FiscalSettingManager.IsProjectLocked)))
                    {
                        icons += editIcon;
                        icons += deleteIcon;
                    }
                    icons += "</span>";
                    if (type != "report") data.Add(icons);
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
                var err = CRMProjectManager.ValidatDelete(id);
                if (err == "")
                {
                    var queryString = Request.RequestUri.ParseQueryString();
                    CRMProjectManager.Delete(id);
                    response = new ApiResponse { Success = true };
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

    }
}
