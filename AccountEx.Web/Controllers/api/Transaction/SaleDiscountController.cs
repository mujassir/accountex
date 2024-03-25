using System;
using System.Net.Http;
using System.Web.Http;
using AccountEx.BussinessLogic;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;

namespace AccountEx.Web.Controllers.api.Transaction
{
    public class SaleDiscountController : BaseApiController
    {
        public ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                var voucherNumber = id;
                var queryString = Request.RequestUri.ParseQueryString();
               // var type = queryString["type"];
                var key = queryString["key"].ToLower();
                var vouchertype = VoucherType.SaleDiscounts;
                //var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
                bool next, previous;
                if (voucherNumber == 0) key = "nextvouchernumber";
                if (key == "nextvouchernumber")
                    voucherNumber = new TransactionRepository().GetNextVoucherNumber(vouchertype);
                var data = new SaleDiscountRepository().GetByVoucherNumber(voucherNumber, vouchertype, key, out next, out previous);
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Order = data,
                        Next = next,
                        Previous = previous,
                        VoucherNumber = voucherNumber
                    }
                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }

        //public ApiResponse Get(int voucher)
        //{
        //    ApiResponse response;
        //    try
        //    {
        //        var result = "";
        //        var queryString = Request.RequestUri.ParseQueryString();
        //        var type = (queryString["type"]);
        //        var vouchertype =(VoucherType)Convert.ToByte(type);
        //        var data = VoucherManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);

        //        response = new ApiResponse
        //        {
        //            Success = true,
        //            Data = data

        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //         response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
        //    }
        //    return response;
        //}

        public ApiResponse Post([FromBody]Voucher input)
        {
            ApiResponse response;


            try
            {
                var err = ServerValidateSave(input);
                if (err == "")
                {
                    input.FiscalId = SiteContext.Current.Fiscal.Id;
                    VoucherManager.Save(input);
                    response = new ApiResponse
                    {
                        Success = true,
                        Data = input.Id
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
        public ApiResponse Post([FromBody]Voucher input, bool bankVoucher)
        {
            ApiResponse response;
            try
            {
                var err = ServerValidateSave(input);
                if (err == "")
                {
                    input.FiscalId = SiteContext.Current.Fiscal.Id;
                    VoucherManager.SaveBankVoucher(input);
                    response = new ApiResponse
                    {
                        Success = true,
                        Data = input.Id
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

        public ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                var type = (queryString["type"]);
                var vouchertype =(VoucherType)Convert.ToByte(type);
                VoucherManager.Delete(id, vouchertype);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }

            return response;
        }

        protected ApiResponse GetDataTable()
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
        private string ServerValidateSave(Voucher input)
        {
            var err = ",";
            try
            {
                var bankType = new List<VoucherType> { VoucherType.BankPayments, VoucherType.BankReceipts };
                if (!FiscalYearManager.IsValidFiscalDate(input.Date))
                {
                    err += "Voucher date should be within current fiscal year.,";
                }
                if (SiteContext.Current.Fiscal == null)
                {
                    err += "No fiscal year found.,";

                }
                if (SiteContext.Current.Fiscal.IsClosed)
                {
                    err += "Fiscal year is closed No action can be done.,";
                }
                if (!bankType.Contains(input.TransactionType) && input.TransactionType != VoucherType.TransferVoucher)
                {
                    if (input.AccountId == null || input.AccountId == 0)
                    {
                        err += "Account is not valid to process the request.,";
                    }
                }

                if (bankType.Contains(input.TransactionType))
                {
                    var isBankSelected = new AccountRepository().IsHeadSelected(input.VoucherItems.Select(p => p.AccountId).ToList(), SettingManager.BankHeadId);
                    if (!isBankSelected)
                    {
                        err += "Atleast one bank must be selected.,";
                    }
                }

                foreach (var item in input.VoucherItems.Where(p => p.AccountId == 0))
                {
                    err += item.AccountCode + "-" + item.AccountName + " is not valid code.,";
                }

                var record = new VoucherTransRepository().GetByVoucherNo(input.VoucherNumber, input.Id, input.TransactionType);

                if (record != null)
                {
                    err += "<li>Voucher no already exist.</li>";
                }
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
