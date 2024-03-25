using System;
using System.Web.Http;
using AccountEx.BussinessLogic;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.DbMapping;

namespace AccountEx.Web.Controllers.api.Manage
{
    public class SiteSettingController : BaseApiController
    {
        // GET api/test
        public ApiResponse Get()
        {
            ApiResponse response;
            try
            {
                var data=new SettingRepository().GetAll();
                response = new ApiResponse
                {
                    Success = true,
                    Data =data 
                    //Data =JsonConvert.SerializeObject(new SettingRepository().GetAll()),
                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        // POST api/test
        public ApiResponse Post([FromBody]SiteSettingContainer input)
        {
            ApiResponse response;
            try
            {
                SettingManager.SaveSettings(input.Settings);
                SettingManager.RefreshSetting();
                response = new ApiResponse { Success = true, };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

    }
}
