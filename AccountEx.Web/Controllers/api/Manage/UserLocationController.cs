using AccountEx.BussinessLogic;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Linq;

namespace AccountEx.Web.Controllers.api.Manage
{
    public class UserLocationController : BaseApiController
    {
        // GET api/test
        public JQueryResponse Get()
        {
            return GetDataTable();
        }
        public ApiResponse Post([FromBody] UserLocation input)
        {
            ApiResponse response;
            try
            {
                var userRep = new GenericRepository<UserLocation>();
                var exist = userRep.FirstOrDefault(e => e.UserId == input.UserId && e.AuthLocationId == input.AuthLocationId);
                if (exist != null) throw new Exception("User Already Authorized");
                userRep.Save(input);
                response = new ApiResponse() { Success = true };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;

        }

        // DELETE api/test/5
        public ApiResponse Delete(int id)
        {
            ApiResponse response;

            var err = ServerValidateDelete(id);
            if (err == "")
            {
                new GenericRepository<UserLocation>().Delete(id);
                response = new ApiResponse { Success = true };
            }
            else
            {
                response = new ApiResponse { Success = false, Error = err };
            }
            return response;
        }

        private JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "Id", "UserId", "CompanyId", "Username", "CompanyName" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();

            var dal = new GenericRepository<UserLocation>();
            var records = dal.AsQueryable().Include(u => u.User).Include(u => u.Location);
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                    p.UserId == SiteContext.Current.User.Id
                    && (p.User.Username.Contains(search) || p.Location.Name.Contains(search))
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
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                var data = new List<string>();

                //data.Add("<td><input type='checkbox' class='checkboxes' value='1' /></td>");
                data.Add(item.User.Username);
                data.Add(item.Location.Name);
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"UserLocations.Delete(" + item.Id + ")\" title='Delete' ></i>";
                var icons = "<span class='action'>";
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
        private string ServerValidateDelete(int id)
        {
            var err = ",";
            try
            {
                var repo = new GenericRepository<UserLocation>();
                var record = repo.GetById(id);
                if (record == null)
                {
                    err += "Data does not exist.";
                }
                if (SiteContext.Current.User.Id == id)
                {
                    err += "Current Logged-in user can't be deleted.";
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
