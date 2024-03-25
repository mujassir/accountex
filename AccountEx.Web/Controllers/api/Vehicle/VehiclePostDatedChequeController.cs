using System;
using System.Net.Http;
using System.Web.Http;
using AccountEx.BussinessLogic;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using AccountEx.CodeFirst.Models.Transactions;
using AccountEx.Repositories.Transactions;
using AccountEx.Repositories.Vehicles;
using AccountEx.CodeFirst.Models.Vehicles;
using AccountEx.Web.Controllers.api.Shared;
using System.Globalization;

namespace AccountEx.Web.Controllers.api.Transaction
{
    public class VehiclePostDatedChequeController : GenericApiController<VehiclePostDatedCheque>
    {

        public ApiResponse Get(string key)
        {
            ApiResponse response;
            try
            {
                switch (key)

                {
                    case "nextvouchernumber":
                        response = GetNextVoucherNo();
                        break;
                    case "loadAgreements":
                        response = LoadAgreements();
                        break;
                    default:
                        response = new ApiResponse { Success = false, Error = "No Action found." };
                        break;
                }
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }

        private ApiResponse GetNextVoucherNo()
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                var voucherNumber = 0;
                var repo = new VehiclePostDatedChequeRepository();
                voucherNumber = repo.GetNextVoucherNumber();
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {


                        VoucherNumber = voucherNumber
                    }
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        private ApiResponse LoadAgreements()
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                var repo = new vw_VehicleSalesRepository();
                response = new ApiResponse
                {
                    Success = true,
                    Data = repo.GetVehiclesWithCustomer()
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        public override ApiResponse Post([FromBody]VehiclePostDatedCheque input)
        {
            ApiResponse response;
            try
            {
                input.FiscalId = SiteContext.Current.Fiscal.Id;
                var err = "";// ServerValidateSave(input);
                if (err == "")
                {
                    response = base.Post(input);
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
        private string ServerValidateSave(ClearingCompany input)
        {
            var err = ",";
            try
            {

                //if (new VehiclePostDatedChequeRepository().IsExist(input.Name, input.Id))
                //{
                //    err += ",ClearingCompany with same name has already been added.";
                //}

            }
            catch (Exception)
            {

                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;
        }
        protected override JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "ChequeNo", "ChequeDate", "AccountName", "RegNo", "Amount", "Bank", "Status", "Remarks", "ChassisNo" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var type = (queryString["type"] + "").Trim();
            //var dal = new ProjectRepository();
            var repo = new GenericRepository<vw_VehiclePostDateCheques>();
            var records = repo.AsQueryable();
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                     p.ChequeNo.Contains(search)
                   );
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
                data.Add(item.ChequeNo);
                data.Add(item.ChequeDate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture));
                data.Add(item.AccountName);
                data.Add(item.ChassisNo);
                data.Add(item.RegNo);
                data.Add(Numerics.IntToString(item.Amount));
                data.Add(item.Bank);
                data.Add(item.Status);
                data.Add(item.Remarks);
             
                var editIcon = "<i class='fa fa-edit' onclick=\"VehiclePostDatedCheque.Edit(" + item.Id + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"VehiclePostDatedCheque.Delete(" + item.Id + ")\" title='Delete' ></i>";
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
    }
}
