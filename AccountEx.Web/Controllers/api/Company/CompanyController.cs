using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AccountEx.Repositories;
using System.Globalization;
using AccountEx.BussinessLogic;
using AccountEx.DbMapping;

namespace AccountEx.Web.Controllers.api
{
    public class CompanyController : BaseApiController
    {
        // GET api/test
        public JQueryResponse Get()
        {
            return GetDataTable();
        }

        public ApiResponse Post(CompanyExtra input)
        {
            ApiResponse response;

            try
            {
                var err = ServerValidateSave(input);
                if (err == "")
                {
                    new CompanyRepository().Save(input);
                    response = new ApiResponse()
                    {
                        Success = true,
                        Data = ""
                    };
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

                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }


            return response;
        }
        private string ServerValidateSave(CompanyExtra input)
        {
            var err = ",";
            try
            {


                var record = new CompanyRepository().GetByName(input.Name, input.Id);
                if (record != null)
                {
                    err += "Company name already exist.,";
                }
                var user = new UserRepository().GetByUsername(input.UserName);
                if (user != null)
                {
                    err += "User name already exist.,";
                }

                if (SiteContext.Current.User.Username.ToUpper() != "KR")
                {

                    err += "You did not have sufficent right to perform the current operation.Warning! Your action has been noted & sent to investigation team.,";
                     throw new OwnException("Invalid Company Creation Attempt:You did not have sufficent right to perform the current operation.Warning! Your action has been noted & sent to investigation team.");
                }

            }
            catch (Exception ex)
            {
                ErrorManager.Log(ex);
                if (string.IsNullOrEmpty(err))
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

               
               

                if (SiteContext.Current.User.Username.ToUpper() != "KR")
                {

                    err += "You did not have sufficent right to perform the current operation.Warning! Your action has been noted & sent to investigation team.,";
                     throw new OwnException("Invalid Company Deletion Attempt:You did not have sufficent right to perform the current operation.Warning! Your action has been noted & sent to investigation team.");
                }

            }
            catch (Exception)
            {

                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;


        }
        public ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                // var accountdetail = new AccountDetailRepository().GetById(id);
                var err = ServerValidateDelete(id);
              
                if (err == "")
                {

                    new CompanyRepository().Delete(id);
                    //base.Delete(id);
                    response = new ApiResponse() { Success = true };
                }
                else
                {
                    response = new ApiResponse() { Success = false, Error = err };
                }

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
            var coloumns = new[] { "Name", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var type = (queryString["type"] + "").Trim();
            var intsearch = Numerics.GetInt((queryString["sSearch"] + "").Trim());
            var dal = new CompanyRepository().AsQueryable();
            var records = dal.AsQueryable();
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
            var projects = orderedList.Skip(displayStart).Take(displayLength).ToList();
            //var projectIds = projects.Select(p => p.Id).ToList();
            //var receipts = new ProjectReceiptRepository().AsQueryable().Where(p => projectIds.Contains(p.ProjectId)).ToList();
            var prReceiptRepo = new ProjectReceiptRepository();
            var rs = new JQueryResponse();
            var sr = 0;
            foreach (var item in projects)
            {
                var data = new List<string>();

                data.Add(item.Name + "");


                var editIcon = "<i class='fa fa-edit' onclick=\"Companies.Edit(" + item.Id + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"Companies.Delete(" + item.Id + ")\" title='Delete' ></i>";
                var icons = "<span class='action'>";
                //icons += editIcon;
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
