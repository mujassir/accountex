using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.BussinessLogic;
using AccountEx.DbMapping;

namespace AccountEx.Web.Controllers.api.Account
{
    public class OldUserController : BaseApiController
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
                new UserRepository().ChangePassword(SiteContext.Current.User.Id, oldPassword, newPassword);
                response = new ApiResponse
                {
                    Success = true,
                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
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
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        public ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                var userole = new UserRoleRepository().GetByUserId(id);
                var roleId = userole != null ? userole.RoleId : 0;
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        User = new UserRepository().GetById(id),
                        RoleAccess = new RoleAccessRepository().GetByRoleId(roleId),
                        RoleActions = new RoleActionRepository().GetByRoleId(roleId)
                    }

                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        // POST api/test
        public ApiResponse Post([FromBody]UserExtra input)
        {
            ApiResponse response;
            try
            {
                input.User.Username = input.User.Username.ToLower();
                input.User.Email = input.User.Email.ToLower();
                var queryString = Request.RequestUri.ParseQueryString();
               // new UserRepository().Save(input);
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
            try
            {
                new UserRepository().Delete(id);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
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

                data.Add("<td><input type='checkbox' class='checkboxes' value='1' /></td>");
                data.Add(item.Username);
                data.Add(item.FirstName);
                data.Add(item.LastName);
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
        //// GET api/test
        //public JQueryResponse Get()
        //{
        //    return GetDataTable();
        //}


        //// GET api/test/5
        //public ApiResponse Get(int id)
        //{
        //    ApiResponse response;
        //    try
        //    {
        //        response = new ApiResponse
        //        {
        //            Success = true,
        //            Data = new UserRepository().GetById(id),

        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //         response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
        //    }
        //    return response;
        //}

        //// POST api/test
        //public ApiResponse Post([FromBody]User input)
        //{
        //    ApiResponse response;
        //    try
        //    {
        //        if (!string.IsNullOrWhiteSpace(input.Hash))
        //            input.Hash = Sha1Sign.Hash(input.Username + input.Hash);
        //        new UserRepository().Save(input);
        //        response = new ApiResponse { Success = true, Data = input.Id };
        //    }
        //    catch (Exception ex)
        //    {
        //         response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
        //    }

        //    return response;

        //}



        //// DELETE api/test/5
        //public ApiResponse Delete(int id)
        //{
        //    ApiResponse response;
        //    try
        //    {
        //        new UserRepository().Delete(id);
        //        response = new ApiResponse { Success = true };
        //    }
        //    catch (Exception ex)
        //    {
        //         response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
        //    }

        //    return response;
        //}

        //private JQueryResponse GetDataTable()
        //{
        //    var queryString = Request.RequestUri.ParseQueryString();
        //    var coloumns = new[] { "Id", "Username", "Email", "", "FirstName", "LastName", "" };
        //    var echo = Convert.ToInt32(queryString["sEcho"]);
        //    var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
        //    var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
        //    var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
        //    var search = (queryString["sSearch"] + "").Trim();
        //    var dal = new UserRepository();
        //    var records = dal.AsQueryable();
        //    var totalRecords = records.Count();
        //    var totalDisplayRecords = totalRecords;
        //    var filteredList = records;
        //    if (search != "")
        //        filteredList = records.Where(p =>
        //            p.Username.Contains(search)
        //            || p.Email.Contains(search)
        //            || p.FirstName.Contains(search)
        //            || p.LastName.Contains(search)
        //            );
        //    var orderedList = filteredList.OrderByDescending(p => p.Id);
        //    if (colIndex < coloumns.Length && coloumns[colIndex] + "" != "")
        //    {
        //        var sortDir = queryString["sSortDir_0"];
        //        orderedList = sortDir == "asc" ? filteredList.OrderBy(coloumns[colIndex]) :
        //            filteredList.OrderByDescending(coloumns[colIndex]);
        //    }
        //    var sb = new StringBuilder();
        //    sb.Clear();

        //    var rs = new JQueryResponse();
        //    foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
        //    {
        //        var data = new List<string>();

        //        data.Add("<td><input type='checkbox' class='checkboxes' value='1' /></td>");
        //        data.Add(item.Username);
        //        data.Add(item.Email);
        //        data.Add(item.FirstName);
        //        data.Add(item.LastName);
        //        var editIcon = "<i class='fa fa-edit' onclick=\"Users.Edit(" + item.Id + ")\" title='Edit' ></i>";
        //        var deleteIcon = "<i class='fa fa-trash-o' onclick=\"Users.Delete(" + item.Id + ")\" title='Delete' ></i>";
        //        var icons = "<span class='action'>";
        //        icons += editIcon;
        //        icons += deleteIcon;
        //        icons += "</span>";
        //        data.Add(icons);
        //        rs.aaData.Add(data);
        //    }
        //    rs.sEcho = echo;
        //    rs.iTotalRecords = totalRecords;
        //    rs.iTotalDisplayRecords = totalDisplayRecords;
        //    return rs;
        //}
    }
}
