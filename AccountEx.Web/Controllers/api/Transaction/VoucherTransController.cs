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
using AccountEx.Repositories.Transactions;

namespace AccountEx.Web.Controllers.api.Transaction
{
    public class VoucherTransController : BaseApiController
    {
        public ApiResponse Get(int id, string name)
        {
            ApiResponse response;
            try
            {
                response = new ApiResponse()
                {
                    Success = true,
                    Data = new
                    {
                        Order = new GenericRepository<Voucher>().GetById(id),
                    }
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
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
                var type = queryString["type"];
                var key = queryString["key"].ToLower();
                var vouchertype = (VoucherType)Convert.ToByte(type);
                //var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
                bool next, previous;
                if (voucherNumber == 0) key = "nextvouchernumber";
                var repo = new VoucherTransRepository();
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

        //public ApiResponse Get(int voucher)
        //{
        //    ApiResponse response;
        //    try
        //    {
        //        var result = "";
        //        var queryString = Request.RequestUri.ParseQueryString();
        //        var type = (queryString["type"]);
        //        var vouchertype =(VoucherType)Convert.ToByte(type);
        //        var data = VoucherManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);

        //        response = new ApiResponse
        //        {
        //            Success = true,
        //            Data = data

        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //         response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
        //    }
        //    return response;
        //}

        public ApiResponse Post([FromBody]Voucher input)
        {
            ApiResponse response;


            try
            {
                var err = ServerValidateSave(input);
                if (err == "")
                {
                    input.FiscalId = SiteContext.Current.Fiscal.Id;
                    VoucherManager.Save(input);
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
        public ApiResponse Post([FromBody]Voucher input, bool bankVoucher)
        {
            ApiResponse response;
            try
            {
                var err = ServerValidateSave(input);
                if (err == "")
                {
                    input.FiscalId = SiteContext.Current.Fiscal.Id;
                    VoucherManager.SaveBankVoucher(input);
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

        public ApiResponse Post(bool allVoucher, bool isfinal, [FromBody]Voucher input)
        {
            ApiResponse response;
            try
            {


                VoucherManager.FinalUnfinalAllVoucher(input, isfinal);

                response = new ApiResponse
                {
                    Success = true,
                    Data = input.Id
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }


            return response;

        }

        public ApiResponse Post([FromBody]Voucher input, bool bankVoucher, bool isfinal)
        {
            ApiResponse response;
            try
            {


                VoucherManager.FinalUnfinalBankVoucher(input, isfinal);

                response = new ApiResponse
                {
                    Success = true,
                    Data = input.Id
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
                var voucherNo = id;
                var err = ServerValidateDelete(voucherNo, vouchertype);
                if (err == "")
                {


                    VoucherManager.Delete(id, vouchertype);
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

        public JQueryResponse GetDataTable()
        {

            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "VoucherNumber", "Date", "Amount", "AccountName", "Username" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var type = queryString["type"];
            var transactiontype = (VoucherType)Convert.ToByte(type);
            var search = (queryString["sSearch"] + "").Trim();
            var branchId = Numerics.GetInt((queryString["branchId"]));
            var customerid = Numerics.GetInt(queryString["FilterCustomer"] + "");
            var fromdate = DateConverter.ConvertStandardDate(queryString["FromDate"] + "");
            var todate = DateConverter.ConvertStandardDate(queryString["ToDate"] + "");
            var records = new GenericRepository<vw_Vouchers>().AsQueryable(true).Where(p => p.TransactionType == transactiontype);

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
                var date = item.Date.ToString() == "" ? "" : item.Date.Value.ToString("dd/MM/yyyy");
                var data = new List<string>();

                data.Add(item.VoucherNumber + "");
                data.Add(date);
                data.Add(Numerics.IntToString(item.Amount));
                if (item.TransactionType != VoucherType.TransferVoucher)
                    data.Add(item.AccountName);

                data.Add(item.Username);


                var editIcon = "<i class='fa fa-edit' onclick=\"VoucherTrans.Edit(" + item.Id + ")\" title='Edit' ></i>";
                if (transactiontype == VoucherType.TransferVoucher)
                    editIcon = "<i class='fa fa-edit' onclick=\"JV.Edit(" + item.Id + ")\" title='Edit' ></i>";
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

        //protected ApiResponse GetDataTable()
        //{
        //    ApiResponse response;
        //    try
        //    {
        //        var result = "";
        //        var queryString = Request.RequestUri.ParseQueryString();
        //        var vouchertype = Convert.ToByte((queryString["type"]));
        //        var voucher = Numerics.GetInt((queryString["voucher"]));
        //        var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
        //        result = JsonConvert.SerializeObject(data, GetJsonSetting());
        //        response = new ApiResponse
        //        {
        //            Success = true,
        //            Data = result

        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //         response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
        //    }
        //    return response;
        //}


        private string ServerValidateSave(Voucher input)
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
                if (SettingManager.UnpresentedChequeHeadId == 0)
                {
                    err += "Unpresented Cheque Account is missing.";
                }
                if (SettingManager.PostDatedChequesHeadId == 0)
                {
                    err += "Post Dated Cheque Account is missing.";
                }
                if (SiteContext.Current.Fiscal.IsClosed)
                {
                    err += "Fiscal year is closed No action can be done.,";
                }
                if (!bankType.Contains(input.TransactionType) && input.TransactionType != VoucherType.TransferVoucher)
                {
                    if (input.AccountId == null || input.AccountId == 0)
                    {
                        err += "Account is not valid to process the request.,";
                    }
                }

                if (bankType.Contains(input.TransactionType))
                {
                    var accounts = input.VoucherItems.Select(p => p.AccountId).ToList();
                    if (input.AccountId.HasValue && input.AccountId > 0)
                        accounts.Add(input.AccountId.Value);
                    var isBankSelected = accountRepo.IsHeadSelected(accounts, SettingManager.BankHeadId);
                    if (!isBankSelected)
                    {
                        err += "Atleast one bank must be selected.,";
                    }
                }

                foreach (var item in input.VoucherItems.Where(p => p.AccountId == 0))
                {
                    err += item.AccountCode + "-" + item.AccountName + " is not valid code.,";
                }

                var isExist = repo.IsVoucherExistByVoucherNo(input.VoucherNumber, input.Id, input.TransactionType);

                if (isExist)
                {
                    err += "<li>Voucher no already exist.</li>";
                }
                if (SettingManager.IsAllowVehicleValidation)
                {
                    var accountIds = input.VoucherItems.Select(p => p.AccountId).ToList();
                    var expensesDetil = new AccountDetailRepository().GetVehcileConfig(accountIds);
                    foreach (var item in expensesDetil)
                    {
                        if (input.VehicleId == null || input.VehicleId == 0)
                        {
                            if (item.IsVehicleRequired.HasValue && item.IsVehicleRequired.Value)
                            {
                                err += "Vehicle must be selected for expense " + item.AccountName;
                            }
                        }
                        if (input.VehicleId > 0 && item.IsUniquePerVehicle.HasValue && item.IsUniquePerVehicle.Value)
                        {
                            isExist = new VoucherTransRepository().IsExpenseExistByVehicleId(input.VehicleId.Value, item.AccountId, input.TransactionType, input.Id);
                            if (isExist)
                            {
                                err += item.AccountName + " can only be added once for this vehicle.";
                            }


                        }

                    }
                }
                if (input.TransactionType == VoucherType.VehiclePayable)
                {
                    if (new PayablePaymentRepository().IsPaymentExist(input.Id))
                    {
                        err += "voucher has payments and can't be updated.,";
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

        private string ServerValidateDelete(int voucherNo, VoucherType type)
        {
            var err = ",";
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                var vouchertype = (VoucherType)Convert.ToByte(type);
                if (!SiteContext.Current.User.IsAdmin)
                {
                    if (!SiteContext.Current.RoleAccess.CanDelete)
                    {
                        err += "you did not have sufficent right to delete the voucher.,";
                    }
                }
                if (vouchertype == VoucherType.VehiclePayable)
                {
                    if (new PayablePaymentRepository().IsPaymentExist(voucherNo, type))
                    {
                        err += "voucher has payments and can't be deleted.,";
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
