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
using AccountEx.CodeFirst;
using AccountEx.BussinessLogic;

namespace AccountEx.Web.Controllers.api.Manage
{
    public class SalaryConfigController : BaseApiController
    {
        public ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                if (id == 0) id = int.MaxValue;

                bool next, previous;
                var salary = new SalaryConfigRepository().GetById(id, QueryString["Key"], out next, out previous);
                if (salary != null) salary.SalaryConfigItems = salary.SalaryConfigItems.Where(p => p.IsActive == true).ToList();
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        SalaryConfig = salary,
                        Next = next,
                        Previous = previous,
                    },
                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }

        public ApiResponse Post([FromBody]SalaryConfig input)
        {
            ApiResponse response;
            try
            {
                var err = ServerValidateSave(input);
                if (err == "")
                {
                    var repo = new SalaryConfigRepository();
                    string configName;
                    if (repo.IsAlreadyExistInDates(input.Id, input.FromDate, input.ToDate, out configName) == false)
                    {
                        repo.Save(input);
                        response = new ApiResponse() { Success = true };
                    }
                    else
                    {
                        response = new ApiResponse()
                        {
                            Success = false,
                            Error = "Configuration already exist in applied dates with Name: " + configName,
                        };
                    }
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
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }

        private string ServerValidateSave(SalaryConfig input)
        {
            var err = ",";
            try
            {
                foreach (var item in input.SalaryConfigItems.Where(p => p.AccountId == 0))
                {
                    err += "Account is not valid to process the request,";
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
