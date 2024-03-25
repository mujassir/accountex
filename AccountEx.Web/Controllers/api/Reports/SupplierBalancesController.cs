using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Linq;

namespace AccountEx.Web.Controllers.api
{
    public class SupplierBalancesController : BaseApiController
    {
        // GET api/test
        public ApiResponse Get()
        {
            return GetBalances();
        }
        private ApiResponse GetBalances()
        {
            var date1 = DateTime.Parse(QueryString["date1"]);
            var date2 = DateTime.Parse(QueryString["date2"]);
            var result = new ReportRepository().GetSupplierBalances(date1, date2)
                .OrderBy(p => p.GroupName).ToList();
            var response = new ApiResponse
            {
                Success = true,
                Data = result,
            };
            return response;
        }
    }
}
