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
    public class CRMUserController : BaseApiController
    {

        public ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                var user = new UserRepository().GetById(id);
                user.Hash = null;
                response = new ApiResponse
                {
                    Success = true,
                    Data = user,

                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
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
                    new UserRepository().SaveCRMUser(input);
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
        public JQueryResponse GetDataTable()
        {

            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                var coloumns = new[] { "Name", "LastName", "Email", "CellNo", "Company", "Web", "" };
                var echo = Convert.ToInt32(queryString["sEcho"]);
                var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
                var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
                var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
                var search = (queryString["sSearch"] + "").Trim();
                var type = (queryString["type"] + "").Trim();
                //var dal = new ProjectRepository();
                var records = new GenericRepository<vw_CRMUser>().AsQueryable();
                var totalRecords = records.Count();

                var filteredList = records;
                if (search != "")
                    filteredList = records.Where(p =>

                         p.Username.Contains(search) ||
                         p.FirstName.Contains(search) ||
                          p.LastName.Contains(search) ||
                          p.LastName.Contains(search) ||
                          p.Email.Contains(search) ||
                          p.CellNo.Contains(search) ||
                          p.UserType.Contains(search) ||
                          p.Domain.Contains(search) ||
                          p.Division.Contains(search) ||
                          p.Region.Contains(search) ||
                          p.RSM.Contains(search)



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
                    data.Add(item.Username);
                    data.Add(item.FirstName);
                    data.Add(item.LastName);
                    data.Add(item.UserType);
                    data.Add(item.Domain);
                    data.Add(item.Division);
                    data.Add(item.Region);
                    data.Add(item.RSM);
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
            catch (Exception ex)
            {
                ErrorManager.Log(ex);
                throw;
            }


        }

        public ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                var err = ServerValidateDelete(id);
                if (err == "")
                {
                    using (var scope = TransactionScopeBuilder.Create())
                    {
                        new UserRoleRepository().Delete(id, "UserId");
                        new UserRepository().Delete(id);
                        scope.Complete();
                    }
                    response = new ApiResponse { Success = true };
                }
                else
                {
                    response = new ApiResponse { Success = false, Error = err };

                }
            }
            catch (Exception ex)
            {

                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }



            return response;
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
