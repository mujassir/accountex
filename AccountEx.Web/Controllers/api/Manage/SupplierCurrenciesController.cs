using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Web.Http;
using System.Net.Http;
using System.Text;
using AccountEx.Common;
using System.Web;
using System.Web.Mvc;
using Entities.CodeFirst;
using AccountEx.Repositories;
using AccountEx.Web.Controllers.api.Shared;
using AccountEx.CodeFirst.Models.Vehicles;
using AccountEx.BussinessLogic;
using AccountEx.BussinessLogic.Vehicles;
using AccountEx.CodeFirst;
using AccountEx.CodeFirst.Models;


namespace AccountEx.Web.Controllers.api.Manage
{
    public class SupplierCurrenciesController : GenericApiController<SupplierCurrency>
    {
        //
        public ApiResponse Get(string key, bool loadInfo)
        {
            ApiResponse response;
            try
            {

                //if (key == "getNextFileNo")
                //    response = GetNextFileNo();
                //else if (key == "loadPurchaseReturnInfo")
                //    response = GetPurchaseReturnInfo();
                //else
                //    response = GetNextFileNo();

                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        public override ApiResponse Post([FromBody]SupplierCurrency input)
        {
            ApiResponse response;
            try
            {
                var err = "";
                err = ValidateSave(input);
                if (err == "")
                {
                    SupplierCurrencyManager.Save(input);
                    response = new ApiResponse { Success = true };
                }
                else
                {
                    response = new ApiResponse { Success = false, Error = err };
                }
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        private string ValidateSave(SupplierCurrency input)
        {
            string err = "";
            //if (new VehicleRepository().IfChasisNumberExists(input.ChassisNo, input.Id))
            //{
            //    err += "Chassis No already exists.,";
            //}
            //if (input.Type != (byte)VehicleType.New)
            //{
            //    if (input.VendorId == null || input.VendorId == 0)
            //    {
            //        err += "Vendor is required.,";
            //    }
            //    if (input.Type != (byte)VehicleType.TradIn)
            //    {
            //        if (SettingManager.TradeInAcccountId == 0)
            //        {
            //            err += "Trade In account is missing.,";
            //        }
            //    }
            //    if (input.Type != (byte)VehicleType.LocalPurchase)
            //    {
            //        if (SettingManager.PurchaseAccountHeadId == 0)
            //        {
            //            err += "Purchase account is missing.,";
            //        }
            //  }
            // }
            return err;
        }

        //
        protected override JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "Currency", "Supplier", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var intsSearch = Numerics.GetInt(search);
            var type = (queryString["type"] + "").Trim();
            //var dal = new ProjectRepository();
            var repo = new GenericRepository<vw_SupplierCurrency>();


            var supplierId = Numerics.GetInt(queryString["supplierId"] + "");
            var records = repo.AsQueryable();

            //if (branchId > 0)
            //    records = records.Where(p => p.BranchId == branchId);

        
            if (supplierId>0)
                records = records.Where(p => p.AccountId==supplierId);

            var userBranchRepo = new UserVehicleBranchesRepository(repo);
            var userBranches = userBranchRepo.GetBranchIdsByUserIds(SiteContext.Current.User.Id);
            var totalRecords = records.Count();
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>

                     p.Currency.Contains(search) ||
                        p.Supplier.Contains(search)
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

            var sr = 0;
            var rs = new JQueryResponse();
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                var data = new List<string>();
                data.Add(item.Supplier);
                data.Add(item.Currency);
                var editIcon = "<i class='fa fa-edit' onclick=\"SupplierCurrencies.Edit(" + item.Id + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"SupplierCurrencies.Delete(" + item.Id + ")\" title='Delete' ></i>";
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


        public override ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                var err = "";
                err = ValidateDelete(id);
                if (err == "")
                {

                    CurrencyRateManager.Delete(id);
                    response = new ApiResponse { Success = true };
                }
                else
                {
                    response = new ApiResponse { Success = false, Error = err };
                }
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }

            return response;
        }

        private string ValidateDelete(int id)
        {
            string err = "";
            if (new VehicleRepository().IfSold(id))
            {
                err += "Chassis is already sold and can't be deleted.,";
            }
            if (new VehicleSaleRepository().IsTradeUnitUsed(id))
            {
                err += "Chassis is used in sale can't be deleted.,";
            }

            return err;
        }

    }
}
