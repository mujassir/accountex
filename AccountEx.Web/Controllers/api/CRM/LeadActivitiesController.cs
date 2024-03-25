using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using AccountEx.BussinessLogic;
using AccountEx.Common;
using AccountEx.Repositories;
using Newtonsoft.Json;
using AccountEx.CodeFirst.Models;
using System.Linq;
using AccountEx.DbMapping;

namespace AccountEx.Web.Controllers.api.CRM
{
    public class LeadActivitiesController : BaseApiController
    {

        public ApiResponse Get(int id)
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var data = new LeadExpectedItemRepository().GetByLeadId(id);
            ApiResponse response;
            try
            {
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

        public ApiResponse Get(int id, string key)
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var data = new LeadConcernedItemRepository().GetByLeadId(id);
            ApiResponse response;
            try
            {
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
        public ApiResponse Get(int id, int type)
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var data = new LeadActivityRepository().GetByLeadId(id);
            ApiResponse response;
            try
            {
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
        //Next Tax No

        public ApiResponse Post([FromBody]LeadConcernedItemExtra input)
        {
            ApiResponse response;
            try
            {
                var err = ServerValidateConcerned(input);
                if (err == "")
                {
                    var LeadConcernedItemRepo = new LeadConcernedItemRepository();
                    var leadid = input.LeadConcernedItems.FirstOrDefault().LeadId;
                    LeadConcernedItemRepo.Save(input.LeadConcernedItems);
                    var data = LeadConcernedItemRepo.GetByLeadId(leadid);
                    response = new ApiResponse
                    {
                        Success = true,
                        Data = data
                    };
                }
                else
                {
                    response = new ApiResponse { Success = false, Error = err };
                }
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        public ApiResponse Post([FromBody]LeadExpectedItemExtra input, string Key)
        {
            ApiResponse response;
            try
            {
                var err = ServerValidateExpected(input);
                if (err == "")
                {
                    var LeadExpectedItemRepo = new LeadExpectedItemRepository();
                    var leadid = input.LeadExpectedItems.FirstOrDefault().LeadId;
                    LeadExpectedItemRepo.Save(input.LeadExpectedItems);
                    var data = LeadExpectedItemRepo.GetByLeadId(leadid);
                    response = new ApiResponse
                    {
                        Success = true,
                        Data = data
                    };
                }
                else
                {
                    response = new ApiResponse
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
        public ApiResponse Post([FromBody]LeadActivity input, int Type)
        {
            ApiResponse response;
            try
            {
                //var err = JobCardManager.ValidateSave(input, false, true);
                //if (err == "")
                //{
                //input.FiscalId = SiteContext.Current.Fiscal.Id;
                var LeadActivityRepo = new LeadActivityRepository();
                LeadActivityRepo.Save(input);
                response = new ApiResponse
                {
                    Success = true,
                    Data = LeadActivityRepo.GetByLeadId(input.LeadId)
                };
                //}
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        private ApiResponse GetDataTable()
        {
            ApiResponse response;
            try
            {
                var result = "";
                var queryString = Request.RequestUri.ParseQueryString();
                var vouchertype = (VoucherType)Convert.ToByte((queryString["type"]));
                var voucher = Numerics.GetInt((queryString["voucher"]));
                var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
                result = JsonConvert.SerializeObject(data, GetJsonSetting());
                response = new ApiResponse
                {
                    Success = true,
                    Data = result

                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        public ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                var type = (queryString["type"]);
                var voucherno = Numerics.GetInt(queryString["voucher"]);
                var vouchertype = (VoucherType)Convert.ToByte(type);
                TransactionManager.Delete(voucherno, vouchertype);
                new SaleServicesItemRepository().DeleteBySaleId(id);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        private string ServerValidateConcerned(LeadConcernedItemExtra input)
        {
            var err = ",";
            try
            {
                foreach (var item in input.LeadConcernedItems.Where(p => p.ItemId == 0))
                {
                    err += "Item is not valid to process the request.,";
                }
                foreach (var item in input.LeadConcernedItems.Where(p => p.LeadId == 0))
                {
                    err += "Lead is not valid to process the request.,";
                }
                var Itemcountlist = input.LeadConcernedItems.GroupBy(p => p.ItemId).Select(p => new
                {
                    ItemId = p.Key,
                    ItemCode = p.FirstOrDefault().ItemCode,
                    ItemName = p.FirstOrDefault().ItemName,
                    Count = p.Count()
                }).Where(p => p.Count > 1).ToList();

                foreach (var item in Itemcountlist)
                {
                    err += item.ItemCode + "-" + item.ItemName + " must be added once in list.(Current Count:" + item.Count + "),";
                }

            }
            catch (Exception)
            {

                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;


        }

        private string ServerValidateExpected(LeadExpectedItemExtra input)
        {
            var err = ",";
            try
            {
                foreach (var item in input.LeadExpectedItems.Where(p => p.ItemId == 0))
                {
                    err += "Item is not valid to process the request.,";
                }
                foreach (var item in input.LeadExpectedItems.Where(p => p.LeadId == 0))
                {
                    err += "Lead is not valid to process the request.,";
                }
                var Itemcountlist = input.LeadExpectedItems.GroupBy(p => p.ItemId).Select(p => new
                {
                    ItemId = p.Key,
                    ItemCode = p.FirstOrDefault().ItemCode,
                    ItemName = p.FirstOrDefault().ItemName,
                    Count = p.Count()
                }).Where(p => p.Count > 1).ToList();

                foreach (var item in Itemcountlist)
                {
                    err += item.ItemCode + "-" + item.ItemName + " must be added once in list.(Current Count:" + item.Count + "),";
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
