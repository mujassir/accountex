using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Web.Http;
using AccountEx.BussinessLogic;
using AccountEx.BussinessLogic.Production;
using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.Production;
using AccountEx.Common;
using AccountEx.Repositories;
using AccountEx.Repositories.Production;
using Elmah.ContentSyndication;

namespace AccountEx.Web.Controllers.api.Transaction
{
    public class ProductionUnitController : BaseApiController
    {
        public JQueryResponse Get()
        {
            return GetDataTable();
        }
        public ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {

                var voucherNumber = id;
                var queryString = Request.RequestUri.ParseQueryString();
                int locationId = 0;
                if (queryString["locationId"] != null)
                {
                    int.TryParse(queryString["locationId"], out locationId);
                }
                var key = queryString["key"].ToLower();
                Object orderinfo = "";
                //var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
                bool next, previous;
                var repo = new ProductionUnitRepository();
                if (voucherNumber == 0) key = "nextvouchernumber";
                if (key == "nextvouchernumber")
                    voucherNumber = repo.GetNextVoucherNumber(locationId);
                var data = repo.GetByVoucherNumber(voucherNumber, key, locationId, out next, out previous);
                
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Order = data,
                        Next = next,
                        Previous = previous,
                        VoucherNumber = voucherNumber,
                    }
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }


        public ApiResponse Get(int productId, bool loadReceipe)
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                var repo = new ProductReceipeRepository();
                var badgeProductRate = 0.0M;
                var receipe = repo.GetByProductId(productId);
                if (receipe != null)
                {
                    var items = receipe.ProductReceipeitems.Select(p => p.ItemId).Distinct().ToList();
                    items.Add(productId);
                    var balances = new TransactionRepository(repo).GetStockAvgRates(items, DateTime.Now);
                    foreach (var item in receipe.ProductReceipeitems)
                    {
                        var avgRate = balances.FirstOrDefault(p => p.ItemId == item.ItemId);
                        if (avgRate != null)
                        {
                            item.Rate = avgRate.Rate;
                            item.Amount = item.Quantity * item.Rate;
                        }

                    }
                    var avgRateObj = balances.FirstOrDefault(p => p.ItemId == productId);
                    if (avgRateObj != null)
                    {
                        badgeProductRate = avgRateObj.Rate;
                    }
                }

                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Receipe = receipe,
                        BadgeProductRate = badgeProductRate

                    }
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        public ApiResponse Post([FromBody] ProductionUnit input)
        {
            ApiResponse response;
            try
            {
                var err = ServerValidateSave(input);
                if (err == "")
                {
                    input.FiscalId = SiteContext.Current.Fiscal.Id;
                    ProductionUnitManager.Save(input);
                    response = new ApiResponse { Success = true, Data = input.Id };
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
        protected JQueryResponse GetDataTable()
        {
            var coloumns = new[] { "VoucherNumber", "Date", "Status", "" };
            var queryString = Request.RequestUri.ParseQueryString();
            var echo = Convert.ToInt32(queryString["sEcho"]);

            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var type = queryString["type"];
            var search = (queryString["sSearch"] + "").Trim();
            var branchId = Numerics.GetInt((queryString["branchId"]));
            var customerid = Numerics.GetInt(queryString["FilterCustomer"] + "");
            var fromdate = DateConverter.ConvertStandardDate(queryString["FromDate"] + "");
            var todate = DateConverter.ConvertStandardDate(queryString["ToDate"] + "");
            var records = new ProductionUnitRepository().AsQueryable(true);

            if (!string.IsNullOrEmpty(queryString["FromDate"] + "") && !string.IsNullOrEmpty(queryString["ToDate"] + ""))
                records = records.Where(p => p.Date >= fromdate && p.Date <= todate);

            var totalRecords = records.Count();
            var totalDisplayRecords = totalRecords;

            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                    p.VoucherNumber.ToString().Contains(search)
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

            var rs = new JQueryResponse();
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                var data = new List<string>();

                data.Add(item.VoucherNumber + "");
                data.Add(item.Date.ToString(AppSetting.GridDateFormat));
                data.Add(GetType(item.Status));
                var editIcon = "<i class='fa fa-edit' onclick=\"Production.Edit(" + item.VoucherNumber + ")\" title='Edit' ></i>";
                //var deleteIcon = "<i class='fa fa-trash-o' onclick=\"VoucherTrans.Delete(" + item.Id + ")\" title='Delete' ></i>";
                var icons = "<span class='action'>";
                icons += editIcon;
                //icons += deleteIcon;
                icons += "</span>";
                data.Add(icons);
                rs.aaData.Add(data);
            }
            rs.sEcho = echo;
            rs.iTotalRecords = totalRecords;
            rs.iTotalDisplayRecords = totalDisplayRecords;
            return rs;
        }

        public ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                var err = ServerValidateDelete(id);
                if (err == "")
                {
                    var itemIds = new ProductionUnitItemRepository().GetAll(x => x.ProductionUnitId == id);
                    new ProductionUnitItemRepository().Delete(itemIds.Select(x => x.Id).ToList());
                    new ProductionUnitRepository().Delete(id);
                    response = new ApiResponse { Success = true };
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

        private string ServerValidateSave(ProductionUnit input)
        {
            var err = ",";
            try
            {
                if (SiteContext.Current.Fiscal == null)
                {
                    err += "No Fiscal year found.,";
                }
                if (SiteContext.Current.Fiscal.IsClosed)
                {
                    err += "Fiscal year is closed No action can be done.,";
                }
                foreach (var item in input.Items.Where(p => p.ItemId == 0))
                {
                    err += item.ItemCode + "-" + item.ItemName + " is no valid.,";
                }
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
                var repo = new ProductionUnitRepository();

                var WIP = repo.GetById(id);
                if (WIP == null)
                {
                    err += "invalid voucher id. or no record exist.";
                }
            }
            catch (Exception)
            {
                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;
        }
        
        private string GetType(ProductionStatus status)
        {
            switch(status) {
                case ProductionStatus.None:
                    return "None";
                case ProductionStatus.StockRequested:
                    return "Stock Requested";
                case ProductionStatus.StockIssued:
                    return "Stock Issued";
                case ProductionStatus.StockReceived:
                    return "Stock Received";
                case ProductionStatus.ProductionStarted:
                    return "In Process";
                case ProductionStatus.ProductionCompleted:
                    return "Completed";
                default:
                    return "";
            }
        }
    }
}
