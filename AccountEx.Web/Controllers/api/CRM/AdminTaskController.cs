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
    public class AdminTaskController : BaseApiController
    {

        public ApiResponse Get(string key)
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var response = new ApiResponse()
            {
                Success = true,
                Data = new { IsProjectLocked = FiscalSettingManager.IsProjectLocked, FiscalSettingManager.IsPmcLocked, FiscalSettingManager.IsProjectPmcTransferred }
            };

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
                var customerId = Numerics.GetInt(queryString["customerId"]);
                var vouchertype = (VoucherType)Convert.ToByte(type);
                //var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
                var pmcRepo = new PMCRepository();

                if (voucherNumber == 0 && key != "getbycutomerid") key = "nextvouchernumber";
                if (key == "nextvouchernumber")
                {
                    invoiceNumber = voucherNumber = pmcRepo.GetNextVoucherNumber();
                }
                var data = new PMC();
                if (key == "getbycutomerid" && customerId > 0)
                {

                    data = pmcRepo.GetByCustomerId(customerId);
                }

                else
                    data = pmcRepo.GetById(id);



                var products = new List<vw_CRMPRoducts>();
                var salePersons = new List<IdName>();
                var projectPmcItemIds = new List<int>();
                if (data != null && data.PMCItems.Count() > 0)
                {

                    if (SiteContext.Current.UserTypeId == CRMUserType.SalesExecutive)
                    {
                        data.PMCItems = data.PMCItems.Where(p => p.SalePersonId == SiteContext.Current.User.Id).ToList();
                    }
                    else if (SiteContext.Current.UserTypeId == CRMUserType.RSM)
                    {
                        //
                    }
                    else if (SiteContext.Current.UserTypeId == CRMUserType.DivisionalHead)
                    {
                        var dbProductIds = data.PMCItems.Select(p => p.ProductId).ToList();
                        var DHproductIds = new CRMProductRepository().GetProductIdsByDivision(dbProductIds, SiteContext.Current.DivisionId);
                        data.PMCItems = data.PMCItems.Where(p => DHproductIds.Contains(p.ProductId)).ToList();
                    }


                    var prductIds = data.PMCItems.Select(p => p.ProductId).ToList();
                    products = new CRMProductRepository().GetProducts(prductIds);


                    var salePersonIds = data.PMCItems.Select(p => p.SalePersonId).ToList();
                    salePersons = new UserRepository().GetIdNames(salePersonIds);

                    var pmcItemIds = data.PMCItems.Select(p => p.Id).ToList();
                    projectPmcItemIds = new CRMProjectRepository().GetPmcItemIds(data.CustomerId, pmcItemIds);

                }
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        PMC = data,
                        Products = products,
                        SalePersons = salePersons,
                        ProjectPmcItemIds = projectPmcItemIds,
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

        public ApiResponse Post(string key)
        {
            var response = new ApiResponse();
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                switch (key)
                {
                    case "transferProjectPmc":
                        AdminTaskManager.TransferProjectPMC();
                        response = new ApiResponse { Success = true, Data = FiscalSettingManager.IsProjectPmcTransferred };
                        break;
                    case "lockProject":
                        AdminTaskManager.LockProject();
                        response = new ApiResponse { Success = true, Data = FiscalSettingManager.IsProjectLocked };
                        break;
                    case "lockPmc":
                        AdminTaskManager.LockPMC();
                        response = new ApiResponse { Success = true, Data = FiscalSettingManager.IsPmcLocked };
                        break;
                    case "createPmc":
                        AdminTaskManager.CreateMissingPMC();
                        response = new ApiResponse { Success = true, Data = FiscalSettingManager.IsPmcLocked };
                        break;
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
                var coloumns = new[] { "SN", "Customer", "SalePerson", "Product", "Vendor", "Division", "Currency", "Price", "AnnualQty", "AnnualPotential", "AnnualRPLPotential", "IsCounter", "IsActive", "Project", "" };
                var echo = Convert.ToInt32(queryString["sEcho"]);
                var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
                var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
                var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
                var search = (queryString["sSearch"] + "").Trim();
                var intSearch = Numerics.GetInt(search);
                var type = (queryString["type"] + "").Trim();

                var fiscalId = Numerics.GetInt(queryString["fiscalId"] + "");
                var productId = Numerics.GetInt(queryString["FilterProduct"] + "");
                var divisionId = Numerics.GetInt(queryString["FilterDivision"] + "");
                var customerId = Numerics.GetInt(queryString["FilterCustomer"] + "");
                var salePersonId = Numerics.GetInt(queryString["FilterSalePersonId"] + "");


                if (fiscalId == 0)
                    fiscalId = SiteContext.Current.Fiscal.Id;

                //var dal = new ProjectRepository();
                var records = new GenericRepository<vw_CRMPMCItems>().AsQueryable();

                records = records.Where(p => p.FiscalId == fiscalId);

                if (customerId > 0)
                    records = records.Where(p => p.CustomerId == customerId);
                if (productId > 0)
                    records = records.Where(p => p.ProductId == productId);
                if (divisionId > 0)
                    records = records.Where(p => p.DivisionId == divisionId);
                if (salePersonId > 0)
                    records = records.Where(p => p.SalePersonId == salePersonId);

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

                var totalRecords = records.Count();
                var filteredList = records;
                if (search != "")
                    filteredList = records.Where(p =>

                         p.Customer.Contains(search) ||
                          p.SalePerson.Contains(search) ||
                           p.Product.Contains(search) ||
                             p.SalePerson.Contains(search) ||
                               p.Division.Contains(search) ||
                                 p.Currency.Contains(search) ||
                                  p.IsCounter.Contains(search) ||
                                    p.IsActive.Contains(search) ||
                                      p.Project.Contains(search) ||
                                        p.Vendor.Contains(search) ||

                          p.SalePerson.Contains(search) ||
                         (intSearch > 0 && p.SN == intSearch)

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
                    data.Add(item.SN + "");
                    data.Add(item.Customer);
                    data.Add(item.SalePerson);
                    data.Add(item.Product);
                    data.Add(item.Vendor);
                    data.Add(item.Division);
                    data.Add("<span class='cursor-pointer' title='Exc. Rate:" + Numerics.IntToString(item.ExcRate) + "' data-toggle='tooltip'>" + item.Currency + "<span>");
                    data.Add(Numerics.DecimalToString(item.Price));
                    data.Add(Numerics.IntToString(item.AnnualQty));
                    data.Add(Numerics.IntToString(item.AnnualPotential));
                    data.Add(Numerics.IntToString(item.AnnualRPLPotential));
                    data.Add(item.IsCounter);
                    var editIcon = "<i class='fa fa-edit btn-edit' data-id='" + item.PmcId + "' title='Edit' ></i>";
                    var viewIcon = "<i class='fa fa-eye btn-view' data-id='" + item.PmcId + "' title='View' ></i>";
                    var deleteIcon = "<i class='fa fa-trash-o btn-delete' data-id='" + item.Id + "' title='Delete' ></i>";
                    var projectButton = "<a data-id='" + item.Id + "' class='action-grid-create-project btn green btn-xs' title='Create Project'>Create Project</a>";
                    if (!string.IsNullOrWhiteSpace(item.Project))
                        projectButton = "<a data-id='" + item.Id + "' class='action-grid-create-project btn green btn-xs' title='View Project'>View</a>";

                    data.Add(item.Project + (item.IsCounter == "Yes" ? (" " + projectButton) : ""));
                    var icons = "<span class='action'>";
                    icons += viewIcon;
                    if (SiteContext.Current.Fiscal.Id == item.FiscalId)
                    {
                        icons += editIcon;
                        icons += deleteIcon;
                    }
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
                PMCManager.Delete(id);
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

    }
}
