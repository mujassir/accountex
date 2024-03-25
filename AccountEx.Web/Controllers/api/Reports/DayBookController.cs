using AccountEx.BussinessLogic;
using AccountEx.Common;
using System;

namespace AccountEx.Web.Controllers.api
{
    public class DayBookController : BaseApiController
    {
        // GET api/test
        //public ApiResponse Get()
        //{
        //    return GetBalances();
        //}
        public ApiResponse Get(DateTime date1, DateTime date2, bool isDebit)
        {
            //var date1 = DateTime.Parse(QueryString["date1"]);
            //var date2 = DateTime.Parse(QueryString["date2"]);
            var data = VoucherManager.GetDetailedVouchers(date1, date2, isDebit);
            var response = new ApiResponse
            {
                Success = true,
                Data = data
            };
            return response;
        }
    }
}
