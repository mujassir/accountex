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

namespace AccountEx.Web.Controllers.api.Manage
{
    public class ServicesController : AccountDetailController
    {

        public ServicesController()
        {
            AccountDetailFormId = (int)AccountDetailFormType.Services;
            HeadAccountId = SettingManager.ServicesHeadId;
        }
        public override JQueryResponse Get()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var type = (queryString["type"] + "").Trim();
            if (type == "report")
                return GetDataTableForReport();
            else
                return GetDataTable();
        }
        public override ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {

                var repo = new AccountDetailRepository();
                var serviceItemrepo = new ServiceItemRepository(repo);
                var service = repo.GetById(id);
                response = new ApiResponse()
                {
                    Success = true,
                    Data = new
                    {
                        Service = service,
                        ServiceItems = serviceItemrepo.GetByServiceId(service.AccountId)
                    }

                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
        public ApiResponse Post([FromBody]SaveServicesExtra input, string Key)
        {
            ApiResponse response;
            try
            {

                var service = input.Service;
                //using (var scope = TransactionScopeBuilder.Create())
                //{
                response = base.Post(service);
                if (response.Success)
                {
                    foreach (var item in input.ServiceItems)
                    {
                        item.ServiceId = service.AccountId;
                    }
                    new ServiceItemRepository().Save(input.ServiceItems, service.AccountId);
                    response = new ApiResponse() { Success = true, Data = input, };
                }
                else
                    return response;
                //    scope.Complete();
                //}
            }


            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
        public override ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                var repo=new AccountDetailRepository();
                var serviceItemRepo = new ServiceItemRepository(repo);

                var ad = repo.GetById(id);
                base.Delete(id);
                serviceItemRepo.DeleteByServiceId(ad.AccountId);
                response = new ApiResponse() { Success = true };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }

            return response;
        }

        //Delete Item
        public ApiResponse Delete(int id, string key)
        {
            ApiResponse response;
            try
            {
                var err = ServerValidateItemDelete(id);
                if (err == "")
                {
                    new ServiceItemRepository().Delete(id);
                    response = new ApiResponse() { Success = true };
                }
                else
                {
                    response = new ApiResponse() { Success = false, Error = err };
                }
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }

            return response;
        }
        protected override JQueryResponse GetDataTable()
        {

            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "Id", "Code", "Name", "Rate", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            //var type = (queryString["type"] + "").Trim();
            //var dal = new ProjectRepository();
            var records = Repository.AsQueryable().Where(p => p.AccountDetailFormId == AccountDetailFormId);
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                    p.Code.Contains(search)
                    || p.Name.Contains(search)
                    );
            var orderedList = filteredList.OrderByDescending(p => p.Id);
            if (colIndex < coloumns.Length && coloumns[colIndex] + "" != "")
            {
                var sortDir = queryString["sSortDir_0"];
                orderedList = sortDir == "asc" ? filteredList.OrderBy(coloumns[colIndex]) :
                    filteredList.OrderByDescending(coloumns[colIndex]);
            }
            var sb = new StringBuilder();
            sb.Clear();

            var sr = 0;
            var rs = new JQueryResponse();
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                var data = new List<string>();
                data.Add("<td><input type='checkbox' class='checkboxes' value='1' /></td>");
                data.Add(item.Code);
                data.Add(item.Name);
                data.Add(item.Rate + "");
                var editIcon = "<i class='fa fa-edit' onclick=\"Services.Edit(" + item.Id + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"Services.Delete(" + item.Id + ")\" title='Delete' ></i>";
                //var icons = "<span class='action'><a class='btn default blue-stripe btn-xs' href='../reports/generalledger?accountId=" + item.AccountId + "'>Ledger</a>";
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
        protected JQueryResponse GetDataTableForReport()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "Id", "Code", "Name", "Rate", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var type = (queryString["type"] + "").Trim();
            //var dal = new ProjectRepository();
            var records = Repository.AsQueryable().Where(p => p.AccountDetailFormId == AccountDetailFormId);
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            var sr = 0;
            if (search != "")
                filteredList = records.Where(p =>
                    p.Code.Contains(search)
                    || p.Name.Contains(search)
                    );
            var orderedList = filteredList.OrderByDescending(p => p.Id);
            if (colIndex < coloumns.Length && coloumns[colIndex] + "" != "")
            {
                var sortDir = queryString["sSortDir_0"];
                orderedList = sortDir == "asc" ? filteredList.OrderBy(coloumns[colIndex]) :
                    filteredList.OrderByDescending(coloumns[colIndex]);
            }
            var sb = new StringBuilder();
            sb.Clear();

            var rs = new JQueryResponse();
            foreach (var item in orderedList.OrderBy(p => p.GroupName).ThenBy(p => p.CityName).Skip(displayStart).Take(displayLength))
            {
                var data = new List<string>();
                data.Add((++sr) + "");
                data.Add(item.Code);
                data.Add(item.Name);
                data.Add(item.Rate + "");
            }
            rs.sEcho = echo;
            rs.iTotalRecords = totalRecords;
            rs.iTotalDisplayRecords = totalDisplayRecords;
            return rs;
        }

        private string ServerValidateItemDelete(int id)
        {
            var err = ",";
            try
            {

                if (new SaleServicesItemRepository().IsItemUsed(id))
                {
                    err += "item is used in services and can't be deleted.";
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
