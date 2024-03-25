using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.CodeFirst;
using Entities.CodeFirst;
using AccountEx.CodeFirst.Models.Vehicles;
using AccountEx.Common.VehicleSystem;
using AccountEx.DbMapping.VehicleSystem;
using AccountEx.DbMapping;

namespace AccountEx.BussinessLogic.Vehicles
{

    public static class VehicleManager
    {


        public static void Save(Vehicle input)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new VehicleRepository();
                var dbVehicle = new Vehicle();
                if (input.Id == 0)
                {
                    if (input.Type != (byte)VehicleType.New)
                    {
                        var headOfficeId = new VehicleBranchRepository(repo).GetHeadOfficeId();
                        var finalStatusId = new VehicleStatusRepository(repo).GetFinalStatusId();
                        input.BranchId = headOfficeId;
                        input.AssignedBranchId = headOfficeId;
                        input.StatusId = finalStatusId;
                    }
                    repo.Save(input);
                    repo.SaveChanges();
                    dbVehicle = input;
                }
                else
                {

                    dbVehicle = repo.GetById(input.Id, true);
                    if (input.Type != (byte)VehicleType.New && (dbVehicle.BranchId == null || dbVehicle.BranchId == 0))
                    {
                        var headOfficeId = new VehicleBranchRepository(repo).GetHeadOfficeId();
                        var finalStatusId = new VehicleStatusRepository(repo).GetFinalStatusId();
                        dbVehicle.BranchId = headOfficeId;
                        dbVehicle.AssignedBranchId = headOfficeId;
                        dbVehicle.StatusId = finalStatusId;
                    }
                    dbVehicle.ChassisNo = input.ChassisNo;
                    dbVehicle.Type = input.Type;
                    dbVehicle.ModelId = input.ModelId;
                    dbVehicle.RegNo = input.RegNo;
                    dbVehicle.EngineNo = input.EngineNo;
                    dbVehicle.FileNo = input.FileNo;
                    dbVehicle.ManufacturerId = input.ManufacturerId;
                    dbVehicle.EnginePowerId = input.EnginePowerId;
                    dbVehicle.ColorId = input.ColorId;
                    dbVehicle.FuelId = input.FuelId;
                    dbVehicle.CarTypeId = input.CarTypeId;
                    dbVehicle.ClearingCompanyId = input.ClearingCompanyId;
                    dbVehicle.YearId = input.YearId;
                    dbVehicle.DoM = input.DoM;
                    dbVehicle.VendorId = input.VendorId;
                    dbVehicle.PurchaseDate = input.PurchaseDate;
                    dbVehicle.PurchasePrice = input.PurchasePrice;
                    dbVehicle.ForexPrice = input.ForexPrice;
                    dbVehicle.ExcRate = input.ExcRate;
                    dbVehicle.CurrencyId = input.CurrencyId;
                    dbVehicle.IsForex = input.IsForex;
                    dbVehicle.MonthId = input.MonthId;
                    repo.Save(dbVehicle);
                }

