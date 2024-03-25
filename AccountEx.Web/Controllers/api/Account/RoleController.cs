
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using AccountEx.BussinessLogic;
using AccountEx.CodeFirst.Models;

namespace AccountEx.Web.Controllers.api
{
    public class RoleController : BaseApiController
    {
        // GET api/test
        public JQueryResponse Get()
        {
            return GetDataTable();
        }


        // GET api/test/5
        public ApiResponse Get(string oldPassword, string newPassword)
        {
            ApiResponse response;
            try
            {
                if (SiteContext.Current.User == null)  throw new OwnException("Session expired, please login again");
                //new UserRepository().ChangePassword(SiteContext.Current.User.Id, oldPassword, newPassword);
                response = new ApiResponse
                {
                    Success = true,
                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }
        public ApiResponse Get(string action)
        {
            ApiResponse response;
            try
            {
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Actions = new ActionRepository().GetAll(),
                    }

                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }
        public ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                var repo = new RoleRepository();
                var role = repo.GetById(id);
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Role = role,
                        RoleAccess = new RoleAccessRepository(repo).GetByRoleId(id),
                        RoleActions = new RoleActionRepository(repo).GetByRoleId(id)
                    }

                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }

        // POST api/test
        public ApiResponse Post([FromBody]Role input)
        {
            ApiResponse response;
            try
            {


                RoleManager.Save(input);
                response = new ApiResponse() { Success = true };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }

            return response;

        }




        //DELETE api/test/5
        public ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {

                response = RoleManager.Delete(id);

            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }

            return response;
        }


        private JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "Name", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var dal = new RoleRepository();
            var records = dal.AsQueryable();
            if (ActionManager.OnlyViewOwnRecord)
                records = records.Where(p => p.CreatedBy == SiteContext.Current.User.Id);
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

            var rs = new JQueryResponse();
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                var data = new List<string>();


                data.Add(item.Name);
                var editIcon = "<i class='fa fa-edit' onclick=\"Roles.Edit(" + item.Id + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"Roles.Delete(" + item.Id + ")\" title='Delete' ></i>";
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
