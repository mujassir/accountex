using System;
using System.Net.Http;
using System.Web.Http;
using AccountEx.BussinessLogic;
using AccountEx.Common;
using AccountEx.Repositories;
using Newtonsoft.Json;
using AccountEx.CodeFirst.Models;
using System.Linq;


namespace AccountEx.Web.Controllers.api.Transaction
{
    public class WheatPurchaseController : BaseApiController
    {
        public ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                var voucherNumber = id;
                var invoiceNumber = 0;
                var queryString = Request.RequestUri.ParseQueryString();
                var key = queryString["key"].ToLower();
                var type =Numerics.GetBool(queryString["type"]);
                //var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
                bool next, previous;
                if (voucherNumber == 0) key = "nextvouchernumber";
                if (key == "nextvouchernumber")
                {
                    voucherNumber = new WheatPurchaseRepository().GetNextVoucherNumber();
                    invoiceNumber = new WheatPurchaseRepository().GetNextInvoiceNumber(type);
                }
                var data = new WheatPurchaseRepository().GetByVoucherNumber(voucherNumber, key, out next, out previous);
                if (invoiceNumber == 0)   //case when voucher no is not zero get existing invoice no in next previous
                    invoiceNumber = data.InvoiceNumber;
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Order = data,
                        Next = next,
                        Previous = previous,
                        VoucherNumber = voucherNumber,
                        InvoiceNumber = invoiceNumber,
                    }
                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }
        public ApiResponse Get(string keyvalue)
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                var type = queryString["type"];
                var isGOVT = type == "PVT" ? false : true;
                var billno = new WheatPurchaseItemRepository().GetNextBillNo(isGOVT);
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        BillNo = billno,
                    }
                };
            }
            catch (Exception ex)
            {
                 ;
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }
        public ApiResponse Post([FromBody]WheatPurchase input)
        {
            ApiResponse response;
            try
            {
                var err = ServerValidateSave(input);
                if (err == "")
                {
                    input.FiscalId = SiteContext.Current.Fiscal.Id;
                    WheatPurchaseManager.Save(input);
                    response = new ApiResponse
                    {
                        Success = true,
                    };
                }
                else
                {
                    response = new ApiResponse
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
                WheatPurchaseManager.Delete(id, VoucherType.Purchase);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }
        private string ServerValidateSave(WheatPurchase input)
        {
            var err = ",";
            try
            {

                var record = new WheatPurchaseRepository().GetByVoucherNo(input.VoucherNumber, input.Id);
                if (SiteContext.Current.Fiscal == null)
                {
                    err += "No fiscal year found.";

                }
                if (SiteContext.Current.Fiscal.IsClosed)
                {
                    err += "Fiscal year is closed No action can be done.,";
                }
                if (!FiscalYearManager.IsValidFiscalDate(input.Date))
                {
                    err += "Voucher date should be within current fiscal year.,";
                }
                if (record != null)
                {
                    err += "Voucher no already exist.";
                }
                foreach (var item in input.WheatPurchaseItems.Where(p => p.ItemId == 0))
                {
                    err += "Item is not valid to process the request.,";
                }
                //else
                //{
                //    var data = new WheatPurchaseItemRepository().CheckIfBillNoExist(input.WheatPurchaseItems.Select(p => p.BillNo).ToList(), input.WheatPurchaseItems.Select(q => q.Id).ToList());

                //    if (data != null)
                //    {
                //        err += "<li>Bill no " + data.BillNo + " already exist.</li>";

                //    }
                //    else if (input.WheatPurchaseItems.Select(p => p.BillNo).Count() != input.WheatPurchaseItems.Select(p => p.BillNo).Distinct().Count())
                //    {

                //        err += "<li>Duplicate bill no exist</li>";


                //    }
               // }
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
