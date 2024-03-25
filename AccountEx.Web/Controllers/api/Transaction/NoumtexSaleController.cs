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
    public class NoumtexSaleController : BaseApiController
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
                var tranRepo = new TransactionRepository();
                var saleRepo = new SaleRepository(tranRepo);
                var dcRepo = new DeliveryChallanRepository(tranRepo);
                var acRepo = new AccountDetailRepository(tranRepo);
                var cpRepo = new CompanyPartnerRepository(tranRepo);
                var dc = new object();
                bool next, previous;
                var companyPartner = new CompanyPartner();
                if (voucherNumber == 0) key = "nextvouchernumber";
                if (key == "nextvouchernumber")
                    //voucherNumber = new TransactionRepository().GetNextVoucherNumber(vouchertype);
                    voucherNumber = saleRepo.GetNextVoucherNumber(vouchertype);

                var data = saleRepo.GetByVoucherNumber(voucherNumber, vouchertype, key, out next, out previous);
                if (data != null && data.CompanyPartnerId != null)
                    companyPartner = cpRepo.GetById(data.CompanyPartnerId.Value);

                var dcs = new object();
                if (data != null)
                {
                    dcs = dcRepo.Get(data.InvoiceDcs.Select(p => p.DcId).ToList());

                }

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
                        Dc = dc,
                        Next = next,
                        Previous = previous,
                        VoucherNumber = voucherNumber,
                        Balance = balance,
                        AgingItems = agingItems,
                        ContactPerson = contactPerson,
                        CompanyPartner = companyPartner
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
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
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

                var data = new DeliveryChallanRepository().AsQueryable(true).Where(p => Ids.Contains(p.Id)).SelectMany(p => p.DCItems).ToList();


                //var data = new DeliveryChallanRepository().AsQueryable(true).Where(p => Ids.Contains(p.Id)).SelectMany(p => p.DCItems).GroupBy(q => new { q.ItemId, q.ItemCode, q.ItemName, q.Weight, q.Width, q.Meters, q.Rolls, q.TotalMeters, q.StandardWeight }).Select(r => new
                //{

                //    r.Key.ItemId,
                //    r.Key.ItemCode,
                //    r.Key.ItemName,
                //    r.Key.Weight,
                //    r.Key.Width,
                //    r.Key.Meters,
                //    r.Key.Rolls,
                //    r.Key.TotalMeters,
                //    r.Key.StandardWeight,
                //    r.FirstOrDefault().UnitType,
                //    r.FirstOrDefault().Rate,
                //    Amount = r.Sum(m => m.Amount),
                //    NetAmount = r.Sum(m => m.NetAmount),
                //    Quantity = r.Sum(m => m.Quantity),


                //}).ToList();



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
                var err = TransactionManager.ValidateSave(input, true);
                if (err == "")
                {
                    input.FiscalId = SiteContext.Current.Fiscal.Id;
                    bool addtransaction = input.TransactionType == VoucherType.SaleReturn ? true : false;
                    TransactionManager.Save(input, addtransaction, true);
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
                var err = ServerValidateDelete();
                if (err == "")
                {
                    var queryString = Request.RequestUri.ParseQueryString();
                    var type = (queryString["type"]);
                    var vouchertype = (VoucherType)Convert.ToByte(type);
                    TransactionManager.Delete(id, vouchertype);
                    response = new ApiResponse { Success = true };
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
        private string ServerValidateDelete()
        {
            var err = ",";
            try
            {

                if (!SiteContext.Current.User.IsAdmin)
                {
                    if (!SiteContext.Current.RoleAccess.CanDelete)
                    {
                        err += "you did not have sufficent right to delete the voucher.,";
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
