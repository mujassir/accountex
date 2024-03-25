using AccountEx.BussinessLogic;
using AccountEx.BussinessLogic.SQL;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace AccountEx.Web.Controllers.api
{
    /// <summary>
    /// API is called from "Alnoor Sync Service", to post Nexus data to Amex DB
    /// </summary>
    public class NexusSyncApiController : BaseApiController
    {

        public string Get()
        {
            return "Nexus Sync Api";
        }

        // POST api/test
        public ApiResponse Post([FromBody]NexusSyncRequest input)
        {
            ApiResponse response;
            try
            {
                NexusSyncManager.InsertRecords(input.TableName, input.Json);

                response = new ApiResponse { Success = true, Data = null };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }

            return response;

        }



        
    }
}
