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
    public class TransController : BaseApiController
    {
        public ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                var voucherNumber = id;
                var invoiceNumber = id;
                var queryString = Request.RequestUri.ParseQueryString();
                var type = queryString["type"];
                var key = queryString["key"].ToLower();
                var vouchertype = (VoucherType)Convert.ToByte(type);
                //var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
                bool next, previous;
                var tranRepo = new TransactionRepository();
                var saleRepo = new SaleRepository(tranRepo);
                var dcRepo = new DeliveryChallanRepository(tranRepo);
                var acRepo = new AccountDetailRepository(tranRepo);
                if (voucherNumber == 0) key = "nextvouchernumber";
                if (key == "nextvouchernumber")
                {
                    voucherNumber = tranRepo.GetNextVoucherNumber(vouchertype);
                    invoiceNumber = tranRepo.GetNextInvoiceNumber(vouchertype);
                }
                var data = saleRepo.GetByVoucherNumber(voucherNumber, vouchertype, key, out next, out previous);
                var dcs = new object();
                if (data != null)
                    dcs = dcRepo.Get(data.InvoiceDcs.Select(p => p.DcId).ToList());
                decimal balance = 0;
                var contactPerson = "";
                var agingItems = new List<SaleAgingItem>();
                if (data != null && vouchertype == VoucherType.Sale && data.AccountId > 0)
                {
                    var detail = acRepo.GetByAccountId(data.AccountId);
                    contactPerson = detail != null ? detail.ContactPerson : "";
                    balance = tranRepo.GetBalance(data.AccountId, data.Date);
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
                            var sales = saleRepo.GetAgingSale(data.Id, data.AccountId, data.Date, 4);
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
                        DeliveryChallans = dcs,
                        Next = next,
                        Previous = previous,
                        VoucherNumber = voucherNumber,
                        InvoiceNumber = invoiceNumber,
                        Balance = balance,
                        AgingItems = agingItems,
                        ContactPerson = contactPerson,
                    }
                };

            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        public ApiResponse Get(int accountid, VoucherType type)
        {
            ApiResponse response;
            try
            {
                var types = new List<int>();
                if (type == VoucherType.Sale)
                {
                    types.Add((int)VoucherType.Sale);
                    types.Add((int)VoucherType.GstSale);
                }
                else
                {
                    types.Add((int)VoucherType.Purchase);
                    types.Add((int)VoucherType.GstPurchase);
                }

                var data = new SaleRepository().GetInvoicesByAccountId(accountid, types);
                response = new ApiResponse()
                {
                    Success = true,
                    Data = data,
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
                ;
            }
            return response;
        }
        public virtual ApiResponse Get(string key, string dcIds)
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                var vouchertype = (byte)((VoucherType)Enum.Parse(typeof(VoucherType), queryString["type"], true));
                var Ids = dcIds.Split(',').Select(p => Numerics.GetInt(p)).ToList();
                var data = new DeliveryChallanRepository().AsQueryable(true).Where(p => Ids.Contains(p.Id)).SelectMany(p => p.DCItems).GroupBy(q => new { q.ItemId, q.ItemCode, q.ItemName }).Select(r => new
                {

                    r.Key.ItemId,
                    r.Key.ItemCode,
                    r.Key.ItemName,
                    Quantity = r.Sum(m => m.Quantity),


                }).ToList();


                response = new ApiResponse
                {
                    Success = true,
                    Data = data

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
                var err = TransactionManager.ValidateSave(input, false, true);
                if (err == "")
                {
                    input.FiscalId = SiteContext.Current.Fiscal.Id;
                    TransactionManager.Save(input);
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
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
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
                var vouchertype = (VoucherType)Convert.ToByte(type);
                TransactionManager.Delete(id, vouchertype);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }

            return response;
        }
    }
}
