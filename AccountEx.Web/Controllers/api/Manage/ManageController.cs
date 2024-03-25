using AccountEx.BussinessLogic;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using AccountEx.DbMapping;

namespace AccountEx.Web.Controllers.api
{
    public class ManageController : BaseApiController
    {
        // GET api/test
        public JQueryResponse Get()
        {
            return GetDataTable();
        }
        private ApiResponse GetAttributes()
        {
            var type = Request.GetQueryString("type");
            var accountType = string.IsNullOrWhiteSpace(type) ? null : new AccountTypeRepository().GetByName(type);
            if (accountType == null) return null;
            var attributes = new AttributeRepository().GetByAccountTypeId(accountType.Id);
            return new ApiResponse
            {
                Success = true,
                Data = new
                {
                    AccountType = accountType,
                    Attributes = attributes,
                }
            };
        }



        // GET api/test/5
        public ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                var key = Request.GetQueryString("key");
                try
                {
                    switch (key)
                    {
                        case "GetAttributes":
                            response = GetAttributes();
                            break;
                        default:
                            response = new ApiResponse { Success = false, Error = "Type not found" };
                            break;
                           
                    }

                }
                catch (Exception ex)
                {
                     response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
                }
                return response;
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        // POST api/test
        public ApiResponse Post([FromBody]LeafAccountExtra input)
        {
            ApiResponse response;
            try
            {

                new AccountRepository().SaveLeaf(input);
                response = new ApiResponse { Success = true, Data = input.Id };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }

            return response;

        }



        // DELETE api/test/5
        public ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                new AccountRepository().Delete(id);
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
            var coloumns = new[] { "Id", "Name", "NoOfQuestions", "TotalTime", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var isClientside = Numerics.GetBool(queryString["IsClientSide"]);
            var dal = new AccountRepository();
            var halldata = new AccountRepository().AsQueryable().Select(p => new { p.Name, p.Id }).ToList();
            var records = dal.AsQueryable();

            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p => p.Name.Contains(search));
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
                //var data = new List<string>();
                //data.Add(item.Id + "");
                //data.Add(item.Name);
                //var subcats = "";
                //foreach (var subcat in item.BranchCategories.ToList())
                //{
                //    subcats += subcat.Category != null ? "<label class='label label-info'>" + subcat.Category.Name + "</label>&nbsp;&nbsp;" : "";
                //}
                //data.Add(subcats);
                //var halls = "";
                //foreach (var hall in item.BranchHalls.ToList())
                //{
                //    var dbhal = halldata.FirstOrDefault(p => p.Id == hall.HallId);
                //    halls += dbhal != null ? "<label class='label label-important'>" + dbhal.Name + "</label>&nbsp;&nbsp;" : "";
                //}
                //data.Add(halls);

                ////data.Add(item.NoOfQuestions + "");
                ////data.Add(item.TotalTime + "");
                //var editIcon = "<i class='icon-edit' onclick=\"Edit(" + item.Id + ")\" data-original-title='Edit' ></i>";
                //var deleteIcon = "<i class='icon-remove' onclick=\"Delete(" + item.Id + ")\" data-original-title='Delete' ></i>";
                //var icons = "<span class='action'>";
                //icons += editIcon;
                //icons += deleteIcon;
                //icons += "</span>";
                //data.Add(icons);
                //rs.aaData.Add(data);
            }
            rs.sEcho = echo;
            rs.iTotalRecords = totalRecords;
            rs.iTotalDisplayRecords = totalDisplayRecords;
            return rs;
        }
    }
}
