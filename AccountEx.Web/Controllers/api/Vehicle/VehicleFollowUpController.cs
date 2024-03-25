
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using AccountEx.BussinessLogic;
using AccountEx.Common;
using AccountEx.Repositories;
using Newtonsoft.Json;
using System.Linq;
using AccountEx.CodeFirst.Models;
using System.Text;
using AccountEx.CodeFirst.Models.Transactions;
using Entities.CodeFirst;
using AccountEx.Repositories.Vehicles;

namespace AccountEx.Web.Controllers.api.Vehicle
{
    public class VehicleFollowUpController : BaseApiController
    {
        public ApiResponse Get()
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                var branchId = Numerics.GetInt((queryString["branchId"]));
                var data = new VehicleRepository().GetVehicleFollowUp(branchId);
                response = new ApiResponse { Success = true, Data = data };
            }
            catch (Exception ex)
            {

                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
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
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;

        }
        public ApiResponse Post([FromBody]VehicleFollowUp input)
        {
            ApiResponse response;
            try
            {
                new VehicleFollowUpRepository().Save(input);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }

    }
}
