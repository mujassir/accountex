using System;
using System.Web.Http;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.BussinessLogic;
using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.Transactions;

namespace AccountEx.Web.Controllers.api.Shared
{
    public abstract class GenericApiController<T> : BaseApiController where T : class
    {
        protected GenericRepository<T> Repository { get; set; }
        protected GenericApiController()
        {
            Repository = new GenericRepository<T>();
        }
        public virtual JQueryResponse Get()
        {
            try
            {
                return GetDataTable();
            }
            catch (Exception ex)
            {

                ErrorManager.Log(ex);
                throw;
            }

        }

        public virtual ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                var data = new GenericRepository<T>().GetById(id);
                int accId = 0;
                if (data != null)
                    accId = (data as dynamic)?.AccountId ?? 0;
                
                response = new ApiResponse
                {
                    Success = true,
                    Data = data,
                    Logs = new
                    {
                       Logs = new GenericRepository<LogData>().Get(x => x.RecordId == id),
                       Adjustments = new GenericRepository<DairyAdjustment>().Get(x => x.ItemId == accId),
                    }
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        public virtual ApiResponse Post([FromBody]T input)
        {
            ApiResponse response;
            try
            {
                new GenericRepository<T>().Save(input);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        public bool IsExist(string name, int id)
        {
            return new GenericRepository<T>().IsExist(name, id);
        }
        public virtual ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                new GenericRepository<T>().Delete(id);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }

            return response;
        }
        protected virtual JQueryResponse GetDataTable()
        {
            throw new NotImplementedException();
        }
    }
}
