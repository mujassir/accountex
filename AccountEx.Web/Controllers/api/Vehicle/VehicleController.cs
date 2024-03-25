using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Web.Http;
using System.Net.Http;
using System.Text;
using AccountEx.Common;
using System.Web;
using System.Web.Mvc;
using Entities.CodeFirst;
using AccountEx.Repositories;
using AccountEx.Web.Controllers.api.Shared;
using AccountEx.CodeFirst;
using AccountEx.BussinessLogic;
using AccountEx.BussinessLogic.Vehicles;
using AccountEx.CodeFirst.Models.Vehicles;


namespace CustomClearance.Web.Controllers.api.Manage
{
    public class VehicleController : GenericApiController<Vehicle>
    {
        public ApiResponse Get(string key, bool loadInfo)
        {
            ApiResponse response;
            try
            {

                if (key == "getNextFileNo")
                    response = GetNextFileNo();
                else if (key == "loadPurchaseReturnInfo")
                    response = GetPurchaseReturnInfo();
                else
                    response = GetNextFileNo();

            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        private ApiResponse GetNextFileNo()
        {
            ApiResponse response;
            try
            {

                var fileNo = new VehicleRepository().GetNextFileNo();
                response = new ApiResponse
                {
                    Success = true,
                    Data = fileNo
                };

            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        private ApiResponse GetPurchaseReturnInfo()
        {
            ApiResponse response;
            try
            {

                var repo = new vw_VehiclesRepository();
                var queryString = Request.RequestUri.ParseQueryString();
                var id = Numerics.GetInt(queryString["id"]);
                var vehicle = repo.GetById(id);
                response = new ApiResponse
                {
                    Success = true,
                    Data = vehicle
                };

            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        public ApiResponse Get(bool gettransferDetail, string transferDetail, int vehicleId, int fromBranchId)
        {
            ApiResponse response;
            try
            {
                var repo = new UserVehicleBranchesRepository();
                var branches = repo.GetByUserIds(SiteContext.Current.User.Id);
                var transferRequest = new VehicleTransferRepository(repo).GetToBranchTransferDetail(vehicleId, fromBranchId);
                var allBranches = new VehicleBranchRepository(repo).GetNames();

                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Branches = branches,
                        IsAdmin = SiteContext.Current.User.IsAdmin,
                        TransferToVehicleDetail = transferRequest,
                        AllBranches = allBranches
                    }
                };

            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        public ApiResponse Post([FromBody]Vehicle input, string key)
        {
            ApiResponse response;
            try
            {
                var err = "";
                err = ValidateSave(input);
                if (err == "")
                {
                    VehicleManager.Save(input);
                    response = new ApiResponse { Success = true };
                }
                else
                {
                    response = new ApiResponse { Success = false, Error = err };
                }
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        public ApiResponse Post(int id, bool purchaseReturn)
        {
            ApiResponse response;
            try
            {

                response = VehicleManager.PurchaseReturn(id);

            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        public ApiResponse Post([FromBody]VehicleRequest input, bool sendRequest)
        {
            ApiResponse response;
            try
            {
                var err = "";
                err = ValidateTransferSave(input);
                if (err == "")
                {
                    new VehicleTransferRepository().Save(input);
                    response = new ApiResponse { Success = true };
                }
                else
                {
                    response = new ApiResponse { Success = false, Error = err };
                }
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        private string ValidateTransferSave(VehicleRequest input)
        {
            string err = "";
            if (!SiteContext.Current.User.IsAdmin)
            {
                if (!new UserVehicleBranchesRepository().IsUserLinkedWithBranch(SiteContext.Current.User.Id))
                {
                    err += "user must be admin or linked with a branch to generate transfer request.,";
                }
            }

            if (new VehicleTransferRepository().IsPendingRequestExist(input.VehicleId))
            {
                err += "vehilce already has a pending transfer request.,";
            }

            if (input.VehicleId == 0)
            {
                err += "vehicle must be selected.,";
            }
            if (input.ToBranchId == 0)
            {
                err += "branch must be selected.,";
            }
            if (!SiteContext.Current.User.IsAdmin)
            {

                if (input.Type == (byte)VehicleRequestType.Send)
                {
                    if (!new UserVehicleBranchesRepository().IsUserLinkedWithBranch(SiteContext.Current.User.Id, input.FromBranchId))
                    {
                        err += "user must be linked with sending branch.,";
                    }
                }




            }
            return err;
        }
        private string ValidateSave(Vehicle input)
        {
            string err = "";
            if (new VehicleRepository().IfChasisNumberExists(input.ChassisNo, input.Id))
            {
                err += "Chassis No already exists.,";
            }
            if (input.Type != (byte)VehicleType.New)
            {
                if (input.VendorId == null || input.VendorId == 0)
                {
                    err += "Vendor is required.,";
                }
                if (input.Type != (byte)VehicleType.TradIn)
                {
                    if (SettingManager.TradeInAcccountId == 0)
                    {
                        err += "Trade In account is missing.,";
                    }
                }
                if (input.Type != (byte)VehicleType.LocalPurchase)
                {
                    if (SettingManager.PurchaseAccountHeadId == 0)
                    {
                        err += "Purchase account is missing.,";
                    }
                }
            }
            return err;
        }

        //
        protected override JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "FileNo", "ChassisNo", "RegNo", "EngineNo", "Manufacturer", "Model", "Color", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var intsSearch = Numerics.GetInt(search);
            var type = (queryString["type"] + "").Trim();
            //var dal = new ProjectRepository();
            var repo = new vw_VehiclesRepository();
            var records = repo.AsQueryable().Where(p => p.Status != (byte)VehicleStatus.PurchaseReturn);
            var userBranchRepo = new UserVehicleBranchesRepository(repo);
            var userBranches = userBranchRepo.GetBranchIdsByUserIds(SiteContext.Current.User.Id);
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>

                     p.Model.Contains(search) ||
                     p.Manufacturer.Contains(search) ||
                     p.Color.Contains(search) ||
                     p.ChassisNo.Contains(search) ||
                     p.EngineNo.Contains(search) ||
                     p.RegNo.Contains(search)
                     || (intsSearch > 0 && p.FileNo == intsSearch));


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
                data.Add(item.FileNo + "");
                data.Add(item.ChassisNo);
                data.Add(item.RegNo);
                data.Add(item.EngineNo);
                data.Add(item.Manufacturer);
                data.Add(item.Model);
                data.Add(item.Color);
                var vehicle = "Chessis No:" + item.ChassisNo + " RegNo No:" + item.RegNo + " Brand:" + item.Manufacturer + " Color:" + item.Color + " CC:" + item.EnginePower + " Model:" + item.Model + " Car Type:" + item.CarType;
                var editIcon = "<i class='fa fa-edit' onclick=\"Vehicles.Edit(" + item.Id + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"Vehicles.Delete(" + item.Id + ")\" title='Delete' ></i>";
                var sendIcon = "<a href='javascript:void(0)' onclick=\"Vehicles.OpenTransferModal(this,'" + item.BranchId + "','" + item.Id + "','" + vehicle + "','" + item.BranchName + "','" + (byte)VehicleRequestType.Send + "'," + (SiteContext.Current.User.IsAdmin ? "true" : "false") + ")\" class='btn default btn-xs green-stripe'>Send</a>";
                var rcvIcon = "<a href='javascript:void(0)' onclick=\"Vehicles.OpenTransferModal(this,'" + item.BranchId + "','" + item.Id + "','" + vehicle + "','" + item.BranchName + "','" + (byte)VehicleRequestType.Recieve + "'," + (SiteContext.Current.User.IsAdmin ? "true" : "false") + ")\" class='btn default btn-xs green-stripe'>Receive</a>";
                var icons = "<span class='action'>";
                icons += editIcon;
                icons += deleteIcon;
                if (item.BranchId > 0 && !item.IsSale)
                {
                    if (SiteContext.Current.User.IsAdmin)
                    {
                        icons += sendIcon;
                    }
                    else
                    {
                        if (userBranches.Contains(Numerics.GetInt(item.BranchId)))
                            icons += sendIcon;
                        else
                            icons += rcvIcon;

                    }

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


        public override ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                var err = "";
                err = ValidateDelete(id);
                if (err == "")
                {

                    VehicleManager.Delete(id);
                    response = new ApiResponse { Success = true };
                }
                else
                {
                    response = new ApiResponse { Success = false, Error = err };
                }
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }

            return response;
        }

        private string ValidateDelete(int id)
        {
            string err = "";
            if (new VehicleRepository().IfSold(id))
            {
                err += "Chassis is already sold and can't be deleted.,";
            }
            if (new VehicleSaleRepository().IsTradeUnitUsed(id))
            {
                err += "Chassis is used in sale can't be deleted.,";
            }

            return err;
        }
    }
}
