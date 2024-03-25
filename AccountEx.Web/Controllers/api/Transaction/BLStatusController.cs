
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
using AccountEx.DbMapping;

namespace AccountEx.Web.Controllers.api.Transaction
{
    public class BLStatusController : BaseApiController
    {
        public ApiResponse Get()
        {
            ApiResponse response;
            try
            {

                var data = new BLRepository().GetBlStatuses();
                response = new ApiResponse
                {
                    Success = true,
                    Data =data
                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }

        public ApiResponse Post([FromBody]List<BLSaveExtra> records)
        {
            ApiResponse response;
            try
            {
                BLStatusManager.Save(records);
                response = new ApiResponse { Success = true, Data = null };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }

    }
}
