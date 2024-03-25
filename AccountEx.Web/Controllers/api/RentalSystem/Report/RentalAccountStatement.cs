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
                var transRepo = new TransactionRepository();
                var voucherNo = transRepo.GetNextVoucherNumber((byte)VoucherType.FortressBankReceipt);
                var voucherNumber = id;
                var queryString = Request.RequestUri.ParseQueryString();
                var key = queryString["key"].ToLower();
                if (voucherNumber == 0) key = "nextvouchernumber";
                if (key == "nextvouchernumber")
                    voucherNumber = transRepo.GetNextVoucherNumber((byte)VoucherType.FortressBankReceipt);
                    response = new ApiResponse
                    {
                        Success = true,
                        Data = new
                        {
                            VoucherNumber = voucherNumber,
                        }
                    };
               
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error =ErrorManager.Get() }; Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return response;
        }
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
                    response = new ApiResponse() { Success = false, Data = null };
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
                response = new ApiResponse() { Success = false, Data = null };
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
                response = new ApiResponse { Success = false, Error =ErrorManager.Get() }; Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return response;
        }

        public ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                BankReceiptManager.Delete(id);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error =ErrorManager.Get() }; Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
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
            err = err.Trim(',');
            return err;
        }
    }
}
