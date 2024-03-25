using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using AccountEx.BussinessLogic;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using Newtonsoft.Json;

namespace AccountEx.Web.Controllers.api.Transaction
{
    public class PharmacieSaleController : BaseApiController
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
                if (voucherNumber == 0) key = "nextvouchernumber";
                if (key == "nextvouchernumber")
                    voucherNumber = new TransactionRepository().GetNextVoucherNumber(vouchertype);
                var data = new SaleRepository().GetByVoucherNumber(voucherNumber, vouchertype, key, out next, out previous);
                var agingItems = new List<SaleAgingItem>();
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
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        public ApiResponse Post([FromBody]Sale input)
        {
            ApiResponse response;
            try
            {
                var err = ServerValidateSave(input);
                if (err == "")
                {
                    input.FiscalId = SiteContext.Current.Fiscal.Id;
                    TransactionManager.Save(input);
                    response = new ApiResponse
                    {
                        Success = true,
                        Data = new
                        {
                            Order = input,
                        }
                    };
                }
                else
                {
                    response = new ApiResponse() { Success = false, Error = err };
                }
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;

        }
        private ApiResponse GetDataTable()
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                var vouchertype =(VoucherType) Convert.ToByte((queryString["type"]));
                var voucher = Numerics.GetInt((queryString["voucher"]));
                var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
                var result = JsonConvert.SerializeObject(data, GetJsonSetting());
                response = new ApiResponse
                {
                    Success = true,
                    Data = result

                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
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
                TransactionManager.Delete(id, vouchertype);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }

            return response;
        }
        private string ServerValidateSave(Sale sale)
        {
            var err = ",";
            try
            {

                var items = sale.SaleItems;
                if (sale.AccountId == 0)
                {
                    err += "Account is not valid to process the request.,";
                }
                if (SiteContext.Current.Fiscal == null)
                {
                    err += "No fiscal year found.";

                }
                if (SiteContext.Current.Fiscal.IsClosed)
                {
                    err += "Fiscal year is closed No action can be done.,";
                }
                if (!FiscalYearManager.IsValidFiscalDate(sale.Date))
                {
                    err += "Voucher date should be within current fiscal year.,";
                }
                var IsVoucherExits = new SaleRepository().IsVoucherExits(sale.VoucherNumber, sale.TransactionType, sale.Id);
                if (IsVoucherExits)
                {
                    err += "Voucher no already exist.,";
                }
                var balances = new TransactionRepository().GetStockOpeningBalance(items.Select(p => p.ItemId).Distinct().ToList());
                if (sale.TransactionType  == VoucherType.Sale)
                {
                    var ditinctitems = items.GroupBy(p => p.ItemId).Select(p => new
                    {
                        ItemId = p.Key,
                        p.FirstOrDefault().ItemCode,
                        p.FirstOrDefault().ItemName,
                        Quantity = p.Sum(q => q.Quantity),
                        Count = p.Count()
                    });

                    foreach (var item in ditinctitems)
                    {
                        var stock = Numerics.GetInt(balances.FirstOrDefault(p => p.AccountId == item.ItemId).Balance);
                        if (item.Count > 1)
                        {
                            err += "Item Code (" + item.ItemCode + "-" + item.ItemName + ") must be added one time.,";
                        }
                        var rb = 0;
                        if (sale.Id == 0)
                        {

                            rb = stock - Numerics.GetInt(item.Quantity);
                            if (rb < 0)
                            {
                                err += "Item Code (" + item.ItemCode + "-" + item.ItemName + ") requested quantity (" + item.Quantity + ") is not available (" + stock + ").,";
                            }
                        }
                        else
                        {
                            var previousvoucher = new SaleRepository().GetById(sale.Id);
                            var preitem = previousvoucher.SaleItems.FirstOrDefault(p => p.ItemId == item.ItemId);
                            if (preitem == null)
                            {
                                rb = stock - Numerics.GetInt(item.Quantity);
                                if (rb < 0)
                                {
                                    err += "Item Code (" + item.ItemCode + "-" + item.ItemName + ") requested quantity (" + item.Quantity + ") is not available(" + stock + ").,";
                                }

                            }
                            else
                            {
                                var qty = Numerics.GetInt(item.Quantity) - Numerics.GetInt(preitem.Quantity);
                                rb = stock - qty;

                                rb = stock - Numerics.GetInt(item.Quantity);
                                if (rb < 0)
                                {
                                    err += "Item Code (" + item.ItemCode + "-" + item.ItemName + ") requested quantity (" + item.Quantity + ") is not available (" + stock + ").,";
                                }
                            }

                        }
                    }

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
