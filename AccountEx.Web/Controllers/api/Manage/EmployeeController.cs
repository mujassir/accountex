using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.BussinessLogic;
using AccountEx.CodeFirst.Models;

namespace AccountEx.Web.Controllers.api.Manage
{
    public class EmployeeController : AccountDetailController
    {
        public EmployeeController()
        {
            AccountDetailFormId = (int)AccountDetailFormType.Employees;
            HeadAccountId = SettingManager.EmployeeHeadId;
        }

        public override ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();

                var repo = new AccountDetailRepository();

                var employee = repo.GetByAccountId(id);
                var bank = repo.GetByAccountId(employee.BankId);

                var key = queryString["requestkey"];
                if (string.IsNullOrWhiteSpace(key) && key != "edit")
                {
                    var salRepo = new SalaryRepository(repo);
                    var transRepo = new TransactionRepository(repo);

                    var leaves = new Salary();
                    leaves = salRepo.GetLeaveRecord(id, DateTime.Now.Year);
                    response = new ApiResponse
                    {
                        Success = true,
                        Data = new
                        {
                            Employee = employee,
                            Leaves = leaves,
                            Balance = transRepo.GetSummary(id)
                        }

                    };
                }
                else
                {

                    response = new ApiResponse
                    {


                        Success = true,
                        Data = new
                        {
                            Employee = employee,
                            Bank = bank,
                        }

                    };
                }


            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }
        //public override ApiResponse Post([FromBody]Employee input)
        //{
        //    ApiResponse response;
        //    try
        //    {
        //        var filename = input.PictureUrl;
        //        if (input.Id == 0 && !string.IsNullOrWhiteSpace(input.n))
        //        {
        //            filename = SaveImage(input);

        //        }
        //        else
        //        {
        //            var dbproduct = new StoreRepository().GetById(input.Id);
        //            if (dbproduct != null && dbproduct.FileName != input.FileName && !string.IsNullOrWhiteSpace(input.FileName))
        //            {
        //                filename = SaveImage(input);
        //                DeleteFile(System.Web.Hosting.HostingEnvironment.MapPath("~/Content/ProductLogos/" + dbproduct.FileName));
        //            }
        //            if (string.IsNullOrWhiteSpace(input.FileName) && dbproduct != null && !string.IsNullOrWhiteSpace(dbproduct.FileName))
        //            {

        //                DeleteFile(System.Web.Hosting.HostingEnvironment.MapPath("~/Content/ProductLogos/" + dbproduct.FileName));

        //            }
        //        }

        //            input.FileName = filename;
        //            new StoreRepository().Save(input);


        //            response = new ApiResponse() { Success = true, Data = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/ProductLogos/") + "\\" + filename };

        //    }
        //    catch (Exception ex)
        //    {
        //         response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
        //    }

        //    return response;

        //}


        //private string SaveImage(Employee input)
        //{
        //    var path = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/ProductLogos");
        //    var filename = new Random().Next() + "_" + input.FileName;
        //    var fullpath = path + "\\" + filename;
        //    var metadataStart = 0;
        //    var base64url = "";
        //    var metastring = "data:image/png;base64,";
        //    metadataStart = input.FileUrl.IndexOf(metastring);
        //    if (metadataStart != -1)
        //    {
        //        // Remove the metadata if found
        //        base64url = input.FileUrl.Remove(metadataStart, metadataStart + metastring.Count());
        //    }
        //    metastring = "data:image/jpeg;base64,";
        //    metadataStart = input.FileUrl.IndexOf(metastring);
        //    if (metadataStart != -1)
        //    {
        //        // Remove the metadata if found
        //        base64url = input.FileUrl.Remove(metadataStart, metadataStart + metastring.Count());
        //    }
        //    metastring = "data:image/gif;base64,";
        //    metadataStart = input.FileUrl.IndexOf(metastring);
        //    if (metadataStart != -1)
        //    {
        //        // Remove the metadata if found
        //        base64url = input.FileUrl.Remove(metadataStart, metadataStart + metastring.Count());
        //    }

        //    var bytes = Convert.FromBase64String(base64url);
        //    using (var imageFile = new FileStream(fullpath, FileMode.Create))
        //    {
        //        imageFile.Write(bytes, 0, bytes.Length);
        //        imageFile.Flush();
        //    }
        //    return filename;
        //}

        //private void DeleteFile(string path)
        //{
        //    try
        //    {

        //        if (File.Exists(path))
        //        {
        //            File.Delete(path);
        //        }
        //    }
        //    catch (Exception)
        //    {


        //    }
        //}
        protected override JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "Id", "Code", "Name", "FatherName", "CNIC", "ContactNumber", "Qualification", "DeploymentStatus", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var type = (queryString["type"] + "").Trim();
            //var dal = new ProjectRepository();
            var records = Repository.AsQueryable().Where(p => p.AccountDetailFormId == AccountDetailFormId);
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                    p.Code.Contains(search)
                    || p.Name.Contains(search)
                    || p.FatherName.Contains(search)
                    || p.CNIC.Contains(search)
                    || p.ContactNumber.Contains(search)
                        //|| p.Designation.Contains(search)
                    || p.DeploymentStatus.Contains(search)
                    || p.Email.Contains(search)
                    || p.Qualification.Contains(search)
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
                var data = new List<string>
                {
                    
                    item.Code,
                    item.Name,
                    item.FatherName,
                    item.CNIC,
                    item.ContactNumber,
                //    item.Designation.ToString()+"",
                    item.Qualification,
                    item.DeploymentStatus,
                    //Numerics.DecimalToString(item.BasicSalary, 0)
                };
                if (type == "report") data.Insert(0, (++sr) + "");
                //else data.Insert(0, "<td><input type='checkbox' class='checkboxes' value='1' /></td>");
                var editIcon = "<i class='fa fa-edit' onclick=\"Employees.Edit(" + item.AccountId + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"Employees.Delete(" + item.Id + ")\" title='Delete' ></i>";
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