                AddTransaction(dbVehicle, repo);
                repo.SaveChanges();
                scope.Complete();
            }

        }
        public static ApiResponse PurchaseReturn(int id)
        {
            ApiResponse response;
            try
            {
                var repo = new VehicleRepository();
                var vehicle = repo.GetById(id);
                var err = ValidatePurchaseReturn(vehicle);
                if (err == "")
                {
                    using (var scope = TransactionScopeBuilder.Create())
                    {


                        vehicle.ReturnRemarks = "";
                        vehicle.Status = (byte)VehicleStatus.PurchaseReturn;
                        vehicle.ReturnDate = DateTime.Now;
                        repo.Save(vehicle);
                        AddPurchaseReturnTransaction(vehicle, repo);
                        repo.SaveChanges();
                        scope.Complete();
                    }
                    response = new ApiResponse()
                    {
                        Success = true,
                        Data = null
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

                response = new ApiResponse()
                {
                    Success = false,
                    Error = ErrorManager.Log(ex)
                };
            }

            return response;



        }
        public static void SaveLogBook(VehicleLogBookSave input)
        {
            var repo = new VehicleRepository();
            var LogBookScanRepo = new VehicleLogBookScanRepository(repo);
            using (var scope = TransactionScopeBuilder.Create())
            {

                var vehilce = repo.GetById(input.Id);
                if (input.Type == (byte)LogBookStatus.Apply)
                {
                    vehilce.LogBookApplied = true;
                    vehilce.LogBookAppliedBy = SiteContext.Current.User.Id;
                    vehilce.LogBookAppliedDate = input.Date;

                }
                else if (input.Type == (byte)LogBookStatus.Received)
                {
                    vehilce.LogBookReceived = true;
                    vehilce.LogBookReceivedBy = SiteContext.Current.User.Id;
                    vehilce.LogBookReceivedDate = input.Date;

                }
                else if (input.Type == (byte)LogBookStatus.Transferred)
                {
                    vehilce.LogBookTransferred = true;
                    vehilce.LogBookTransferredBy = SiteContext.Current.User.Id;
                    vehilce.LogBookTransferredDate = input.Date;

                }
                string query = "Delete from VehicleLogBookScanes where VehicleId=" + input.Id + " and Type=" + input.Type + " and CompanyId=" + SiteContext.Current.User.CompanyId;
                LogBookScanRepo.ExecuteQuery(query);
                foreach (var item in input.VehicleLogBookScanes)
                {
                    item.Type = input.Type;
                }
                LogBookScanRepo.Add(input.VehicleLogBookScanes.ToList());
                repo.SaveChanges();
                scope.Complete();
            }


        }
        public static void AddTransaction(Vehicle v, BaseRepository repo)
        {
            var dt = DateTime.Now;
            var transRepo = new TransactionRepository(repo);
            var vehicleRepo = new VehicleRepository(repo);
            var voucherTypes = new List<VoucherType>() { VoucherType.TradeIn, VoucherType.LocalPurchase, VoucherType.BLPurchase };
            transRepo.HardDeleteByReferenceIdTransactionType(v.Id, voucherTypes);
            var vehicle = vehicleRepo.GetVehicleDetailForBLTransaction(new List<int> { v.Id }).FirstOrDefault();
            var extraComments = "";
            if (v.IsForex)
            {
                var currency = new CurrencyRepository().GetById(Numerics.GetInt(v.CurrencyId));

                extraComments = " " + (currency != null ? currency.Name : " ") + " " + v.ForexPrice + " @ " + v.ExcRate;
            }

            var voucherNo = 0;
            var trans = new List<Transaction>();
            if (v.Type == (byte)VehicleType.TradIn)
            {
                voucherNo = transRepo.GetNextVoucherNumber(VoucherType.TradeIn);

                var comments = "trade in amount paid against vehicle " + vehicle.Name + extraComments;

                trans.Add(new Transaction
                {
                    AccountId = Numerics.GetInt(v.VendorId),
                    InvoiceNumber = voucherNo,
                    VoucherNumber = voucherNo,
                    Date = v.PurchaseDate.HasValue ? v.PurchaseDate.Value : DateTime.Now,
                    TransactionType = VoucherType.TradeIn,
                    EntryType = (byte)EntryType.Item,
                    Credit = v.PurchasePrice,
                    FiscalId = SiteContext.Current.Fiscal.Id,
                    Comments = comments,
                    BranchId = v.BranchId,
                    ReferenceId = v.Id,
                    MainEntityId = v.Id

                });
                trans.Add(new Transaction
                {
                    AccountId = SettingManager.TradeInAcccountId,
                    InvoiceNumber = voucherNo,
                    VoucherNumber = voucherNo,
                    Date = v.PurchaseDate.HasValue ? v.PurchaseDate.Value : DateTime.Now,
                    TransactionType = VoucherType.TradeIn,
                    EntryType = (byte)EntryType.Item,
                    Debit = v.PurchasePrice,
                    FiscalId = SiteContext.Current.Fiscal.Id,
                    Comments = comments,
                    BranchId = v.BranchId,
                    ReferenceId = v.Id,
                    MainEntityId = v.Id
                });
            }
            if (v.Type == (byte)VehicleType.LocalPurchase)
            {
                voucherNo = transRepo.GetNextVoucherNumber(VoucherType.LocalPurchase);
                var comments = "Purchase amount paid against vehicle " + vehicle.Name + extraComments;

                trans.Add(new Transaction
                {
                    AccountId = Numerics.GetInt(v.VendorId),
                    InvoiceNumber = voucherNo,
                    VoucherNumber = voucherNo,
                    Date = v.PurchaseDate.HasValue ? v.PurchaseDate.Value : DateTime.Now,
                    TransactionType = VoucherType.LocalPurchase,
                    EntryType = (byte)EntryType.Item,
                    Credit = v.PurchasePrice,
                    FiscalId = SiteContext.Current.Fiscal.Id,
                    Comments = comments,
                    BranchId = v.BranchId,
                    ReferenceId = v.Id,
                    MainEntityId = v.Id

                });
                trans.Add(new Transaction
                {
                    AccountId = SettingManager.PurchaseAccountHeadId,
                    InvoiceNumber = voucherNo,
                    VoucherNumber = voucherNo,
                    Date = v.PurchaseDate.HasValue ? v.PurchaseDate.Value : DateTime.Now,
                    TransactionType = VoucherType.LocalPurchase,
                    EntryType = (byte)EntryType.Item,
                    Debit = v.PurchasePrice,
                    FiscalId = SiteContext.Current.Fiscal.Id,
                    Comments = comments,
                    BranchId = v.BranchId,
                    ReferenceId = v.Id,
                    MainEntityId = v.Id
                });
            }

            if (v.Type == (byte)VehicleType.New && v.BranchId > 0)
            {
                voucherNo = transRepo.GetNextVoucherNumber(VoucherType.BLPurchase);
                var comments = "BL Purchase amount paid against vehicle " + vehicle.Name + extraComments;
                trans.Add(new Transaction
                {
                    AccountId = Numerics.GetInt(v.VendorId),
                    InvoiceNumber = voucherNo,
                    VoucherNumber = voucherNo,
                    Date = v.PurchaseDate.HasValue ? v.PurchaseDate.Value : DateTime.Now,
                    TransactionType = VoucherType.BLPurchase,
                    EntryType = (byte)EntryType.Item,
                    Credit = v.PurchasePrice,
                    FiscalId = SiteContext.Current.Fiscal.Id,
                    Comments = comments,
                    BranchId = v.BranchId,
                    ReferenceId = v.Id,
                    MainEntityId = v.Id

                });
                trans.Add(new Transaction
                {
                    AccountId = SettingManager.PurchaseAccountHeadId,
                    InvoiceNumber = voucherNo,
                    VoucherNumber = voucherNo,
                    Date = v.PurchaseDate.HasValue ? v.PurchaseDate.Value : DateTime.Now,
                    TransactionType = VoucherType.BLPurchase,
                    EntryType = (byte)EntryType.Item,
                    Debit = v.PurchasePrice,
                    FiscalId = SiteContext.Current.Fiscal.Id,
                    Comments = comments,
                    BranchId = v.BranchId,
                    ReferenceId = v.Id,
                    MainEntityId = v.Id
                });
            }
            foreach (var item in trans)
            {
                item.VoucherNumber = voucherNo;
                item.CreatedDate = v.CreatedAt != DateTime.MinValue ? v.CreatedAt : dt;
            }

            transRepo.Add(trans, true);

        }
        public static void AddPurchaseReturnTransaction(Vehicle v, BaseRepository repo)
        {
            var dt = v.ReturnDate.HasValue ? v.ReturnDate.Value : DateTime.Now;
            var transRepo = new TransactionRepository(repo);
            var vehicleRepo = new VehicleRepository(repo);
            var voucherType = VoucherType.PurchaseReturn;
            var trans = new List<Transaction>();
            var voucherNo = transRepo.GetNextVoucherNumber(voucherType);
            var vehicle = vehicleRepo.GetVehicleDetailForBLTransaction(new List<int> { v.Id }).FirstOrDefault();
            var comments = "Purchase return against vehicle " + vehicle.Name;
            var supplierId = Numerics.GetInt(v.VendorId);


            trans.Add(new Transaction
            {
                AccountId = Numerics.GetInt(v.VendorId),
                InvoiceNumber = voucherNo,
                VoucherNumber = voucherNo,
                Date = dt,
                TransactionType = voucherType,
                EntryType = (byte)EntryType.MasterDetail,
                Debit = v.PurchasePrice,
                FiscalId = SiteContext.Current.Fiscal.Id,
                Comments = comments,
                BranchId = v.BranchId,
                ReferenceId = v.Id,
                MainEntityId = v.Id

            });
            trans.Add(new Transaction
            {
                AccountId = SettingManager.PurchaseAccountHeadId,
                InvoiceNumber = voucherNo,
                VoucherNumber = voucherNo,
                Date = dt,
                TransactionType = voucherType,
                EntryType = (byte)EntryType.Item,
                Credit = v.PurchasePrice,
                FiscalId = SiteContext.Current.Fiscal.Id,
                Comments = comments,
                BranchId = v.BranchId,
                ReferenceId = v.Id,
                MainEntityId = v.Id
            });

            foreach (var item in trans)
            {
                item.VoucherNumber = voucherNo;
                item.CreatedDate = v.CreatedAt != DateTime.MinValue ? v.CreatedAt : dt;
            }

            transRepo.Add(trans, true);

        }
        public static void AddJvTransaction(Voucher v)
        {
            var dt = DateTime.Now;
            new TransactionRepository().HardDelete(v.VoucherNumber, v.TransactionType);
            var trans = v.VoucherItems.Select(item => new Transaction
            {
                AccountId = item.AccountId,
                InvoiceNumber = v.InvoiceNumber,
                VoucherNumber = v.VoucherNumber,
                TransactionType = v.TransactionType,
                EntryType = (byte)EntryType.Item,
                BranchId = v.BranchId,
                Credit = Numerics.GetDecimal(item.Credit),
                Debit = Numerics.GetDecimal(item.Debit)
            }).ToList();
            foreach (var item in trans)
            {
                item.VoucherNumber = v.VoucherNumber;
                item.CreatedDate = dt;
                item.Date = v.Date != DateTime.MinValue ? v.Date : dt;
                item.Comments = v.Comments;
            }
            new TransactionRepository().Add(trans);
        }

        public static string ValidatePurchaseReturn(Vehicle v)
        {
            var err = ",";
            try
            {
                var saleRepo = new VehicleSaleRepository();

                if (v.IsSale)
                {
                    err += "vehicle is sold, It can't be return.,";
                }
                if (v.Status == (byte)VehicleStatus.PurchaseReturn)
                {
                    err += "Vehicle is already returned.,";
                }
                if (v.Type == (byte)VehicleType.TradIn && saleRepo.IsTradeUnitUsed(v.Id))
                {
                    err += "tarde unit is used in sale and can't be return.,";
                }
                if (SettingManager.PurchaseAccountHeadId == 0)
                {
                    err += "purchase account is missing.,";
                }

                if (SiteContext.Current.Fiscal == null)
                {
                    err += "No fiscal year found.,";

                }
                if (SiteContext.Current.Fiscal.IsClosed)
                {
                    err += "Fiscal year is closed No action can be done.,";
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
        public static void Delete(int voucherno, VoucherType transactiontype)
        {

            new TransactionRepository().HardDelete(voucherno, transactiontype);
            new VoucherTransRepository().DeleteByVoucherNumber(voucherno, transactiontype);

        }

        public static Voucher GetVocuherDetail(int voucherno, byte transactiontype, string key)
        {
            var d = new Voucher();
            //if (key == "next")
            //{
            //    voucherno = voucherno + 1;
            //}

            //else if (key == "previous")
            //{
            //    voucherno = voucherno - 1;
            //}
            //else if (key == "first")
            //{
            //    voucherno = 1;
            //}
            //else if (key == "last")
            //{
            //    voucherno = 1;
            //}
            //d = new VoucherTransRepository().GetByVoucherNumber(voucherno, transactiontype, key) ??
            //    new Voucher() { VoucherNumber = voucherno };
            return d;
        }

        public static List<InvoiceExtra> GetOrderListing()
        {
            //var order = repo.Where(p => p.Status == (byte)TransactionStatus.Pending || p.Status == (byte)TransactionStatus.Pending);


            var orderrepo = new OrderBookingRepository().AsQueryable();
            var dcrepo = new DeliveryChallanRepository().AsQueryable();
            var salerepo = new SaleRepository().AsQueryable();
            var orders = orderrepo.Where(p => p.Status == (byte)TransactionStatus.Pending);
            var dcs = dcrepo.Where(p => p.Status == (byte)TransactionStatus.Pending);
            var odlist = new List<InvoiceExtra>();
            foreach (var item in orders)
            {
                var od = new InvoiceExtra()
                {
                    VoucherNumber = item.VoucherNumber,
                    OrderDate = item.Date.ToString(AppSetting.DateFormat),
                    DCNo = 0,
                    OrderNo = item.VoucherNumber,
                    DcDate = "",
                    SaleNo = 0,
                    SaleDate = "",
                    GpNo = "",
                    Customer = item.AccountCode + "-" + item.AccountName,
                    NetTotal = item.NetTotal,
                    OrderStatus = item.Status == (byte)TransactionStatus.Pending ? "Pending" : "Partial Delivered",
                    Type = "Order"
                };
                odlist.Add(od);
            }
            foreach (var item in dcs)
            {
                var od = new InvoiceExtra()
                {
                    OrderNo = item.OrderNo,
                    //OrderDate = item.OrderDate.ToString(AppSetting.DateFormat),
                    DCNo = item.VoucherNumber,
                    DcDate = item.Date.ToString(AppSetting.DateFormat),
                    SaleNo = 0,
                    SaleDate = "",
                    GpNo = "",
                    //Customer = item.AccountCode + "-" + item.AccountName,
                    NetTotal = 0,
                    OrderStatus = "Delivered",
                    Type = "DC"


                };
                odlist.Add(od);
            }
            var salecount = 100 - odlist.Count();
            var sales = salerepo.Take(salecount);
            var dcnos = sales.Select(p => p.DCNo).ToList();
            var dcnoss = sales.Select(p => p.DCNo).ToList();
            var dclookup = dcrepo.Where(p => dcnos.Contains(p.VoucherNumber));
            var ordernos = dclookup.Select(p => p.OrderNo).ToList();
            var orderlookup = orderrepo.Where(p => ordernos.Contains(p.VoucherNumber));
            foreach (var item in sales)
            {
                var order = new Order();
                var dc = dclookup.FirstOrDefault(p => p.VoucherNumber == item.DCNo);
                if (dc != null)
                    order = orderlookup.FirstOrDefault(p => p.VoucherNumber == dc.OrderNo);
                else
                    dc = new DeliveryChallan();
                if (order == null) continue;
                var od = new InvoiceExtra()
                {
                    OrderNo = order.VoucherNumber,
                    OrderDate = order.Date.ToString(AppSetting.DateFormat),
                    DCNo = item.DCNo,
                    DcDate = dc.Date.ToString(AppSetting.DateFormat),
                    SaleNo = item.VoucherNumber,
                    SaleDate = item.Date.ToString(AppSetting.DateFormat),
                    GpNo = dc.GatePassNo.ToString(),
                    Customer = item.AccountCode + "-" + item.AccountName,
                    NetTotal = item.NetTotal,
                    OrderStatus = "Invoiced",
                    Type = "Sale"


                };
                odlist.Add(od);
            }
            odlist = odlist.OrderByDescending(p => p.Type == "Order").ThenByDescending(p => p.Type == "DC").ToList();
            return odlist;



        }

        public static void Delete(int id)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {

                var repo = new VehicleRepository();
                var transRepo = new TransactionRepository(repo);
                var voucherTypes = new List<VoucherType>() { VoucherType.TradeIn, VoucherType.LocalPurchase };
                transRepo.HardDeleteByReferenceIdTransactionType(id, voucherTypes);
                repo.Delete(id);
                repo.SaveChanges();
                scope.Complete();
            }
        }



    }
}
