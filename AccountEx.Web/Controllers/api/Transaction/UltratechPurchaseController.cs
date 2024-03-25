using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using AccountEx.BussinessLogic;
using AccountEx.Common;
using AccountEx.Repositories;
using Newtonsoft.Json;
using AccountEx.CodeFirst.Models;


namespace AccountEx.Web.Controllers.api.Transaction
{
    public class UltratechPurchaseController : BaseApiController
    {
        public ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                var voucherNumber = id;
                var queryString = Request.RequestUri.ParseQueryString();
                var type = queryString["type"];
                var key = queryString["key"].ToLower();
                var vouchertype =(VoucherType)Convert.ToByte(type);
                //var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
                bool next, previous;
                var saleRepo = new SaleRepository();
                var transactionRepo = new TransactionRepository(saleRepo);
                
                if (voucherNumber == 0) key = "nextvouchernumber";
                if (key == "nextvouchernumber")
                    voucherNumber = transactionRepo.GetNextVoucherNumber(vouchertype);
               
                    var data = saleRepo.GetByVoucherNumber(voucherNumber, vouchertype, key, out next, out previous);
                    response = new ApiResponse
                    {
                        Success = true,
                        Data = new
                        {
                            Order = data,
                            Next = next,
                            Previous = previous,
                            VoucherNumber = voucherNumber,
                        }
                    };
                
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }

        public ApiResponse Post([FromBody]Sale input)
        {
            ApiResponse response;
            try
            {
                var err = TransactionManager.ValidateSave(input);
                if (err == "")
                {
                    input.FiscalId = SiteContext.Current.Fiscal.Id;
                    UltratechPurchaseManager.Save(input);
                    response = new ApiResponse
                    {
                        Success = true,
                        Data = new { Order = input }
                    };
                }
                else
                {
                    response = new ApiResponse()
                    {
                        Success = false,
                        Error = err
                    };
                }
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }
        private ApiResponse GetDataTable()
        {
            ApiResponse response;
            try
            {
                var result = "";
                var queryString = Request.RequestUri.ParseQueryString();
                var vouchertype = (VoucherType)Convert.ToByte((queryString["type"]));
                var voucher = Numerics.GetInt((queryString["voucher"]));
                var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
                result = JsonConvert.SerializeObject(data, GetJsonSetting());
                response = new ApiResponse
                {
                    Success = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }

        public ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                var type = (queryString["type"]);
                var vouchertype =(VoucherType)Convert.ToByte(type);
                UltratechPurchaseManager.Delete(id, vouchertype);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }
        private string ServerValidateSave(Sale input)
        {
            var err = ",";
            try
            {
                err = TransactionManager.ValidateSave(input);
            }
            catch (Exception)
            {
                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;
        }
    }
}
