using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Web.Controllers.api.Shared;
using AccountEx.CodeFirst.Models.COA;
using AccountEx.Repositories;
using AccountEx.CodeFirst;
using System.Web.Http;
using AccountEx.BussinessLogic;

namespace AccountEx.Web.Controllers
{
    public class VehicleRequestController : GenericApiController<VehicleRequest>
    {
        public ApiResponse Post([FromBody]VehicleRequest input, bool acceptReject)
        {
            ApiResponse response;
            try
            {
                //var err = "";
                //err = ValidateTransferSave(input);
                //if (err == "")
                //{
                new VehicleTransferRepository().UpdateStatus(input);
                response = new ApiResponse { Success = true };
                //}
                //else
                //{
                //    response = new ApiResponse { Success = false, Error = err };
                //}
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        protected override JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "ChassisNo", "RegNo", "EnginePower", "EngineNo", "Model", "Fuel", "Color", "OwnerBranch", "ReceivingBranch", "Date", "RequestType", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var type = (queryString["type"] + "").Trim();
            var branchId = Numerics.GetInt((queryString["branchId"]));
            var repo = new vw_VehicleSendRequestRepository();
            var userBranchRepo = new UserVehicleBranchesRepository(repo);
            var userBranches = userBranchRepo.GetBranchIdsByUserIds(SiteContext.Current.User.Id);
            var records = repo.AsQueryable().Where(p => p.Status == (byte)VehicleRequestStatus.Pending);
            if (!SiteContext.Current.User.IsAdmin)
                records = records.Where(p => (p.Type == (byte)VehicleRequestType.Send && userBranches.Contains(p.ReceivingBranchId)) || (p.Type == (byte)VehicleRequestType.Recieve && userBranches.Contains(p.OwnerBranchId)));
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                     p.OwnerBranch.Contains(search) ||
                     p.ReceivingBranch.Contains(search) ||
                     p.Date.ToString().Contains(search) ||
                     p.Manufacturer.Contains(search) ||
                     p.DoM.Contains(search) ||
                     p.ChassisNo.Contains(search) ||
                     p.Model.Contains(search) ||
                     p.RegNo.Contains(search) ||
                     p.EnginePower.Contains(search) ||
                     p.Fuel.Contains(search) ||
                     p.EngineNo.Contains(search) ||
                      p.RequestType.Contains(search) ||
                     p.Color.Contains(search)
);


            var orderedList = filteredList.OrderByDescending(p => p.Id);
            //if (colIndex < coloumns.Length && coloumns[colIndex] + "" != "")
            //{
            //    var sortDir = queryString["sSortDir_0"];
            //    orderedList = sortDir == "asc" ? filteredList.OrderBy(coloumns[colIndex]) :
            //        filteredList.OrderByDescending(coloumns[colIndex]);
            //}
            var sb = new StringBuilder();
            sb.Clear();

            var sr = 0;
            var rs = new JQueryResponse();
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                var data = new List<string>();
                data.Add(item.ChassisNo);
                data.Add(item.RegNo);
                data.Add(item.EnginePower);
                data.Add(item.EngineNo);
                data.Add(item.Model);
                data.Add(item.Fuel);
                data.Add(item.Color);
                data.Add(item.OwnerBranch);
                data.Add(item.ReceivingBranch);
                data.Add(item.Date.ToString(AppSetting.GridDateFormat));
                data.Add("<span class='label label-success label-sm'>" + item.RequestType + "</span>");
                var vehicle = item.ChassisNo + "-" + item.Model + "-" + item.Color + "-" + item.Manufacturer;
                var acceptIcon = "<a href='javascript:void(0)' onclick=\"VehicleSendRequest.OpenTransferModal(this,'" + item.Id + "','" + vehicle + "','" + item.OwnerBranch + "','" + item.ReceivingBranch + "'," + (byte)VehicleRequestStatus.Accepted + ")\" class='btn default btn-xs green-stripe'>Accept</a>";
                // var rcvIcon = "<a href='javascript:void(0)' onclick=\"Vehicles.OpenTransferModal(this,'" + item.BranchId + "','" + item.Id + "','" + vehicle + "','" + item.BranchName + "',false,'" + (byte)VehicleRequestType.Recieve + "'," + (SiteContext.Current.User.IsAdmin ? "true" : "false") + ",'" + (branch != null ? branch.Name : "") + "')\" class='btn default btn-xs green-stripe'>Reject</a>";
                var icons = "<span class='action'>";
                icons += acceptIcon;
                // icons += rejectIcon;
                icons += "</span>";
                if (type != "report") data.Add(icons);
                rs.aaData.Add(data);
            }
            rs.sEcho = echo;
            rs.iTotalRecords = totalRecords;
            rs.iTotalDisplayRecords = totalDisplayRecords;
            return rs;
            //return base.GetDataTable();
        }
    }
}
