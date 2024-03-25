using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.Web.Controllers.api.Shared;
using System.Web.Http;
using System.Web;
using AccountEx.BussinessLogic;
namespace AccountEx.Web.Controllers.api.HRM
{
    public class FiscalController : GenericApiController<Fiscal>
    {

        //// <summary>
        /// Used for fiscal year closing
        /// </summary>
        /// <param name="closeFiscalYear"></param>
        /// <returns></returns>
        public ApiResponse Get(string closeFiscalYear)
        {
            ApiResponse response;
            try
            {
                FiscalYearManager.CloseFiscalYear();
                SiteContext.Current.Fiscal.IsClosed = true;
                response = new ApiResponse() { Success = true, };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }
        /// <summary>
        /// Used for fiscal year reopening
        /// </summary>
        /// <param name="closeFiscalYear"></param>
        /// <returns></returns>
        public ApiResponse Get(int id, string openFiscalYear)
        {
            ApiResponse response;
            try
            {
                FiscalYearManager.ReopenFiscalYear();
                SiteContext.Current.Fiscal.IsClosed = false;

                response = new ApiResponse() { Success = true, };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }


        public override ApiResponse Post([FromBody]Fiscal input)
        {
            ApiResponse response;
            try
            {
                var repo = new FiscalRepository();
                string configName;
                if (repo.IsAlreadyExistInDates(input.Id, input.FromDate, input.ToDate, out configName) == false)
                {
                    if (input.IsDefault)
                    {
                        var defaultfiscal = repo.GetDefaultFiscal();
                        if (defaultfiscal != null)
                        {
                            defaultfiscal.IsDefault = false;
                            repo.Update(defaultfiscal);
                        }
                    }
                    repo.Save(input);
                    response = new ApiResponse() { Success = true };
                }
                else
                {
                    response = new ApiResponse()
                    {
                        Success = false,
                        Error = "Configuration already exist in applied dates with Name: " + configName,
                    };
                }

            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }

        protected override JQueryResponse GetDataTable()
        {
            //  var queryString = Request.RequestUri.ParseQueryString();
            var queryString = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var coloumns = new[] { "Name", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var records = Repository.AsQueryable();

            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                    p.Name.Contains(search)
                    );
            var totalRecords = filteredList.Count();
            var totalDisplayRecords = totalRecords;
            var orderedList = filteredList.OrderByDescending(p => p.Id);
            var sb = new StringBuilder();
            sb.Clear();

            var rs = new JQueryResponse();
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                var data = new List<string>();

                data.Add("<td><input type='checkbox' class='checkboxes' value='1' /></td>");
                data.Add(item.Name);

                var editIcon = "<i class='fa fa-edit' onclick=\"Fiscals.Edit(" + item.Id + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"Fiscals.Delete(" + item.Id + ")\" title='Delete' ></i>";
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

    }
}
