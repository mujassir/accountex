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
    public class MiscChargesController : BaseApiController
    {

        public ApiResponse Get(string key, string shops)
        {
            ApiResponse response;
            try
            {
                var data = new ShopRepository().GetAll();
                var tenants = new RentAgreementRepository().GetAll();
                response = new ApiResponse()
                {
                    Success = true,
                    //Data = data,
                    Data = new
                    {
                        Shops = data,
                        Tenants = tenants,
                    }
                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }
        public ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                var voucherNumber = id;
                var queryString = Request.RequestUri.ParseQueryString();
                var key = queryString["key"].ToLower();
                bool next, previous;
                if (voucherNumber == 0) key = "nextvouchernumber";
                if (key == "nextvouchernumber")
                    voucherNumber = new MiscChargesRepository().GetNextVoucherNumber();
                
                    var data = new MiscChargesRepository().GetByVoucherNumber(voucherNumber, key, out next, out previous);
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
        public ApiResponse Post([FromBody]MiscCharge input)
        {
            ApiResponse response;
            try
            {
                var err = ValidateSave(input);
                if (err == "")
                {

                    input.FiscalId = SiteContext.Current.Fiscal.Id;
                    MiscChargesManager.Save(input);
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
                MiscChargesManager.Delete(id);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }

            return response;
        }
        private string ServerValidateDelete(int id)
        {
            var err = ",";
            try
            {
                var GN = new DeliveryChallanRepository().GetById(id);

                if (GN != null)
                {
                    err += "goods note is used in invoice note can't be deleted.";
                }

            }
            catch (Exception)
            {
                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;
        }
        public static string ValidateSave(MiscCharge input)
        {
            var err = ",";
            try
            {


                if (SiteContext.Current.Fiscal == null)
                {
                    err += "No fiscal year found.,";

                }
                if (SiteContext.Current.Fiscal.IsClosed)
                {
                    err += "Fiscal year is closed No action can be done.,";
                }
                var record = new MiscChargesRepository().GetByVoucherNumber(input.VoucherNumber, input.Id);
                if (record != null)
                {
                    err += "Voucher no already exist.,";
                }

                foreach (var item in input.MiscChargeItems.Where(p => p.TenantAccountId == 0))
                {
                    err += item.TenantAccountCode + "-" + item.TenantAccountName + " is not valid.,";
                }
                foreach (var item in input.MiscChargeItems.Where(p => p.ChargesAccountId == 0))
                {
                    err += item.ChargesAccountCode + "-" + item.ChargesAccountName + " is not valid.,";
                }
                //if (!allowDupliateItem)
                //{
                //    var Itemcountlist = input.SaleItems.GroupBy(p => p.ItemId).Select(p => new
                //    {
                //        ItemId = p.Key,
                //        ItemCode = p.FirstOrDefault().ItemCode,
                //        ItemName = p.FirstOrDefault().ItemName,
                //        Count = p.Count()
                //    }).Where(p => p.Count > 1).ToList();

                //    foreach (var item in Itemcountlist)
                //    {
                //        err += item.ItemCode + "-" + item.ItemName + " must be added once in list.(Current Count:" + item.Count + "),";
                //    }
                //}


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
