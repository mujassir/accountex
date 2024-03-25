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
using Newtonsoft.Json;
using System.Web.Http;
using Repositories.COA;
using AccountEx.Repositories.COA;

namespace AccountEx.Web.Controllers.api.Manage
{
    public class RentOpeningBalanceController : GenericApiController<RentOpeningBalance>
    {
        protected override JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "Name", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var type = (queryString["type"] + "").Trim();
            //var dal = new ProjectRepository();
            var records = new GenericRepository<vw_RentOpeningBalance>().AsQueryable();
            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>

                     p.TenantAccountName.Contains(search) ||
                      p.ShopNo.Contains(search) ||
                       p.Block.Contains(search)

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
                data.Add(item.TenantAccountName);
                data.Add(item.ShopNo);
                data.Add(item.Block);
                data.Add(Numerics.IntToString(item.Rent));
                data.Add(Numerics.IntToString(item.UC));
                data.Add(Numerics.IntToString(item.Electricity));
                data.Add(Numerics.IntToString(item.SurCharge));
                var editIcon = "<i class='fa fa-edit' onclick=\"RentOpeningBalance.Edit(" + item.Id + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"RentOpeningBalance.Delete(" + item.Id + ")\" title='Delete' ></i>";
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
        public ApiResponse Get(int id, string key)
        {
            ApiResponse response;
            try
            {
                var voucherNumber = id;
                var queryString = Request.RequestUri.ParseQueryString();
                var type = queryString["type"];
                //var key = queryString["key"].ToLower();
                var vouchertype =(VoucherType)Convert.ToByte(type);
                bool next, previous;
                if (voucherNumber == 0) key = "nextvouchernumber";
                var repo = new RentAgreementRepository();
                if (key == "nextvouchernumber")
                    voucherNumber = repo.GetNextVoucherNumber();
                var data = repo.GetByVoucherNumber(voucherNumber, key, out next, out previous);
                if (data != null && data.RentAgreementSchedules != null)
                    data.RentAgreementSchedules = data.RentAgreementSchedules.Where(p => p.IsRenewed == false).ToList();
                var Vrecord = new vw_RentAgreements();
                var agreementState = new AgreementEditingState();
                if (data != null)
                {
                    Vrecord = new vw_RentAgreementsRepository().GetByAccountIdShopId(data.TenantAccountId, data.ShopId);
                    agreementState = repo.GetAgreementEditingState(data.Id);
                }
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Order = data,
                        VRecord = Vrecord,
                        State = agreementState,
                        Next = next,
                        Previous = previous,
                        VoucherNumber = voucherNumber
                    }
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }



        public override ApiResponse Post([FromBody]RentOpeningBalance input)
        {
            ApiResponse response;
            try
            {
                var err = ServerValidateSave(input);
                if (err == "")
                {
                    RentOpeningBalanceManager.Save(input);
                    response = new ApiResponse
                    {
                        Success = true,
                    };
                }
                else
                {
                    response = new ApiResponse
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

        public override ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                var err = ServerValidateDelete(id);
                if (err == "")
                {
                    RentOpeningBalanceManager.Delete(id);
                    response = new ApiResponse
                    {
                        Success = true,
                    };
                }
                else
                {
                    response = new ApiResponse
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





        private string ServerValidateSave(RentOpeningBalance input)
        {
            var err = ",";
            try
            {
                if (new RentOpeningBalanceRepository().IsExistByRentAgreementId(input.RentAgreementId, input.Id))
                {
                    err += "Opening balance has already been entered for this agreement.,";
                }

                if (new RentMonthlyLiabilityRepository().IsExistByRentAgreementId(input.RentAgreementId, input.Month, input.Year, input.RentItemId))
                {
                    err += "Liability already exist for current tenanat against selected month.,";
                }
                //if (!new ElectricityUnitRepository().CheckIfElectrictyExist(input.Year, input.Month))
                //{
                //    err += "Electricity record not entered for current month.,";
                //}
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
                if (new RentMonthlyLiabilityRepository().CheckIfRentLiabilityGenerated(id))
                {
                    err += "This agreement is attached with rent so it cannot be deleted.,";
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
