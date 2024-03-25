using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Linq;
using AccountEx.BussinessLogic;
using AccountEx.Common;
using AccountEx.Repositories;
using Newtonsoft.Json;
using AccountEx.CodeFirst.Models;


namespace AccountEx.Web.Controllers.api.Transaction
{
    public class IrisSaleController : BaseApiController
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
                var vouchertype = (VoucherType)Convert.ToByte(type);
                //var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
                bool next, previous;
                if (voucherNumber == 0) key = "nextvouchernumber";
                if (key == "nextvouchernumber")
                    voucherNumber = new TransactionRepository().GetNextVoucherNumber(vouchertype);



                var data = new SaleRepository().GetByVoucherNumber(voucherNumber, vouchertype, key, out next, out previous);
                decimal balance = 0;
                var contactPerson = "";
                var agingItems = new List<SaleAgingItem>();
                if (data != null && vouchertype == VoucherType.Sale && data.AccountId > 0)
                {
                    var detail = new AccountDetailRepository().GetByAccountId(data.AccountId);
                    contactPerson = detail != null ? detail.ContactPerson : "";
                    balance = new TransactionRepository().GetBalance(data.AccountId, data.Date);
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
                            var sales = new SaleRepository().GetAgingSale(data.Id, data.AccountId, data.Date, 4);
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
                ;
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
                    IrisManager.Save(input);

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
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
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
                var voucherno = Numerics.GetInt(queryString["voucher"]);
                var vouchertype = (VoucherType)Convert.ToByte(type);
                TransactionManager.Delete(voucherno, vouchertype);
                new SaleServicesItemRepository().DeleteBySaleId(id);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }

            return response;
        }
        private string ServerValidateSave(Sale input)
        {
            var err = ",";
            try
            {
                err = TransactionManager.ValidateSave(input, true) + ",";
                foreach (var item in input.SaleItems.Where(p => p.SalesmanId == 0))
                {
                    err += item.SalesmanCode + "-" + item.SalesmanName + " is not valid saleman.,";
                }
                //var salemanList = input.SaleItems.GroupBy(p => p.SalesmanId).Select(p => new
                //{
                //    ItemId = p.Key,
                //    SalesmanCode = p.FirstOrDefault().SalesmanCode,
                //    SalesmanName = p.FirstOrDefault().SalesmanName,
                //    Count = p.Count()
                //}).Where(p => p.Count > 1).ToList();

                //foreach (var saleman in salemanList)
                //{
                //    err += saleman.SalesmanCode + "-" + saleman.SalesmanName + " must be added once in item list.(Current Count:" + saleman.Count + "),";
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
