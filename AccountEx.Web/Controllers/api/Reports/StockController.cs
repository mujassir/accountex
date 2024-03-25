using System.Web.Http;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Linq;

namespace AccountEx.Web.Controllers.api
{
    public class StockController : BaseApiController
    {
        // GET api/test
        public ApiResponse Get()
        {
            var Key = Request.GetQueryString("Key");
            if (Key == "GetIrisStock")
                return GetIrisStock();
            else if (Key == "GetStockByQuantity")
                return GetStockByQuantity();
            else if (Key == "GetStockByWeight")
                return GetStockByWeight();
            else
                return GetStock();
        }
        private ApiResponse GetStockByWeight()
        {
            var date1 = Convert.ToDateTime(Request.GetQueryString("fromdate"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("todate"));
            var accountId = Numerics.GetInt(Request.GetQueryString("accountId"));
            var transactions = new TransactionRepository().GetStockByWeight(date1, date2, accountId);
            foreach (var item in transactions)
            {
                item.GroupName = !string.IsNullOrWhiteSpace(item.GroupName) ? item.GroupName.Trim() : "";
            }
            var response = new ApiResponse
            {
                Success = true,
                Data = transactions
            };
            return response;
        }
        private ApiResponse GetStockByQuantity()
        {
            var date1 = Convert.ToDateTime(Request.GetQueryString("fromdate"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("todate"));
            var accountId = Numerics.GetInt(Request.GetQueryString("accountId"));
            var transactions = new TransactionRepository().GetStockByQuantity(date1, date2, accountId);
            foreach (var item in transactions)
            {
                item.GroupName = !string.IsNullOrWhiteSpace(item.GroupName) ? item.GroupName.Trim() : "";
            }
            var response = new ApiResponse
            {
                Success = true,
                Data = transactions
            };
            return response;
        }
        private ApiResponse GetStock()
        {
            var date1 = Convert.ToDateTime(Request.GetQueryString("fromdate"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("todate"));
            var accountId = Numerics.GetInt(Request.GetQueryString("accountId"));
            var transactions = new TransactionRepository().GetStock(date1, date2, accountId);
                //.Where(p => !string.IsNullOrWhiteSpace(p.GroupName)).ToList();

            foreach (var item in transactions)
            {
                item.GroupName = !string.IsNullOrWhiteSpace(item.GroupName) ? item.GroupName.Trim() : "";
            }
            var response = new ApiResponse
             {
                 Success = true,
                 Data = transactions
             };
            return response;
        }
        private ApiResponse GetIrisStock()
        {
            var date1 = Convert.ToDateTime(Request.GetQueryString("fromdate"));
            var date2 = Convert.ToDateTime(Request.GetQueryString("todate"));
            var accountId = Numerics.GetInt(Request.GetQueryString("accountId"));
            var transactions = new ReportRepository().GetIrisStock(date1, date2).ToList();

            //foreach (var item in transactions)
            //{
            //    item.GroupName = item.GroupName.Trim();
            //}
            var response = new ApiResponse
            {
                Success = true,
                Data = transactions
            };
            return response;
        }

    }
}
