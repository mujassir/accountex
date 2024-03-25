using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Web.Controllers.api.Shared;
using AccountEx.CodeFirst.Models.COA;
using AccountEx.Repositories;
using AccountEx.BussinessLogic;
using System.Web.Http;
namespace AccountEx.Web.Controllers.api.Manage
{
    public class ShipController : GenericApiController<Ship>
    {

        public override ApiResponse Post([FromBody]Ship input)
        {
            ApiResponse response;
            try
            {
                var err = ServerValidate(input);
                if (err == "")
                {

                    response = base.Post(input);
                }
                else
                {
                    response = new ApiResponse() { Success = false, Error = err };
                }
            }
            catch (Exception ex)
            {
                response = new ApiResponse() { Success = false, Error = ex.Message };
            }
            return response;
        }
        private string ServerValidate(Ship input)
        {
            var err = ",";
            try
            {

                if (new ShipRepository().CheckIfVessleExists(input.VESSEL, input.Name,input.ShipperId, input.Id))
                {
                    err += "Vessle no already exist.,";
                }

            }
            catch (Exception)
            {

                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;


        }
        public ApiResponse Delete(int id, string key)
        {
            ApiResponse response;
            try
            {
                var err = "";
                err = ValidateDelete(id);
                if (err == "")
                {
                    new ShipRepository().Delete(id);
                    response = new ApiResponse { Success = true };
                }
                else
                {
                    response = new ApiResponse { Success = false, Error = err };
                }
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        private string ValidateDelete(int id)
        {
            string err = "";
            if (new BLRepository().CheckIfShipIdExists(id))
            {
                err += "This ship cannot not be deleted because it has been used in bill of lading";
            }
            return err;
        }

        protected override JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "Name", "VESSEL", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var type = (queryString["type"] + "").Trim();
            var records = Repository.AsQueryable();
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                     p.Name.Contains(search)
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
                //data.Add("<td><input type='checkbox' class='checkboxes' value='1' /></td>");
                data.Add(item.Name);
                data.Add(item.VESSEL);
                var editIcon = "<i class='fa fa-edit' onclick=\"Ships.Edit(" + item.Id + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"Ships.Delete(" + item.Id + ")\" title='Delete' ></i>";
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
