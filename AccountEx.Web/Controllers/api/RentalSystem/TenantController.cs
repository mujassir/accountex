using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AccountEx.BussinessLogic;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.DbMapping;

namespace AccountEx.Web.Controllers.api.Manage
{
    public class TenantController : AccountDetailController
    {
        public TenantController()
        {
            AccountDetailFormId = (int)AccountDetailFormType.Tenant;
            HeadAccountId = SettingManager.TenantHeadId;
        }
        public override ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                var data = new AccountDetailRepository().GetById(id);
                var partners = new TenantPartnerRepository().GetByTenantId(data.AccountId);
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Tenant = data,
                        Partners = partners
                    }

                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        public ApiResponse Post(TenantExtra input, string key)
        {
            ApiResponse response;
            try
            {
                var accountdetailresponse = base.Post(input.AccountDetail);
                if (accountdetailresponse.Success)
                {
                    input.TenantPartners.ForEach(p => p.TenantId = input.AccountDetail.AccountId);
                    new TenantPartnerRepository().Save(input.TenantPartners);
                    response = new ApiResponse()
                    {
                        Success = true,
                        Data = null,
                    };
                }
                else
                {
                    response = new ApiResponse() { Success = false, Error = accountdetailresponse.Error };

                }

            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
                ;
            }

            return response;
        }

        public override ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                var record = new AccountDetailRepository().GetById(id);
                var accountdetailresponse = base.Delete(id);
                if (accountdetailresponse.Success)
                {
                    new TenantPartnerRepository().DeleteByTenantId(record.AccountId);
                    response = new ApiResponse() { Success = true, Data = null };

                }
                else
                {
                    response = new ApiResponse()
                    {
                        Success = false,
                        Error = accountdetailresponse.Error
                    };
                }

            }
            catch (Exception ex)
            {
                response = new ApiResponse() { Success = false, Error = ex.Message }; ;
            }
            return response;
        }

        protected override JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "Id", "Code", "Name", "ContactNumber", "Email", "Address", "" };
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
            if (search != "")
                filteredList = records.Where(p =>
                    p.Code.Contains(search)
                    || p.Name.Contains(search)
                    || p.BrandName.Contains(search)
                    || p.Type.Contains(search)
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
                if (type == "report") data.Add((++sr) + "");
                //else data.Add("<td><input type='checkbox' class='checkboxes' value='1' /></td>");
                data.Add(item.Code);
                data.Add(item.Name);
                data.Add(item.Type);
                //data.Add(item.Business);
                data.Add(item.BrandName);

                var editIcon = "<i class='fa fa-edit' onclick=\"Tenant.Edit(" + item.Id + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"Tenant.Delete(" + item.Id + ")\" title='Delete' ></i>";
                var printIcon = "<i class='fa fa-print' onclick=\"Tenant.Print(" + item.Id + ")\" title='Print' ></i>";
                var icons = "<span class='action'>";
                icons += editIcon;
                icons += deleteIcon;
                icons += printIcon;
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
