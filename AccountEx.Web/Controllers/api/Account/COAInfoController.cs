using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using AccountEx.BussinessLogic;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.DbMapping;

namespace AccountEx.Web.Controllers.api.Account
{
    public class CoaInfoController : BaseApiController
    {


        public JQueryResponse Get()
        {

            return GetDataTable();
        }

        public ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                var data = new AccountRepository().GetById(id);
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
        public ApiResponse Post([FromBody]LeafAccountExtra input)
        {
            ApiResponse response;
            try
            {

                input.DisplayName = input.Name;
                input.HasChild = false;
                input.IsLive = true;
                input.IsSystemAccount = false;

                var err = ServerValidateSave(input);
                if (err == "")
                {
                    new AccountRepository().SaveLeaf(input);
                    response = new ApiResponse() { Success = true, Data = input.Id };
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
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }

            return response;

        }


        private string ServerValidateSave(LeafAccountExtra input)
        {
            var err = ",";
            try
            {

                var record = new AccountRepository().GetByCode(input.AccountCode, input.Id);

                if (record != null)
                {
                    err += "Account code alrady exist.";
                }
                record = new AccountRepository().GetByName(input.Name, input.Id);

                if (record != null)
                {
                    err += ",Account name already exist.";
                }

            }
            catch (Exception)
            {

                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;


        }

        // DELETE api/test/5
        public ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                //var account = new AccountRepository().GetById(id);
                var err = ServerValidateDelete(id);
                if (err == "")
                {

                    new AccountRepository().Delete(id);
                    response = new ApiResponse() { Success = true };
                }
                else
                {
                    response = new ApiResponse() { Success = false, Error = err };
                }

            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }

            return response;
        }
        private string ServerValidateDelete(int id)
        {
            var err = ",";
            try
            {

                if (new AccountRepository().CheckIfLeafAccountExist(id))
                {
                    err += "Account has child accounts and can't be deleted.";
                }
                var leafaccounts = new AccountRepository().GetLeafAccounts(id).Select(p => p.Id).ToList();
                leafaccounts.Add(id);
                if (new TransactionRepository().CheckIfTransactionExist(leafaccounts))
                {
                    err += ",Account has transactions and can't be deleted.";
                }

            }
            catch (Exception)
            {

                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;


        }



        protected JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "Id", "AccountCode", "Name" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var account = Numerics.GetInt((queryString["account"] + "").Trim());
            var typeids = new AccountRepository().GetIdsByName(SettingManager.AccountType);
            var currentaccount = typeids.FirstOrDefault(p => p == account);
            var parentAccount = new AccountRepository().GetById(currentaccount);
            var dal = new AccountRepository();
            var leafaccounts = new AccountRepository().GetLeafAccountsDetail(currentaccount);
            var records = dal.AsQueryable();
            var ids = leafaccounts.Select(p => p.Id).ToList();
            records = records.Where(p => ids.Contains(p.Id));
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                    p.Name.Contains(search)
                    || p.AccountCode.Contains(search)

                    );
            var totalRecords = filteredList.Count();
            var totalDisplayRecords = totalRecords;
            var orderedList = filteredList.OrderByDescending(p => p.Id);
            if (colIndex < coloumns.Length && coloumns[colIndex] + "" != "")
            {
                var sortDir = queryString["sSortDir_0"];
                orderedList = sortDir == "asc" ? filteredList.OrderBy(coloumns[colIndex]) :
                    filteredList.OrderByDescending(coloumns[colIndex]);
            }
            var sb = new StringBuilder();
            sb.Clear();
            var alldata = displayLength >= 0 ? orderedList.Skip(displayStart).Take(displayLength).ToList() : orderedList.ToList();

            var rs = new JQueryResponse();
            foreach (var item in alldata)
            {

                var data = new List<string>
                {

                    item.AccountCode,
                    item.Name,
                    parentAccount.DisplayName
                  
                };
                var editIcon = "<i class='fa fa-edit' onclick=\"COA.Edit(" + item.Id + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"COA.Delete(" + item.Id + ")\" title='Delete' ></i>";
                var icons = "<span class='action'>";
                icons += editIcon;
                icons += deleteIcon;
                icons += "</span>";
                data.Add(icons);
                rs.aaData.Add(data);
            }
            rs.sEcho = echo;
            rs.iTotalRecords = totalRecords;
            rs.iTotalDisplayRecords = totalDisplayRecords;
            return rs;
        }
    }
}
