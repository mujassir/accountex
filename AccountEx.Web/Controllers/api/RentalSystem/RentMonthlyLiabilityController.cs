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


namespace AccountEx.Web.Controllers.api.Transaction
{
    public class RentMonthlyLiabilityController : BaseApiController
    {

        //This method is user to print Rent Challan(Call is genrated from Rent Challan Page)
        public ApiResponse Get(int month, int year, int accountid, int shopId,int id)
        {
            ApiResponse response;
            try
            {
                var shop = new Shop();
                var tenant = new AccountDetail();
                var eCharges = new Object();
                var partner = "";
                //var data = new RentMonthlyLiabilityRepository().GetByMonthYearTenant(month, year, accountid, shopId,id);
                //if (data != null)
                //{
                //    shop = new ShopRepository().AsQueryable().Where(p => p.Id == data.RentDetail.ShopId).FirstOrDefault();
                //    eCharges = new RentMonthlyLiabilityRepository().GetElecCharges(data.RentDetail.ElectricityUnitItemId);
                //    tenant = new AccountDetailRepository().GetByAccountId(data.RentDetail.TenantAccountId);
                //    partner = new TenantPartnerRepository().AsQueryable().FirstOrDefault(p => p.TenantId == accountid).Name;
                //}
                //CreateBarCode(data.Id.ToString());
                //string parCode = System.Web.Hosting.HostingEnvironment.MapPath("~/Barcodes/") + data.Id + ".gif";
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                       
                        ShopData = shop,
                        Echarges = eCharges,
                        Tenant = tenant,
                        Partner = partner,
                    }

                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }
        public ApiResponse Get()
        {
            ApiResponse response;
            try
            {
                var repo = new RentMonthlyLiabilityRepository();
                var challanExtra = new List<ChallanExtra>();
                var queryString = Request.RequestUri.ParseQueryString();
                var voucherNumber = Numerics.GetInt(queryString["voucherno"].ToLower());
                var key = queryString["key"].ToLower();
                bool next, previous;
                var month = Numerics.GetInt(queryString["month"].ToLower());
                var year = Numerics.GetInt(queryString["year"].ToLower());

                var rentDetail = new RentDetail();

                if (key == "bymonthyear")
                {
                    voucherNumber = repo.GetVoucherNoByMonthYear(month, year);
                    key = "same";
                    if (voucherNumber == 0)
                        voucherNumber = repo.GetNextVoucherNumber();
                }
                else
                {

                    if (key == "nextvouchernumber")
                        voucherNumber = repo.GetNextVoucherNumber();

                }
                rentDetail = repo.GetByVoucherNumber(voucherNumber, key, out next, out previous);
                if (rentDetail != null)
                {
                    month = rentDetail.Month;
                    year = rentDetail.Year;
                }
                if (month > 0 && year > 0)
                {
                    challanExtra = repo.GetLiabilityByMonthYear(month, year);
                    var test = challanExtra.FirstOrDefault(p => p.TenantAccountId == 72258 && p.ShopId == 31248);
                }





                response = new ApiResponse
                {
                    Data = new
                    {
                        Challan = rentDetail,
                        ChallanExtra = challanExtra,
                        VoucherNumber = rentDetail != null ? rentDetail.VoucherNumber : voucherNumber,
                        Next = next,
                        Previous = previous,
                    },
                    Success = true,
                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }

        public ApiResponse Post([FromBody]RentDetail input)
        {
            ApiResponse response;
            try
            {
                var err = ValidateSave(input);
                if (err == "")
                {

                    input.FiscalId = SiteContext.Current.Fiscal.Id;
                    RentMonthlyLiabilityManager.Save(input);
                    response = new ApiResponse
                    {
                        Success = true,
                        Data = new { Order = input }
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
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }


        //This method is user to create Rent Challan(Call is genrated from Rent Challan Page)
        public ApiResponse Post([FromBody]Challan input, bool createChallan)
        {
            ApiResponse response;
            try
            {
                var err = ValidateCreateChallan(input);
                if (err == "")
                {

                    input.FiscalId = SiteContext.Current.Fiscal.Id;
                    ChallanManager.CreateRentChallan(input);
                    response = new ApiResponse
                    {
                        Success = true,
                        Data = new { Order = input }
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
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }

        private ApiResponse GetDataTable()
        {
            ApiResponse response;
            try
            {
                var result = "";
                var queryString = Request.RequestUri.ParseQueryString();
                var vouchertype =(VoucherType) Convert.ToByte((queryString["type"]));
                var voucher = Numerics.GetInt((queryString["voucher"]));
                var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
                result = JsonConvert.SerializeObject(data, GetJsonSetting());
                response = new ApiResponse
                {
                    Success = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }
            return response;
        }

        public ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                var err = ServerValidateDelete(id);
                if (err == "")
                {
                    RentMonthlyLiabilityManager.Delete(id);
                    response = new ApiResponse { Success = true };
                }
                else
                {
                    response = new ApiResponse { Success = false, Error = err };
                }

                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
            }

            return response;
        }
        public ApiResponse Delete(int id, string key)
        {
            ApiResponse response;
            try
            {
                var err = ServerValidateDeleteForSingleLiability(id);
                if (err == "")
                {
                    RentMonthlyLiabilityManager.DeleteForSingleLiability(id);
                    response = new ApiResponse { Success = true };
                }
                else
                {
                    response = new ApiResponse { Success = false, Error = err };
                }

            }
            catch (Exception ex)
            {
                 response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };  ;
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
                var monthlyLiability = challanRepo.GetById(id);
                foreach (var item in monthlyLiability.ChallanItems)
                {
                    var isReceived = challanItemRepo.CheckIfChallanReceived(item.Id);
                    if (isReceived)
                    {
                        err += "There are some rent liabilities cannot be deleted because challans of those liabilities has been received.,";
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
        private string ServerValidateDeleteForSingleLiability(int id)
        {
            var err = ",";
            try
            {
                var isReceived = new ChallanItemRepository().CheckIfChallanReceived(id);
                if (isReceived)
                {
                    err += "This challan has already been received.,";
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
                //var record = new RentMonthlyLiabilityRepository().IsVoucherNumberExist(input.Id, input.VoucherNumber, VoucherType.RentMonthlyLiability);
                //if (record)
                //{
                //    err += "Voucher no already exist.,";
                //}

                //foreach (var item in input.RentDetailItems.Where(p => p.TenantAccountId == 0))
                //{
                //    err += item.TenantAccountName + " is not valid.,";
                //}

                //var tenantList = input.RentDetailItems.GroupBy(p => new { p.TenantAccountId, p.ShopId }).Select(p => new
                //{
                //    TenantAccountId = p.Key.TenantAccountId,
                //    TenantAccountName = p.FirstOrDefault().TenantAccountName,
                //    ShopNo = p.FirstOrDefault().ShopNo,
                //    Count = p.Count()
                //}).Where(p => p.Count > 1).ToList();

                //foreach (var tenant in tenantList)
                //{
                //    err += tenant.TenantAccountName + " must be added once against shop no." + tenant.ShopNo + ".(Current Count:" + tenant.Count + "),";
                //}


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
