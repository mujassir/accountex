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
    public class ChallanController : BaseApiController
    {
        public ApiResponse Get(int rentAgreementId, VoucherType type, string key)
        {
            ApiResponse response;
            try
            {
                var paidAmount = 0.0M;
                var noOfPaidIstallment = 0;
                var noOfUnPaidIstallment = 0;
                var repo = new RentAgreementRepository();
                var shopRepo = new ShopRepository(repo);
                var challanRepo = new ChallanRepository(repo);
                var challanItemRepo = new ChallanItemRepository(repo);
                var rentAgreement = repo.GetById(rentAgreementId);
                if (challanRepo.AsQueryable().Any(p => p.RentAgreementId == rentAgreement.Id && p.TransactionType == type && p.IsReceived))
                {
                    var paidChallan = challanRepo.AsQueryable().Where(p => p.RentAgreementId == rentAgreement.Id && p.TransactionType == type && p.IsReceived).Select(p => new { p.NetAmount, p.NumberOfInstallment }).ToList();
                    paidAmount = paidChallan.Sum(p => p.NetAmount);
                    noOfPaidIstallment = paidChallan.Sum(p => p.NumberOfInstallment);
                }
                response = new ApiResponse()
                {
                    Success = true,
                    Data = new
                    {
                        Agreement = rentAgreement,
                        PaidAmount = paidAmount,
                        AmountPerInstallment = type == VoucherType.SecurityMoney ? rentAgreement.SecurityPerInstallment : rentAgreement.PossessionPerInstallment,
                        TotalAmount = type == VoucherType.SecurityMoney ? rentAgreement.SecurityBalance : rentAgreement.PossessionBalance,
                        PaidIstallment = noOfPaidIstallment,
                        TotalIstallment = type == VoucherType.SecurityMoney ? rentAgreement.SecurityInstallment : rentAgreement.PossessionInstallment,
                        AllChallan = challanRepo.GetAllByAgreemntId(rentAgreementId, type)
                    }
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
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
                var vouchertype = (VoucherType)Convert.ToByte(type);
                bool next, previous;
                var paidAmount = 0.0M;
                var prevPaidAmount = 0.0M;

                var previousInstallments = 0;

                var rentAgreement = new RentAgreement();
                var repo = new ChallanRepository();
                var rentRepo = new RentAgreementRepository(repo);
                var challanRepo = new ChallanItemRepository(repo);
                var shopCode = "";
                var block = "";
                var partner = "";
                if (voucherNumber == 0) key = "nextvouchernumber";
                if (key == "nextvouchernumber")
                {
                    voucherNumber = repo.GetNextVoucherNumber(vouchertype);
                    invoiceNumber = repo.GetNextInvoiceNumber(vouchertype);
                }
                var data = repo.GetByVoucherNumber(voucherNumber, vouchertype, (byte)EntryType.Manual, key, out next, out previous);
                if (data != null)
                {
                    var item = data.ChallanItems.FirstOrDefault();
                    var shop = new ShopRepository().AsQueryable().FirstOrDefault(p => p.Id == item.ShopId);
                    shopCode = shop.ShopCode;
                    var blockinfo = new BlockRepository().GetById(shop.BlockId);
                    block = blockinfo.Name;

                    rentAgreement = rentRepo.AsQueryable().Where(p => p.Id == item.RentAgreementId).FirstOrDefault();

                    var IsPreviousAmountReceived = new GenericRepository<ChallanItem>().AsQueryable().Where(p => p.RentAgreementId == item.RentAgreementId && p.TransactionType == vouchertype && p.IsReceived).ToList();
                    if (IsPreviousAmountReceived.Count > 0)
                        prevPaidAmount = new GenericRepository<ChallanItem>().AsQueryable().Where(p => p.RentAgreementId == item.RentAgreementId && p.TransactionType == vouchertype && p.IsReceived).Select(p => new { p.NetAmount }).Sum(p => p.NetAmount);


                    paidAmount = new GenericRepository<ChallanItem>().AsQueryable().Where(p => p.RentAgreementId == item.RentAgreementId && p.TransactionType == vouchertype && p.ChallanId == data.Id).Select(p => new { p.NetAmount }).Sum(p => p.NetAmount);
                    if (repo.AsQueryable().Any(p => p.Id != data.Id && p.TransactionType == data.TransactionType && p.ChallanItems.Any(q => q.RentAgreementId == item.RentAgreementId && p.EntryType == (byte)EntryType.Manual)))
                        previousInstallments = repo.AsQueryable().Where(p => p.Id != data.Id && p.TransactionType == data.TransactionType && p.ChallanItems.Any(q => q.RentAgreementId == item.RentAgreementId && p.EntryType == (byte)EntryType.Manual)).Sum(p => p.NumberOfInstallment);

                    partner = new TenantPartnerRepository().AsQueryable().FirstOrDefault(p => p.TenantId == rentAgreement.TenantAccountId).Name;

                }
                var now = DateTime.Now;
                var dueDate = new DateTime(now.Year, now.Month, 15);
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Order = data,
                        RentAgreement = rentAgreement,
                        PaidAmount = paidAmount,
                        PrevPaidAmount = prevPaidAmount,
                        Next = next,
                        Previous = previous,
                        VoucherNumber = voucherNumber,
                        InvoiceNumber = invoiceNumber,
                        PreviousInstallments = previousInstallments,
                        ShopCode = shopCode,
                        Block = block,
                        Partner = partner,
                        DueDate = dueDate.ToString("dd MMM, yyyy")
                    }
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
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
                var challan = challanRepo.PrintChallanById(id);
                if (data != null)
                {
                    var vouchertype = data.TransactionType;
                    var entryType = (byte)EntryType.Manual;
                    var item = data.ChallanItems.FirstOrDefault();
                    rentAgreement = rentRepo.AsQueryable().Where(p => p.Id == item.RentAgreementId).FirstOrDefault();
                    if (challanRepo.AsQueryable().Any(p => p.RentAgreementId == rentAgreement.Id && p.TransactionType == vouchertype && p.IsReceived && p.EntryType == entryType))
                    {
                        var paidChallan = challanRepo.AsQueryable().Where(p => p.RentAgreementId == rentAgreement.Id && p.TransactionType == vouchertype && p.IsReceived && p.EntryType == entryType).Select(p => new { p.NetAmount, p.NumberOfInstallment }).ToList();
                        paidAmount = paidChallan.Sum(p => p.NetAmount);
                        noOfPaidIstallment = paidChallan.Sum(p => p.NumberOfInstallment);
                    }


                }
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Order = data,
                        Challan = challan,
                        RentAgreement = rentAgreement,
                        PaidAmount = paidAmount,
                        PrevPaidAmount = prevPaidAmount,
                        PreviousInstallments = noOfPaidIstallment,
                    }
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }

        public ApiResponse Post([FromBody]Challan entity)
        {
            ApiResponse response;
            try
            {
                var err = ChallanManager.ValidateSave(entity, false, false);
                if (err == "")
                {

                    entity.FiscalId = SiteContext.Current.Fiscal.Id;
                    new ChallanRepository().Save(entity);
                    response = new ApiResponse() { Success = true, Data = entity };
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
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
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
                    new ChallanRepository().Delete(id);
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
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
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
