using AccountEx.BussinessLogic;
using AccountEx.Common;
using AccountEx.DbMapping;
using AccountEx.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace AccountEx.Web.Controllers.api
{
    public class PurchaseController : BaseApiController
    {
        // GET api/test
        public JQueryResponse Get()
        {
            return GetDataTable();
        }


        // GET api/test/5
        public ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                var result = "";
                Request.RequestUri.ParseQueryString();
                var data = new PurchaseRepository().GetById(id);
                new Dictionary<string, bool>();
                var totalitem = new PurchaseTotalItemRepository().GetByPurchaseId(id);
                result = JsonConvert.SerializeObject(data, GetJsonSetting());
                var result1 = JsonConvert.SerializeObject(totalitem, GetJsonSetting());
                var subdata = new
                {
                    Data = result,
                    TotalItems = result1

                };
                response = new ApiResponse
                {
                    Success = true,
                    Data = subdata

                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        // POST api/test
        public ApiResponse Post([FromBody]PurchaseExtra input)
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                if (queryString["mode"] == "add")
                {

                    new PurchaseRepository().Save(input);

                }
                else
                {
                    new PurchaseRepository().Update(input);
                }

                response = new ApiResponse { Success = true, Data = input.Id };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }

            return response;

        }



        // DELETE api/test/5
        public ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                new AccountRepository().Delete(id);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }

            return response;
        }

        private JQueryResponse GetDataTable() { return null; }
    }
}
