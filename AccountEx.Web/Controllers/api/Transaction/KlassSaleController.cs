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
using System.Text;

namespace AccountEx.Web.Controllers.api.Transaction
{
    public class KlassSaleController : BaseApiController
    {
        public JQueryResponse Get()
        {
            return GetDataTable();
        }
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
                var vouchertype =(VoucherType)Convert.ToByte(type);
                //var data =KlassTransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
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
                if (type  == VoucherType.Sale)
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
                var dcID = Ids.Any() ? Ids.FirstOrDefault() : 0;
                var dc = new DeliveryChallanRepository().AsQueryable(true).Where(p => p.Id == dcID).Select(p => new
                {
                    p.AccountId,
                    p.PartyAddress
                });
                var data = new DeliveryChallanRepository().AsQueryable(true).Where(p => Ids.Contains(p.Id)).SelectMany(p => p.DCItems).GroupBy(q => new { q.Id, q.ItemId, q.ItemCode, q.ItemName }).Select(r => new
                {
                    r.Key.Id,
                    r.Key.ItemId,
                    r.Key.ItemCode,
                    r.Key.ItemName,
                    Quantity = r.Sum(m => m.Quantity),
                }).OrderBy(p => p.Id).ToList();
                //var data = new DeliveryChallanRepository().AsQueryable(true).FirstOrDefault(p => Ids.Contains(p.Id))
                //var data = new DeliveryChallanRepository().AsQueryable(true).FirstOrDefault(p => Ids.Contains(p.Id)).DCItems(p => p.DCItems).G.Select(r => new
                //{
                //    r.Key.ItemId,
                //    r.Key.ItemCode,
                //    r.Key.ItemName,
                //    Quantity = r.Sum(m => m.Quantity),
                //}).ToList();


                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        DcDetail = dc,
                        DCList = data
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
                var err =KlassTransactionManager.ValidateSave(input, false, true);
                if (err == "")
                {
                    foreach (var item in input.SaleItems)
                    {
                        item.Amount = item.Quantity * item.Rate;
                        item.DiscountAmount = (item.Amount * item.DiscountPercent) / 100;
                        item.NetAmount = Numerics.GetInt(item.Amount - item.DiscountAmount);
                    }
                    input.QuantityTotal = input.SaleItems.Sum(p => p.Quantity);
                    input.Discount = input.SaleItems.Sum(p => p.DiscountAmount);
                    input.GrossTotal = input.SaleItems.Sum(p => p.Amount);
                    input.NetTotal = Numerics.GetInt(input.GrossTotal - input.Discount);
                    input.FiscalId = SiteContext.Current.Fiscal.Id;
                   KlassTransactionManager.Save(input, true, false);
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
        protected JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "VoucherNumber", "AccountCode", "AccountName", "OrderNo", "Date" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var dcNumbers = new List<int>();
            var type = Convert.ToByte(queryString["type"]);
            var pageType = Convert.ToByte(queryString["pageType"]);


            var search = (queryString["sSearch"] + "").Trim();
            var intSearch = Numerics.GetInt((queryString["sSearch"] + "").Trim());
            if (!string.IsNullOrWhiteSpace(queryString["dcNumbers"]))
                dcNumbers = (queryString["dcNumbers"].Split(',')).ToList().Select(p => Numerics.GetInt(p)).ToList();
            //var records = new DeliveryChallanRepository().GetPendingDeliveryChallan(type);
            var records = new GenericRepository<vw_PendingDeliveryChallan>().AsQueryable(true).Where(p => p.Status != (byte)TransactionStatus.Delivered);
            if (pageType > 0)
                records = records.Where(p => p.InvoiceTransactionType == pageType);

            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                    p.VoucherNumber == intSearch ||
                    p.OrderNumber == intSearch ||
                    p.AccountName.Contains(search) ||
                    p.AccountCode.Contains(search)

                    );
            var orderedList = filteredList.OrderByDescending(p => p.Id);
            //if (colIndex < coloumns.Length && coloumns[colIndex] + "" != "")
            //{
            //    var sortDir = queryString["sSortDir_0"];
            //    orderedList = sortDir == "asc" ? filteredList.OrderBy(coloumns[colIndex]) :
            //        filteredList.OrderByDescending(coloumns[colIndex]);
            //}
            var sb = new StringBuilder();
            sb.Clear();

            var rs = new JQueryResponse();
            foreach (var item in orderedList)
            {
                var data = new List<string>();
                var strChecked = "";
                if (dcNumbers.Contains(item.VoucherNumber))
                    strChecked = "checked='checked'";
                data.Add("<td><input type='checkbox' " + strChecked + " class='checkboxes' value='" + (string.IsNullOrWhiteSpace(strChecked) ? "0" : "1") + "' /></td>");
                data.Add("<input type='text' class='DCNo form-control hide' value='" + item.VoucherNumber + "' /><input type='text' class='DcId hide' value='" + item.Id + "' />" + item.VoucherNumber + "");
                data.Add(item.AccountCode);
                data.Add(item.AccountName);
                data.Add(item.OrderNumber + "");
                //data.Add(item.OrderDate.ToString(AppSetting.GridDateFormat));
                data.Add(item.Date.ToString(AppSetting.GridDateFormat1));
                rs.aaData.Add(data);
            }
            rs.sEcho = echo;
            rs.iTotalRecords = totalRecords;
            rs.iTotalDisplayRecords = totalDisplayRecords;
            return rs;
        }

        public ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                var type = (queryString["type"]);
                var vouchertype =(VoucherType)Convert.ToByte(type);
               KlassTransactionManager.Delete(id, vouchertype);
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
