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
using AccountEx.Repositories.Transactions;
using AccountEx.CodeFirst.Models.Vehicles;

namespace AccountEx.Web.Controllers.api
{
    public class VehicleSaleController : BaseApiController
    {
        public JQueryResponse Get()
        {
            return GetDataTable();
        }
        public virtual ApiResponse Get(int installmentNo, int vehicleSaleId, int vehicleId)
        {
            ApiResponse response;
            try
            {
                var types = new List<VoucherType>() { VoucherType.VCR, VoucherType.VBR };
                var vehicleIds = new List<int>() { vehicleId };
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Installments = new VehicleSaleDetailRepository().GetPendingInstalment(vehicleSaleId),
                        PaidInstallments = new VehiclePaymentRepository().GetPaidInstalments(vehicleSaleId),
                        Vehicle = new VehicleRepository().GetVehicles(vehicleIds).FirstOrDefault(),
                        OtherDetail = new
                        {
                            VoucherNo = new TransactionRepository().GetNextVoucherNumber(types),
                            TotalOutStanding = new VehicleSaleDetailRepository().GetTotalOutStanding(vehicleSaleId),
                            CurrentMonthOutStanding = new VehicleSaleDetailRepository().GetCurrentMonthOutStanding(vehicleSaleId),

                        }
                    }
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        public virtual ApiResponse Get(bool loadExpense, int vehicleId)
        {
            ApiResponse response;
            try
            {

                response = new ApiResponse
                {
                    Success = true,
                    Data = new TransactionRepository().GetVehicleExpenses(vehicleId)

                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        public virtual ApiResponse Get(bool loadTradeVehicle, string tradeVehicle, int customerId)
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                var branchId = Numerics.GetInt((queryString["branchId"]));
                response = new ApiResponse
                {
                    Success = true,
                    Data = new VehicleRepository().GetTradeInVehicles(customerId, branchId)

                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        public ApiResponse Get(bool doPrinting, int id, string printKey)
        {
            ApiResponse response;
            try
            {
                var repo = new VehiclePaymentRepository();
                if (printKey == "CRPrint")
                {

                    var vp = repo.GetById(id);
                    var vs = new vw_VehicleSalesRepository(repo).GetById(vp.VehicleSaleId);
                    response = new ApiResponse
                    {
                        Success = true,
                        Data = new
                        {
                            Payment = vp,
                            Sale = vs

                        }
                    };
                }
                else
                {
                    var data = new vw_VehicleSalesRepository().GetById(id);

                    response = new ApiResponse
                    {
                        Success = true,
                        Data = data
                    };
                }

            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        public ApiResponse Get(bool saleAvailable)
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                var branchId = Numerics.GetInt((queryString["branchId"]));
                var data = new VehicleRepository().GetVehicleAvailbaleForSale(branchId);
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
        public ApiResponse Get(int id, bool byvoucher)
        {
            ApiResponse response;
            try
            {
                var voucherNumber = id;
                var invoiceNumber = id;
                var queryString = Request.RequestUri.ParseQueryString();
                var type = queryString["type"];
                var key = queryString["key"].ToLower();
                var vouchertype =(VoucherType)Convert.ToByte(type);
                //var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
                bool next, previous;
                var vehicleSaleRepo = new VehicleSaleRepository();
                var vehicleRepo = new VehicleRepository(vehicleSaleRepo);
                if (voucherNumber == 0) key = "nextvouchernumber";
                if (key == "nextvouchernumber")
                {
                    voucherNumber = vehicleSaleRepo.GetNextVoucherNumber(vouchertype);
                    invoiceNumber = vehicleSaleRepo.GetNextInvoiceNumber(vouchertype);
                }

                var data = vehicleSaleRepo.GetByVoucherNumber(voucherNumber, vouchertype, key, out next, out previous);
                var customer = new AccountDetailRepository().GetContactDetail(data != null ? data.AccountId : 0);
                response = new ApiResponse
                                {
                                    Success = true,
                                    Data = new
                                    {
                                        Voucher = data,
                                        Vehicle = new vw_VehiclesRepository().GetById(data != null ? data.VehicleId : 0),
                                        TradeVehicle = new vw_VehiclesRepository().GetById(data != null ? Numerics.GetInt(data.TradeInVehicleId) : 0),
                                        Next = next,
                                        Previous = previous,
                                        VoucherNumber = voucherNumber,
                                        InvoiceNumber = invoiceNumber,
                                        Customer = customer

                                    }
                                };

            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        public virtual ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                response = new ApiResponse
                {
                    Success = true,
                    Data = new VehicleSaleRepository().GetById(id),
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        public virtual ApiResponse Get(int id, bool loadCancellationInfo, string strloadCancellationInfo)
        {

            ApiResponse response;
            try
            {


                var vehicleSaleRepo = new VehicleSaleRepository();
                var data = vehicleSaleRepo.GetById(id);
                var PaidAmount = new VehiclePaymentRepository().GetPaidAmount(id);
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Voucher = data,
                        TradeVehicle = new vw_VehiclesRepository().GetById(data != null ? Numerics.GetInt(data.TradeInVehicleId) : 0),
                        PaidAmount = PaidAmount,
                    }
                };

            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        public ApiResponse Get(int id, string laodDatakey)
        {


            ApiResponse response;
            try
            {
                var repo = new VehicleRepository();
                if (laodDatakey == "loadDocuments")
                {

                    var documents = new VehicleSaleDocumentRepository(repo).GetBySaleId(id);
                    response = new ApiResponse
                    {
                        Success = true,
                        Data = new
                        {
                            Documents = documents

                        }
                    };
                }
                else
                {
                    var data = new vw_VehicleSalesRepository().GetById(id);

                    response = new ApiResponse
                    {
                        Success = true,
                        Data = data
                    };
                }

            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;



        }

        public ApiResponse Post([FromBody]VehicleSale input)
        {
            ApiResponse response;
            try
            {
                var err = ValidateSave(input);
                if (err == "")
                {
                    input.FiscalId = SiteContext.Current.Fiscal.Id;
                    VehicleSaleManager.Save(input);
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
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }
        public ApiResponse Post([FromBody]VehicleSale input, bool Cancellationdeal)
        {
            ApiResponse response;
            try
            {

                input.FiscalId = SiteContext.Current.Fiscal.Id;
                response = VehicleSaleManager.CancellDeal(input);

            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }



        public ApiResponse Post(int id, string key)
        {
            ApiResponse response;
            try
            {
                if (key == "markFinal")
                    VehicleSaleManager.MarkFinal(id);
                else if (key == "markDelivery")
                    VehicleSaleManager.MarkDelivery(id);
                else if (key == "markVoid")
                    VehicleSaleManager.MarkVoid(id);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        //This is for paying installments
        public ApiResponse Post(VehicleInstallmentPayment input, string key)
        {
            ApiResponse response;
            try
            {
                VehicleSaleManager.PayInstallments(input);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        //This is for paying installments
        public ApiResponse Post(List<SaleDocument> documents, string savedocuments)
        {
            ApiResponse response;
            try
            {
                VehicleSaleManager.SaveDocuments(documents);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        public virtual ApiResponse Delete(int id, string key)
        {
            ApiResponse response;
            try
            {
                if (key == "deleteInstallment")
                {

                    response = VehicleSaleManager.DeleteInstallments(id);

                }
                else
                {

                    response = VehicleSaleManager.Delete(id);
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
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "VoucherNumber", "Date", "Customer", "ChassisNo", "RegNo", "Manufacturer", "SalePrice", "Received", "Balance", "CreateByName", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var Intsearch = Numerics.GetInt((queryString["sSearch"] + "").Trim());
            var type =(VoucherType) Convert.ToByte((queryString["type"]));
            var branchId = Numerics.GetInt((queryString["branchId"]));
            //var dal = new ProjectRepository();
            var records = new vw_VehicleSalesRepository().AsQueryable();
            records = records.Where(p => p.BranchId == branchId);
            if (type  == VoucherType.vehiclecashsale)
                records = records.Where(p => p.SaleType == VoucherType.vehiclecashsale);
            else
                records = records.Where(p => p.SaleType == VoucherType.vehicleinstallmentsale);


            var totalRecords = records.Count();
           
            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                     p.Manufacturer.Contains(search) ||
                      (Intsearch > 0 && p.VoucherNumber == Intsearch) ||
                     p.Customer.Contains(search) ||
                     p.Color.Contains(search) ||
                     p.ChassisNo.Contains(search) ||
                      p.RegNo.Contains(search) ||
                     p.EnginePower.Contains(search)
                     ||
                     p.CreateByName.Contains(search)
                   );


            var totalDisplayRecords = filteredList.Count(); ;
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
                data.Add(item.VoucherNumber + "");
                data.Add(item.Date.ToString(AppSetting.GridDateFormat));
                data.Add(item.Customer);
                data.Add(item.ChassisNo);
                data.Add(item.RegNo);
                data.Add(item.Manufacturer);
                data.Add(Numerics.IntToString(item.SalePrice));
                data.Add(Numerics.IntToString(item.Received));
                data.Add(Numerics.IntToString(item.Balance));
                data.Add(item.CreateByName);
                //var editIcon = "<i class='fa fa-edit' onclick=\"VehicleSale.Edit(" + item.Id + "," + item.VoucherNumber + ")\" title='Edit' ></i>";
                //var deleteIcon = "<i class='fa fa-trash-o' onclick=\"VehicleSale.Delete(" + item.Id + "," + item.VoucherNumber + ")\" title='Delete' ></i>";



                var actionHtml = "<div class='btn-group'><button type='button' class='btn btn-success dropdown-toggle btn-xs' data-toggle='dropdown' data-hover='dropdown' data-delay='1000' data-close-others='true' aria-expanded='false'>Actions<i class='fa fa-angle-down'></i></button><ul class='dropdown-menu' role='menu'>";
                var editIcon = "<li><a target='_blank' href='javascript:void(0)' onclick=\"VehicleSale.Edit(" + item.Id + "," + item.VoucherNumber + ")\" title='Edit' class='btn default btn-xs green-stripe'>Edit</a></li>";
                var viewIcon = "<li><a target='_blank' href='javascript:void(0)' onclick=\"VehicleSale.Edit(" + item.Id + "," + item.VoucherNumber + ")\" title='View' class='btn default btn-xs green-stripe'>View</a></li>";
                var deleteIcon = "<li><a target='_blank' href='javascript:void(0)'  onclick=\"VehicleSale.Delete(" + item.Id + "," + item.VoucherNumber + ")\" title='Delete' class='btn default btn-xs green-stripe'>Delete</a></li>";
                var markFinalIcon = "<li><a target='_blank' href='javascript:void(0)' onclick=\"VehicleSale.MarkFinal(" + item.Id + "," + (item.IsFinal ? "false" : "true") + ")\"' class='btn default btn-xs green-stripe'>" + (!item.IsFinal ? " Mark Final" : " Mark Unfinsihed") + "</a></li>";
                var finaledLabel = "<li><a target='_blank' href='javascript:void(0)' class='btn default btn-xs green-stripe'>Mark Final</a></li>";
                var btnDeliver = "<li><a target='_blank' href='javascript:void(0)'  onclick=\"VehicleSale.MarkDelivered(" + item.Id + ")\"' class='btn default btn-xs red-stripe'>Delivered</a></li>";
                var btnMarkVoid = "<li><a target='_blank' href='javascript:void(0)' onclick=\"VehicleSale.MarkVoid(" + item.Id + "," + (item.IsVoid ? "false" : "true") + ")\"' class='btn default btn-xs green-stripe'>" + (!item.IsVoid ? " Mark Void" : " Mark Valid") + "</a></li>";
                var btnPrintDeliveryNote = "<li><a target='_blank' href='javascript:void(0)' onclick=\"VehicleSale.PrintDeliveryNote(" + item.Id + ")\"' class='btn default btn-xs green-stripe'>Print Delivery Note</a></li>";
                var btnUploadDocument = "<li><a target='_blank' href='javascript:void(0)' onclick=\"VehicleSale.LoadDocuments(" + item.Id + ")\"' class='btn default btn-xs green-stripe'>Upload Document</a></li>";
                if (!item.IsFinal)
                {

                    actionHtml += editIcon + deleteIcon + markFinalIcon;

                }
                else
                {
                    //actionHtml += viewIcon;
                    actionHtml += viewIcon + markFinalIcon;

                }
                if (!item.IsDelivered)
                    actionHtml += btnDeliver;
                else
                    actionHtml += btnPrintDeliveryNote;


                actionHtml += btnUploadDocument;
                actionHtml += btnMarkVoid;
                actionHtml += "</ul></div>";

                data.Add(actionHtml);
                rs.aaData.Add(data);
            }
            rs.sEcho = echo;
            rs.iTotalRecords = totalRecords;
            rs.iTotalDisplayRecords = totalDisplayRecords;
            return rs;
        }

        public static string ValidateSave(VehicleSale input)
        {
            var err = ",";
            try
            {
                var saleRepo = new VehicleSaleRepository();
                var paymentRepo = new VehiclePaymentRepository(saleRepo);
                if (!SiteContext.Current.User.IsAdmin)
                {
                    if (input.Id == 0)
                    {
                        if (!SiteContext.Current.RoleAccess.CanCreate)
                        {
                            err += "you did not have sufficent right to add new voucher.,";
                        }
                    }
                    else
                    {
                        if (!SiteContext.Current.RoleAccess.CanUpdate)
                        {
                            err += "you did not have sufficent right to update voucher.,";
                        }

                    }
                }
                if (input.Id > 0)
                {
                    if (paymentRepo.IsExistBySaleId(input.Id))
                    {
                        err += "Agreement is linked with installment payment and can't be updated.,";
                    }
                }
                if (input.VehicleId == input.TradeInVehicleId)
                {
                    err += "sold and trade in vehilce can't be same.,";
                }
                if (SettingManager.SaleAccountHeadId == 0)
                {
                    err += "sale account is missing.,";
                }
                if (SettingManager.TrackerPurchaseAcccountId == 0)
                {
                    err += "tracker purchase account is missing.,";
                }
                if (SettingManager.TrackerSaleAcccountId == 0)
                {
                    err += "tracker sale account is missing.,";
                }
                if (SettingManager.InsurancePurchaseAccountId == 0)
                {
                    err += "insurance purchase account is missing.,";
                }
                if (SettingManager.InsuranceSaleAccountId == 0)
                {
                    err += "insurance sale account is missing.,";
                }
                if (SettingManager.LogBookAcAcccountId == 0)
                {
                    err += "logbook account is missing.,";
                }
                if (input.AccountId == 0)
                {
                    err += "Account is not valid to process the request.,";
                }

                if (SiteContext.Current.Fiscal == null)
                {
                    err += "No fiscal year found.,";

                }
                if (SiteContext.Current.Fiscal.IsClosed)
                {
                    err += "Fiscal year is closed No action can be done.,";
                }
                if (!FiscalYearManager.IsValidFiscalDate(input.Date))
                {
                    err += "Voucher date should be within current fiscal year.,";
                }

                var isExist = saleRepo.IsVoucherExits(input.VoucherNumber, input.TransactionType, input.Id);
                if (isExist)
                {
                    err += "Voucher no already exist.,";
                }


            }
            catch (Exception ex)
            {
                ErrorManager.Log(ex);
                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;


        }


    }
}
