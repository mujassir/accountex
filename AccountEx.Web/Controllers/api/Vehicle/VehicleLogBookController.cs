
using System;
using System.Web.Http;
using AccountEx.BussinessLogic;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.Repositories.Vehicles;
using AccountEx.BussinessLogic.Vehicles;
using AccountEx.DbMapping.VehicleSystem;

namespace AccountEx.Web.Controllers.api.Vehicle
{
    public class VehicleLogBookController : BaseApiController
    {
        public ApiResponse Get()
        {
            ApiResponse response;
            try
            {
                var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
                var soldStatus = Request.GetQueryString("soldStatus");
                var transfeerStatus = Request.GetQueryString("transfeerStatus");

                var data = new VehicleRepository().GetVehicleLogBooks(branchId,soldStatus,transfeerStatus);
                response = new ApiResponse { Success = true, Data = data };
            }
            catch (Exception ex)
            {

                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;

        }
        public ApiResponse Get(bool viewScan)
        {
            ApiResponse response;
            try
            {
                var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
                var vehicleId = Numerics.GetInt(Request.GetQueryString("vehicleId"));
                var repo = new VehicleLogBookScanRepository();
                var scans = repo.GetByVehicleById(vehicleId);
                response = new ApiResponse { Success = true, Data = scans };
            }
            catch (Exception ex)
            {

                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;

        }
        public ApiResponse Get(int vehicleId, int customerId)
        {
            ApiResponse response;
            try
            {
                var data = new VehicleFollowUpRepository().GetFollowUps(vehicleId, customerId);
                response = new ApiResponse { Success = true, Data = data };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;

        }
        public ApiResponse Post([FromBody]VehicleLogBookSave input)
        {
            ApiResponse response;
            try
            {
                VehicleManager.SaveLogBook(input);
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
