using System;
using System.Web.Http;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.BussinessLogic;
using AccountEx.CodeFirst.Models;

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
                response = new ApiResponse
                {
                    Success = true,
                    Data = new GenericRepository<T>().GetById(id),
                    Logs = new GenericRepository<LogData>().Get(x => x.RecordId == id)
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
