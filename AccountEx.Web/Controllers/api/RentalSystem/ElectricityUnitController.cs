using AccountEx.BussinessLogic;
using AccountEx.CodeFirst.Models.COA;
using AccountEx.CodeFirst.Models.Views;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.Repositories.COA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AccountEx.Web.Controllers.api.Manage
{
    public class ElectricityUnitController : ApiController
    {
        public ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                var tenantsAgreemnet = new List<vw_TenantAgreements>();
                var repo = new ElectricityUnitRepository();

                var queryString = Request.RequestUri.ParseQueryString();
                var month = Numerics.GetInt(queryString["month"].ToLower());
                var year = Numerics.GetInt(queryString["year"].ToLower());
                var voucherNumber = Numerics.GetInt(queryString["voucherno"].ToLower());
                var key = queryString["key"].ToLower();
                bool next, previous;
                var voucherNo = new ElectricityUnitRepository().GetNextVoucherNo();
                //var blockId = Numerics.GetInt(queryString["blockId"].ToLower());
                if (month == 0 && year == 0)
                {
                    month = System.DateTime.Now.Month;
                    year = System.DateTime.Now.Year;
                }
                var electricityUnit1 = repo.GetRecordByMonthYear(month, year);
                var previousReading = new ElectricityUnit();





                var electricityUnit = new ElectricityUnit();

                if (key == "bymonthyear")
                {
                    voucherNumber = repo.GetVoucherNoByMonthYear(month, year);
                    key = "same";
                    if (voucherNumber == 0)
                        voucherNumber = repo.GetNextVoucherNo();
                }
                else
                {

                    if (key == "nextvouchernumber")
                        voucherNumber = repo.GetNextVoucherNo();

                }
                electricityUnit = repo.GetByVoucherNo(voucherNumber, key, out next, out previous);
                if (electricityUnit != null)
                {
                    month = electricityUnit.Month;
                    year = electricityUnit.Year;
                }
                if (electricityUnit == null)
                {

                    previousReading = repo.GetPreviousReading(month, year);
                }

                tenantsAgreemnet = new vw_TenantAgreemntRepository().GetAll(p => p.Status != (byte)AgreementStatus.Transfeer);
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        VoucherNo = electricityUnit != null ? electricityUnit.VoucherNo : voucherNumber,
                        ElectricityUnit = electricityUnit,
                        PreviousReading = previousReading,
                        TenantsAgreemnet = tenantsAgreemnet,
                        Next = next,
                        Previous = previous,
                    }
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }

        public ApiResponse Post([FromBody]ElectricityUnit input)
        {
            ApiResponse response;
            try
            {
                var err = ServerValidateSave(input);
                if (err != "")
                {
                    response = new ApiResponse()
                    {
                        Success = false,
                        Error = err
                    };
                }
                else
                {
                    var repo = new ElectricityUnitRepository();
                    var repoElectricityUnit = new ElectricityUnitItemRepository(repo);

                    input.FiscalId = SiteContext.Current.Fiscal.Id;
                    if (input.Id == 0)
                        repo.Save(input, true, true);
                    else
                    {
                        var item = input.ElectricityUnitItems.CloneWithJson().ToList();
                        input.ElectricityUnitItems = null;
                        repo.Save(input, true, false);

                        repoElectricityUnit.Save(item);
                        repo.SaveChanges();
                    }


                    response = new ApiResponse
                    {
                        Success = true,
                        Data = new
                        {
                            VoucherNo = repo.GetNextVoucherNo(),
                            //Order = input,
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }

        private string ServerValidateSave(ElectricityUnit input)
        {
            var err = "";
            var record = new ElectricityUnitRepository().CheckIfVoucherNoExist(input.VoucherNo, input.Id);
            if (record == true)
            {
                err += "<li>Voucher no already exists.</li>";
            }
            var isExist = new ElectricityUnitRepository().CheckIfElectrictyExist(input.Id, input.Year, input.Month);
            if (isExist)
            {
                err += "<li>Electricity has already been added for selected month & year.</li>";
            }
            return err;

        }

        public ApiResponse Get()
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                var key = queryString["key"].ToLower();
                var month = Numerics.GetInt(queryString["month"].ToLower());
                var year = Numerics.GetInt(queryString["year"].ToLower());
                //var voucherno = Numerics.GetInt(queryString["voucherno"].ToLower());
                //var tenantsdata = new ElectricityUnitRepository().GetRecordByYear(month, year, voucherno, out next, out previous);
                var tenantsdata = new ElectricityUnitRepository().GetRecordByMonthYear(month, year);


                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        TenantsData = tenantsdata,
                    }
                };
            }
            catch (Exception ex)
            {
                ;
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        public ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                new ElectricityUnitRepository().DeleteByVoucherNo(id);
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
    }
}



