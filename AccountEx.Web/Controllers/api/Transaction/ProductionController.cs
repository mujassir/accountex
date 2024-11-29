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
using System.Data.Entity.Validation;

namespace AccountEx.Web.Controllers.api.Transaction
{
    public class ProductionController : BaseApiController
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
                var key = queryString["key"].ToLower();
                var type = queryString["type"];
                var entrytype = Convert.ToByte(type);
                Object orderinfo = "";
                //var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
                bool next, previous;
                var transactionRepo = new TransactionRepository();
                var workInProgressRepo = new WorkInProgressRepository(transactionRepo);
                var orderBookingRepo = new OrderBookingRepository(transactionRepo);
                if (voucherNumber == 0) key = "nextvouchernumber";
                if (key == "nextvouchernumber")
                    voucherNumber = transactionRepo.GetNextVoucherNumber(VoucherType.Production);
                var data = workInProgressRepo.GetByVoucherNumber(voucherNumber, key, out next, out previous);
                if (data != null)
                {
                    orderinfo = orderBookingRepo.AsQueryable(true).Where(p => p.Id == data.OrderId).Select(p => new
                    {
                        orderdate = p.Date
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

        public ApiResponse Get(int orderno, VoucherType type)
        {
            ApiResponse response;
            try
            {
                var orderBookingRepo = new OrderBookingRepository();
                var dcRepo = new DCItemRepository(orderBookingRepo);

                var queryString = Request.RequestUri.ParseQueryString();
                int locationId = 0;
                if (queryString["locationId"] != null)
                {
                    int.TryParse(queryString["locationId"], out locationId);
                }
                var rawmaterial = dcRepo.GetByOrderNo(orderno, type);
                var order = orderBookingRepo.GetByVoucherNumber(orderno, VoucherType.SaleOrder, locationId);
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        RawMaterial = rawmaterial,
                        Order = order
                    }
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        public ApiResponse Post([FromBody]WorkInProgress input)
        {
            ApiResponse response;
            try
            {
                var err = ServerValidateSave(input);
                if (err == "")
                {
                    input.FiscalId = SiteContext.Current.Fiscal.Id;
                    ProductionManager.Save(input, true, false);
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
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            //catch (Exception ex)
            //{
            //     response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            //}

            return response;

        }
        protected JQueryResponse GetDataTable()
        {
            var coloumns = new[] { "VoucherNumber", "InvoiceNumber", "Date", "NetTotal", "FinishedNetTotal", "TotalCharges", "Difference", "" };
            var queryString = Request.RequestUri.ParseQueryString();
            var echo = Convert.ToInt32(queryString["sEcho"]);
           
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var type = queryString["type"];
            var search = (queryString["sSearch"] + "").Trim();
            var branchId = Numerics.GetInt((queryString["branchId"]));
            var customerid = Numerics.GetInt(queryString["FilterCustomer"] + "");
            var fromdate = DateConverter.ConvertStandardDate(queryString["FromDate"] + "");
            var todate = DateConverter.ConvertStandardDate(queryString["ToDate"] + "");
            var records = new WorkInProgressRepository().AsQueryable(true);

            //if (branchId > 0)
            //    records = records.Where(p => p.BranchId == branchId);

            if (!string.IsNullOrEmpty(queryString["FromDate"] + "") && !string.IsNullOrEmpty(queryString["ToDate"] + ""))
                records = records.Where(p => p.Date >= fromdate && p.Date <= todate);
            //if (!string.IsNullOrEmpty(queryString["FilterCustomer"] + ""))
            //    records = records.Where(p => p.VoucherItems.Any(q => q.AccountId == customerid));

            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;

            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                    p.AccountName.Contains(search)
                    );
            var orderedList = filteredList.OrderByDescending(p => p.Id);
            if (colIndex < coloumns.Length && coloumns[colIndex] + "" != "")
            {
                var sortDir = queryString["sSortDir_0"];
                orderedList = sortDir == "asc" ? filteredList.OrderBy(coloumns[colIndex]) :
                    filteredList.OrderByDescending(coloumns[colIndex]);
            }
            var sb = new StringBuilder();
            sb.Clear();

            var rs = new JQueryResponse();
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                var data = new List<string>();

                data.Add(item.VoucherNumber + "");
                data.Add(item.InvoiceNumber + "");
                data.Add(item.Date.ToString(AppSetting.GridDateFormat));
                data.Add(Numerics.IntToString(item.NetTotal));
                data.Add(Numerics.IntToString(item.FinishedNetTotal));
                data.Add(Numerics.IntToString(item.TotalCharges));
                data.Add(Numerics.DecimalToString(item.Difference));
                var editIcon = "<i class='fa fa-edit' onclick=\"Production.Edit(" + item.VoucherNumber + ")\" title='Edit' ></i>";
                //var deleteIcon = "<i class='fa fa-trash-o' onclick=\"VoucherTrans.Delete(" + item.Id + ")\" title='Delete' ></i>";
                var icons = "<span class='action'>";
                icons += editIcon;
                //icons += deleteIcon;
                icons += "</span>";
                data.Add(icons);
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
                // var vouchertype =(VoucherType)Convert.ToByte(type);
                var voucherNo = Numerics.GetInt(queryString["voucherNo"]);
                var err = ServerValidateDelete(id);
                if (err == "")
                {
                    ProductionManager.Delete(voucherNo, VoucherType.Production);
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

        private string ServerValidateSave(WorkInProgress input)
        {
            var err = ",";
            try
            {
                var orderBookingRepo = new OrderBookingRepository();
                var dcRepo = new DeliveryChallanRepository(orderBookingRepo);
                var workInProgressRepo = new WorkInProgressRepository(orderBookingRepo);

                var record = workInProgressRepo.IsVoucherExists(input.VoucherNumber, input.Id);
                if (SiteContext.Current.Fiscal == null)
                {
                    err += "No Fiscal year found.,";
                }
                if (SiteContext.Current.Fiscal.IsClosed)
                {
                    err += "Fiscal year is closed No action can be done.,";
                }
                foreach (var item in input.WPItems.Where(p => p.ItemId == 0))
                {
                    err += item.ItemCode + "-" + item.ItemName + " is no valid.,";
                }
                if (record)
                {
                    err += "Voucher no already exist.,";
                }
                record = workInProgressRepo.IsBookNoExists(input.InvoiceNumber, input.Id);
                if (record)
                {
                    err += "Book no already exist.,";
                }

                //var order = orderBookingRepo.GetByVoucherNumber(input.OrderNo, VoucherType.SaleOrder, (byte)OrderType.Production);

                //if (order == null)
                //{
                //    err += "Invalid order no.,";
                //}
                //var WP = workInProgressRepo.GetByOrderNumber(input.OrderNo, input.Id);

                //if (WP != null)
                //{
                //    err += "Order no is already used in Work in Progress (Voucher No:" + WP.VoucherNumber + ").,";
                //}
                //if (input.Id > 0)
                //{
                //    var dbWIP = workInProgressRepo.GetById(input.Id);
                //    if (dbWIP.VoucherNumber != input.VoucherNumber)
                //    {
                //        err += "can't change GINP no.please use previous GINP no.(" + dbWIP.VoucherNumber + "),";
                //    }
                //    if (dbWIP.OrderNo != input.OrderNo)
                //    {
                //        err += "can't change order no.please use previous order no.(" + dbWIP.OrderNo + "),";
                //    }
                //    var FGRN = dcRepo.GetByOrderNumber(dbWIP.OrderNo, VoucherType.FGRN);
                //    if (FGRN != null)
                //    {
                //        err += "Work in Progress no is used in Good Received Note and Can't be updated.";

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
        private string ServerValidateDelete(int id)
        {
            var err = ",";
            try
            {
                var dcRepo = new DeliveryChallanRepository();
                var workInProgressRepo = new WorkInProgressRepository(dcRepo);

                var WIP = workInProgressRepo.GetById(id);
                if (WIP == null)
                {
                    err += "invalid WIP no. or no record exist.";
                }
                else
                {
                    var FGRN = dcRepo.GetByOrderNumber(WIP.OrderNo, VoucherType.FGRN);
                    if (FGRN != null)
                    {
                        err += "Work in Progress no is used in Good Received Note and Can't be deleted.";
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
