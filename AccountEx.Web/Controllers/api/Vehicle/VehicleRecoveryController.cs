
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
using AccountEx.CodeFirst.Models.Transactions;
using Entities.CodeFirst;
using AccountEx.Common.VehicleSystem;
using AccountEx.CodeFirst.Models.Vehicles;
using AccountEx.Repositories.Vehicles;

namespace AccountEx.Web.Controllers.api.Transaction
{
    public class VehicleRecoveryController : BaseApiController
    {
        public ApiResponse Get()
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                var branchId = Numerics.GetInt((queryString["branchId"]));
                var data = new VehicleRepository().GetVehicleRecovery(branchId);
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
        public ApiResponse Get(int vehicleId, int customerId, int saleId)
        {
            ApiResponse response;
            try
            {
                var followups = new VehicleFollowUpRepository().GetFollowUps(vehicleId, customerId);
                var auction = new VehicleAcutionRepository().GetBySaleId(saleId);
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        FollowUps = followups,
                        Auction = auction
                    }
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;

        }

        public ApiResponse Post(int id, byte status)
        {
            ApiResponse response;
            try
            {
                var data = new VehicleSale();
                var agreement = new PrintFurtherAgreement();
                var letter = new PrintRepossessionLetter();

                if (status == (byte)RecoveryStatus.PrintPossession || status == (byte)RecoveryStatus.PrintNotficationLetter)
                {
                    data = new VehicleSaleRepository().GetById(id);
                    var balance = new VehicleSaleRepository().GetBalance(id, data.AccountId, data.VehicleId);
                    letter = VehicleSaleManager.PrintPossessionLetter(id);
                    return response = new ApiResponse
                    {
                        Success = true,
                        Data = new
                        {
                            Sale = data,
                            Letter = letter,
                            Balance = balance
                        }
                    };
                }
                if (status == (byte)RecoveryStatus.PrintFurtherAgreement)
                {
                    data = new VehicleSaleRepository().GetById(id);
                    var balance = new VehicleSaleRepository().GetBalance(id, data.AccountId, data.VehicleId);
                    agreement = VehicleSaleManager.PrintFurtherAgreement(id);
                    return response = new ApiResponse
                    {
                        Success = true,
                        Data = new
                        {
                            Sale = data,
                            Letter = agreement,
                            Balance = balance
                        }
                    };
                }
                if (status == (byte)RecoveryStatus.FinalAuctionnerCharges)
                {
                    data = new VehicleSaleRepository().GetById(id);
                    var auction = new VehicleAcutionRepository().GetBySaleId(id);
                    return response = new ApiResponse
                    {
                        Success = true,
                        Data = new
                        {
                            Sale = data,
                            Auction = auction
                        }
                    };
                }
                else if (status != (byte)RecoveryStatus.CustomerReturn)
                {
                    data = new VehicleSaleRepository().UpdateRecoveryStatus(id, status);
                }

                else
                {
                    data = new VehicleSaleRepository().GetById(id);
                }
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Sale = data,
                        Letter = agreement
                    }
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        //This is for paying recovery installments......return to Customer
        //public ApiResponse Post(VehicleInstallmentPayment input, bool returntoCustomer)
        //{
        //    ApiResponse response;
        //    try
        //    {
        //        VehicleSaleManager.PayInstallmentsByRecovery(input);
        //        response = new ApiResponse { Success = true };
        //    }
        //    catch (Exception ex)
        //    {
        //        response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
        //    }
        //    return response;
        //}

        //issue possession letter & craete advertisement
        public ApiResponse Post(VehicleAcution input, byte status)
        {
            ApiResponse response;
            try
            {

                if (status == (byte)RecoveryStatus.InProcess)
                {
                    VehicleSaleManager.IssuePossessionLetter(input);
                }
                else if (status == (byte)RecoveryStatus.NotficationLetter)
                {
                    VehicleSaleManager.CreateNotification(input);
                }
                else if (status == (byte)RecoveryStatus.FinalAuctionnerCharges)
                {
                    VehicleSaleManager.FinalizeAuctionnerCharges(input);
                }
                else
                {
                    VehicleSaleManager.CreateAdvertisement(input);
                }
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        //This is for paying recovery installments......return to Customer
        public ApiResponse Post(VehiclePenalty input, bool processSettlement, byte status)
        {
            ApiResponse response;
            try
            {
                VehicleSaleManager.AddPenalty(input);
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
