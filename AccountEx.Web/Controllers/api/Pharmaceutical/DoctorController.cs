using AccountEx.BussinessLogic;
using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using AccountEx.CodeFirst.Models.Pharmaceutical;
using AccountEx.Repositories.Pharmaceutical;
using AccountEx.Web.Controllers.api.Shared;

namespace AccountEx.Web.Controllers.api.Pharmaceutical
{
    public class DoctorController : GenericApiController<Doctor>
    {
        //public override ApiResponse Post([FromBody]Doctor entity)
        //{
        //    ApiResponse response;
        //    try
        //    {
        //        new DoctorRepository().Save(entity);
        //        response = new ApiResponse() { Success = true };
        //    }
        //    catch (Exception ex)
        //    {
        //        response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
        //    }
        //    return response;
        //}
        public override ApiResponse Post([FromBody]Doctor input)
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
        private string ServerValidateSave(Doctor input)
        {
            var err = ",";
            try
            {

                if (new DoctorRepository().IsExist(input.Name, input.Id))
                {
                    err += ",Doctor with same name has already been added.";
                }

            }
            catch (Exception)
            {

                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;


        }
        protected override JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "Name", "Speciality", "Designation", "PhoneNumber", "City", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var type = (queryString["type"] + "").Trim();
            //var dal = new ProjectRepository();
            var records = Repository.AsQueryable();
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                     p.Name.Contains(search) ||
                     p.PhoneNumber.Contains(search) ||
                     p.City.Contains(search) ||
                     p.Speciality.Contains(search) ||
                     p.Designation.Contains(search)
                   );
            var orderedList = filteredList.OrderByDescending(p => p.Id);
            var sb = new StringBuilder();
            sb.Clear();

            var sr = 0;
            var rs = new JQueryResponse();
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                var data = new List<string>();
                data.Add(item.Name);
                data.Add(item.Speciality);
                data.Add(item.Designation);
                data.Add(item.PhoneNumber);
                data.Add(item.City);              
                var editIcon = "<i class='fa fa-edit' onclick=\"Doctors.Edit(" + item.Id + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"Doctors.Delete(" + item.Id + ")\" title='Delete' ></i>";
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
