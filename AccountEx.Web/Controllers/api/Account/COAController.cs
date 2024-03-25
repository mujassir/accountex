using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AccountEx.BussinessLogic;
using AccountEx.Common;
using AccountEx.Repositories;
using Newtonsoft.Json;
using AccountEx.BussinessLogic.Security;
using AccountEx.DbMapping;

namespace AccountEx.Web.Controllers.api.Account
{
    public class CoaController : BaseApiController
    {
        // GET api/test
        public ApiResponse Get()
        {
            var key = Request.GetQueryString("key");
            var response = new ApiResponse();
            try
            {
                switch (key)
                {
                    case "GetAccountTypes":
                        response = new ApiResponse
                        {
                            Success = true,
                            Data = new AccountTypeRepository().GetAll()
                        };
                        break;
                    case "GetLeafAccounts":
                        response = new ApiResponse
                        {
                            Success = true,
                            Data = GetLeafAccounts(Numerics.GetInt(Request.GetQueryString("AccountId")))
                        };
                        break;
                    case "GetBothLeafAccounts":
                        Numerics.GetInt(Request.GetQueryString("ItemAccountId"));
                        var type = Request.GetQueryString("type");
                        var formtype = type == "sale" || type == "salereturn" ? (int)AccountDetailFormType.Customers : (int)AccountDetailFormType.Suppliers;
                        response = new ApiResponse
                        {
                            Success = true,
                            Data = new
                            {

                                MasterAccounts = new AccountDetailRepository().AsQueryable().Where(p => p.AccountDetailFormId == formtype)
                                .ToList().Select(p => new
                                {
                                    Id = p.AccountId,
                                    Name = p.Code + "-" + p.Name,
                                    p.Code,
                                    Title = p.Name,
                                    p.Address
                                }).ToList(),
                                ItemAccounts = new AccountDetailRepository().AsQueryable().Where(p => p.AccountDetailFormId == (int)AccountDetailFormType.Products).ToList().Select(p => new
                                {
                                    Id = p.AccountId,
                                    Name = p.Code + "-" + p.Name,
                                    p.Code,
                                    Title = p.Name,
                                }).ToList()
                            }
                        };
                        break;
                    case "GetBothLeafAccountsWithPurchaseRate":
                        response = new ApiResponse
                        {
                            Success = true,
                            Data = new
                            {
                                MasterAccounts = new AccountDetailRepository().GetAll(AccountDetailFormType.Suppliers).Select(p => new
                                {
                                    Id = p.AccountId,
                                    Name = p.Code + "-" + p.Name,
                                    p.Address
                                }),
                                ItemAccounts = new AccountDetailRepository().GetAll(AccountDetailFormType.Products).Select(p => new
                                {
                                    p.AccountId,
                                    Name = p.Code + "-" + p.Name,
                                    p.PurchasePrice,
                                    p.Code
                                }),
                            }
                        };
                        break;
                    case "GetAttributes":
                        response = new ApiResponse
                        {
                            Success = true,
                            Data = new AccountAttributeRepository().GetByAccountId(Numerics.GetInt(Request.GetQueryString("AccountId")))
                        };
                        break;
                    case "GetClients":
                        response = new ApiResponse
                        {
                            Success = true,
                            Data = new AccountRepository().GetClients(),
                        };
                        break;

                    case "CheckIfLoadCOA":
                        response = CheckIfLoadCoa();
                        break;
                    case "LoadCOA":
                        response = LoadCOA();
                        break;
                    case "LoadAccountDetail":
                        response = new ApiResponse
                        {
                            Success = true,
                            Data = new AccountDetailRepository().GetAll().Where(p => p.Code != null),
                        };
                        break;
                    case "GetEmployees":

                        response = new ApiResponse
                        {
                            Success = true,
                            Data = new AccountDetailRepository().GetAll(AccountDetailFormType.Employees).Select(p => new
                            {
                                Id = p.AccountId,
                                Name = p.Code + "-" + p.Name

                            }),
                        };
                        break;
                    case "GetAccountSuggestions":
                        return GetAccountSuggestions();
                }
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;

            }
            return response;
        }
        public ApiResponse Get(string key, int headAccountId)
        {
            ApiResponse response;
            try
            {
                response = new ApiResponse
                {
                    Success = true,
                    Data = AccountManager.GetNextAccountCode(headAccountId),

                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
                 ;
            }
            return response;
        }
        private ApiResponse LoadCOA()
        {


            var lastDate = DateConverter.ConvertStandardDate(Request.GetQueryString("LastDate"));
       
            ApiResponse response;
            try
            {
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        COA = new AccountRepository().GetAll<COAEx>(),
                        LastDate = new AccountRepository().GetLastActivityDate(),
                        StorageKey = Sha2Sign.Hash(SiteContext.Current.User.CompanyId)
                    }
                };

            }
            catch (Exception)
            {

                response = new ApiResponse
                {
                    Success = false,

                };
            }

            return response;


        }
        private ApiResponse CheckIfLoadCoa()
        {


            var lastDate = DateConverter.ConvertStandardDate(Request.GetQueryString("LastDate"));
            var companyId = Request.GetQueryString("StorageKey");
            var currentCompanyId = Sha2Sign.Hash(SiteContext.Current.User.CompanyId);
            ApiResponse response;
            try
            {
                var lastActivityDate = new AccountRepository().GetLastActivityDate();
                var loadCoa = lastActivityDate > lastDate || currentCompanyId != companyId;

                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        LoadCOA = loadCoa
                    }

                };

            }
            catch (Exception)
            {

                response = new ApiResponse
                {
                    Success = true,

                };
            }

            return response;
        }
        private ApiResponse GetAccountSuggestions()
        {
            return new ApiResponse { Success = true, Data = new AccountRepository().GetAll() };
        }

        private static IEnumerable<IdName> GetLeafAccounts(int id)
        {
            return new AccountRepository().GetLeafAccounts(id);
        }

        // GET api/test/5
        public ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                var branch = new AccountRepository().GetById(id);
                var allowedChild = new Dictionary<string, bool> { { "BranchCategories", true }, { "BranchHalls", true } };
                var result = JsonConvert.SerializeObject(SetJsonValue(branch, allowedChild, new[] { "Branch", "BranchHall", "Category", "Tables" }), Formatting.Indented, GetJsonSetting());
                response = new ApiResponse
                {
                    Success = true,
                    Data = result

                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }

        // POST api/test
        public ApiResponse Post([FromBody]LeafAccountExtra input)
        {
            ApiResponse response;
            try
            {

                new AccountRepository().SaveLeaf(input);
                response = new ApiResponse { Success = true, Data = input.Id };
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
                new AccountRepository().Delete(id);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }

            return response;
        }

    }
}
