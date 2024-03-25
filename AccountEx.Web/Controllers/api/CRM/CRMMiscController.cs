using AccountEx.Common;
using System;
using System.Net.Http;
using AccountEx.Repositories;
using System.Text.RegularExpressions;
using System.Web.Http;
using AccountEx.BussinessLogic;
using System.Collections.Generic;
using System.Linq;
using AccountEx.DbMapping;
using AccountEx.CodeFirst.Models.CRM;
using AccountEx.Repositories.Config;

namespace AccountEx.Web.Controllers.api
{
    public class CRMMiscController : BaseApiController
    {


        public ApiResponse Get()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var response = new ApiResponse();
            var key = queryString["key"];
            switch (key)
            {
                case "GetSalesManByCustomerId":
                    response = GetSalesManByCustomerId();
                    break;
                case "GetProducts":
                    response = GetProducts();
                    break;
                case "GetProductsIdName":
                    response = GetProductsIdName();
                    break;
                case "GetCustomerIdName":
                    response = GetCustomerIdName();
                    break;
                case "GetCustomerIdNameByUserId":
                    response = GetCustomerIdNameByUserId();
                    break;
                case "GetCustomerIdNameBySalePersonId":
                    response = GetCustomerIdNameBySalePersonId();
                    break;
                case "GetProjects":
                    response = GetProjects();
                    break;
                case "GetProjectLinkedProduct":
                    response = GetProjectLinkedProduct();
                    break;


                case "GetProjectsWithPMCDetail":
                    response = GetProjectsWithPMCDetail();
                    break;
                case "GetDivisionByCategoryId":
                    response = GetDivisionByCategoryId();
                    break;
                case "GetSubGroupByGroupId":
                    response = GetSubGroupByGroupId();
                    break;
                case "GetGroupByDivisionId":
                    response = GetGroupByDivisionId();
                    break;
                case "GetCustomerCategories":
                    response = GetGroupByDivisionId();
                    break;
               

            }
            return response;
        }

        public ApiResponse Post()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var response = new ApiResponse();
            var key = queryString["key"];
            switch (key)
            {
                case "MergeProducts":
                    response = MergeProducts();
                    break;

            }
            return response;
        }

        private ApiResponse GetSalesManByCustomerId()
        {
            var customerId = Numerics.GetInt(Request.GetQueryString("customerId"));
            ApiResponse response;
            try
            {
                var data = new CRMCustomerRepository().GetSalesPersonByCustomerId(customerId);
                if (SiteContext.Current.UserTypeId == CRMUserType.SalesExecutive)
                {
                    data = data.Where(p => p.Id == SiteContext.Current.User.Id).ToList();
                }
                response = new ApiResponse { Success = true, Data = data };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
        private ApiResponse GetProducts()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            ApiResponse response;
            try
            {
                var data = new CRMProductRepository().GetAll();
                response = new ApiResponse { Success = true, Data = data };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
        private ApiResponse GetProductsIdName()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            ApiResponse response;
            try
            {
                var data = new CRMProductRepository().GetNames();
                response = new ApiResponse { Success = true, Data = data };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
        private ApiResponse GetCustomerIdName()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            ApiResponse response;
            try
            {
                var data = new GenericRepository<vw_CRMCustomers>().GetNames();
                response = new ApiResponse { Success = true, Data = data };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
        private ApiResponse GetDivisionByCategoryId()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            ApiResponse response;
            try
            {
                var categoryId = Numerics.GetInt(queryString["categoryId"]);
                var data = new DivisionRepository().GetIdNameByCategoryId(categoryId);
                response = new ApiResponse { Success = true, Data = data };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
        private ApiResponse GetGroupByDivisionId()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            ApiResponse response;
            try
            {
                var divisionId = Numerics.GetInt(queryString["divisionId"]);
                var data = new ProductGroupRepository().GetIdNameByDivisionId(divisionId);
                response = new ApiResponse { Success = true, Data = data };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }

        private ApiResponse MergeProducts()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            ApiResponse response;
            try
            {
                var fromProductId = Numerics.GetInt(Request.GetQueryString("fromProductId"));
                var toProductId = Numerics.GetInt(Request.GetQueryString("toProductId"));
                new ProductRepository().MergeProducts(fromProductId, toProductId);
                response = new ApiResponse { Success = true, Data = null };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }


        private ApiResponse GetCustomerCategories()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            ApiResponse response;
            try
            {
                var customerId = Numerics.GetInt(queryString["customerId"]);
                var data = new CRMCustomerRepository().GetCategoriesWithCustomer(customerId);
                response = new ApiResponse { Success = true, Data = data };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }




        private ApiResponse GetSubGroupByGroupId()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            ApiResponse response;
            try
            {
                var groupId = Numerics.GetInt(queryString["groupId"]);
                var data = new ProductSubGroupRepository().GetIdNameByGroupId(groupId);
                response = new ApiResponse { Success = true, Data = data };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
        private ApiResponse GetCustomerIdNameByUserId()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            ApiResponse response;
            try
            {
                var customerType = Numerics.GetInt(queryString["customerType"]);
                var type = (CRMCustomerType)customerType;
                var data = new CRMCustomerRepository().GetByUserTypeId(SiteContext.Current.UserTypeId, type);
                response = new ApiResponse { Success = true, Data = data };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
        private ApiResponse GetCustomerIdNameBySalePersonId()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var salePersonId = Numerics.GetInt(queryString["salePersonId"]);
            ApiResponse response;
            try
            {
                var data = new CRMCustomerRepository().GetCustomerIdName(salePersonId);
                response = new ApiResponse { Success = true, Data = data };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }


        private ApiResponse GetProjects()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var customerId = Numerics.GetInt(queryString["customerId"]);
            ApiResponse response;
            try
            {
                var data = new CRMProjectRepository().GetProjects(customerId);
                response = new ApiResponse { Success = true, Data = data };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
        private ApiResponse GetProjectLinkedProduct()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var customerId = Numerics.GetInt(queryString["customerId"]);
            ApiResponse response;
            try
            {
                var data = new CRMProjectRepository().GetProjectsProduct(customerId);
                response = new ApiResponse { Success = true, Data = data };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
        private ApiResponse GetProjectsWithPMCDetail()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var customerId = Numerics.GetInt(queryString["customerId"]);
            ApiResponse response;
            try
            {
                var data = new CRMProjectRepository().GetProjectsWithPMCDetail(customerId);
                response = new ApiResponse { Success = true, Data = data };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }


    }
}
