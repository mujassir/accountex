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
    public static class PayablePaymentManager
    {
        public static void Save(BL input)
        {

            using (var scope = TransactionScopeBuilder.Create())
            {
                var blRepo = new BLRepository();
                var vehicleRepo = new VehicleRepository(blRepo);
                //var finalBl = input.BL.CloneWithJson();
                //foreach (var item in input.BLItemExtra.Where(p => p.VehicleId == 0).ToList())
                //{
                //    var vehicle = new Vehicle()
                //    {
                //        //BrandName = item.BrandName,
                //        //Model = item.Model,
                //        //EngineNo = item.EngineNo,
                //        //CarType = item.CarType,
                //        //RegNo = item.RegNo,
                //        //EnginePower=item.EnginePower,
                //        //ChassisNo = item.ChassisNo,
                //        EntryNo = item.EntryNo,
                //        ClearingCompany = item.ClearingCompany,
                //        StatusId = item.StatusId
                //    };

                //    vehicleRepo.Add(vehicle);
                //    item.VehicleId = vehicle.Id;
                //    var BLItem = new BLItem()
                //    {
                //        VehicleId = vehicle.Id,
                //        BLId = input.BL.Id
                //    };
                //    finalBl.BLItems.Add(BLItem);
                //}
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
                    Comments = charges.FirstOrDefault(p => p.Id == item.ChargeId).Name + " against " + vd.Name + " having BL No." + BL.BLNumber
                }).ToList()

                      );

                //Supplier Payable
                if (vd.PurchasePrice > 0)
                {
                    trans.Add(new Transaction
                        {
                            ReferenceId = BL.Id,
                            AccountId = BL.SupplierId,
                            InvoiceNumber = voucherNo,
                            VoucherNumber = BL.VoucherNumber,
                            TransactionType = VoucherType.BL,
                            EntryType = (byte)EntryType.VehicleSupplier,
                            Credit = vd.PurchasePrice,
                            Debit = 0,
                            MainEntityId = vehicle.VehicleId,
                            Comments = "purchase price against " + vd.Name + " having BL No." + BL.BLNumber
                        }

                         );
                }



            }
            var amount = vehicles.Sum(p => p.PurchasePrice);
            trans.Add(new Transaction
                       {
                           ReferenceId = BL.Id,
                           AccountId = SettingManager.PurchaseAccountHeadId,
                           InvoiceNumber = voucherNo,
                           VoucherNumber = BL.VoucherNumber,
                           TransactionType = VoucherType.BL,
                           EntryType = (byte)EntryType.Item,
                           Debit = amount,
                           Credit = 0,
                           Comments = "purchase price having BL No." + BL.BLNumber
                       });
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
            transRepo.HardDelete(v.VoucherNumber, VoucherType.BLPayment);
            var Ids = BLs.SelectMany(p => p.BLCharges).Select(p => p.ChargeId).Distinct().ToList();
            var charges = new AccountRepository().GetNamesByIds(Ids);
            var vehicleIds = BLs.SelectMany(p => p.BLItems).Select(p => p.VehicleId).Distinct().ToList();
            var vehicles = new VehicleRepository().GetVehicleDetailForBLTransaction(vehicleIds);
            var voucherNo = v.VoucherNumber;
            var trans = v.VoucherItems.Select(item => new Transaction
                       {
                           ReferenceId = item.BLChargeId,
                           AccountId = Numerics.GetInt(v.AccountId),
                           InvoiceNumber = v.InvoiceNumber,
                           VoucherNumber = v.VoucherNumber,
                           TransactionType = v.TransactionType,
                           EntryType = (byte)EntryType.Item,
                           Comments = item.Description,
                           Credit = Numerics.GetInt(item.Amount)
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
                                 VoucherNumber = BL.VoucherNumber,
                                 TransactionType = v.TransactionType,
                                 EntryType = (byte)EntryType.Item,
                                 Debit = item.Amount / totalVehicle,
                                 Credit = 0,
                                 Comments = charges.FirstOrDefault(p => p.Id == blCharge.ChargeId).Name + " payment against vehicle " + vd.Name + " having BL No." + BL.BLNumber
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
        public static void Delete(int id)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var blRepo = new BLRepository();
                blRepo.Delete(id);
                blRepo.SaveChanges();
                scope.Complete();
            }
        }
    }
}
