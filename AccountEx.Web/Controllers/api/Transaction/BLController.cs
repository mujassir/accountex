using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using AccountEx.BussinessLogic;
using AccountEx.Common;
using AccountEx.Repositories;
using Newtonsoft.Json;
using System.Linq;
using AccountEx.CodeFirst.Models;
using System.Text;
using AccountEx.CodeFirst.Models.Transactions;
using Entities.CodeFirst;
using AccountEx.CodeFirst.Models.Views;
using AccountEx.CodeFirst.Models.Vehicles;
using AccountEx.BussinessLogic.Vehicles;



namespace AccountEx.Web.Controllers.api.Transaction
{
    public class BLController : BaseApiController
    {
        public JQueryResponse Get()
        {


            return GetDataTable();
        }

        public ApiResponse Get(string key, int shipperId)
        {
            ApiResponse response;
            try
            {
                var ships = new ShipRepository().GetByShipperId(shipperId);

                response = new ApiResponse
                {
                    Success = true,
                    Data = ships
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
                var voucherNumber = Numerics.GetInt(id);
                var currency = new object();
                var queryString = Request.RequestUri.ParseQueryString();
                var type = queryString["type"];
                var key = queryString["key"].ToLower();
                //var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
                bool next, previous;
                var blRepo = new BLRepository();
                if (voucherNumber == 0) key = "nextvouchernumber";
                if (key == "nextvouchernumber")
                    voucherNumber = blRepo.GetNextVoucherNumber(voucherNumber);
                var data = blRepo.GetByVoucherNumber(voucherNumber, key, out next, out previous);
                var vehicles = new List<vw_Vehicles>();
                if (data != null && data.BLItems != null)
                {
                    var vehicleIds = data.BLItems.Select(p => p.VehicleId).ToList();

                    vehicles = new VehicleRepository().GetAll(vehicleIds);
                }
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Order = data,
                        Vehicles = vehicles,
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
        public ApiResponse Post([FromBody]BL input)
        {
            ApiResponse response;
            response = BLManager.Save(input);
            return response;

        }
        public ApiResponse Post([FromBody]BL Bl, bool isfinal)
        {
            ApiResponse response;
            try
            {


                BLManager.FinalUnfinal(Bl, isfinal);

                response = new ApiResponse
                {
                    Success = true,
                    Data = Bl.Id
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }


            return response;

        }
        public ApiResponse Delete(int id)
        {
            var response = BLManager.Delete(id);
            return response;
        }
        public ApiResponse Delete(int id, string key)
        {

            var response = BLManager.DeleteForSingleBLItem(id);
            return response;
        }



        protected JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "BLNumber", "ShipName", "DepartureDate", "ArrivalDate", "AddedUnits", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var Intsearch = Numerics.GetInt((queryString["sSearch"] + "").Trim());
            var type = Convert.ToByte((queryString["type"]));
            //var dal = new ProjectRepository();
            var records = new GenericRepository<vw_BLS>().AsQueryable();

            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                     p.BLNumber.Contains(search) ||
                      (Intsearch > 0 && p.AddedUnits == Intsearch) ||
                     p.ShipName.Contains(search)

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
                data.Add(item.BLNumber + "");
                data.Add(item.ShipName + "");
                data.Add(item.DepartureDate.HasValue ? item.DepartureDate.Value.ToString(AppSetting.GridDateFormat) : "");
                data.Add(item.ArrivalDate.HasValue ? item.ArrivalDate.Value.ToString(AppSetting.GridDateFormat) : "");
                data.Add(item.AddedUnits + "");

                var editIcon = "<i class='fa fa-edit' onclick=\"BLs.Edit(" + item.Id + "," + item.VoucherNumber + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"BLs.Delete(" + item.Id + "," + item.VoucherNumber + ")\" title='Delete' ></i>";
                var markFinalIcon = "<a target='_blank' href='javascript:void(0)' onclick=\"BLs.MarkFinal(" + item.Id + ")\"' class='btn default btn-xs green-stripe'>Mark Final</a>";
                var finaledLabel = "<a target='_blank' href='javascript:void(0)' class='btn default btn-xs green-stripe'>Mark Final</a>";
                var icons = "<span class='action'>";
                icons += editIcon;

                if (!item.IsFinal)
                {

                    icons += deleteIcon;
                    //icons += markFinalIcon;
                }
                //else
                //    icons += finaledLabel;
                icons += "</span>";
                data.Add(icons);
                rs.aaData.Add(data);
            }
            rs.sEcho = echo;
            rs.iTotalRecords = totalRecords;
            rs.iTotalDisplayRecords = totalDisplayRecords;
            return rs;
        }

    }
}
