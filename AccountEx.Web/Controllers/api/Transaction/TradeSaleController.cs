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

namespace AccountEx.Web.Controllers.api.Transaction
{
    public class TradeSaleController : BaseApiController
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
                var adRepo = new AccountDetailRepository(saleRepo);

                if (voucherNumber == 0) key = "nextvouchernumber";
                if (key == "nextvouchernumber")
                    voucherNumber = transactionRepo.GetNextVoucherNumber(vouchertype);
              
                    var data = saleRepo.GetByVoucherNumber(voucherNumber, vouchertype, key, out next, out previous);
                    decimal balance = 0;
                    var contactPerson = "";
                    var agingItems = new List<SaleAgingItem>();
                    if (data != null && vouchertype == VoucherType.Sale && data.AccountId > 0)
                    {
                        var detail = adRepo.GetByAccountId(data.AccountId);
                        contactPerson = detail != null ? detail.ContactPerson : "";
                        balance = transactionRepo.GetBalance(data.AccountId, data.Date);
                        var runningBalance = balance;
                        if (balance > 0)
                        {
                            agingItems.Add(new SaleAgingItem
                            {
                                VoucherNumber = data.VoucherNumber,
                                Date = data.Date,
                                DueAmount = data.NetTotal,
                                Balance = balance,
                                Age = 0,
                            });
                            runningBalance -= data.NetTotal;
                            if (runningBalance > 0)
                            {
                                var sales =  saleRepo.GetAgingSale(data.Id, data.AccountId, data.Date, 4);
                                foreach (var item in sales)
                                {
                                    agingItems.Add(new SaleAgingItem
                                    {
                                        VoucherNumber = item.VoucherNumber,
                                        Date = item.Date,
                                        DueAmount = item.NetTotal,
                                        Balance = runningBalance,
                                        Age = data.Date.Subtract(item.Date).Days
                                    });
                                    runningBalance -= item.NetTotal;
                                    if (runningBalance < 1) break;
                                }
                            }
                        }
                        agingItems.Reverse();
                    }
                    response = new ApiResponse
                    {
                        Success = true,
                        Data = new
                        {
                            Order = data,
                            Next = next,
                            Previous = previous,
                            VoucherNumber = voucherNumber,
                            Balance = balance,
                            AgingItems = agingItems,
                            ContactPerson = contactPerson,
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
                var err = TradeSaleTransactionManager.ValidateSave(input);
                if (err == "")
                {
                    input.FiscalId = SiteContext.Current.Fiscal.Id;
                    TradeSaleTransactionManager.Save(input);
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
                var data = TradeSaleTransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
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
                TradeSaleTransactionManager.Delete(id, vouchertype);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }

            return response;
        }

    }
}
