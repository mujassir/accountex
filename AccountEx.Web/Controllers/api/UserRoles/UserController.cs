using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using AccountEx.CodeFirst.Models;
using AccountEx.BussinessLogic;

namespace AccountEx.Web.Controllers.api
{
    public class UserController : BaseApiController
    {
        // GET api/test
        public JQueryResponse Get()
        {
            return GetDataTable();
        }

        // GET api/test/5
        public ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                response = new ApiResponse
                {
                    Success = true,
                    Data = new UserRepository().GetById(id),

                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        // GET api/Get ACtions
        public ApiResponse Get(string action, int actionid)
        {
            ApiResponse response;
            try
            {
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Actions = new ActionRepository().GetAll().OrderBy(p => p.SequenceNo).ToList(),
                    }

                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }

        public ApiResponse Get(string roleids)
        {

            ApiResponse response;
            try
            {

                var ids = roleids.Split(',').Where(p => !string.IsNullOrEmpty(p)).Select(p => int.Parse(p)).ToList();
                //var access = new RoleAccessRepository().GetByRoleId(ids);
                //var menuItems = new MenuItemRepository().GetAll().Select(p => new MenuItemExtra(p)).ToList();
                //menuItems = menuItems.Where(p => access.Any(q => q.MenuItemId == p.Id && q.CanView)).ToList();

                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        RoleAccess = new RoleAccessRepository().GetCommonAccess(ids),
                        RoleActions = new RoleActionRepository().GetByRoleId(ids)
                    }

                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        // GET api/For change password
        public ApiResponse Get(string oldPassword, string newPassword)
        {
            ApiResponse response;
            try
            {
                if (SiteContext.Current.User == null) throw new OwnException("Session expired, please login again");
                new UserRepository().ChangePassword(SiteContext.Current.User.Id, oldPassword, newPassword);
                response = new ApiResponse
                {
                    Success = true,
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }


        public ApiResponse Post([FromBody]User input)
        {
            ApiResponse response;
            try
            {
                var err = ValidateSave(input);
                if (err == "")
                {
                    input.Username = input.Username.ToLower();
                    input.Email = (input.Email + "").ToLower();
                    var userroles = new List<UserRole>();
                    var queryString = Request.RequestUri.ParseQueryString();
                    new UserRepository().Save(input, queryString["hash"]);
                    response = new ApiResponse() { Success = true };
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

        // DELETE api/test/5
        public ApiResponse Delete(int id)
        {
            ApiResponse response;

            var err = ServerValidateDelete(id);
            if (err == "")
            {
                new UserRoleRepository().DeleteByUserId(id);
                new UserRepository().Delete(id);
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
            var coloumns = new[] { "Id", "Username", "Email", "", "FirstName", "LastName", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var dal = new UserRepository();
            var records = dal.AsQueryable();
            if (ActionManager.OnlyViewOwnRecord)
                records = records.Where(p => p.CreatedBy == SiteContext.Current.User.Id);
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                    p.Username.Contains(search)
                    || p.Email.Contains(search)
                    || p.FirstName.Contains(search)
                    || p.LastName.Contains(search)
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
                data.Add(item.Username);
                data.Add(item.FirstName);
                data.Add(item.LastName);
                data.Add(item.Role);
                data.Add(item.Email);
                var editIcon = "<i class='fa fa-edit' onclick=\"Users.Edit(" + item.Id + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"Users.Delete(" + item.Id + ")\" title='Delete' ></i>";
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
        private static string ValidateSave(User user)
        {
            var err = ",";
            try
            {
                var repo = new UserRepository();

                if (repo.IsUserExist(user.Username, user.Id))
                {
                    err += "Username has already been taken.,";
                }




            }
            catch (Exception)
            {

                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;


        }
        private string ServerValidateDelete(int id)
        {
            var err = ",";
            try
            {
                var repo = new UserRepository();
                var record = repo.GetById(id);
                if (record != null && record.IsAdmin)
                {
                    if (!repo.CheckIfAdminUserExist(id))
                        err += "This is the last admin user you cannot delete it.";
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
