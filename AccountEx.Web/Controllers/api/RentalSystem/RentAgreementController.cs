using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.Web.Controllers.api.Shared;
using System.Web.Http;
using System.Web;
using AccountEx.CodeFirst.Models.COA;
using AccountEx.BussinessLogic;
using Newtonsoft.Json;
using Repositories.COA;
using AccountEx.CodeFirst.Models.Views;
using AccountEx.DbMapping;

namespace AccountEx.Web.Controllers.api.Manage
{
    public class RentAgreementController : BaseApiController
    {
        public JQueryResponse Get()
        {
            return GetDataTable();
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

        public ApiResponse Get(string key)
        {
            var response = new ApiResponse();
            switch (key)
            {
                case "GetAllShops":
                    response = GetShops();
                    break;
                case "GetSettings":
                    response = GetSettings();
                    break;
                case "GetRentAgreementsWithTenants":
                    response = GetRentAgreementsWithTenants();
                    break;
                case "GetTransferInfo":
                    response = GetTransferInfo();
                    break;
            }

            return response;
        }

        public ApiResponse Post([FromBody]RentAgreementExtra input)
        {
            ApiResponse response;
            try
            {
                var err = ServerValidateSave(input.RentAgreements);
                if (err == "")
                {
                    if (input.RentAgreements.Id == 0)
                    {
                        input.RentAgreements.FiscalId = SiteContext.Current.Fiscal.Id;
                        RentAgreementManager.Save(input.RentAgreements);
                    }
                    else
                    {
                        RentAgreementManager.Update(input);

                    }

                    // input.Challans.FiscalId = SiteContext.Current.Fiscal.Id;
                    //foreach (var item in input.Challans.ChallanItems)
                    //{
                    //    item.RentAgreementId = input.RentAgreements.Id;
                    //}
                    //new ChallanRepository().Save(input.Challans);
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

        public ApiResponse Post([FromBody]RentAgreement input, bool transfeer)
        {
            ApiResponse response;
            try
            {

                input.FiscalId = SiteContext.Current.Fiscal.Id;
                response = RentAgreementManager.Transfeer(input);

            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
        public ApiResponse Delete(int id, int voucherNumber)
        {
            ApiResponse response;
            try
            {
                var err = ServerValidateDelete(id);
                if (err == "")
                {
                    RentAgreementManager.Delete(id, voucherNumber);
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

        private ApiResponse GetRentAgreementsWithTenants()
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                var includeTransfeered = Numerics.GetBool(queryString["includeTransfeered"]);

                var data = new List<vw_GetRentAgreementsWithTenants>();
                if (includeTransfeered)
                    data = new vw_GetRentAgreementsWithTenantsRepository().GetAll();
                else
                    data = new vw_GetRentAgreementsWithTenantsRepository().GetAll(p => p.Status != (byte)AgreementStatus.Transfeer);
                response = new ApiResponse { Success = true, Data = data };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;

        }
        private ApiResponse GetTransferInfo()
        {

            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                var id = Numerics.GetInt(queryString["agreementId"]);
                var voucherNumber = id;
                var type = queryString["type"];
                //var key = queryString["key"].ToLower();
                var vouchertype =(VoucherType)Convert.ToByte(type);
                var repo = new RentAgreementRepository();
                var data = repo.GetById(id);
                var Vrecord = new vw_RentAgreements();
                var agreementState = new AgreementEditingState();
                if (data != null)
                {
                    Vrecord = new vw_RentAgreementsRepository().GetById(id);
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
                    }
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;

        }


        private ApiResponse GetShops()
        {
            ApiResponse response;
            try
            {
                var rentAgrRepo = new RentAgreementRepository();
                var shopRepo = new ShopRepository(rentAgrRepo);
                var shopIds = rentAgrRepo.AsQueryable(true).Select(p => p.ShopId).ToList();
                var data = shopRepo.AsQueryable().Where(p => !shopIds.Contains(p.Id) && p.IsActive).ToList();
                response = new ApiResponse
                {
                    Success = true,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;

        }

        private ApiResponse GetSettings()
        {
            ApiResponse response;
            try
            {
                var setting = new List<SettingExtra>();
                setting.Add(new SettingExtra { Key = "RentPerSqft", Value = SettingManager.RentPerSqft });
                setting.Add(new SettingExtra { Key = "UCPercent", Value = SettingManager.UCPercent });
                setting.Add(new SettingExtra { Key = "PossessionChargesPerSqft", Value = SettingManager.PossessionChargesPerSqft });
                setting.Add(new SettingExtra { Key = "SecurityMoneyMonths", Value = SettingManager.SecurityMoneyMonths });
                var data = JsonConvert.SerializeObject(setting);
                response = new ApiResponse
                {
                    Success = true,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;

        }

        private string ServerValidateSave(RentAgreement input)
        {
            var err = ",";
            try
            {
                if (input.Id > 0)
                {
                    if (new ChallanRepository().CheckIfRentLiabilityGenerated(input.Id))
                    {
                        err += "This agreement is attached with rent so it cannot be updated.,";
                    }
                }
                if (SiteContext.Current.Fiscal == null)
                {
                    err += "No fiscal year found.,";
                }
                if (SiteContext.Current.Fiscal.IsClosed)
                {
                    err += "Fiscal year is closed No action can be done.,";
                }
                var record = new RentAgreementRepository().GetByVoucherNumber(input.VoucherNumber, input.Id);
                if (record != null)
                {
                    err += "Voucher no already exist.,";
                }
                if (SettingManager.PossessionChargesId == 0)
                {
                    err += "Possession Charge account is missing.,";
                }
                if (SettingManager.SecurityMoneyId == 0)
                {
                    err += "Security money account is missing.,";
                }
            }
            catch (Exception)
            {

                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;
        }

        protected JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "ShopCode", "ShopNo", "Block", "TotalArea", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var type = (queryString["type"] + "").Trim();
            //var dal = new ShopRepository();
            // var records = dal.AsQueryable();
            var records = new vw_RentAgreementsRepository().AsQueryable(true).Where(p => p.Status != (byte)AgreementStatus.Transfeer);
            var totalRecords = records.Count();
            
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                    p.TenantName.Contains(search)
                    || p.ShopNo.Contains(search)
                    || p.Block.Contains(search)
                   );
            var totalDisplayRecords = filteredList.Count();
            var orderedList = filteredList.OrderByDescending(p => p.Id);
            var sb = new StringBuilder();
            sb.Clear();

            var sr = 0;
            var rs = new JQueryResponse();
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                var data = new List<string>();
                //data.Add("<td><input type='checkbox' class='checkboxes' value='1' /></td>");
                //data.Add(item.ShopCode);
                //data.Add(item.ShopNo);

                //var tenant = new AccountDetailRepository().AsQueryable().FirstOrDefault(p => p.AccountId == item.TenantAccountId).Name;
                //var shop = new ShopRepository().AsQueryable().FirstOrDefault(p => p.Id == item.ShopId);
                data.Add(item.TenantName);
                data.Add(item.ShopNo);
                data.Add(item.Block);
                data.Add("");
                data.Add("");
                //data.Add(item.StartDate.HasValue? item.StartDate.Value.ToString("dd/MM/yyyy") : "");
                //data.Add(item.EndDate.HasValue ? item.EndDate.Value.Date.ToString("dd/MM/yyyy") : "");

                var editIcon = "<i class='fa fa-edit' onclick=\"RentAgreement.LoadVoucher('same', " + item.VoucherNumber + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"RentAgreement.Delete(" + item.Id + ", " + item.VoucherNumber + ")\" title='Delete' ></i>";
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

        private string ServerValidateDelete(int id)
        {
            var err = ",";
            try
            {
                var repo = new RentAgreementRepository();
                if (new RentMonthlyLiabilityRepository(repo).CheckIfRentLiabilityGenerated(id))
                {
                    err += "This agreement is attached with rent so it cannot be deleted.,";
                }
                if (new ChallanRepository(repo).CheckIfChallanExist(id))
                {
                    err += "This agreement is attached with challan so it cannot be deleted.,";
                }
                if (repo.CheckIfTransfeeredFromOtherAgreement(id))
                {
                    err += "This agreement is transfeered from other agreement and can't be deleted.,";
                }
                if (repo.CheckIfTransfeeredFromOtherAgreement(id))
                {
                    err += "This agreement is already transfeered can't be deleted.,";
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
