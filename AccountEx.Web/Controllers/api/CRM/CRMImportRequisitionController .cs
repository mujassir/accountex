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
    public class CRMImportRequisitionController : BaseApiController
    {

        public ApiResponse Get(string dataKey)
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var response = new ApiResponse();
            var key = queryString["dataKey"];
            switch (key)
            {
                case "GetSupplierAddress":
                    response = GetSupplierAddress();
                    break;
                case "GetCustomer":
                    response = GetCustomer();
                    break;
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
                var requisitionType = (CRMImportRequisitionType)Convert.ToByte(type);
                //var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
                bool next, previous;
                var requisitionRepo = new CRMImportRequisitionRepository();

                if (voucherNumber == 0 && key != "last" && key != "first") key = "nextvouchernumber";
                if (key == "nextvouchernumber")
                {
                    invoiceNumber = voucherNumber = requisitionRepo.GetNextVoucherNumber();
                }
                var data = requisitionRepo.GetByVoucherNumber(voucherNumber, requisitionType, key, out next, out previous);
                var customer = new CRMCustomer();
                var address = "";
                var history = new List<ImportRequisitionHisotryExtra>();
                if (data != null)
                {
                    customer = new CRMCustomerRepository().GetById(data.CustomerId);
                    address = new CRMVendorRepository().GetAddress(data.SupplierId);

                    if (requisitionType == CRMImportRequisitionType.RSM)
                    {
                        history = requisitionRepo.GetHistoryByVoucherNo(data.VoucherNumber);
                    }


                    if (SiteContext.Current.UserTypeId == CRMUserType.DivisionalHead)
                    {
                        var dbProductIds = data.CRMImportRequisitionItems.Select(p => p.ItemId).ToList();
                        var DHproductIds = new CRMProductRepository().GetProductIdsByDivision(dbProductIds, SiteContext.Current.DivisionId);
                        data.CRMImportRequisitionItems = data.CRMImportRequisitionItems.Where(p => DHproductIds.Contains(p.ItemId)).ToList();
                    }


                }
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Requisition = data,
                        Customer = customer,
                        History = history,
                        Address = address,
                        Next = next,
                        Previous = previous,
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

        public ApiResponse Post([FromBody]CRMImportRequisition input, CRMImportRequisitionType type)
        {
            ApiResponse response;
            try
            {
                var err = CRMImportRequisitionManager.ValidateSave(input, type);
                if (err == "")
                {
                    CRMImportRequisitionManager.Save(input, type);
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
        public JQueryResponse GetDataTable()
        {
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                var coloumns = new[] { "PINumber", "Date", "Customer", "CustomerNTN", "POL", "POD", "Supplier", "ImportStatus", "" };
                var echo = Convert.ToInt32(queryString["sEcho"]);
                var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
                var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
                var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
                var search = (queryString["sSearch"] + "").Trim();
                var intSearch = Numerics.GetInt(search);
                var requisitionType = (CRMImportRequisitionType)Numerics.GetByte((queryString["type"] + "").Trim());
                //var dal = new ProjectRepository();
                var records = new GenericRepository<vw_CRMImportRequisition>().AsQueryable(true);
                if (SiteContext.Current.UserTypeId == CRMUserType.SalesExecutive)
                {
                    records = records.Where(p => p.CreatedBy == SiteContext.Current.User.Id);
                }
                else if (SiteContext.Current.UserTypeId == CRMUserType.RSM)
                {
                    records = records.Where(p => p.RegionId == SiteContext.Current.RegionId);
                }
                else if (SiteContext.Current.UserTypeId == CRMUserType.DivisionalHead)
                {
                    var Ids = new CRMImportRequisitionRepository().GetImportIdsByDivisionId(SiteContext.Current.DivisionId);
                    records = records.Where(p => Ids.Contains(p.Id));
                }

                var statuses = new List<CRMImportRequisitionStatus>() { CRMImportRequisitionStatus.Pending, CRMImportRequisitionStatus.Review };
                if (requisitionType == CRMImportRequisitionType.DH)
                {
                    statuses = new List<CRMImportRequisitionStatus>() { CRMImportRequisitionStatus.Pending, CRMImportRequisitionStatus.Revised };
                }
                else if (requisitionType == CRMImportRequisitionType.RSM)
                {
                    statuses = new List<CRMImportRequisitionStatus>() { CRMImportRequisitionStatus.Approved };
                }
                records = records.Where(p => statuses.Contains(p.Status));

                var totalRecords = records.Count();
                var filteredList = records;
                if (search != "")
                    filteredList = records.Where(p =>
                         p.Customer.Contains(search) ||
                          p.CustomerNTN.Contains(search) ||
                           p.POL.Contains(search) ||
                            p.POD.Contains(search) ||
                              p.ImportStatus.Contains(search) ||
                             p.Supplier.Contains(search) ||
                         (p.PINumber.Contains(search))

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
                    data.Add(item.Date.ToString(AppSetting.GridDateFormat));
                    data.Add(item.Customer);
                    data.Add(item.CustomerNTN);
                    data.Add(item.POL);
                    data.Add(item.POD);
                    data.Add(item.Supplier);
                    data.Add("<span class='label label-success'> " + item.ImportStatus + " </span>");
                    var editIcon = "<i class='fa fa-edit btn-edit' data-id='" + item.VoucherNumber + "' title='Edit' ></i>";
                    var viewIcon = "<i class='fa fa-eye btn-view' data-id='" + item.Id + "' title='View' ></i>";
                    var deleteIcon = "<i class='fa fa-trash-o btn-delete' data-id='" + item.VoucherNumber + "' title='Delete' ></i>";
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
                CRMImportRequisitionManager.Delete(id);
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
        private ApiResponse GetCustomer()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var customerId = Numerics.GetInt(queryString["customerId"]);
            ApiResponse response;
            try
            {
                var data = new CRMCustomerRepository().GetById(customerId);
                response = new ApiResponse { Success = true, Data = data };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
        private ApiResponse GetSupplierAddress()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var supplierId = Numerics.GetInt(queryString["supplierId"]);
            ApiResponse response;
            try
            {
                var data = new CRMVendorRepository().GetAddress(supplierId);
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
