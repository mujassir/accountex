using AccountEx.Common;
using System;
using System.Net.Http;
using AccountEx.Repositories;
using System.Text.RegularExpressions;
using System.Web.Http;
using AccountEx.BussinessLogic;
using System.Collections.Generic;
using System.Linq;
using AccountEx.DbMapping;

namespace AccountEx.Web.Controllers.api
{
    public class MiscController : BaseApiController
    {

        public ApiResponse Get(int id)
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var response = new ApiResponse();
            var key = queryString["key"];
            switch (key)
            {
                case "GetCustomerProducs":
                    response = GetCustomerProducts();
                    break;
                case "GetPreviousBalance":
                    response = GetPreviousBalance();
                    break;







            }
            return response;
        }

        public ApiResponse Get()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var response = new ApiResponse();
            var key = queryString["key"];
            switch (key)
            {
                case "GetCustomerProducts":
                    response = GetCustomerProducts();
                    break;
                case "GetPreviousBalance":
                    response = GetPreviousBalance();
                    break;
                case "GetAvgRate":
                    response = GetAvgRate();
                    break;


                case "GetAccountByCode":
                    response = GetAccountByCode();
                    break;
                case "GetAccountDetailByCode":
                    response = GetAccountDetailByCode();
                    break;
                case "GetTransporterDetailByCode":
                    response = GetTransporterDetailByCode();
                    break;
                case "GetDeliveryChallan":
                    response = GetDeliveryChallan();
                    break;
                case "GetTransporters":
                    response = GetTransporters();
                    break;
                case "GetStockPreviousBalance":
                    response = GetStockPreviousBalance();
                    break;
                case "loaddiscounts":
                    response = LoadDiscounts();
                    break;
                case "GetPreviousBalanceWithExludingTradeIn":
                    response = GetPreviousBalanceWithExludingTradeIn();
                    break;

                case "GetCurrencies":
                    response = GetCurrencies();
                    break;




            }
            return response;
        }

        private ApiResponse GetCustomerProducts()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            ApiResponse response;
            try
            {
                var data = new CustomerDiscountRepository().GetAll();
                response = new ApiResponse { Success = true, Data = data };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
        private ApiResponse LoadDiscounts()
        {


            var lastDate = DateConverter.ConvertStandardDate(Request.GetQueryString("LastDate"));
            var companyId = Numerics.GetInt(Request.GetQueryString("CompnayId"));
            ApiResponse response;
            try
            {
                var lastActivityDate = new CustomerDiscountRepository().GetLastActivityDate();
                var loaddiscount = lastActivityDate > lastDate || SiteContext.Current.User.CompanyId != companyId;

                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Discounts = new CustomerDiscountRepository().GetAll<DiscountTradeEx>(),
                        LastDate = new CustomerDiscountRepository().GetLastActivityDate(),
                        CompanyId = SiteContext.Current.User.CompanyId
                    }
                };

            }
            catch (Exception)
            {

                response = new ApiResponse
                {
                    Success = true,

                };
            }

            return response;
        }
        private ApiResponse GetAvgRate()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var itemId = Numerics.GetInt(queryString["itemId"]);
            ApiResponse response;
            try
            {
                var Ids = new List<int>() { itemId };
                var balances = new TransactionRepository().GetStockAvgRates(Ids,DateTime.Now);
                var avgRate = balances != null ? balances.FirstOrDefault() : new ItemAvgRates();
                response = new ApiResponse { Success = true, Data = avgRate };


            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
        private ApiResponse GetPreviousBalance()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var accountid = Numerics.GetInt(queryString["accountid"]);
            ApiResponse response;
            try
            {
                if (!string.IsNullOrEmpty(queryString["getids"]))
                {
                    var record = new AccountDetailRepository().GetByAccountId(accountid);
                    response = new ApiResponse
                    {
                        Success = true,
                        Data = new
                        {
                            PreviousBalance = new TransactionRepository().GetBalance(accountid, DateTime.Now),
                            SalemanId = record.SalemanId,
                            OrderTakerId = record.OrderTakerId,
                            TerritoryManagerId = record.TerritoryManagerId,
                        }
                    };
                }
                else
                {
                    response = new ApiResponse { Success = true, Data = new TransactionRepository().GetBalance(accountid, DateTime.Now) };
                }

            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
        private ApiResponse GetCurrencies()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var date = DateConverter.ConvertFromDmy(Request.GetQueryString("date"));
            ApiResponse response;
            try
            {

                response = new ApiResponse { Success = true, Data = new CurrencyRateRepository().GetByDate(date) };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }


        private ApiResponse GetPreviousBalanceWithExludingTradeIn()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var accountid = Numerics.GetInt(queryString["accountId"]);
            var vehicleId = Numerics.GetInt(queryString["vehicleId"]);
            ApiResponse response;
            try
            {

                response = new ApiResponse { Success = true, Data = new TransactionRepository().GetChassisAdvanceBalance(accountid, vehicleId) };


            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
        private ApiResponse GetStockPreviousBalance()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var itemid = Numerics.GetInt(queryString["accountid"]);
            ApiResponse response;
            try
            {

                response = new ApiResponse { Success = true, Data = new TransactionRepository().GetStockOpeningBalance(itemid) };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
        private ApiResponse GetAccountByCode()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var code = queryString["code"];
            ApiResponse response;
            try
            {

                response = new ApiResponse { Success = true, Data = new AccountRepository().GetAccountByCode(code) };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
        private ApiResponse GetAccountDetailByCode()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var code = queryString["code"];
            var formid = Numerics.GetInt(queryString["formid"]);

            ApiResponse response;
            try
            {

                response = new ApiResponse { Success = true, Data = new AccountDetailRepository().GetByAccountCode(code, formid) };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
        private ApiResponse GetTransporterDetailByCode()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var code = queryString["code"];
            ApiResponse response;
            try
            {

                response = new ApiResponse { Success = true, Data = new TransporterRepository().GetByCode(code) };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        private ApiResponse GetDeliveryChallan()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var code = queryString["code"];
            ApiResponse response;
            try
            {

                response = new ApiResponse { Success = true, Data = new TransporterRepository().GetByCode(code) };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        private ApiResponse GetTransporters()
        {

            ApiResponse response;
            try
            {

                response = new ApiResponse { Success = true, Data = new TransporterRepository().GetAll() };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

    }
}
