using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.Web.Controllers.api.Shared;
using System.Web.Http;
using AccountEx.BussinessLogic;
using Newtonsoft.Json;
using AccountEx.BussinessLogic.CRM;

namespace AccountEx.Web.Controllers.api.CRM
{
    public class LeadsController : BaseApiController
    {
        //LeadRepository LeadRepo = new LeadRepository();
        public JQueryResponse Get()
        {
        return GetDataTable();
        }
        public ApiResponse Get(string key)
        {
            ApiResponse response;
            try
            {
                var LeadNo = LeadsManager.GetNextLeadNo();
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        LeadNo = LeadNo,
                    }
                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }

        public ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                new GenericRepository<Lead>().Delete(id);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }

            return response;
        }

        public ApiResponse Post([FromBody]Lead input)
        {
            ApiResponse response;
            try
            {
                    LeadsManager.Save(input);
                    response = new ApiResponse
                    {
                        Success = true,
                        Data = new
                        {
                            Lead = input,
                        }
                    };
                
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }

        public ApiResponse Get(int id)
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var data = new LeadRepository().GetById(id);
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
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }


        protected  JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "LeadOwner", "Company", "Customer", "Mobile", "Sector", "IndustryName", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var type = (queryString["type"] + "").Trim();
            var dal = new LeadRepository();
            var records = dal.AsQueryable();
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>

                     p.Company.Contains(search) ||
                     p.LeadOwner.Contains(search) ||
                     p.Customer.Contains(search)

                   );


            var orderedList = filteredList.OrderByDescending(p => p.Id);
            //if (colindex < coloumns.length && coloumns[colindex] + "" != "")
            //{
            //    var sortdir = querystring["ssortdir_0"];
            //   orderedlist = sortdir == "asc" ? filteredlist.orderby(coloumns[colindex]) :
            //       filteredlist.orderbydescending(coloumns[colindex]);
            //}
            var sb = new StringBuilder();
            sb.Clear();

            var sr = 0;
            var rs = new JQueryResponse();
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                var data = new List<string>();
                var salesman = item.LeadOwner;

                //data.Add("<td><input type='checkbox' class='checkboxes' value='1' /></td>");
                //data.Add("<a href='/crm/leadactivities?leadid=" + item.Id + "' target='_blank'>" + item.FirstName + " " + item.LastName + "</a>");
                data.Add("<a href='/crm/leadactivities?leadid=" + item.Id + "' target='_blank'>" + item.LeadOwner + " </a>");
                data.Add(item.Customer);
                data.Add(item.Company);
                data.Add(item.Mobile);
                var editIcon = "<i class='fa fa-edit' onclick=\"Leads.Edit(" + item.Id + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"Leads.Delete(" + item.Id + ")\" title='Delete' ></i>";
                var icons = "<span class='action'>";
                icons += editIcon;
                icons += deleteIcon;
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
