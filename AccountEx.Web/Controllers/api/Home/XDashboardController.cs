using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Net.Http;
using AccountEx.BussinessLogic;
using AccountEx.Common;
using AccountEx.Repositories;
using Newtonsoft.Json;
using System.Text;
using AccountEx.DbMapping;

namespace AccountEx.Web.Controllers.api.Reports
{
    public class XDashboardController : BaseApiController
    {
        public virtual ApiResponse Get(string key)
        {
            // var key = Request.GetQueryString("key");
            var response = new ApiResponse();
            try
            {
                switch (key)
                {
                    case "GetDashboard":
                        var name = QueryString["DashboardName"];
                        var lastLoadTime = DateConverter.ConvertStandardDate(QueryString["lastLoadTime"], DateTime.MinValue.AddYears(2000));
                        var dashboard = DashboardManager.GetDashboard(name, lastLoadTime);
                        response = new ApiResponse
                        {
                            Success = true,
                            Data = dashboard
                        };
                        break;
                }

            }
            catch (Exception ex)
            {
               response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }


        public ApiResponse Post([FromBody]WidgetObject input)
        {
            ApiResponse response;
            try
            {
                var dt = DashboardManager.GetReportData(input);
                response = new ApiResponse() { Success = true, Data = dt };
            }
            catch (Exception ex)
            {
               response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }


    }
}
