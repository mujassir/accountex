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
    public class CalendarEventController : BaseApiController
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
                case "GetProjects":
                    response = GetProjects();
                    break;
                case "GetNextVoucherNumber":
                    response = GetNextVoucherNumber();
                    break;
            }
            return response;
        }
        public ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                var date = DateConverter.ConvertFromDmy(Request.GetQueryString("date"));
                var data = new CalendarEventRepository().GetEvents(date);
                response = new ApiResponse { Success = true, Data = data };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;




        }
        public ApiResponse Post(bool syncGoogleCalendarEvents)
        {
            ApiResponse response;
            try
            {
                GoogleCalendarManager.SyncEvents(SiteContext.Current.User.CompanyId, SiteContext.Current.User.Id);
                response = new ApiResponse { Success = true, Data = null };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;




        }

        public ApiResponse Post([FromBody]List<CalendarEvent> events)
        {
            ApiResponse response;
            try
            {
                var err = CalendarEventManager.ValidateSave(events);
                if (err == "")
                {
                    events.ForEach(p => p.FiscalId = SiteContext.Current.Fiscal.Id);
                    foreach (var item in events)
                    {
                        item.StartTime = item.StartTime.NewDateWithOldTime(item.Date.Value);
                        item.EndTime = item.EndTime.Value.NewDateWithOldTime(item.Date.Value);
                    }
                    CalendarEventManager.Save(events);
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
                var coloumns = new[] { "Date", "VisitNo", "Customer", "StartTime", "EndTime", "Mode", "Description", "Status", "Project", "SalePerson", "" };
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
                var date = DateConverter.ConvertStandardDate(queryString["FilterDate"] + "");


                //var dal = new ProjectRepository();
                var records = new GenericRepository<vw_CRMCalendarEvents>().AsQueryable();
                // records = records.Where(p => p.FiscalId == fiscalId);


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
                if (salePersonId > 0)
                    records = records.Where(p => p.SalePersonId == salePersonId);
                if (date != DateTime.MinValue)
                    records = records.Where(p => p.Date == date);

                var totalRecords = records.Count();
                var filteredList = records;
                if (search != "")
                    filteredList = records.Where(p =>

                         p.Customer.Contains(search) ||
                         p.VisitNo.ToString().Contains(search) ||
                          p.Mode.Contains(search) ||
                          p.Description.Contains(search) ||
                            p.Project.Contains(search) ||
                          p.Status.Contains(search) ||
                           p.SalePerson.Contains(search)

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
                    data.Add(item.Date.HasValue ? item.Date.Value.ToString(AppSetting.GridDateFormat) : "");
                    data.Add(item.VisitNo + "");
                    data.Add(item.Customer);
                    data.Add(item.StartTime.ToString(AppSetting.GridTimeFormat));
                    data.Add(item.EndTime.Value.ToString(AppSetting.GridTimeFormat));
                    data.Add(item.Mode);
                    data.Add(item.Description);
                    data.Add(item.Status);
                    data.Add(item.Project);
                    data.Add(item.SalePerson);
                    var editIcon = "<i class='fa fa-edit btn-edit' data-id='" + item.Id + "' title='Edit' ></i>";
                    var viewIcon = "<i class='fa fa-eye btn-view' data-id='" + item.Id + "' title='View' ></i>";
                    var deleteIcon = "<i class='fa fa-trash-o btn-delete' data-id='" + item.Id + "' title='Delete' ></i>";
                    var icons = "<span class='action'>";

                    icons += viewIcon;
                    icons += "</span>";
                    if (type != "report") data.Add("");
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
                CalendarEventManager.Delete(id);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }

            return response;
        }

        private ApiResponse GetNextVoucherNumber()
        {
            ApiResponse response;

            try
            {
                var regionId = Numerics.GetInt(Request.GetQueryString("regionId"));
                var voucherNo = new CalendarEventRepository().GetNextVoucherNumber();
                response = new ApiResponse { Success = true, Data = voucherNo };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
        private ApiResponse GetProducts()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            ApiResponse response;
            try
            {
                var data = new CRMProductRepository().GetOwnProductsIdName();
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
            ApiResponse response;
            try
            {
                var data = new CRMProjectRepository().GetProjects(customerId);
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
