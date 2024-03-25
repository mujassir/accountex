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
using AccountEx.CodeFirst.Models.Nexus;
using AccountEx.CodeFirst.Models.Lab;
using AccountEx.Common.Nexus;

namespace AccountEx.Web.Controllers.api.Manage
{
    public class NexusBulkPostedCaseController : BaseApiController
    {
        public JQueryResponse Get()
        {
            return GetDataTable();
        }

        public ApiResponse Get(string dataKey)
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var response = new ApiResponse();
            var key = queryString["dataKey"];
            switch (key)
            {
                case "GetTests":
                    response = GetTests();
                    break;
                case "GetCases":
                    response = GetCases();
                    break;
                case "GetPostedCases":
                    response = GetPostedCases();
                    break;
                case "GetDepartments":
                    response = GetDepartments();
                    break;
            }
            return response;
        }
        public ApiResponse Get(long id)
        {
            ApiResponse response;
            try
            {
                var repo = new NexusCaseRepository();
                var nexusCase = repo.GetFromNexusTableByCaseId(id);
                var postedCase = new NexusPostedCases();
                if (nexusCase != null)
                    postedCase = repo.GetByCaseId(id);
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Case = postedCase,
                        NexusCase = nexusCase,
                    }

                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        public ApiResponse Post([FromBody]List<NexusUnpostedCaseSave> unPostedCases)
        {
            ApiResponse response;
            try
            {
                var err = NexusCaseManager.ValidateSave(unPostedCases);
                if (err == "")
                {

                    NexusCaseManager.Save(unPostedCases);
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
        public ApiResponse Post([FromBody]List<NexusUnpostedCaseSave> unPostedCases,bool update)
        {
            ApiResponse response;
            try
            {
                var err = NexusCaseManager.ValidateSave(unPostedCases);
                if (err == "")
                {

                    NexusCaseManager.Update(unPostedCases);
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
        protected JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "CaseNumber", "PatientName", "RegistrationDate", "ReportingDate", "ReferenceName", "ConsultantName", "TotalAmount", "Discount", "Less", "PatientName", "NetAmount", "PaidAmount", "Due", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var type = (queryString["type"] + "").Trim();
            //var dal = new ProjectRepository();
            var records = new NexusCaseRepository().AsQueryableCaseView();
            var totalRecords = records.Count();
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>

                     p.CaseNumber.Contains(search) ||
                      p.PatientName.Contains(search) ||
                      p.ReferenceName.Contains(search) ||
                      p.ConsultantName.Contains(search)
                   );

            var totalDisplayRecords = filteredList.Count();
            var orderedList = filteredList.OrderByDescending(p => p.ID);
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
                data.Add(item.CaseNumber);
                data.Add(item.PatientName);
                data.Add(item.RegistrationDate.ToString(AppSetting.GridDateFormat));
                data.Add(item.ReportingDate.ToString(AppSetting.GridDateFormat));
                data.Add(item.ReferenceName);
                data.Add(item.ConsultantName);
                data.Add(Numerics.IntToString(item.TotalAmount));
                data.Add(Numerics.IntToString(item.Discount));
                data.Add(Numerics.IntToString(item.Less));
                data.Add(Numerics.IntToString(item.NetAmount));
                data.Add(Numerics.IntToString(item.PaidAmount));
                data.Add(Numerics.IntToString(item.Due));
                var editIcon = "<i class='fa fa-edit btn-edit' data-id='" + item.ID + "' title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o btn-delete' data-id='" + item.ID + "' title='Delete' ></i>";
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

        public ApiResponse Delete(string id)
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                NexusCaseManager.Delete(id);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }

            return response;
        }

        private ApiResponse GetTests()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            ApiResponse response;
            try
            {
                var data = new NexusCaseRepository().GetTest();
                response = new ApiResponse { Success = true, Data = data };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
        private ApiResponse GetCases()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var fromDate = DateConverter.ConvertStandardDate(Request.GetQueryString("fromDate"));
            var toDate = DateConverter.ConvertStandardDate(Request.GetQueryString("toDate"));
            ApiResponse response;
            try
            {
                var data = new NexusCaseRepository().GetUnpostedCases(fromDate, toDate, NexusCaseType.Departmental);
                response = new ApiResponse { Success = true, Data = data };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
        private ApiResponse GetPostedCases()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var fromDate = DateConverter.ConvertStandardDate(Request.GetQueryString("fromDate"));
            var caseNo = Request.GetQueryString("caseNo");
            var toDate = DateConverter.ConvertStandardDate(Request.GetQueryString("toDate"));
            ApiResponse response;
            try
            {
                var repo = new NexusCaseRepository();
                var nexusCases = repo.GetPostedCasesForUpdate(fromDate, toDate, NexusCaseType.Departmental, caseNo);
                var caseIds = nexusCases.Select(p => p.CaseId).Distinct().ToList();
                var postedCases = repo.GetByCaseIds(caseIds);
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        NexusCases = nexusCases,
                        PostedCases = postedCases,

                    }
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }

        private ApiResponse GetDepartments()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            ApiResponse response;
            try
            {
                var data = new AccountRepository().GetLeafAccount(SettingManager.CustomerHeadId);
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
