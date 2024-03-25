using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.Nexus;
using AccountEx.CodeFirst.Models.Transactions;
using AccountEx.CodeFirst.Models.Vehicles;
using AccountEx.Common;
using AccountEx.DbMapping.Lab;
using AccountEx.Repositories;
using AccountEx.Repositories.Lab;
using Entities.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AccountEx.BussinessLogic
{
    public static class TestManager
    {
        public static ApiResponse Save(TestSaveExtra testSaveExtra)
        {
            ApiResponse response;
            //var err = ServerValidateSave(input);
            var err = "";
            if (err == "")
            {
                using (var scope = TransactionScopeBuilder.Create())
                {
                    var repo = new TestRepository();
                    var departmentTestRepo = new DepartmentTestRepository(repo);
                    var rateListRepo = new DepartmentRateListRepository(repo);
                    var test = testSaveExtra.Test;
                    var testDepartments = testSaveExtra.TestDepartments;
                    var newDepartmentTest = new List<DepartmentTest>();
                    repo.Save(test);
                    repo.SaveChanges();
                    var dbTepartmentTest = departmentTestRepo.GetByTestId(test.Id);
                    var departmentIds = testDepartments.Select(p => p.DepartmentAccountId).ToList();
                    var deletedIds = dbTepartmentTest.Where(p => !departmentIds.Contains(p.DepartmentAccountId)).Select(p => p.Id).ToList();

                    foreach (var item in testDepartments)
                    {
                        if (!dbTepartmentTest.Any(p => p.DepartmentAccountId == item.DepartmentAccountId))
                        {
                            newDepartmentTest.Add(new DepartmentTest()
                            {
                                TestId = test.Id,
                                DepartmentAccountId = item.DepartmentAccountId
                            });
                        }

                    }
                    departmentTestRepo.Delete(deletedIds);
                    departmentTestRepo.Save(newDepartmentTest);
                    rateListRepo.AddUpdateRateList(test.Id, testDepartments);
                    repo.SaveChanges();
                    scope.Complete();
                }
                response = new ApiResponse() { Success = true };
            }
            else
            {
                response = new ApiResponse() { Success = false, Error = err };
            }

            return response;
        }


        public static ApiResponse GetTest(int id)
        {
            ApiResponse response;
            var repo = new TestRepository();
            var departmentTestRepo = new DepartmentTestRepository(repo);
            var rateListRepo = new DepartmentRateListRepository(repo);
            var test = repo.GetById(id);
            var dbTepartmentTest = departmentTestRepo.GetByTestId(test.Id);
            var prices = rateListRepo.GetLatestPrices(test.Id, dbTepartmentTest.Select(p => p.DepartmentAccountId).ToList());
            var pr = new ParameterRepository(repo).GetByParameterIds(test.TestParameters.Select(p => p.ParameterId).ToList());
            return response = new ApiResponse()
            {
                Success = true,
                Data = new
                {
                    Test = test,
                    TestDepartments = dbTepartmentTest,
                    Prices = prices,
                    Parameters = pr

                }
            };


        }

        private static string ServerValidateSave(TestSaveExtra input)
        {
            var err = ",";
            try
            {
                var orderbookingRepo = new BLRepository();
                var dcRepo = new DeliveryChallanRepository(orderbookingRepo);
                var jobORRepo = new JobOrderRequisitionRepository(orderbookingRepo);
                if (!SiteContext.Current.User.IsAdmin)
                {
                    if (input.Test.Id == 0)
                    {
                        if (!SiteContext.Current.RoleAccess.CanCreate)
                        {
                            err += "you did not have sufficent right to add new Test.,";
                        }
                    }
                    else
                    {
                        if (!SiteContext.Current.RoleAccess.CanUpdate)
                        {
                            err += "you did not have sufficent right to update the Test.,";
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
