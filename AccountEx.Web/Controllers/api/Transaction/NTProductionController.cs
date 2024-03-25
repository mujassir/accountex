using System;
using System.Net.Http;
using System.Web.Http;
using AccountEx.BussinessLogic;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using Newtonsoft.Json;
using System.Data.Entity.Validation;

namespace AccountEx.Web.Controllers.api.Transaction
{
    public class NTProductionController : BaseApiController
    {
        public ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                var voucherNumber = id;
                var queryString = Request.RequestUri.ParseQueryString();

                var key = queryString["key"].ToLower();

                //var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
                bool next, previous;
                var workInProgressRepo = new WorkInProgressRepository();
                if (voucherNumber == 0) key = "nextvouchernumber";
                if (key == "nextvouchernumber")
                    voucherNumber = workInProgressRepo.GetNextVoucherNumber();
                var data = workInProgressRepo.GetByVoucherNumber(voucherNumber, key, out next, out previous);

                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Order = data,
                        Next = next,
                        Previous = previous,
                        VoucherNumber = voucherNumber,
                    }
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        public ApiResponse Post([FromBody]WorkInProgress input)
        {
            ApiResponse response;
            try
            {
                //input.AccountId = 35217;
                input.AccountId = SettingManager.FinishedProductionHeadId;
                var err = ServerValidateSave(input);
                if (err == "")
                {
                    input.FiscalId = SiteContext.Current.Fiscal.Id;
                    var repo = new WorkInProgressRepository();
                    if (input.Id == 0)
                    {
                        input.CreatedDate = DateTime.Now;
                        repo.Add(input, true, false);
                    }
                    else
                    {
                        repo.Update(input);
                    }
                    repo.SaveChanges();
                    response = new ApiResponse { Success = true, Data = input.Id };
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
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            //catch (Exception ex)
            //{
            //     response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            //}
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

                ProductionManager.Delete(id, VoucherType.Production);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }

            return response;
        }
        private string ServerValidateSave(WorkInProgress input)
        {
            var err = ",";
            try
            {
                if (input.AccountId == 0)
                {
                    err += "Account is not valid to process the request.,";
                }
                if (SiteContext.Current.Fiscal.IsClosed)
                {
                    err += "Fiscal year is closed No action can be done.,";
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
