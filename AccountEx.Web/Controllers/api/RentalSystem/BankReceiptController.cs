using AccountEx.BussinessLogic;
using AccountEx.CodeFirst.Models.COA;
using AccountEx.CodeFirst.Models.Transactions;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.Repositories.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AccountEx.Web.Controllers.api.Transaction
{
    public class BankReceiptController : ApiController
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
                var vouchertype = VoucherType.FortressBankReceipt;
                //var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
                bool next, previous;
                var tranRepo = new TransactionRepository();
                var challanRepo = new ChallanRepository(tranRepo);
                var acRepo = new AccountDetailRepository(tranRepo);
                if (voucherNumber == 0) key = "nextvouchernumber";
                if (key == "nextvouchernumber")
                {
                    voucherNumber = tranRepo.GetNextVoucherNumber(vouchertype);
                    invoiceNumber = tranRepo.GetNextInvoiceNumber(vouchertype);
                }
                var data = challanRepo.GetByRcvNumber(voucherNumber, vouchertype, key, out next, out previous);
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Challans = data,
                        Next = next,
                        Previous = previous,
                        VoucherNumber = voucherNumber,
                        InvoiceNumber = invoiceNumber,
                    }
                };

            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        //private ApiResponse Get(int id)
        //{
        //    ApiResponse response;
        //    try
        //    {
        //        var transRepo = new TransactionRepository();
        //        var voucherNo = transRepo.GetNextVoucherNumber(VoucherType.FortressBankReceipt);
        //        var voucherNumber = id;
        //        var queryString = Request.RequestUri.ParseQueryString();
        //        var key = queryString["key"].ToLower();
        //        if (voucherNumber == 0) key = "nextvouchernumber";
        //        if (key == "nextvouchernumber")
        //            voucherNumber = transRepo.GetNextVoucherNumber(VoucherType.FortressBankReceipt);
        //        response = new ApiResponse
        //        {
        //            Success = true,
        //            Data = new
        //            {
        //                VoucherNumber = voucherNumber,
        //            }
        //        };

        //    }
        //    catch (Exception ex)
        //    {
        //        response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
        //    }
        //    return response;
        //}
        public ApiResponse Get(bool loadchallan)
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                //  var challans = new GenericRepository<ChallanItem>().GetAll();
                var challans = new ChallanItemRepository().GetUnReceivedChallans();
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Challans = challans
                    },
                };

            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }

        public ApiResponse Get(bool loadchallan, int challanNumber, string key)
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                var challans = new ChallanRepository().AsQueryable().FirstOrDefault(p => p.Id == challanNumber && p.IsReceived == false);
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Challans = challans
                    },
                };

            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }


        public ApiResponse Post([FromBody]List<ReceiveRentChallanExtra> challans)
        {
            ApiResponse response;
            try
            {
                var err = ServerValidateSave(challans);
                if (err == "")
                {
                    ChallanManager.ReceiveRentalChallan(challans);
                    response = new ApiResponse() { Success = true };
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

        public ApiResponse Delete(int id, string key)
        {
            ApiResponse response;
            try
            {

                var repo = new BankReceiptRepository();
                var challanRepo = new ChallanRepository(repo);
                var transactionRepo = new TransactionRepository(repo);
                if (key == "byvoucherNo")
                {
                    var challans = challanRepo.GetByRcvNo(id);
                    var err = ServerValidateDelete(id, key, challans);
                    if (err == "")
                    {
                        BankReceiptManager.DeleteByVoucherNo(id);
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
                else
                {

                    var challans = challanRepo.GetChallanById(id);
                    var err = ServerValidateDelete(id, key, challans);
                    if (err == "")
                    {
                        BankReceiptManager.Delete(id);
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



            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }

            return response;
        }



        private string ServerValidateSave(List<ReceiveRentChallanExtra> challans)
        {
            var err = ",";

            //if (SettingManager.RentalIncomeHeadId == 0)
            //{
            //    err += "Fee receiving Account is missing.,";
            //}
            if (SettingManager.BankAccountId == 0)
            {
                err += "Bank Account is missing.,";
            }
            if (SettingManager.AdjustmentHeadId == 0)
            {
                err += "Adjustment Account is missing.,";
            }
            if (SettingManager.SurchargeAccountId == 0)
            {
                err += "Surcharge Account is missing.,";
            }
            var groupChallans = challans.GroupBy(p => p.Id).Select(p => new
            {
                ChallanNo = p.Key,
                Count = p.Count()
            }).Where(p => p.Count > 1).ToList();

            foreach (var item in groupChallans)
            {
                err += item.ChallanNo + " must be added once in list.(Current Count:" + item.Count + "),";
            }
            var challanIds = challans.Select(p => p.Id).Distinct().ToList();
            var dbChallans = new ChallanRepository().GetByIds(challanIds);
            var rentChallans = dbChallans.Where(p => p.TransactionType  == VoucherType.RC).ToList();
            var ElectricityChallan = dbChallans.Where(p => p.TransactionType  == VoucherType.ElectictyChallan).ToList();
            foreach (var rent in rentChallans)
            {
                var lpChallan = new ChallanRepository().GetLastPeriodChallan(Numerics.GetInt(rent.RentAgreementId), VoucherType.RC);
                if (lpChallan != null && (lpChallan.FromDate != rent.FromDate || lpChallan.ToDate != rent.ToDate))
                {
                    var lastPeriod = UtilityFunctionManager.GetChallanPeriod(lpChallan);
                    err += "Challan No. " + rent.Id + " should be last period (" + lastPeriod + ") challan to receive.,";

                }
            }
            foreach (var ec in ElectricityChallan)
            {

                var lpChallan = new ChallanRepository().GetLastPeriodChallan(Numerics.GetInt(ec.RentAgreementId), VoucherType.ElectictyChallan);
                if (lpChallan != null && (lpChallan.FromDate != ec.FromDate || lpChallan.ToDate != ec.ToDate))
                {
                    var lastPeriod = UtilityFunctionManager.GetChallanPeriod(lpChallan);
                    err += "Challan No. " + ec.Id + " should be last period (" + lastPeriod + ") challan to receive.,";

                }

            }


            err = err.Trim(',');
            return err;
        }
        private string ServerValidateDelete(int id, string key, List<Challan> challans)
        {
            var err = ",";
            var rentRepo = new RentMonthlyLiabilityRepository();
            var rentChallans = challans.Where(p => p.TransactionType  == VoucherType.RC).ToList();
            var ElectricityChallan = challans.Where(p => p.TransactionType  == VoucherType.ElectictyChallan).ToList();
            foreach (var rent in rentChallans)
            {
                var lpChallan = new ChallanRepository().GetLastPeriodChallan(Numerics.GetInt(rent.RentAgreementId), VoucherType.RC);
                if (lpChallan != null && (lpChallan.FromDate != rent.FromDate || lpChallan.ToDate != rent.ToDate))
                {
                    var lastPeriod = UtilityFunctionManager.GetChallanPeriod(lpChallan);
                    err += "Challan No. " + rent.Id + " should be last period (" + lastPeriod + ") challan to delete.,";

                }
            }

            foreach (var ec in ElectricityChallan)
            {

                var lpChallan = new ChallanRepository().GetLastPeriodChallan(Numerics.GetInt(ec.RentAgreementId), VoucherType.ElectictyChallan);
                if (lpChallan != null && (lpChallan.FromDate != ec.FromDate || lpChallan.ToDate != ec.ToDate))
                {
                    var lastPeriod = UtilityFunctionManager.GetChallanPeriod(lpChallan);
                    err += "Challan No. " + ec.Id + " should be last period (" + lastPeriod + ") challan to delete.,";

                }

            }

            err = err.Trim(',');
            return err;
        }
    }
}
