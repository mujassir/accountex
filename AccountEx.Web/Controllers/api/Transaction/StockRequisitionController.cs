﻿using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using AccountEx.BussinessLogic;
using Elmah.ContentSyndication;

namespace AccountEx.Web.Controllers.api.Transaction
{
    public class StockRequisitionController : BaseApiController
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
                var repo = new StockRequisitionRepository();
                var voucherNumber = id;
                var queryString = Request.RequestUri.ParseQueryString();
                var key = queryString["key"].ToLower();
                var voucher = queryString["voucher"];
                int locationId = 0;
                if (queryString["locationId"] != null)
                {
                    int.TryParse(queryString["locationId"], out locationId);
                }
                //var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
                bool next, previous;
                if (voucherNumber == 0) key = "nextvouchernumber";
                if (key == "nextvouchernumber")
                    voucherNumber = repo.GetNextVoucherNumber(locationId);
                var data = repo.GetByVoucherNumber(voucherNumber, key, locationId, out next, out previous);
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
        public ApiResponse Post([FromBody]StockRequisition input)
        {
            ApiResponse response;
            try
            {
                var err = ServerValidateSave(input);
                // var err = "";
                if (err == "")
                {
                    input.FiscalId = SiteContext.Current.Fiscal.Id;
                    StockRequisitionManager.Save(input);
                    response = new ApiResponse { Success = true, Data = input.Id };
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
                int locationId = 0;
                if (queryString["locationId"] != null)
                {
                    int.TryParse(queryString["locationId"], out locationId);
                }
                var err = ServerValidateDelete(id, locationId);
                if (err == "")
                {
                    new StockRequisitionRepository().DeleteByVoucherNumber(id, locationId);
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
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }
        private string ServerValidateSave(StockRequisition input)
        {
            var err = ",";
            try
            {
                var repo = new StockRequisitionRepository();
                var record = repo.CheckIsVoucherNoExist(input.VoucherNumber, input.Id, input.AuthLocationId);
                foreach (var item in input.StockRequisitionItems.Where(p => p.ItemId == 0))
                {
                    err += item.ItemCode + "-" + item.ItemName + " is no valid.,";
                }
                if (!FiscalYearManager.IsValidFiscalDate(input.Date))
                {
                    err += "Date should be within current fiscal year.,";
                }
                if (SiteContext.Current.Fiscal == null)
                {
                    err += "No Fiscal year found.,";
                }
                if (SiteContext.Current.Fiscal.IsClosed)
                {
                    err += "Fiscal year is closed No action can be done.,";
                }
                if (record)
                {
                    err += "Voucher no already exist.,";
                }
                record = repo.CheckIdInvoiceNoExist(input.InvoiceNumber, input.Id, input.AuthLocationId);

                if (record)
                {
                    err += "Book no already exist.,";
                }
                var Itemcountlist = input.StockRequisitionItems.GroupBy(p => p.ItemId).Select(p => new
                {
                    ItemId = p.Key,
                    ItemCode = p.FirstOrDefault().ItemCode,
                    ItemName = p.FirstOrDefault().ItemName,
                    Count = p.Count()
                }).Where(p => p.Count > 1).ToList();

                foreach (var item in Itemcountlist)
                {
                    err += item.ItemCode + "-" + item.ItemName + " must be added once in item list.(Current Count:" + item.Count + "),";
                }
                if (input.Id > 0)
                {
                    var dbStockRequestion = repo.GetById(input.Id);
                    if (dbStockRequestion.VoucherNumber != input.VoucherNumber)
                    {
                        err += "can't change SRN no.please use previous SRN no.(" + dbStockRequestion.VoucherNumber + "),";
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
        private string ServerValidateDelete(int id, int locationId)
        {
            var err = ",";
            try
            {
                //here id is voucher no
                var repo = new StockRequisitionRepository();
                var orderbookingrepo = new OrderBookingRepository();
                var stockRequestion = repo.GetByVoucherNo(id, locationId);

                if (stockRequestion != null)
                {
                    var order = orderbookingrepo.GetBySRN(stockRequestion.VoucherNumber, VoucherType.PurchaseOrder, stockRequestion.AuthLocationId);
                    if (order != null)
                    {
                        err += "SRN is used in order and can't be deleted.(Order no:" + order.VoucherNumber + ").,";
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
        protected JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "SRN No", "Date", "AuthLocationId" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var type = Convert.ToByte(queryString["type"]);
            var search = (queryString["sSearch"] + "").Trim();
            var Intsearch = Numerics.GetInt((queryString["sSearch"] + "").Trim());
            var records = new StockRequisitionRepository().AsQueryable(true).Where(p => p.Status == (byte)TransactionStatus.Pending);

            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                      p.VoucherNumber == Intsearch
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
                var code = (item.VoucherCode != null ? item.VoucherCode + "-" : "") + item.VoucherNumber;
                data.Add("<input type='text' class='VoucherNumber form-control hide' value='" + code + "' data-location-Id='" + item.AuthLocationId +"' data-location-code='" + item.VoucherCode +"' />" + code + "");
                //data.Add(item.AccountName);
                //data.Add(item.SalesmanName);
                data.Add(item.Date.ToString(AppSetting.GridDateFormat));
                //data.Add((item.DeliveryDate.HasValue ? item.DeliveryDate.Value.ToString(AppSetting.GridDateFormat) : ""));
                rs.aaData.Add(data);
            }
            rs.sEcho = echo;
            rs.iTotalRecords = totalRecords;
            rs.iTotalDisplayRecords = totalDisplayRecords;
            return rs;
        }
    }
}
