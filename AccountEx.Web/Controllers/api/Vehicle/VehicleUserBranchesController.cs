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
using AccountEx.Repositories.Transactions;
using AccountEx.CodeFirst.Models.Vehicles;

namespace AccountEx.Web.Controllers.api
{
    public class VehicleUserBranchesController : BaseApiController
    {
        
     
        public virtual ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                response = new ApiResponse
                {
                    Success = true,
                    Data = new UserVehicleBranchesRepository(). GetBranchIdsByUserIds(id),
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        public ApiResponse Post([FromBody]List<UserVehicleBranch> records)
        {
            ApiResponse response;
            try
            {
                new UserVehicleBranchesRepository().Save(records);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }

      


    }
}
