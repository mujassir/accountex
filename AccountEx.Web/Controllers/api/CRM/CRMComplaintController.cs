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
    public class CRMComplaintController : BaseApiController
    {
        public ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                var data = new GenericRepository<vw_CRMCompliants>().GetById(id);
                response = new ApiResponse
                {
                    Success = true,
                    Data = data,

                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        public ApiResponse Get(string dataKey)
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var response = new ApiResponse();
            switch (dataKey)
            {
                case "GetNextVoucherNumber":
                    response = GetNextVoucherNumber();
                    break;
                case "GetComplaintFiles":
                    response = GetComplaintFiles();
                    break;
            }
            return response;
        }
        private ApiResponse GetNextVoucherNumber()
        {
            ApiResponse response;

            try
            {
                var regionId = Numerics.GetInt(Request.GetQueryString("regionId"));
                var voucherNo = new CRMCompliantRepository().GetNextVoucherNumber();
                response = new ApiResponse { Success = true, Data = voucherNo };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
        private ApiResponse GetComplaintFiles()
        {
            ApiResponse response;

            try
            {
                var complaintId = Numerics.GetInt(Request.GetQueryString("complaintId"));
                var documents = new CRMCompliantFileRepository().GetByCompaintId(complaintId);
                response = new ApiResponse { Success = true, Data = documents };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }

        //public ApiResponse Get(int id)
        //{
        //    ApiResponse response;
        //    try
        //    {
        //        var regionId = Numerics.GetInt(Request.GetQueryString("regionId"));
        //        var data = new CRMCompliantRepository().GetPendingComplaints(regionId);
        //        response = new ApiResponse { Success = true, Data = data };
        //    }
        //    catch (Exception ex)
        //    {
        //        response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
        //    }
        //    return response;




        //}

        public ApiResponse Post([FromBody]CRMComplaint complaint)
        {
            ApiResponse response;
            try
            {
                var err = CRMComplaintManager.ValidateSave(complaint);
                if (err == "")
                {
                    complaint.FiscalId = SiteContext.Current.Fiscal.Id;
                    CRMComplaintManager.Save(complaint);
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
        //public ApiResponse Post([FromBody]List<CRMComplaint> complaints)
        //{
        //    ApiResponse response;
        //    try
        //    {
        //        var err = CRMComplaintManager.ValidateSave(complaints);
        //        if (err == "")
        //        {
        //            CRMComplaintManager.Save(complaints);
        //            response = new ApiResponse
        //            {
        //                Success = true,
        //            };
        //        }
        //        else
        //        {
        //            response = new ApiResponse()
        //            {
        //                Success = false,
        //                Error = err
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
        //    }
        //    return response;

        //}
        public ApiResponse Post(string key, int id, CRMComplaintStatus statusId)
        {
            ApiResponse response;
            try
            {
                CRMComplaintManager.ChangeStatus(id, statusId);
                response = new ApiResponse
                {
                    Success = true,
                    Data = new { UserId = SiteContext.Current.User.Id, Date = DateTime.Now.ToString("dd/MM/yyyy") }
                };

            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;

        }
        public ApiResponse Post(List<CRMComplaintFile> documents, string savedocuments, int complaintId)
        {
            ApiResponse response;
            try
            {
                CRMComplaintManager.SaveDocuments(documents, complaintId);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        public JQueryResponse GetDataTable()
        {
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                var coloumns = new[] { "Date", "Customer", "OGPNo", "InvoiceNumber", "Product", "Currency", "Rate", "Quantity", "Amount", "SaleType", "DeliveryType", "Project", "SalePerson", "" };
                var echo = Convert.ToInt32(queryString["sEcho"]);
                var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
                var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
                var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
                var search = (queryString["sSearch"] + "").Trim();
                var intSearch = Numerics.GetInt(search);
                var type = (queryString["type"] + "").Trim();
                var sortDirection = queryString["sSortDir_0"];
                var sortColumn = coloumns[colIndex];
                var records = new GenericRepository<vw_CRMCompliants>().AsQueryable();
                if (SiteContext.Current.UserTypeId == CRMUserType.SalesExecutive)
                {
                    //records = records.Where(p => p.SalePersonId == SiteContext.Current.User.Id);
                    records = records;
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
                          p.Project.Contains(search) ||
                           p.Product.Contains(search) ||
                             p.ActualProduct.Contains(search) ||
                                 p.ActualProductDivision.Contains(search) ||
                                  p.Product.Contains(search) ||
                                    p.CaseType.Contains(search) ||
                                      p.Priority.Contains(search) ||
                                        p.Lab.Contains(search) ||
                                          p.AssignedToUser.Contains(search) ||
                                            p.ResolvedByUser.Contains(search) ||
                                              p.CreateByUser.Contains(search) ||
                                                p.Status.Contains(search) ||
                                                 p.TestType.Contains(search) ||
                         (intSearch > 0 && p.VoucherNo == intSearch)
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
                    data.Add(item.VoucherNo + "");
                    data.Add(item.Customer);
                    data.Add(item.Project);
                    data.Add(item.ActualProduct);
                    data.Add(Numerics.DecimalToString(item.Price));
                    data.Add(item.ActualProductDivision);
                    data.Add(item.Vendor);
                    data.Add(item.Product);
                    data.Add(item.CaseType);
                    data.Add(item.Priority);
                    data.Add(item.TestType);
                    data.Add(item.Description);
                    data.Add(item.Lab);
                    data.Add(item.Status);
                    data.Add(item.CreateByUser);
                    data.Add(item.ResolvedByUser);

                    var actionHtml = "<div class='btn-group'><button type='button' class='btn btn-success dropdown-toggle btn-xs' data-toggle='dropdown' data-hover='dropdown' data-delay='1000' data-close-others='true' aria-expanded='false'>Actions<i class='fa fa-angle-down'></i></button><ul class='dropdown-menu' role='menu'>";
                    var editIcon = "<li><a target='_blank' href='javascript:void(0)' data-id='" + item.Id + "' title='Edit' class='btn default btn-xs green-stripe btn-edit'>Edit</a></li>";
                    var viewIcon = "<li><a target='_blank' href='javascript:void(0)' data-id='" + item.Id + "' title='View' class='btn default btn-xs green-stripe btn-view'>View</a></li>";
                    var deleteIcon = "<li><a target='_blank' href='javascript:void(0)'  data-id='" + item.Id + "' title='Delete' class='btn default btn-xs green-stripe btn-delete'>Delete</a></li>";
                    actionHtml += viewIcon + editIcon + deleteIcon;

                    var btnUploadDocument = "<li><a target='_blank' href='javascript:void(0)' onclick=\"Complaint.LoadFiles(" + item.Id + ",this)\"' class='btn default btn-xs green-stripe'>Upload Document</a></li>";
                    if (SiteContext.Current.UserTypeId == CRMUserType.LabUser)
                    {
                        actionHtml += btnUploadDocument;
                    }


                    actionHtml += "</ul></div>";
                    data.Add(actionHtml);
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
                CRMComplaintManager.Delete(id);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }

            return response;
        }


    }
}
