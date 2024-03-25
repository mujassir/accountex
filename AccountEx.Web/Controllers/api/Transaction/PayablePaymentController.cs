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

namespace AccountEx.Web.Controllers.api.Transaction
{
    public class PayablePaymentController : BaseApiController
    {
        public ApiResponse Get()
        {

            var queryString = Request.RequestUri.ParseQueryString();
            var supplierId = Numerics.GetInt(queryString["supplierId"]);
            ApiResponse response;
            try
            {
                response = new ApiResponse()
                {
                    Success = true,
                    Data = new VoucherRepository().GetPedningVouchers(VoucherType.VehiclePayable, supplierId)
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
                var vouchertype = (VoucherType)Convert.ToByte(VoucherType.PayablePayment);
                //var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
                bool next, previous;
                if (voucherNumber == 0) key = "nextvouchernumber";
                var repo = new PayablePaymentRepository();
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
                        Invoice = data != null ? new VoucherRepository().GetVoucherByIdforPayablePayment(VoucherType.VehiclePayable, data.VoucherId) : null,
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


        public ApiResponse Post([FromBody]PayablePayment input)
        {
            ApiResponse response;


            try
            {
                //var err = ServerValidateSave(input);
                var err = "";
                if (err == "")
                {
                    input.FiscalId = SiteContext.Current.Fiscal.Id;
                    VoucherManager.SavePayablePayment(input);
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
                    var vouchertype = (VoucherType)Convert.ToByte(type);
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
    }
}
