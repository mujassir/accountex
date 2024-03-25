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
    public class EventModeController : GenericApiController<EventMode>
    {

        public override ApiResponse Post([FromBody]EventMode input)
        {
            ApiResponse response;
            try
            {
                var err = ServerValidateSave(input);
                if (err == "")
                {
                    response = base.Post(input);
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
        private string ServerValidateSave(EventMode input)
        {
            var err = ",";
            try
            {

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
            var coloumns = new[] { "Name", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var type = (queryString["type"] + "").Trim();
            //var dal = new ProjectRepository();
            var records = Repository.AsQueryable();
            var totalRecords = records.Count();

            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>

                     p.Name.Contains(search)

                   );

            var totalDisplayRecords = filteredList.Count();
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
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                var data = new List<string>();
                data.Add(item.Name);
                var editIcon = "<i class='fa fa-edit btn-edit' data-id='" + item.Id + "' title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o btn-delete' data-id='" + item.Id + "' title='Delete' ></i>";
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
