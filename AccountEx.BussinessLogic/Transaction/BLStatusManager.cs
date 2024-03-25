using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.Transactions;
using AccountEx.CodeFirst.Models.Vehicles;
using AccountEx.Common;
using AccountEx.DbMapping;
using AccountEx.Repositories;
using AccountEx.Repositories.Transactions;
using Entities.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AccountEx.BussinessLogic
{
    public static class BLStatusManager
    {
        public static void Save(List<BLSaveExtra> BLs)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {

                var repo = new BLStatusHistoryRepository();
                var vehicleRepo = new VehicleRepository(repo);
                var blRepo = new BLRepository(repo);
                var blIds = BLs.Select(p => p.BLId).ToList();
                var vehicleIds = BLs.SelectMany(p => p.Vehicles).Select(p => p.Id).ToList();
                var vehicles = vehicleRepo.GetAll(p => vehicleIds.Contains(p.Id));
                var dbBLs = blRepo.GetAll(p => blIds.Contains(p.Id));
                var vehicleStatuses = new VehicleStatusRepository(repo).GetAll();
                var headOfficeId = new VehicleBranchRepository(repo).GetHeadOfficeId();

                foreach (var BL in BLs)
                {

                    var records = BL.Vehicles;
                    var dbBL = dbBLs.FirstOrDefault(p => p.Id == BL.BLId);
                    var status = vehicleStatuses.FirstOrDefault(p => p.Id == BL.StatusId);
                    var dbVehcile = new Vehicle();
                    foreach (var item in records)
                    {
                        //var record = repo.GetByBLItemId(item.Id);
                        item.StatusId = BL.StatusId;
                        dbVehcile = vehicles.FirstOrDefault(p => p.Id == item.Id);
                        if (BL.StatusId != dbVehcile.StatusId)
                        {
                            var record = repo.AsQueryable().Where(p => p.VehicleId == item.Id && p.ToDate == null).FirstOrDefault();
                            if (record != null)
                            {
                                record.VehicleId = item.Id;
                                record.ToDate = DateTime.Now;
                                repo.Save(record, false, false);
                            }
                            var bLStatusHistory = new BLStatusHistory();
                            bLStatusHistory.VehicleId = item.Id;
                            bLStatusHistory.Status = Numerics.GetByte(item.StatusId);
                            bLStatusHistory.FromDate = DateTime.Now;

                            repo.Add(bLStatusHistory, false, false);
                            dbVehcile.StatusId = item.StatusId;
                            dbBL.StatusId = BL.StatusId;
                        }
                        if (status != null && status.IsFinal)
                        {
                            dbVehcile.BranchId = headOfficeId;
                            dbVehcile.AssignedBranchId = item.AssignedBranchId;
                            dbVehcile.SalePrice = item.SalePrice;
                            dbVehcile.PurchasePrice = item.PurchasePrice;
                            dbVehcile.PurchaseDate = dbBL.Date;
                            dbVehcile.VendorId = dbBL.SupplierId;
                        }

                    }
                    if (status != null && status.IsFinal)
                    {
                        dbBL.DeliveryOrder = BL.DeliveryOrder;
                        dbBL.PortCharges = BL.PortCharges;
                        dbBL.Fare = BL.Fare;
                        dbBL.Storage = BL.Storage;
                        AddTransaction(dbVehcile, repo);
                    }


                }
                repo.SaveChanges();
                scope.Complete();
            }
        }

        public static void AddTransaction(Vehicle v, BaseRepository repo)
        {

            var dt = DateTime.Now;
            var trans = new List<Transaction>();
            var transRepo = new TransactionRepository(repo);
            var voucherNo = transRepo.GetNextVoucherNumber(VoucherType.BLPurchase);
            var vehcileDetail = "Chessis No:" + v.ChassisNo + " RegNo No:" + v.RegNo;
            var comments = "BL Purchase amount paid against vehicle " + vehcileDetail;
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
                MainEntityId = v.Id,


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


            foreach (var item in trans)
            {
                item.CreatedDate = dt;
                item.FiscalId = SiteContext.Current.Fiscal.Id;
            }
            transRepo.Add(trans, false, true);
        }
    }
}
