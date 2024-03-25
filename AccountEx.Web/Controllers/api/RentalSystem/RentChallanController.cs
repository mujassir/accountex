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
using AccountEx.CodeFirst.Models.COA;
using System.Globalization;


namespace AccountEx.Web.Controllers.api.Transaction
{
    public class RentChallanController : BaseApiController
    {

        //This method is user to print Rent Challan(Call is genrated from Rent Challan Page)
        public ApiResponse Get(int month, int year, int toMonth, int toYear, int accountid, int shopId, int id, bool loadAllPrevious)
        {
            ApiResponse response;
            try
            {
                var data = new RentMonthlyLiabilityRepository().GetByMonthYearTenant(month, year, toMonth, toYear, accountid,id, loadAllPrevious);
                foreach (var item in data.AllChallans)
                {
                    item.Duration = UtilityFunctionManager.GetChallanPeriod(item.Month, item.Year, item.ToMonth, item.ToYear);
                }
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        data,
                    }

                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }


        //This method is user to print Rent Challan(Call is genrated from Rent Challan Page)
        public ApiResponse Get(int Id, bool printChallan)
        {
            ApiResponse response;
            try
            {
                var data = new RentMonthlyLiabilityRepository().PrintRentChallan(Id);
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        data,
                    }

                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        //This method is user to create Rent Challan(Call is genrated from Rent Challan Page)
        public ApiResponse Post([FromBody]Challan input, bool createChallan)
        {
            ApiResponse response;
            try
            {
                input.FiscalId = SiteContext.Current.Fiscal.Id;
                var fromDate = new DateTime(input.Year, input.Month, 1);
                int days = DateTime.DaysInMonth(input.ToYear, input.ToMonth);
                var toDate = new DateTime(input.ToYear, input.ToMonth, days);
                input.FromDate = fromDate;
                input.ToDate = toDate;
                var err = ValidateCreateChallan(input);
                if (err == "")
                {


                    ChallanManager.CreateRentChallan(input);
                    response = new ApiResponse
                    {
                        Success = true,
                        Data = input
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

        //protected override JQueryResponse GetDataTable()
        //{
        //    var queryString = Request.RequestUri.ParseQueryString();
        //    var coloumns = new[] { "VoucherNumber", "DueDate", "DueDate", "NetAmount", "IsReceived", "" };
        //    var echo = Convert.ToInt32(queryString["sEcho"]);
        //    var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
        //    var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
        //    var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
        //    var search = (queryString["sSearch"] + "").Trim();
        //    var type = (queryString["type"] + "").Trim();
        //    var records = Repository.AsQueryable();
        //    var totalRecords = records.Count();
        //    var totalDisplayRecords = totalRecords;
        //    var filteredList = records;
        //    if (search != "")
        //        filteredList = records.Where(p =>
        //             p.Name.Contains(search)
        //           );

        //    var orderedList = filteredList.OrderByDescending(p => p.Id);
        //    if (colIndex < coloumns.Length && coloumns[colIndex] + "" != "")
        //    {
        //        var sortDir = queryString["sSortDir_0"];
        //        orderedList = sortDir == "asc" ? filteredList.OrderBy(coloumns[colIndex]) :
        //            filteredList.OrderByDescending(coloumns[colIndex]);
        //    }
        //    var sb = new StringBuilder();
        //    sb.Clear();

        //    var sr = 0;
        //    var rs = new JQueryResponse();
        //    foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
        //    {
        //        var data = new List<string>();
        //        //data.Add("<td><input type='checkbox' class='checkboxes' value='1' /></td>");
        //        data.Add(item.Name);
        //        var editIcon = "<i class='fa fa-edit' onclick=\"Ships.Edit(" + item.Id + ")\" title='Edit' ></i>";
        //        var deleteIcon = "<i class='fa fa-trash-o' onclick=\"Ships.Delete(" + item.Id + ")\" title='Delete' ></i>";
        //        var icons = "<span class='action'>";
        //        icons += editIcon;
        //        icons += deleteIcon;
        //        icons += "</span>";
        //        if (type != "report") data.Add(icons);
        //        rs.aaData.Add(data);
        //    }
        //    rs.sEcho = echo;
        //    rs.iTotalRecords = totalRecords;
        //    rs.iTotalDisplayRecords = totalDisplayRecords;
        //    return rs;
        //}

        public ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                var err = ServerValidateDelete(id);
                if (err == "")
                {
                    RentMonthlyLiabilityManager.DeleteRent(id);
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


        private string ServerValidateDelete(int id)
        {
            var err = ",";
            try
            {
                var challanRepo = new ChallanRepository();
                var challanItemRepo = new ChallanItemRepository(challanRepo);
                var rent = challanRepo.GetById(id);
                var lpChallan = new ChallanRepository().GetLastPeriodChallan(Numerics.GetInt(rent.RentAgreementId), VoucherType.RC);
                if (lpChallan != null && (lpChallan.FromDate != rent.FromDate || lpChallan.ToDate != rent.ToDate))
                {
                    var lastPeriod = UtilityFunctionManager.GetChallanPeriod(lpChallan);
                    err += "Challan No. " + rent.Id + " should be last period (" + lastPeriod + ") challan to delete.,";

                }
                var isReceived = new ChallanRepository().IsChallanReceived(id);
                if (isReceived)
                {
                    err += "Challan is received and can't be deleted.,";
                }
            }
            catch (Exception)
            {
                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;
        }


        public static string ValidateCreateChallan(Challan input)
        {
            var err = ",";
            try
            {


                if (SiteContext.Current.Fiscal == null)
                {
                    err += "No fiscal year found.,";

                }
                if (SiteContext.Current.Fiscal.IsClosed)
                {
                    err += "Fiscal year is closed No action can be done.,";
                }
                var item = input.ChallanItems.FirstOrDefault();
                if (item.TenantAccountId == 0)
                {
                    err += "Tenant account is not valid.,";
                }
                if (item.RentAgreementId == 0)
                {
                    err += "agreement is not valid.,";
                }


                if (!input.IsOpening)
                {
                    var data = new RentMonthlyLiabilityRepository().GetByMonthYearTenant(input.Month, input.Year, input.ToMonth, input.ToYear, Numerics.GetInt(input.TenantAccountId), Numerics.GetInt(input.RentAgreementId));

                    var alreadyChallanTotal = data.AllChallans.Sum(p => p.TotalAmount);
                    var currentTotal = input.NetAmount;
                    var rd = data.RentDetail;
                    var netBilling = rd.MonthlyRent + rd.RentArrears + rd.UCPercent + rd.UCPercentArears + rd.SurCharge;
                    if (alreadyChallanTotal + currentTotal > netBilling)
                    {
                        err += "total challan amount can't exceed net billing.,";
                    }
                }
                else if (new ChallanRepository().IsOpeningChallanExist(Numerics.GetInt(input.RentAgreementId), VoucherType.RC))
                {
                    err += "opening balance challan has already been added for this tenanat.,";

                }
                var lastChallan = new ChallanRepository().GetChallan(input.FromDate.Value, input.ToDate.Value, Numerics.GetInt(input.RentAgreementId));
                if (lastChallan != null && (lastChallan.FromDate != input.FromDate || lastChallan.ToDate != input.ToDate))
                {
                    var lastPeriod = UtilityFunctionManager.GetChallanPeriod(lastChallan);
                    err += "Challan is already created for this period.please use the last perdiod to create new challan( " + lastPeriod + " ).";
                }
                if (string.IsNullOrWhiteSpace(err.Trim(',')) && lastChallan == null)
                {
                    var lpChallan = new ChallanRepository().GetLastPeriodChallan(input.FromDate.Value, Numerics.GetInt(input.RentAgreementId), VoucherType.RC);
                    if (lpChallan != null)
                    {
                        var data = new RentMonthlyLiabilityRepository().GetByMonthYearTenant(lpChallan.Month, lpChallan.Year, lpChallan.ToMonth, lpChallan.ToYear, Numerics.GetInt(lpChallan.TenantAccountId), Numerics.GetInt(lpChallan.RentAgreementId));

                        var alreadyChallanTotal = data.AllChallans.Sum(p => p.TotalAmount);
                        var rd = data.RentDetail;
                        var netBilling = rd.MonthlyRent + rd.RentArrears + rd.UCPercent + rd.UCPercentArears + rd.SurCharge;
                        if (alreadyChallanTotal < netBilling)
                        {

                            var lastPeriod = UtilityFunctionManager.GetChallanPeriod(lpChallan);
                            var balance = netBilling - alreadyChallanTotal;
                            err += "last period (" + lastPeriod + ") have pending challan of amount " + Numerics.GetInt(balance) + ".please create previous period challan.,";
                        }
                        if (lpChallan.ToDate.Value.AddDays(1) != input.FromDate)
                        {

                            var month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(lpChallan.ToDate.Value.AddDays(1).Month);
                            err += "challan is missing for the month of " + month + " " + lpChallan.ToDate.Value.AddDays(1).Year + ".please create previous month challan.,";
                        }
                    }
                }

            }
            catch (Exception)
            {

                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;


        }
        public static string ValidateSave(RentDetail input)
        {
            var err = ",";
            try
            {


                if (SiteContext.Current.Fiscal == null)
                {
                    err += "No fiscal year found.,";

                }
                if (SiteContext.Current.Fiscal.IsClosed)
                {
                    err += "Fiscal year is closed No action can be done.,";
                }
                var record = new RentMonthlyLiabilityRepository().IsVoucherNumberExist(input.Id, input.VoucherNumber, VoucherType.RentMonthlyLiability);
                if (record)
                {
                    err += "Voucher no already exist.,";
                }

                foreach (var item in input.RentDetailItems.Where(p => p.TenantAccountId == 0))
                {
                    err += item.TenantAccountName + " is not valid.,";
                }

                var tenantList = input.RentDetailItems.GroupBy(p => new { p.TenantAccountId, p.ShopId }).Select(p => new
                {
                    TenantAccountId = p.Key.TenantAccountId,
                    TenantAccountName = p.FirstOrDefault().TenantAccountName,
                    ShopNo = p.FirstOrDefault().ShopNo,
                    Count = p.Count()
                }).Where(p => p.Count > 1).ToList();

                foreach (var tenant in tenantList)
                {
                    err += tenant.TenantAccountName + " must be added once against shop no." + tenant.ShopNo + ".(Current Count:" + tenant.Count + "),";
                }


            }
            catch (Exception)
            {

                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;


        }

        private void CreateBarCode(string cahllanNo)
        {

            // mrno = mrno.Replace("/", "");
            cahllanNo = cahllanNo.Trim();
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/Barcodes/") + cahllanNo + ".gif";

            // mrno = "*" + mrno + "_" + mrno + "*";
            cahllanNo = "*" + cahllanNo + "*";

            System.IO.File.Delete(path);
            // ClearDirectoryB();
            var b = new BarcodeLib.Barcode();
            int W = 300;
            int H = 20;
            b.Alignment = BarcodeLib.AlignmentPositions.CENTER;
            var type = BarcodeLib.TYPE.CODE39;
            try
            {
                b.RotateFlipType = System.Drawing.RotateFlipType.RotateNoneFlipNone;
                b.LabelPosition = BarcodeLib.LabelPositions.BOTTOMCENTER;
                System.Drawing.Image img = b.Encode(type, cahllanNo + "", System.Drawing.Color.Black, System.Drawing.Color.White, W, H);
                img.Save(path);
            }
            catch (Exception ex)
            {

                var data = ex.Data.ToString();
            }
        }
    }
}
