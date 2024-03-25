using System;
using System.Net.Http;
using System.Web.Http;
using AccountEx.BussinessLogic;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using AccountEx.CodeFirst.Models.Transactions;
using AccountEx.Repositories.Transactions;
using AccountEx.Repositories.Vehicles;
using AccountEx.CodeFirst.Models.Vehicles;

namespace AccountEx.Web.Controllers.api.Transaction
{
    public class ForexVoucherController : BaseApiController
    {
        //public ApiResponse Get()
        //{

        //    var queryString = Request.RequestUri.ParseQueryString();
        //    var customerId = Numerics.GetInt(queryString["AccountId1"]);
        //    ApiResponse response;
        //    try
        //    {
        //        response = new ApiResponse()
        //        {
        //            Success = true,
        //            Data = new VoucherRepository().GetPedningVouchers(VoucherType.VehiclePayable, customerId)
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
        //    }
        //    return response;
        //}
        public ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                var voucherNumber = id;
                var queryString = Request.RequestUri.ParseQueryString();
                var type = queryString["type"];
                var key = queryString["key"].ToLower();
                var vouchertype = VoucherType.ForexVoucher;
                //var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
                bool next, previous;
                if (voucherNumber == 0) key = "nextvouchernumber";
                var repo = new VehicleVoucherRepository();
                if (key == "nextvouchernumber")

                    //This line of code has benn commented because we are allowing finalization on Bank receipt,payments vouchers 
                    // and if we allow finalization then next voucher should be extracted from Voucher Table not from Transactions Table

                    //voucherNumber = new TransactionRepository().GetNextVoucherNumber(vouchertype);

                    voucherNumber = repo.GetNextVoucherNumber(vouchertype);
                var data = repo.GetByVoucherNumber(voucherNumber, vouchertype, key, out next, out previous);
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
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }



        public ApiResponse Post([FromBody]VehicleVoucher input)
        {
            ApiResponse response;
            try
            {

                var err = ServerValidateSave(input);
                if (err == "")
                {
                    input.FiscalId = SiteContext.Current.Fiscal.Id;
                    VoucherManager.SaveForexVoucher(input);
                    response = new ApiResponse
                    {
                        Success = true,
                        Data = input.Id
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
                    var vouchertype =(VoucherType)Convert.ToByte(type);
                    VoucherManager.DeleteVeicleVoucher(id, vouchertype);
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


        private string ServerValidateSave(VehicleVoucher input)
        {
            var err = ",";
            try
            {
                var repo = new VoucherTransRepository();
                var accountRepo = new AccountRepository(repo);
                var bankType = new List<VoucherType> { VoucherType.BankPayments, VoucherType.BankReceipts };
                if (!SiteContext.Current.User.IsAdmin)
                {
                    if (input.Id == 0)
                    {
                        if (!SiteContext.Current.RoleAccess.CanCreate)
                        {
                            err += "you did not have sufficent right to add new voucher.,";
                        }
                    }
                    else
                    {
                        if (!SiteContext.Current.RoleAccess.CanUpdate)
                        {
                            err += "you did not have sufficent right to update voucher.,";
                        }
                    }
                }
                if (!FiscalYearManager.IsValidFiscalDate(input.Date))
                {
                    err += "Voucher date should be within current fiscal year.,";
                }
                if (SiteContext.Current.Fiscal == null)
                {
                    err += "No fiscal year found.,";

                }


                if (SiteContext.Current.Fiscal.IsClosed)
                {
                    err += "Fiscal year is closed No action can be done.,";
                }

                if (input.AccountId == 0)
                {
                    err += "Account is not valid to process the request.,";
                }
                if (input.AccountId1== 0)
                {
                    err += "Supplier Account is not valid to process the request.,";
                }

                var isExist = repo.IsVoucherExistByVoucherNo(input.VoucherNumber, input.Id, input.TransactionType);

                if (isExist)
                {
                    err += "<li>Voucher no already exist.</li>";
                }
                if (SettingManager.CurrencyAdjustmentAccountId == 0)
                {
                    err += "Currency adjustment Account is missing.";
                }

            }
            catch (Exception)
            {

                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;


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
        public ApiResponse Get(bool doPrinting, int id, string printKey)
        {
            ApiResponse response;
            try
            {
                // var repo = new VehiclePaymentRepository();
                //if (printKey == "CRPrint")
                //{

                //    var vp = repo.GetById(id);
                //    var vs = new vw_VehicleSalesRepository(repo).GetById(vp.VehicleSaleId);
                //    response = new ApiResponse
                //    {
                //        Success = true,
                //        Data = new
                //        {
                //            Payment = vp,
                //            Sale = vs

                //        }
                //    };
                //}
                // else
                {
                    // var data = new vw_VehicleSalesRepository().GetById(id);
                    var repo = new VehicleVoucherRepository();
                    var records = repo.PrintVehicleVoucherById(id);
                    response = new ApiResponse
                    {
                        Success = true,
                        Data = records
                    };
                }

            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        private ApiResponse PrintVehicleVoucherById()
        {

            var branchId = Numerics.GetInt(Request.GetQueryString("branchId"));
            var repo = new VehicleVoucherRepository();
            var records = repo.PrintVehicleVoucherById(branchId);
            var response = new ApiResponse
            {
                Success = true,
                Data = records
            };
            return response;

        }
        public JQueryResponse GetDataTable()
        {

            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "VoucherNumber", "Amount", "AccountName", "Username", "Date" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var type = queryString["type"];
            var transactiontype = VoucherType.ForexVoucher;
            var search = (queryString["sSearch"] + "").Trim();
            var customerid = Numerics.GetInt(queryString["FilterCustomer"] + "");
            var fromdate = DateConverter.ConvertStandardDate(queryString["FromDate"] + "");
            var todate = DateConverter.ConvertStandardDate(queryString["ToDate"] + "");
            var records = new GenericRepository<vw_VehicleVouchers>().AsQueryable(true).Where(p => p.TransactionType == transactiontype);

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
                data.Add(item.Date.Value.ToString("dd/MM/yyyy"));
                data.Add(item.AccountName);
                data.Add(Numerics.IntToString(item.Amount));
                data.Add(item.Username);

                var editIcon = "<i class='fa fa-edit' onclick=\"ForexVoucher.Edit(" + item.VoucherNumber + ")\" title='Edit' ></i>";
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
    }
}
