using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.Web.Controllers.api.Shared;
using System.Web.Http;
using System.Web;
using AccountEx.CodeFirst.Models.COA;
using AccountEx.BussinessLogic;
using Newtonsoft.Json;

namespace AccountEx.Web.Controllers.api.Manage
{
    public class MiscChargesChallanController : BaseApiController
    {
        public ApiResponse Get(int rentAgreementId, string key)
        {
            ApiResponse response;
            try
            {
                var type = VoucherType.MiscCharges;
                var challanRepo = new ChallanRepository();
                response = new ApiResponse()
                                {
                                    Success = true,
                                    Data = new
                                    {

                                        AllChallan = challanRepo.GetAllByAgreemntId(rentAgreementId, type, 100)
                                    }
                                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        public ApiResponse Get(int id, bool printChallan)
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                var paidAmount = 0.0M;
                var prevPaidAmount = 0.0M;
                var noOfPaidIstallment = 0;

                var rentAgreement = new RentAgreement();
                var challanRepo = new ChallanRepository();
                var rentRepo = new RentAgreementRepository(challanRepo);
                var challanRepo1 = new ChallanItemRepository(challanRepo);
                var data = challanRepo.GetById(id);
                var challan = challanRepo.PrintMiscChallanById(id);

                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {

                        Challan = challan,

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
                var invoiceNumber = id;
                var queryString = Request.RequestUri.ParseQueryString();
                var type = (queryString["type"] + "").Trim();
                var key = queryString["key"].ToLower();
                var vouchertype =(VoucherType)Convert.ToByte(type);
                bool next, previous;
                var repo = new ChallanRepository();
                if (voucherNumber == 0) key = "nextvouchernumber";
                if (key == "nextvouchernumber")
                {
                    voucherNumber = repo.GetNextVoucherNumber(vouchertype);
                    invoiceNumber = repo.GetNextInvoiceNumber(vouchertype);
                }
                var data = repo.GetByVoucherNumber(voucherNumber, vouchertype, key, out next, out previous);
                var partner = "";
                var business = "";
                if (data != null)
                {
                    var tenantId = data.ChallanItems.AsQueryable().Select(p => p.TenantAccountId).FirstOrDefault();
                    partner = new TenantPartnerRepository().AsQueryable().FirstOrDefault(p => p.TenantId == tenantId).Name;
                    business = new ChallanItemRepository().AsQueryable().FirstOrDefault(p => p.TenantAccountId == tenantId).Business;
                }
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Order = data,
                        Next = next,
                        Previous = previous,
                        VoucherNumber = voucherNumber,
                        InvoiceNumber = invoiceNumber,
                        Partner = partner,
                        Business = business,
                    }
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        public ApiResponse Post([FromBody]Challan entity)
        {
            ApiResponse response;
            try
            {
                entity.FiscalId = SiteContext.Current.Fiscal.Id;
                ChallanManager.CreateMiscChallan(entity);
                response = new ApiResponse() { Success = true, Data = entity };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        public ApiResponse Delete(int id)
        {
            var response = new ApiResponse();
            try
            {
                var err = ServerValidateDelete(id);
                if (err == "")
                {
                    ChallanManager.Delete(id, VoucherType.MiscCharges);
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
        private string ServerValidateDelete(int id)
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
                if (new ChallanRepository().IsChallanReceived(id))
                {
                    err += "Challan is received & could not be deleted.,";
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
