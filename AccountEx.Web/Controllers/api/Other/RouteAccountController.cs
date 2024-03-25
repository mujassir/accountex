using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Linq;
using System.Web.Http;
using AccountEx.BussinessLogic;

namespace AccountEx.Web.Controllers.api
{
    public class RouteAccountController : BaseApiController
    {

        // POST api/test
        public ApiResponse Post([FromBody]CodeFirst.Models.Account input)
        {
            ApiResponse response;
            try
            {
                input.DisplayName = input.Name;
                input.Level = 1;
                input.HasChild = false;
                input.IsSystemAccount = true;
                input.IsLive = true;
                var err = AccountManager.ServerValidateSave(input);
                if (err == "")
                {

                    new AccountRepository().Save(input);
                    response = new ApiResponse { Success = true, Data = input.Id };
                }
                else
                {
                    response = new ApiResponse { Success = false, Error = err };
                }

            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }

            return response;

        }
        // DELETE api/test/5
        public ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                var repo = new AccountRepository();
                var account = repo.GetById(id);
                var err = AccountManager.ServerValidateDelete(account,repo);
                if (err == "")
                {
                    repo.Delete(id);
                    response = new ApiResponse { Success = true };
                }
                else
                {
                    response = new ApiResponse { Success = false, Error = err };
                }

            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }

            return response;
        }


    }
}
