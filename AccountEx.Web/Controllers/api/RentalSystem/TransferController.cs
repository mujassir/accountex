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
    public class TransferController : BaseApiController
    {
        public ApiResponse Get(int id, string key)
        {
            ApiResponse response;
            try
            {
                var voucherNumber = id;
                var queryString = Request.RequestUri.ParseQueryString();
                var type = queryString["type"];
                //var key = queryString["key"].ToLower();
                var vouchertype =(VoucherType)Convert.ToByte(type);
                bool next, previous;
                if (voucherNumber == 0) key = "nextvouchernumber";
                if (key == "nextvouchernumber")
                    voucherNumber = new TransferRepository().GetNextVoucherNumber();
                var data = new TransferRepository().GetByVoucherNumber(voucherNumber, key, out next, out previous);
                var shopinfo = new Shop();
                var TransferBy = new Object();
                var TransferTo = new Object();
                if (data != null)
                {
                    shopinfo = new ShopRepository().GetById(data.ShopId);
                    TransferBy= new AccountDetailRepository().AsQueryable().Where(p => p.AccountId == data.TransferByTenantAccountId).Select(p => new
                     {
                         TransferByAccountCode = p.Code,
                         TransferByTenantName = p.Name,
                         TransferByCNIC = p.CNIC,
                         TransferByBusiness = p.Business,
                         TransferByNumberOfPartners = p.NumberOfPartners,
                         TransferByContactNo = p.ContactNumber
                     }
                     ).FirstOrDefault();
                    TransferTo = new AccountDetailRepository().AsQueryable().Where(p => p.AccountId == data.TransferToTenantAccountId).Select(p => new
                    {
                        TransferToAccountCode = p.Code,
                        TransferToTenantName = p.Name,
                        TransferToCNIC = p.CNIC,
                        TransferToBusiness = p.Business,
                        TransferToNumberOfPartners = p.NumberOfPartners,
                        TransferToContactNo = p.ContactNumber
                    }
                  ).FirstOrDefault();
                }
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Order = data,
                        ShopInfo = shopinfo,
                        TransferBy=TransferBy,
                        TransferTo=TransferTo,
                        Next = next,
                        Previous = previous,
                        VoucherNumber = voucherNumber
                    }
                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }

        public ApiResponse Get(string key)
        {
            var response = new ApiResponse();
            switch (key)
            {
                case "GetAllShops":
                    response = GetShops();
                    break;
                case "GetSettings":
                    response = GetSettings();
                    break;
            }

            return response;
        }

        public ApiResponse Post([FromBody]Transfer input)
        {
            ApiResponse response;
            try
            {
                input.FiscalId = SiteContext.Current.Fiscal.Id;
                var repo = new TransferRepository();
                if (input.Id == 0)
                {
                    repo.Add(input);
                }
                else
                {
                    repo.Update(input);
                }
                response = new ApiResponse
                {
                    Success = true,
                };

            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }

        public ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                new TransferRepository().Delete(id);
                response = new ApiResponse
                {
                    Success = true,
                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }

        public ApiResponse GetShops()
        {
            ApiResponse response;
            try
            {
                var data = new ShopRepository().GetAll();
                var tenants = new RentAgreementRepository().GetAll();
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Shops = data,
                        Tenants = tenants,
                    }
                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;

        }

        public ApiResponse GetSettings()
        {
            ApiResponse response;
            try
            {
                var setting = new List<SettingExtra>();
                setting.Add(new SettingExtra { Key = "TransferCharges", Value = SettingManager.TransferCharges });
                var data = JsonConvert.SerializeObject(setting);
                response = new ApiResponse
                {
                    Success = true,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;

        }

    }
}
