using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Web.Controllers.api.Shared;
using System.Web.Http;
using AccountEx.Repositories;
using AccountEx.Repositories.Config;
using AccountEx.BussinessLogic;
using AccountEx.CodeFirst.Models.CRM;

namespace AccountEx.Web.Controllers.api.CRM
{
    public class CRMCustomerController : GenericApiController<CRMCustomer>
    {

        public override ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                var data = new GenericRepository<CRMCustomer>().GetById(id);
                if (SiteContext.Current.UserTypeId == CRMUserType.SalesExecutive)
                {
                    data.CRMCustomerSalePersons = data.CRMCustomerSalePersons.Where(p => p.UserId == SiteContext.Current.User.Id).ToList();
                }
                response = new ApiResponse
                {
                    Success = true,
                    Data = data,

                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        public override ApiResponse Post([FromBody]CRMCustomer input)
        {
            ApiResponse response;
            try
            {
                var err = ServerValidateSave(input);
                if (err == "")
                {
                    new CRMCustomerRepository().Save(input);
                    response = new ApiResponse { Success = true, Data = input.Id };
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
        private string ServerValidateSave(CRMCustomer input)
        {
            var err = ",";
            try
            {
                //if (SiteContext.Current.UserTypeId == CRMUserType.SalesExecutive)
                //{
                //    err += "You did not have sufficeient right to add/update record.";
                //}
                if (Repository.IsExist(input.Name, input.Id))
                {
                    err += ",Name already exist.";
                }

            }
            catch (Exception ex)
            {
                err = ErrorManager.Log(ex);
            }
            err = err.Trim(',');
            return err;


        }
        protected override JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "Name", "Region", "", "", "ClusterType", "Industry", "ShippingAddress", "Email", "CellNo", "Web","Active", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var pageSize = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var startRow = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var type = (queryString["type"] + "").Trim();
            var sortDirection = queryString["sSortDir_0"];
            var sortColumn = coloumns[colIndex];
            var records = new CRMCustomerRepository().GetListing(startRow, pageSize, sortColumn, sortDirection, search);
            var totalRecords = 0;
            var totalDisplayRecords = 0;
            if (records.Any())
            {
                var fr = records.FirstOrDefault();
                totalRecords = fr.TotalRows;
                totalDisplayRecords = fr.TotalFilterRows;
            }



            var sb = new StringBuilder();
            sb.Clear();
            var rs = new JQueryResponse();
            foreach (var item in records)
            {
                var data = new List<string>();
                data.Add(item.Name);
                data.Add(item.Region);
                data.Add(item.SalePerson);
                data.Add(item.Category);
                data.Add(item.ClusterType);
                data.Add(item.Industry);
                data.Add(item.ShippingAddress);
                data.Add(item.Email);
                data.Add(item.CellNo);
                data.Add(item.Web);
                data.Add(item.Active);
                var editIcon = "<i class='fa fa-edit btn-edit' data-id='" + item.Id + "' title='Edit' ></i>";
                var viewIcon = "<i class='fa fa-eye btn-view' data-id='" + item.Id + "' title='View' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o btn-delete' data-id='" + item.Id + "' title='Delete' ></i>";
                var icons = "<span class='action'>";
                if (SiteContext.Current.UserTypeId == CRMUserType.SalesExecutive)
                {
                    icons += viewIcon;
                }
                else
                {
                    icons += editIcon;
                    icons += deleteIcon;
                }
                icons += "</span>";
                if (type != "report") data.Add(icons);
                rs.aaData.Add(data);
            }
            rs.sEcho = echo;
            rs.iTotalRecords = totalRecords;
            rs.iTotalDisplayRecords = totalDisplayRecords;
            return rs;
        }


    }


}
