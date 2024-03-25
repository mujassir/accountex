using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using AccountEx.BussinessLogic;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;

namespace AccountEx.Web.Controllers.api.Transaction
{
    public class JobOrderRequisitionController : BaseApiController
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
                var queryString = Request.RequestUri.ParseQueryString();
                var type = (VoucherType)Convert.ToByte(queryString["type"]);
                var key = queryString["key"].ToLower();
                var reqRepo = new JobOrderRequisitionRepository();
                var orderRepo = new OrderBookingRepository(reqRepo);
                //var vouchertype =(VoucherType)Convert.ToByte(type);
                var vouchertype = new List<VoucherType>();

                /////code for serviceorder addition
                if (type == VoucherType.Requisition)
                {
                    vouchertype.Add(VoucherType.Requisition);
                }
                else
                {
                    vouchertype.Add(VoucherType.CustomerServiceOrder);
                    vouchertype.Add(VoucherType.SiteServiceOrder);
                    vouchertype.Add(VoucherType.RepairingServiceOrder);
                }
                //////////end
                object orderinfo = "";
                //var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
                bool next, previous;
                if (voucherNumber == 0) key = "nextvouchernumber";
                if (key == "nextvouchernumber")
                    voucherNumber = reqRepo.GetNextVoucherNumber(vouchertype);
                var data = reqRepo.GetByVoucherNumber(voucherNumber, vouchertype, key, out next, out previous);
                if (data != null)
                {
                    orderinfo = orderRepo.AsQueryable(true).Where(p => p.Id == data.OrderId).Select(p =>
                         new
                         {
                             p.Date,
                             p.DeliveryDate,
                         }
                         ).FirstOrDefault();
                }
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Order = data,
                        OrderInfo = orderinfo,
                        Next = next,
                        Previous = previous,
                        VoucherNumber = voucherNumber
                    }
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        public ApiResponse Post([FromBody]Requisition input)
        {
            ApiResponse response;
            try
            {
                var err = RequisitionManager.ValidateSave(input);
                if (err == "")
                {
                    input.FiscalId = SiteContext.Current.Fiscal.Id;
                    RequisitionManager.Save(input);
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
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }


        public ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                var err = RequisitionManager.ValidateDelete(id);
                if (err == "")
                {
                    var queryString = Request.RequestUri.ParseQueryString();
                    RequisitionManager.Delete(id);
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


        protected JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "RequisitionNo", "Requisition Date", "OrderNo" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var voucherType = (VoucherType)Convert.ToByte(queryString["type"]);
            var pageType = (VoucherType)Convert.ToByte(queryString["key"]);
            var search = (queryString["sSearch"] + "").Trim();
            var intSearch = Numerics.GetInt((queryString["sSearch"] + "").Trim());
            var records = new JobOrderRequisitionRepository().AsQueryable(true);
            if (pageType == VoucherType.GINP)
            {
                records = records.Where(p => p.TransactionType == voucherType && p.Status != (byte)TransactionStatus.Delivered);
            }
            else if (pageType == VoucherType.Services)
            {
                switch (voucherType)
                {
                    case VoucherType.CustomerServiceOrder:
                        records = records.Where(p => p.TransactionType == VoucherType.CustomerServiceOrder && p.Status != (byte)TransactionStatus.Delivered);
                        break;
                    case VoucherType.SiteServiceOrder:
                        records = records.Where(p => p.TransactionType == VoucherType.SiteServiceOrder && p.Status != (byte)TransactionStatus.Delivered);
                        break;
                    case VoucherType.RepairingServiceOrder:
                        records = records.Where(p => p.TransactionType == VoucherType.RepairingServiceOrder && p.Status != (byte)TransactionStatus.Delivered);
                        break;
                    default:
                        records = records.Where(p => (p.TransactionType == VoucherType.CustomerServiceOrder || p.TransactionType == VoucherType.SiteServiceOrder || p.TransactionType == VoucherType.RepairingServiceOrder) && (p.Status != (byte)TransactionStatus.Delivered));
                        break;
                }
            }
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                    p.VoucherNumber == intSearch ||
                     p.OrderNo == intSearch

                    );
            var orderedList = filteredList.OrderByDescending(p => p.Id);
            var totalRecords = orderedList.Count();
            var totalDisplayRecords = totalRecords;
            //if (colIndex < coloumns.Length && coloumns[colIndex] + "" != "")
            //{
            //    var sortDir = queryString["sSortDir_0"];
            //    orderedList = sortDir == "asc" ? filteredList.OrderBy(coloumns[colIndex]) :
            //        filteredList.OrderByDescending(coloumns[colIndex]);
            //}
            var sb = new StringBuilder();
            sb.Clear();

            var rs = new JQueryResponse();
            var allRecords = new List<Requisition>();
            if (displayLength == -1)
                allRecords = orderedList.ToList();
            else
                allRecords = orderedList.Skip(displayStart).Take(displayLength).ToList();

            foreach (var item in allRecords)
            {
                var data = new List<string>();
                data.Add("<input type='text' class='RequisitionNo form-control hide' value='" + item.VoucherNumber + "' />" + item.VoucherNumber + "");
                data.Add("<input type='text' class='TransactionType form-control hide' value='" + (byte)item.TransactionType + "' />" + item.OrderNo + "");
                data.Add((item.Date.HasValue ? item.Date.Value.ToString(AppSetting.GridDateFormat) : ""));
                data.Add(item.OrderNo.ToString());

                //data.Add((item.OrderDate.HasValue ? item.OrderDate.Value.ToString(AppSetting.GridDateFormat) : ""));
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
