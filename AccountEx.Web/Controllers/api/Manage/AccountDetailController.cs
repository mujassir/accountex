using System;
using System.Linq;
using AccountEx.BussinessLogic;
using AccountEx.Common;
using AccountEx.Repositories;
using System.Globalization;
using System.Transactions;
using System.Web;
using AccountEx.CodeFirst.Models;
using AccountEx.Web.Controllers.api.Shared;

namespace AccountEx.Web.Controllers.api.Manage
{
    public class AccountDetailController : GenericApiController<AccountDetail>
    {
        public int AccountDetailFormId { get; set; }
        public int HeadAccountId { get; set; }
        /// <summary>
        /// Get next increamented Account code with ParentId
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ApiResponse Get(string key)
        {
            // var queryString = Request.RequestUri.ParseQueryString();
            var queryString = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var accountId = Numerics.GetInt(queryString["AccountId"]);
            ApiResponse response;
            try
            {
                response = new ApiResponse
                {
                    Success = true,
                    Data = AccountManager.GetNextAccountCode(accountId == 0 ? HeadAccountId : accountId),
                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }

        public ApiResponse Get(string PartyAddress, int AccountId)
        {
            ApiResponse response;
            try
            {
                response = new ApiResponse
                {
                    Success = true,
                    Data = new AccountDetailRepository().GetAddressByAccountId(AccountId),
                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }


        public override ApiResponse Post(AccountDetail accountDetail)
        {
            ApiResponse response;
            try
            {
                response = AccountManager.Save(accountDetail, AccountDetailFormId, HeadAccountId);
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }

        public override ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                response = AccountManager.DeleteFromAccountDetail(id,AccountDetailFormId);
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }

            return response;
        }

    }
}
