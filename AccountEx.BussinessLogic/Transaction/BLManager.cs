using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.Transactions;
using AccountEx.CodeFirst.Models.Vehicles;
using AccountEx.Common;
using AccountEx.Repositories;
using Entities.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AccountEx.BussinessLogic
{
    public static class BLManager
    {
        public static ApiResponse Save(BL input)
        {
            ApiResponse response;
            try
            {
                var err = ServerValidateSave(input);
                if (err == "")
                {
                    using (var scope = TransactionScopeBuilder.Create())
                    {
                        var blRepo = new BLRepository();
                        var vehicleRepo = new VehicleRepository(blRepo);

                        if (input.Id == 0)
                        {
                            blRepo.Add(input);
                        }
                        else
                        {
                            blRepo.Update(input);
                        }
                        blRepo.SaveChanges();
                        scope.Complete();
                    }
                    response = new ApiResponse() { Success = true };
                }
                else
                {
                    response = new ApiResponse() { Success = false, Error = err };
                }
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }
        private static string ServerValidateSave(BL input)
        {
            var err = ",";
            try
            {
                var orderbookingRepo = new BLRepository();
                var dcRepo = new DeliveryChallanRepository(orderbookingRepo);
                var jobORRepo = new JobOrderRequisitionRepository(orderbookingRepo);
                var record = orderbookingRepo.GetByVoucherNumber(input.VoucherNumber, input.Id);
                if (!SiteContext.Current.User.IsAdmin)
                {
                    if (input.Id == 0)
                    {
                        if (!SiteContext.Current.RoleAccess.CanCreate)
                        {
                            err += "you did not have sufficent right to add new BL.,";
                        }
                    }
                    else
                    {
                        if (!SiteContext.Current.RoleAccess.CanUpdate)
                        {
                            err += "you did not have sufficent right to update BL.,";
                        }
                    }
                }

                if (input.SupplierId == 0)
                {
                    err += "Supplier is not valid to process the request.,";
                }
                if (SiteContext.Current.Fiscal == null)
                {
                    err += "No Fiscal year found.,";
                }
                if (SiteContext.Current.Fiscal.IsClosed)
                {
                    err += "Fiscal year is closed No action can be done.,";
                }
                foreach (var item in input.BLItems.Where(p => p.VehicleId == 0).ToList())
                {
                    err += "Vehicle with Chassis No. " + item.ChassisNo + " is not valid.,";
                }
                if (record != null)
                {
                    err += "Voucher no already exist.,";
                }
                var Itemcountlist = input.BLItems.GroupBy(p => p.ChassisNo).Select(p => new
                {
                    ChassisNo = p.Key,
                    Count = p.Count()
                }).Where(p => p.Count > 1).ToList();

                foreach (var item in Itemcountlist)
                {
                    err += "Vehicle with Chassis No. " + item.ChassisNo + " must be added once in item list.(Current Count:" + item.Count + "),";
                }
                if (input.Id > 0)
                {

                }

            }
            catch (Exception)
            {

                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;


        }
        public static void SaveBLPayment(Voucher voucher)
        {

            var repo = new VoucherTransRepository();
            var accountRepo = new AccountRepository(repo);
            var transRepo = new TransactionRepository(repo);
            var blRepo = new BLRepository(repo);
            using (var scope = TransactionScopeBuilder.Create())
            {
                var isFinal = true;
                voucher.TransactionType = VoucherType.BLPayment;
                voucher.VoucherNumber = transRepo.GetNextVoucherNumber(voucher.TransactionType);
                voucher.IsFinal = isFinal;
                if (voucher.Id == 0)
                {
                    voucher.CreatedAt = DateTime.Now;
                    repo.Add(voucher, true, false);
                }
                else
                {
                    repo.Update(voucher);
                }
                var blIds = voucher.VoucherItems.Where(p => p.BLId.HasValue).Select(p => p.BLId.Value).ToList();
                var bls = blRepo.GetByIds(blIds);
                foreach (var item in voucher.VoucherItems)
                {
                    if (bls.Any(p => p.Id == item.BLId && p.BLCharges.Any(q => q.Id == item.BLChargeId)))
                        bls.FirstOrDefault(p => p.Id == item.BLId).BLCharges.FirstOrDefault(p => p.Id == item.BLChargeId).IsPaid = true;
                }
                AddPaymentTransaction(voucher, bls, repo);
                repo.SaveChanges();
                scope.Complete();
            }
        }
        public static void FinalUnfinal(BL dbBL, bool isfinal)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new BLRepository();
                var tranRepo = new TransactionRepository(repo);
                var accountRepo = new AccountRepository(repo);
                dbBL = repo.GetById(dbBL.Id);
                dbBL.IsFinal = isfinal;
                if (!isfinal)
                {
                    tranRepo.HardDelete(dbBL.VoucherNumber, VoucherType.BL);
                }
                else
                    AddTransaction(dbBL, repo);
                repo.SaveChanges();
                scope.Complete();
            }
        }
        public static void AddTransaction(BL BL, BaseRepository baseRepo)
        {
            var dt = DateTime.Now;
            var headOfficeId = new VehicleBranchRepository(baseRepo).GetHeadOfficeId();
            var transRepo = new TransactionRepository(baseRepo);
            transRepo.HardDelete(BL.VoucherNumber, VoucherType.BL);
            var Ids = BL.BLCharges.Select(p => p.ChargeId).Distinct().ToList();
            var charges = new AccountRepository().GetNamesByIds(Ids);
            var vehicleIds = BL.BLItems.Select(p => p.VehicleId).Distinct().ToList();
            var vehicles = new VehicleRepository().GetVehicleDetailForBLTransaction(vehicleIds);
            var voucherNo = BL.VoucherNumber;
            var trans = BL.BLCharges.Select(item => new Transaction
            {
                ReferenceId = BL.Id,
                AccountId = item.SupplierId,
                InvoiceNumber = voucherNo,
                VoucherNumber = BL.VoucherNumber,
                TransactionType = VoucherType.BL,
                EntryType = (byte)EntryType.ServicesSupplier,
                Credit = item.Amount,
                Debit = 0,
                BranchId = headOfficeId,
                Comments = "charges against BL No." + BL.BLNumber,

            }).ToList();
            var totalVehicle = BL.BLItems.Count();
            foreach (var vehicle in BL.BLItems)
            {
                var vd = vehicles.FirstOrDefault(p => p.Id == vehicle.VehicleId);
                trans.AddRange(

                     BL.BLCharges.Select(item => new Transaction
                     {
                         ReferenceId = BL.Id,
                         AccountId = item.ChargeId,
                         InvoiceNumber = voucherNo,
                         VoucherNumber = BL.VoucherNumber,
                         TransactionType = VoucherType.BL,
                         EntryType = (byte)EntryType.Charges,
                         Debit = item.Amount / totalVehicle,
                         Credit = 0,
                         MainEntityId = vehicle.VehicleId,
                         Comments = charges.FirstOrDefault(p => p.Id == item.ChargeId).Name + " against " + vd.Name + " having BL No." + BL.BLNumber,
                         BranchId = headOfficeId
                     }).ToList()

                      );

                //Supplier Payable
                //This transaction has been moved to BL Status controller and Vehicle Setup
                //if (vd.PurchasePrice > 0)
                //{
                //    trans.Add(new Transaction
                //        {
                //            ReferenceId = BL.Id,
                //            AccountId = BL.SupplierId,
                //            InvoiceNumber = voucherNo,
                //            VoucherNumber = BL.VoucherNumber,
                //            TransactionType = VoucherType.BL,
                //            EntryType = (byte)EntryType.VehicleSupplier,
                //            Credit = vd.PurchasePrice,
                //            Debit = 0,
                //            MainEntityId = vehicle.VehicleId,
                //            Comments = "purchase price against " + vd.Name + " having BL No." + BL.BLNumber
                //        }

                //         );
                //}



            }
            //This transaction has been moved to BL Status controller and Vehicle Setup
            //var amount = vehicles.Sum(p => p.PurchasePrice);
            //trans.Add(new Transaction
            //           {
            //               ReferenceId = BL.Id,
            //               AccountId = SettingManager.PurchaseAccountHeadId,
            //               InvoiceNumber = voucherNo,
            //               VoucherNumber = BL.VoucherNumber,
            //               TransactionType = VoucherType.BL,
            //               EntryType = (byte)EntryType.Item,
            //               Debit = amount,
            //               Credit = 0,
            //               Comments = "purchase price having BL No." + BL.BLNumber
            //           });
            foreach (var item in trans)
            {
                item.VoucherNumber = BL.VoucherNumber;
                item.CreatedDate = BL.CreatedAt;
                item.Date = BL.Date ?? dt;
                item.FiscalId = SiteContext.Current.Fiscal.Id;
            }

            transRepo.Add(trans);
        }
        public static void AddPaymentTransaction(Voucher v, List<BL> BLs, BaseRepository baseRepo)
        {


            var dt = DateTime.Now;
            var transRepo = new TransactionRepository(baseRepo);
            var headOfficeId = new VehicleBranchRepository(baseRepo).GetHeadOfficeId();
            transRepo.HardDelete(v.VoucherNumber, VoucherType.BLPayment);
            var Ids = BLs.SelectMany(p => p.BLCharges).Select(p => p.ChargeId).Distinct().ToList();
            var charges = new AccountRepository().GetNamesByIds(Ids);
            var vehicleIds = BLs.SelectMany(p => p.BLItems).Select(p => p.VehicleId).Distinct().ToList();
            var vehicles = new VehicleRepository().GetVehicleDetailForBLTransaction(vehicleIds);
            var voucherNo = v.VoucherNumber;
            var trans = v.VoucherItems.Select(item => new Transaction
            {
                ReferenceId = item.Id,
                AccountId = Numerics.GetInt(v.AccountId),
                InvoiceNumber = v.InvoiceNumber,
                VoucherNumber = v.VoucherNumber,
                TransactionType = v.TransactionType,
                EntryType = (byte)EntryType.Item,
                Comments = item.Description,
                Credit = Numerics.GetInt(item.Amount),
                BranchId = headOfficeId
            }).ToList();

            foreach (var item in v.VoucherItems)
            {

                foreach (var BL in BLs)
                {
                    var totalVehicle = BL.BLItems.Count();
                    foreach (var blItem in BL.BLItems)
                    {
                        var vd = vehicles.FirstOrDefault(p => p.Id == blItem.VehicleId);

                        var blCharge = BL.BLCharges.FirstOrDefault(p => p.Id == item.BLChargeId);
                        trans.Add(

                            new Transaction
                            {
                                ReferenceId = item.Id,
                                AccountId = item.AccountId,
                                MainEntityId = blItem.VehicleId,
                                InvoiceNumber = voucherNo,
                                VoucherNumber = voucherNo,
                                TransactionType = v.TransactionType,
                                EntryType = (byte)EntryType.Item,
                                Debit = item.Amount / totalVehicle,
                                Credit = 0,
                                Comments = charges.FirstOrDefault(p => p.Id == blCharge.ChargeId).Name + " payment against vehicle " + vd.Name + " having BL No." + BL.BLNumber,
                                BranchId = headOfficeId
                            }

                              );


                    }
                }

            }


            foreach (var item in trans)
            {
                item.VoucherNumber = v.VoucherNumber;
                item.CreatedDate = v.CreatedAt;
                item.Date = v.Date;
                item.FiscalId = SiteContext.Current.Fiscal.Id;
            }

            transRepo.Add(trans);
        }
        public static ApiResponse Delete(int id)
        {


            ApiResponse response;
            try
            {

                var repo = new BLRepository();
                var vehicleRepo = new VehicleRepository(repo);
                var dbBl = repo.GetById(id);
                var err = ServerValidateDelete(id, dbBl, repo);
                if (err == "")
                {
                    using (var scope = TransactionScopeBuilder.Create())
                    {


                        var vehicleIds = dbBl.BLItems.Select(p => p.VehicleId).ToList();
                        var vehicles = vehicleRepo.GetAll(p => vehicleIds.Contains(p.Id));
                        foreach (var dbVehcile in vehicles)
                        {
                            dbVehcile.StatusId = null;
                            dbVehcile.BranchId = null;
                            dbVehcile.AssignedBranchId = null;
                            dbVehcile.PurchasePrice = 0;
                            dbVehcile.PurchaseDate = null;
                            dbVehcile.VendorId = null;
                        }
                        repo.Delete(id);
                        repo.SaveChanges();
                        scope.Complete();
                    }
                    response = new ApiResponse { Success = true };
                }
                else
                {
                    response = new ApiResponse() { Success = false, Error = err };
                }
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;

        }
        private static string ServerValidateDelete(int id, BL bl, BaseRepository baseRepo)
        {
            var err = ",";
            try
            {
                var vehicleRepo = new VehicleRepository(baseRepo);
                var vehicleIds = bl.BLItems.Select(p => p.VehicleId).ToList();
                if (!SiteContext.Current.User.IsAdmin)
                {
                    if (!SiteContext.Current.RoleAccess.CanDelete)
                    {
                        err += "you did not have sufficent right to delete the voucher.,";
                    }
                }
                if (vehicleRepo.IfSoldVehicleExist(vehicleIds))
                {
                    err += "Bl contains sold vehicles and can't be deleted.,";
                }
                var isPaid = new BLChargeRepository().CheckIfPaidcharge(id);
                if (isPaid)
                {
                    err += "BL is linked with BL Payments and can't be deleted.,";
                }
            }
            catch (Exception)
            {

                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;


        }
        public static ApiResponse DeleteForSingleBLItem(int id)
        {

            ApiResponse response;
            try
            {
                var repo = new BLItemRepository();
                var vehicleRepo = new VehicleRepository(repo);
                var dbItem = repo.GetById(id);
                var vehicle = vehicleRepo.GetById(dbItem.VehicleId);
                var err = ServerValidateDeleteForSingleBlItem(id, dbItem, vehicle);
                if (err == "")
                {
                    using (var scope = TransactionScopeBuilder.Create())
                    {
                        repo.Delete(id);
                        vehicle.StatusId = null;
                        vehicle.BranchId = null;
                        vehicle.AssignedBranchId = null;
                        vehicle.PurchasePrice = 0;
                        vehicle.PurchaseDate = null;
                        vehicle.VendorId = null;
                        repo.SaveChanges();
                        scope.Complete();
                    }
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

        private static string ServerValidateDeleteForSingleBlItem(int id, BLItem blItem, Vehicle vehicle)
        {
            var err = ",";
            try
            {
                var isReceived = new ChallanItemRepository().CheckIfChallanReceived(id);
                if (vehicle.IsSale)
                {
                    err += "Vehicle is sold and can't be delete from current BL.,";
                }
                var isPaid = new BLChargeRepository().CheckIfPaidcharge(blItem.BLId);
                if (isPaid)
                {
                    err += "Vehicle is linked with BL Payments and can't be delete from current BL.,";
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
