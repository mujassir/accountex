using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.Vehicles;
using AccountEx.Common;
using AccountEx.Common.VehicleSystem;
using AccountEx.Repositories;
using Entities.CodeFirst;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transaction = AccountEx.CodeFirst.Models.Transaction;

namespace AccountEx.BussinessLogic
{
    public static class VehicleSaleManager
    {
        public static void Save(VehicleSale input)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new VehicleSaleRepository();
                var vehicleRepo = new VehicleRepository(repo);
                input.TransactionType = VoucherType.Sale;

                if (input.Id == 0)
                {
                    input.VoucherNumber = repo.GetNextVoucherNumber(input.TransactionType);
                    repo.Add(input);
                }
                else
                    repo.Update(input);
                var vehicle = vehicleRepo.GetById(input.VehicleId);
                vehicle.IsSale = true;
                //AddTransaction(input, repo);
                repo.SaveChanges();
                scope.Complete();
            }
        }
        public static ApiResponse CancellDeal(VehicleSale input)
        {

            ApiResponse response;
            try
            {


                using (var scope = TransactionScopeBuilder.Create())
                {
                    var repo = new VehicleSaleRepository();
                    var vehicleRepo = new VehicleRepository(repo);
                    input.TransactionType = VoucherType.Sale;

                    var sale = repo.GetById(input.Id);
                    var vehicle = vehicleRepo.GetById(input.VehicleId);
                    var tradeVehicle = new Vehicle();
                    if (sale.IsTradeIn)
                        tradeVehicle = vehicleRepo.GetById(Numerics.GetInt(sale.TradeInVehicleId));
                    var err = ValidateCancelDeal(input, sale, tradeVehicle);
                    if (err == "")
                    {
                        sale.IsTradeUnitReturned = sale.IsTradeIn ? input.IsTradeUnitReturned : false;
                        sale.DepreciationAmount = input.DepreciationAmount;
                        sale.ReturnAmount = input.ReturnAmount;
                        sale.ReturnPaymentMode = input.ReturnPaymentMode;
                        sale.ReturnAccountId = input.ReturnAccountId;
                        sale.RecoveryStatus = (byte)RecoveryStatus.SaleReturn;
                        sale.CancellationDate = input.CancellationDate;

                        vehicle.IsSale = false;
                        vehicle.Status = (byte)VehicleStatus.SaleReturn;
                        if (sale.IsTradeIn)
                        {

                            if (!tradeVehicle.IsSale)
                            {
                                if (input.IsTradeUnitReturned)
                                {
                                    tradeVehicle.Status = (byte)VehicleStatus.PurchaseReturn;

                                }
                            }
                        }
                        AddDealCancellTransaction(sale, input, tradeVehicle, repo);
                        repo.SaveChanges();
                        scope.Complete();
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
        public static void IssuePossessionLetter(VehicleAcution input)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new VehicleAcutionRepository();
                var vehicleRepo = new VehicleRepository(repo);
                var saleRepo = new VehicleSaleRepository(repo);
                if (input.Id == 0)
                {

                    repo.Add(input);
                }
                else
                    repo.Update(input);
                saleRepo.UpdateRecoveryStatus(input.SaleId, (byte)RecoveryStatus.InProcess);
                repo.SaveChanges();
                scope.Complete();
            }
        }
        public static PrintRepossessionLetter PrintPossessionLetter(int saleId)
        {

            var repo = new VehicleAcutionRepository();
            return repo.PrintPossessionLetter(saleId);

        }
        public static PrintFurtherAgreement PrintFurtherAgreement(int saleId)
        {

            var repo = new VehicleAcutionRepository();
            return repo.PrintFurtherAgreement(saleId);

        }
        public static void CreateAdvertisement(VehicleAcution input)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new VehicleAcutionRepository();
                var vehicleRepo = new VehicleRepository(repo);
                var saleRepo = new VehicleSaleRepository(repo);
                var dbAuction = repo.GetBySaleId(input.SaleId);
                if (dbAuction != null)
                {
                    dbAuction.Newspaper = input.Newspaper;
                    //dbAuction.AdvertisementDays = input.AdvertisementDays;
                    dbAuction.AdvertisementDate = input.AdvertisementDate;


                }
                saleRepo.UpdateRecoveryStatus(input.SaleId, (byte)RecoveryStatus.Advertisement);
                repo.SaveChanges();
                scope.Complete();
            }
        }
        public static void CreateNotification(VehicleAcution input)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new VehicleAcutionRepository();
                var vehicleRepo = new VehicleRepository(repo);
                var saleRepo = new VehicleSaleRepository(repo);
                var dbAuction = repo.GetBySaleId(input.SaleId);
                if (dbAuction != null)
                {

                    dbAuction.NotificationDays = input.NotificationDays;



                }
                saleRepo.UpdateRecoveryStatus(input.SaleId, (byte)RecoveryStatus.NotficationLetter);
                repo.SaveChanges();
                scope.Complete();
            }
        }
        public static void FinalizeAuctionnerCharges(VehicleAcution input)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new VehicleAcutionRepository();
                var vehicleRepo = new VehicleRepository(repo);
                var saleRepo = new VehicleSaleRepository(repo);
                var dbAuction = repo.GetBySaleId(input.SaleId);
                if (dbAuction != null)
                {

                    dbAuction.Charges = input.Charges;
                    dbAuction.IsChargesFinalized = true;
                    dbAuction.AuctionerFinalizationDate = input.AuctionerFinalizationDate;
                }
                AddAuctionnerTransaction(dbAuction, repo);
                repo.SaveChanges();
                scope.Complete();
            }
        }
        public static void AddPenalty(VehiclePenalty input)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new VehiclePenaltyRepository();
                var vehicleRepo = new VehicleRepository(repo);
                var saleRepo = new VehicleSaleRepository(repo);
                if (input.Id == 0)
                {

                    repo.Add(input);
                }
                else
                    repo.Update(input);
                saleRepo.UpdateRecoveryOnCustomerReturn(input.VehicleSaleId, (byte)RecoveryStatus.CustomerReturn, input.Remarks);
                AddPenaltyTransaction(input, repo);
                repo.SaveChanges();
                scope.Complete();
            }
        }
        public static void MarkFinal(int id)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new VehicleSaleRepository();
                var vehicleSale = repo.GetById(id);
                if (vehicleSale.IsFinal)
                {
                    vehicleSale.IsFinal = false;
                    AddTransaction(vehicleSale, repo, true);
                }
                else
                {
                    vehicleSale.IsFinal = true;
                    AddTransaction(vehicleSale, repo, false);

                }

                repo.SaveChanges();
                scope.Complete();

            }
        }
        public static void MarkDelivery(int id)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new VehicleSaleRepository();
                var vehicleSale = repo.GetById(id);
                vehicleSale.IsDelivered = true;
                vehicleSale.DeliveredBy = SiteContext.Current.User.Id;
                vehicleSale.DeliveredDate = DateTime.Now;
                //vehicleSale.DeliveredBy = SiteContext.Current.User.Id;
                repo.SaveChanges();
                scope.Complete();

            }
        }
        public static void MarkVoid(int id)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new VehicleSaleRepository();
                var vehicleSale = repo.GetById(id);
                if (!vehicleSale.IsVoid)
                {
                    vehicleSale.IsVoid = true;
                    vehicleSale.VoidBy = SiteContext.Current.User.Id;
                    vehicleSale.VoidDate = DateTime.Now;
                }
                else
                {
                    vehicleSale.IsVoid = false;
                    vehicleSale.VoidBy = null;
                    vehicleSale.VoidDate = null;
                }
                //vehicleSale.DeliveredBy = SiteContext.Current.User.Id;
                repo.SaveChanges();
                scope.Complete();
            }



        }


        public static void AddTransaction(VehicleSale s, BaseRepository repo, bool deleteOnly)
        {
            var dt = DateTime.Now;
            var transRepo = new TransactionRepository(repo);
            var vehicleRepo = new VehicleRepository(repo);
            var types = new List<VoucherType>();
            types.Add(VoucherType.VSD);
            var vehicle = vehicleRepo.GetVehicleDetailForBLTransaction(new List<int> { s.VehicleId }).FirstOrDefault();
            var rcvVocuherNo = transRepo.GetNextVoucherNumber(types);

            //Delete Sale Transactions
            transRepo.HardDelete(s.VoucherNumber, s.TransactionType);

            //Delete receiveing Transactions
            transRepo.HardDeleteByReferenceIdTransactionType(s.Id, types);
            if (!deleteOnly)
            {
                var trans = new List<Transaction>
            {
                new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = SettingManager.SaleAccountHeadId,
                    ToAccountId = s.AccountId,
                    TransactionType =s.TransactionType,
                    EntryType = (byte) EntryType.MasterDetail,
                    Quantity = 1,
                    Credit=s.SalePrice,
                    ReferenceId=s.Id,
                    Comments=vehicle.Name,
                    Date =  s.Date,
                    BranchId = s.BranchId,

                },
                 new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = s.AccountId,
                    ToAccountId = SettingManager.SaleAccountHeadId,
                    TransactionType =s.TransactionType,
                    EntryType = (byte) EntryType.MasterDetail,
                    Quantity = 1,
                    Debit=s.SalePrice,
                    ReferenceId=s.Id,
                    MainEntityId = s.VehicleId,
                    Comments=vehicle.Name,
                    Date =  s.Date,
                    BranchId = s.BranchId,
                },

                new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = SettingManager.LogBookAcAcccountId,
                     ToAccountId = s.AccountId,
                    TransactionType =s.TransactionType,
                    EntryType = (byte) EntryType.MasterDetail,
                    Quantity = 1,
                    Credit=s.LogBookFee,
                    ReferenceId=s.Id,
                    Comments = "logboook fee against " + vehicle.Name,
                    Date =  s.Date,
                    BranchId = s.BranchId,
                },
                 new Transaction
                {
                    InvoiceNumber = s.InvoiceNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = s.AccountId,
                    ToAccountId = SettingManager.LogBookAcAcccountId,
                    TransactionType =s.TransactionType,
                    EntryType = (byte) EntryType.MasterDetail,
                    Quantity = 1,
                    Debit=s.LogBookFee,
                    ReferenceId=s.Id,
                    MainEntityId = s.VehicleId,
                    Comments = "logboook fee against " + vehicle.Name,
                    Date =  s.Date,
                    BranchId = s.BranchId,
                },
               
            };
                //foreach (var item in s.VehicleSaleDetails)
                //{

                //    var comment = "Due against " + vehicle.Name + " for installment no. " + item.InstalmentNo + " having due date " + item.InstallmentDate.ToString("dd-MM-yyyy");
                //    trans.Add(
                //         new Transaction
                //    {
                //        InvoiceNumber = s.InvoiceNumber,
                //        VoucherNumber = s.VoucherNumber,
                //        AccountId = s.AccountId,
                //        TransactionType = s.TransactionType,
                //        EntryType = (byte)EntryType.Installement,
                //        Quantity = 1,
                //        Debit = item.Amount,
                //        ReferenceId = s.Id,
                //        Comments = comment,
                //        MainEntityId = s.VehicleId,
                //        Date = item.InstallmentDate
                //    });
                //}


                foreach (var item in s.VehicleSaleDeposits)
                {


                    trans.Add(new Transaction
                     {
                         InvoiceNumber = rcvVocuherNo,
                         VoucherNumber = rcvVocuherNo,
                         AccountId = s.AccountId,
                         ToAccountId = Numerics.GetInt(item.AccountId),
                         TransactionType = VoucherType.VSD,
                         EntryType = (byte)EntryType.Receiving,
                         Quantity = 1,
                         Credit = item.Amount,
                         Comments = "Payment against " + vehicle.Name,
                         ReferenceId = s.Id,
                         MainEntityId = s.VehicleId,
                         Date = item.Date,
                         BranchId = s.BranchId,
                     });
                    trans.Add(new Transaction
                    {
                        InvoiceNumber = rcvVocuherNo,
                        VoucherNumber = rcvVocuherNo,
                        AccountId = Numerics.GetInt(item.AccountId),
                        ToAccountId = s.AccountId,
                        TransactionType = VoucherType.VSD,
                        EntryType = (byte)EntryType.Receiving,
                        Quantity = 1,
                        Debit = item.Amount,
                        Comments = "Payment against " + vehicle.Name,
                        ReferenceId = s.Id,
                        Date = item.Date,
                        BranchId = s.BranchId,
                    });
                }
                if (s.IsTrackerAdded)
                {
                    trans.Add(new Transaction
                    {
                        InvoiceNumber = s.VoucherNumber,
                        VoucherNumber = s.VoucherNumber,
                        AccountId = Numerics.GetInt(s.TrackerSupplierId),
                        ToAccountId = SettingManager.TrackerPurchaseAcccountId,
                        TransactionType = s.TransactionType,
                        EntryType = (byte)EntryType.TrackerPurchase,
                        Quantity = 1,
                        Credit = s.TrackerSellingPrice,
                        Comments = "Tracker Purchase against " + vehicle.Name,
                        ReferenceId = s.Id,
                        MainEntityId = s.Id,
                        Date = s.Date,
                        BranchId = s.BranchId,
                    });
                    trans.Add(new Transaction
                    {
                        InvoiceNumber = s.VoucherNumber,
                        VoucherNumber = s.VoucherNumber,
                        AccountId = SettingManager.TrackerPurchaseAcccountId,
                        ToAccountId = Numerics.GetInt(s.TrackerSupplierId),
                        TransactionType = s.TransactionType,
                        EntryType = (byte)EntryType.TrackerPurchase,
                        Quantity = 1,
                        Debit = s.TrackerSellingPrice,
                        Comments = "Tracker Purchase against " + vehicle.Name,
                        ReferenceId = s.Id,
                        MainEntityId = s.Id,
                        Date = s.Date,
                        BranchId = s.BranchId,
                    });

                    ///Tracker Receiveable against customer
                    trans.Add(new Transaction
                    {
                        InvoiceNumber = s.VoucherNumber,
                        VoucherNumber = s.VoucherNumber,
                        AccountId = Numerics.GetInt(s.AccountId),
                        ToAccountId = SettingManager.TrackerSaleAcccountId,
                        TransactionType = s.TransactionType,
                        EntryType = (byte)EntryType.TrackerSale,
                        Quantity = 1,
                        Debit = s.TrackerSalePrice,
                        Comments = "Tracker Receivable against " + vehicle.Name,
                        ReferenceId = s.Id,
                        MainEntityId = s.VehicleId,
                        Date = s.Date,
                        BranchId = s.BranchId,
                    });
                    trans.Add(new Transaction
                    {
                        InvoiceNumber = s.VoucherNumber,
                        VoucherNumber = s.VoucherNumber,
                        AccountId = SettingManager.TrackerSaleAcccountId,
                        ToAccountId = Numerics.GetInt(s.AccountId),
                        TransactionType = s.TransactionType,
                        EntryType = (byte)EntryType.TrackerSale,
                        Quantity = 1,
                        Credit = s.TrackerSalePrice,
                        Comments = "Tracker Receivable  against " + vehicle.Name,
                        ReferenceId = s.Id,
                        Date = s.Date,
                        BranchId = s.BranchId,
                    });
                }
                if (s.IsInsurenceAdded)
                {
                    trans.Add(new Transaction
                    {
                        InvoiceNumber = s.VoucherNumber,
                        VoucherNumber = s.VoucherNumber,
                        AccountId = Numerics.GetInt(s.InsurenceSupplierId),
                        ToAccountId = SettingManager.InsurancePurchaseAccountId,
                        TransactionType = s.TransactionType,
                        EntryType = (byte)EntryType.Insurance,
                        Quantity = 1,
                        Credit = s.InsurenceSellingPrice,
                        Comments = "Insurance against " + vehicle.Name,
                        ReferenceId = s.Id,
                        Date = s.Date,
                        BranchId = s.BranchId,
                    });
                    trans.Add(new Transaction
                    {
                        InvoiceNumber = s.VoucherNumber,
                        VoucherNumber = s.VoucherNumber,
                        AccountId = SettingManager.InsurancePurchaseAccountId,
                        ToAccountId = Numerics.GetInt(s.InsurenceSupplierId),
                        TransactionType = s.TransactionType,
                        EntryType = (byte)EntryType.Insurance,
                        Quantity = 1,
                        Debit = s.InsurenceSellingPrice,
                        Comments = "Insurance against " + vehicle.Name,
                        ReferenceId = s.Id,
                        Date = s.Date,
                        BranchId = s.BranchId,
                    });

                    ///Insurance Receiveable against customer
                    trans.Add(new Transaction
                    {
                        InvoiceNumber = s.VoucherNumber,
                        VoucherNumber = s.VoucherNumber,
                        AccountId = Numerics.GetInt(s.AccountId),
                        ToAccountId = SettingManager.InsuranceSaleAccountId,
                        TransactionType = s.TransactionType,
                        EntryType = (byte)EntryType.InsuranceSale,
                        Quantity = 1,
                        Debit = s.InsurenceSalePrice,
                        Comments = "Insurance Receivable against " + vehicle.Name,
                        ReferenceId = s.Id,
                        MainEntityId = s.VehicleId,
                        Date = s.Date,
                        BranchId = s.BranchId,
                    });
                    trans.Add(new Transaction
                    {
                        InvoiceNumber = s.VoucherNumber,
                        VoucherNumber = s.VoucherNumber,
                        AccountId = SettingManager.InsuranceSaleAccountId,
                        ToAccountId = Numerics.GetInt(s.AccountId),
                        TransactionType = s.TransactionType,
                        EntryType = (byte)EntryType.InsuranceSale,
                        Quantity = 1,
                        Credit = s.InsurenceSalePrice,
                        Comments = "Insurance Receivable  against " + vehicle.Name,
                        ReferenceId = s.Id,
                        Date = s.Date,
                        BranchId = s.BranchId,
                    });

                }
                if (s.TrackerReceivedAmount > 0)
                {
                    trans.Add(new Transaction
                    {
                        InvoiceNumber = rcvVocuherNo,
                        VoucherNumber = rcvVocuherNo,
                        AccountId = s.AccountId,
                        ToAccountId = Numerics.GetInt(s.TrackerAccountId),
                        TransactionType = VoucherType.VSD,
                        EntryType = (byte)EntryType.Receiving,
                        Quantity = 1,
                        Credit = s.TrackerReceivedAmount,
                        Comments = "tracker receipt against " + vehicle.Name,
                        ReferenceId = s.Id,
                        MainEntityId = s.VehicleId,
                        Date = s.Date,
                        BranchId = s.BranchId,
                    });
                    trans.Add(new Transaction
                    {
                        InvoiceNumber = rcvVocuherNo,
                        VoucherNumber = rcvVocuherNo,
                        AccountId = Numerics.GetInt(s.TrackerAccountId),
                        ToAccountId = s.AccountId,
                        TransactionType = VoucherType.VSD,
                        EntryType = (byte)EntryType.Receiving,
                        Quantity = 1,
                        Debit = s.TrackerReceivedAmount,
                        Comments = "tracker receipt against " + vehicle.Name,
                        ReferenceId = s.Id,
                        Date = s.Date,
                        BranchId = s.BranchId,
                    });
                }
                if (s.InsurenceReceivedAmount > 0)
                {
                    trans.Add(new Transaction
                    {
                        InvoiceNumber = rcvVocuherNo,
                        VoucherNumber = rcvVocuherNo,
                        AccountId = s.AccountId,
                        ToAccountId = Numerics.GetInt(s.InsuranceAccountId),
                        TransactionType = VoucherType.VSD,
                        EntryType = (byte)EntryType.Receiving,
                        Quantity = 1,
                        Credit = s.InsurenceReceivedAmount,
                        Comments = "insurance receipt against " + vehicle.Name,
                        ReferenceId = s.Id,
                        MainEntityId = s.VehicleId,
                        Date = s.Date,
                        BranchId = s.BranchId,
                    });
                    trans.Add(new Transaction
                    {
                        InvoiceNumber = rcvVocuherNo,
                        VoucherNumber = rcvVocuherNo,
                        AccountId = Numerics.GetInt(s.InsuranceAccountId),
                        ToAccountId = s.AccountId,
                        TransactionType = VoucherType.VSD,
                        EntryType = (byte)EntryType.Receiving,
                        Quantity = 1,
                        Debit = s.InsurenceReceivedAmount,
                        Comments = "insurance receipt against " + vehicle.Name,
                        ReferenceId = s.Id,
                        Date = s.Date,
                        BranchId = s.BranchId,
                    });
                }


                foreach (var item in trans)
                {

                    item.CreatedDate = dt;
                    item.FiscalId = SiteContext.Current.Fiscal.Id;
                }
                transRepo.Add(trans);
            }
        }
        public static void AddDealCancellTransaction(VehicleSale dbSale, VehicleSale input, Vehicle tradeVehicle, BaseRepository repo)
        {
            var dt = dbSale.CancellationDate.HasValue ? dbSale.CancellationDate.Value : DateTime.Now;
            var transRepo = new TransactionRepository(repo);
            var vehicleRepo = new VehicleRepository(repo);
            var vehicle = vehicleRepo.GetVehicleDetailForBLTransaction(new List<int> { dbSale.VehicleId }).FirstOrDefault();
            var TradevehicleforComment = vehicleRepo.GetVehicleDetailForBLTransaction(new List<int> { Numerics.GetInt(dbSale.TradeInVehicleId) }).FirstOrDefault();
            var voucherType = VoucherType.SaleReturn;
            var voucherNo = transRepo.GetNextVoucherNumber(VoucherType.SaleReturn);

            var trans = new List<Transaction>
            {
                new Transaction
                {
                    InvoiceNumber = voucherNo,
                    VoucherNumber = voucherNo,
                    AccountId = SettingManager.SaleReturnAccountHeadId,
                    TransactionType =voucherType,
                    EntryType = (byte) EntryType.MasterDetail,
                    Quantity = 1,
                    Debit=dbSale.SalePrice,
                    ReferenceId=dbSale.Id,
                    Comments="sale return"+vehicle.Name,
                    Date =  dt,
                    BranchId=dbSale.BranchId

                },
                 new Transaction
                {
                    InvoiceNumber =voucherNo,
                    VoucherNumber =voucherNo,
                    AccountId = dbSale.AccountId,
                    TransactionType =voucherType,
                    EntryType = (byte) EntryType.MasterDetail,
                    Quantity = 1,
                    Credit=dbSale.SalePrice,
                    ReferenceId=dbSale.Id,
                    MainEntityId = dbSale.VehicleId,
                    Comments="sale return"+vehicle.Name,
                    Date =  dt,
                     BranchId=dbSale.BranchId
                },

                new Transaction
                {
                    InvoiceNumber =voucherNo,
                    VoucherNumber =voucherNo,
                    AccountId = SettingManager.LogBookAcAcccountId,
                    TransactionType =voucherType,
                    EntryType = (byte) EntryType.MasterDetail,
                    Quantity = 1,
                    Debit=dbSale.LogBookFee,
                    ReferenceId=dbSale.Id,
                    Comments = "logboook fee return against " + vehicle.Name,
                   Date =  dt,
                    BranchId=dbSale.BranchId
                },
                 new Transaction
                {
                    InvoiceNumber =voucherNo,
                    VoucherNumber =voucherNo,
                    AccountId = dbSale.AccountId,
                    TransactionType =voucherType,
                    EntryType = (byte) EntryType.MasterDetail,
                    Quantity = 1,
                    Credit=dbSale.LogBookFee,
                    ReferenceId=dbSale.Id,
                    MainEntityId = dbSale.VehicleId,
                    Comments = "logboook fee return against " + vehicle.Name,
                   Date =  dt,
                    BranchId=dbSale.BranchId
                },
               
            };

            if (dbSale.IsTrackerAdded)
            {
                trans.Add(new Transaction
                {
                    InvoiceNumber = voucherNo,
                    VoucherNumber = voucherNo,
                    AccountId = Numerics.GetInt(dbSale.TrackerSupplierId),
                    TransactionType = voucherType,
                    EntryType = (byte)EntryType.TrackerPurchase,
                    Quantity = 1,
                    Debit = dbSale.TrackerSellingPrice,
                    Comments = "Tracker Purchase return against " + vehicle.Name,
                    ReferenceId = dbSale.Id,
                    Date = dt,
                    BranchId = dbSale.BranchId

                });
                trans.Add(new Transaction
                {
                    InvoiceNumber = voucherNo,
                    VoucherNumber = voucherNo,
                    AccountId = SettingManager.TrackerPurchaseAcccountId,
                    TransactionType = voucherType,
                    EntryType = (byte)EntryType.TrackerPurchase,
                    Quantity = 1,
                    Credit = dbSale.TrackerSellingPrice,
                    Comments = "Tracker Purchase return against " + vehicle.Name,
                    ReferenceId = dbSale.Id,
                    Date = dt,
                    BranchId = dbSale.BranchId
                });

                ///Tracker Receiveable against customer
                trans.Add(new Transaction
                {
                    InvoiceNumber = voucherNo,
                    VoucherNumber = voucherNo,
                    AccountId = Numerics.GetInt(dbSale.AccountId),
                    TransactionType = voucherType,
                    EntryType = (byte)EntryType.TrackerSale,
                    Quantity = 1,
                    Credit = dbSale.TrackerSalePrice,
                    Comments = "Tracker return against " + vehicle.Name,
                    ReferenceId = dbSale.Id,
                    MainEntityId = dbSale.VehicleId,
                    Date = dt,
                    BranchId = dbSale.BranchId
                });
                trans.Add(new Transaction
                {
                    InvoiceNumber = voucherNo,
                    VoucherNumber = voucherNo,
                    AccountId = SettingManager.TrackerSaleAcccountId,
                    TransactionType = voucherType,
                    EntryType = (byte)EntryType.TrackerSale,
                    Quantity = 1,
                    Debit = dbSale.TrackerSalePrice,
                    Comments = "Tracker return  against " + vehicle.Name,
                    ReferenceId = dbSale.Id,
                    Date = dt,
                    BranchId = dbSale.BranchId
                });
            }
            if (dbSale.IsInsurenceAdded)
            {
                trans.Add(new Transaction
                {
                    InvoiceNumber = voucherNo,
                    VoucherNumber = voucherNo,
                    AccountId = Numerics.GetInt(dbSale.InsurenceSupplierId),
                    TransactionType = voucherType,
                    EntryType = (byte)EntryType.Insurance,
                    Quantity = 1,
                    Debit = dbSale.InsurenceSellingPrice,
                    Comments = "Insurance return against " + vehicle.Name,
                    ReferenceId = dbSale.Id,
                    Date = dt,
                    BranchId = dbSale.BranchId
                });
                trans.Add(new Transaction
                {
                    InvoiceNumber = voucherNo,
                    VoucherNumber = voucherNo,
                    AccountId = SettingManager.InsurancePurchaseAccountId,
                    TransactionType = voucherType,
                    EntryType = (byte)EntryType.Insurance,
                    Quantity = 1,
                    Credit = dbSale.InsurenceSellingPrice,
                    Comments = "Insurance return against " + vehicle.Name,
                    ReferenceId = dbSale.Id,
                    Date = dt,
                    BranchId = dbSale.BranchId
                });

                ///Insurance Receiveable against customer
                trans.Add(new Transaction
                {
                    InvoiceNumber = voucherNo,
                    VoucherNumber = voucherNo,
                    AccountId = Numerics.GetInt(dbSale.AccountId),
                    TransactionType = voucherType,
                    EntryType = (byte)EntryType.InsuranceSale,
                    Quantity = 1,
                    Credit = dbSale.InsurenceSalePrice,
                    Comments = "Insurance return against " + vehicle.Name,
                    ReferenceId = dbSale.Id,
                    MainEntityId = dbSale.VehicleId,
                    Date = dt,
                    BranchId = dbSale.BranchId
                });
                trans.Add(new Transaction
                {
                    InvoiceNumber = voucherNo,
                    VoucherNumber = voucherNo,
                    AccountId = SettingManager.InsuranceSaleAccountId,
                    TransactionType = voucherType,
                    EntryType = (byte)EntryType.InsuranceSale,
                    Quantity = 1,
                    Debit = dbSale.InsurenceSalePrice,
                    Comments = "Insurance return  against " + vehicle.Name,
                    ReferenceId = dbSale.Id,
                    Date = dt,
                    BranchId = dbSale.BranchId
                });


            }
            //Deperciation Amount/Transactions
            trans.Add(new Transaction
            {
                InvoiceNumber = voucherNo,
                VoucherNumber = voucherNo,
                AccountId = dbSale.AccountId,
                TransactionType = voucherType,
                EntryType = (byte)EntryType.Depriciation,
                Quantity = 1,
                Debit = dbSale.ReturnAmount,
                Comments = "Deposit return " + vehicle.Name,
                ReferenceId = dbSale.Id,
                MainEntityId = dbSale.VehicleId,
                Date = dt,
                BranchId = dbSale.BranchId
            });
            trans.Add(new Transaction
            {
                InvoiceNumber = voucherNo,
                VoucherNumber = voucherNo,
                AccountId = Numerics.GetInt(dbSale.ReturnAccountId),
                TransactionType = voucherType,
                EntryType = (byte)EntryType.Receiving,
                Quantity = 1,
                Credit = dbSale.ReturnAmount,
                Comments = "Deposit return " + vehicle.Name,
                ReferenceId = dbSale.Id,
                Date = dt,
                BranchId = dbSale.BranchId
            });

            trans.Add(new Transaction
            {
                InvoiceNumber = voucherNo,
                VoucherNumber = voucherNo,
                AccountId = dbSale.AccountId,
                TransactionType = voucherType,
                EntryType = (byte)EntryType.Depriciation,
                Quantity = 1,
                Debit = input.DepreciationAmount,
                Comments = "Depreciation against " + vehicle.Name,
                ReferenceId = dbSale.Id,
                MainEntityId = dbSale.VehicleId,
                Date = dt,
                BranchId = dbSale.BranchId
            });
            trans.Add(new Transaction
            {
                InvoiceNumber = voucherNo,
                VoucherNumber = voucherNo,
                AccountId = SettingManager.DepriciationAcccountId,
                TransactionType = voucherType,
                EntryType = (byte)EntryType.Receiving,
                Quantity = 1,
                Credit = input.DepreciationAmount,
                Comments = "Depreciation against " + vehicle.Name,
                ReferenceId = dbSale.Id,
                Date = dt,
                BranchId = dbSale.BranchId
            });

            if (dbSale.IsTradeIn && input.IsTradeUnitReturned)
            {
                trans.Add(new Transaction
                {
                    AccountId = Numerics.GetInt(dbSale.AccountId),
                    InvoiceNumber = voucherNo,
                    VoucherNumber = voucherNo,
                    Date = dt,
                    TransactionType = voucherType,
                    EntryType = (byte)EntryType.Item,
                    Debit = dbSale.TradeInPrice,
                    FiscalId = SiteContext.Current.Fiscal.Id,
                    Comments = "trade unit return against " + TradevehicleforComment.Name,
                    ReferenceId = dbSale.Id,
                    MainEntityId = dbSale.TradeInVehicleId,
                    BranchId = dbSale.BranchId

                });
                trans.Add(new Transaction
                {
                    AccountId = SettingManager.TradeInAcccountId,
                    InvoiceNumber = voucherNo,
                    VoucherNumber = voucherNo,
                    Date = dt,
                    TransactionType = voucherType,
                    EntryType = (byte)EntryType.Item,
                    Credit = dbSale.TradeInPrice,
                    FiscalId = SiteContext.Current.Fiscal.Id,
                    Comments = "trade unit return against " + TradevehicleforComment.Name,
                    ReferenceId = dbSale.Id,
                    BranchId = dbSale.BranchId
                });
            }
            foreach (var item in trans)
            {

                item.CreatedDate = dt;
                item.FiscalId = SiteContext.Current.Fiscal.Id;
            }
            transRepo.Add(trans);
        }

        public static void AddPenaltyTransaction(VehiclePenalty vp, BaseRepository repo)
        {
            var dt = DateTime.Now;
            var transRepo = new TransactionRepository(repo);
            var saleRepo = new VehicleSaleRepository(repo);
            var vehicleRepo = new VehicleRepository(repo);
            var customerId = saleRepo.GetCustomerId(vp.VehicleSaleId);
            var branchId = saleRepo.GetBranchId(vp.VehicleSaleId);
            var voucherNo = transRepo.GetNextVoucherNumber(VoucherType.Penalty);
            var vehicle = vehicleRepo.GetVehicleDetailForBLTransaction(new List<int> { vp.VehicleId }).FirstOrDefault();
            var comments = "Penalty against " + vehicle.Name;
            var trans = new List<Transaction>
            {
                new Transaction
                {
                    InvoiceNumber = voucherNo,
                    VoucherNumber = voucherNo,
                    AccountId = SettingManager.PenaltyHeadId,
                    TransactionType = VoucherType.Penalty,
                    EntryType = (byte) EntryType.MasterDetail,
                    Quantity = 1,
                    Credit=vp.Amount,
                    ReferenceId=vp.Id,
                    Comments=comments,
                    BranchId=branchId

                },
                new Transaction
                {
                    InvoiceNumber = voucherNo,
                    VoucherNumber = voucherNo,
                    AccountId = customerId,
                    TransactionType = VoucherType.Penalty,
                    EntryType = (byte) EntryType.Item,
                    Quantity = 1,
                    Debit=vp.Amount,
                    ReferenceId=vp.Id,
                    MainEntityId=vp.VehicleId,
                    Comments=comments,
                    BranchId=branchId
                },
            };




            foreach (var item in trans)
            {
                item.VoucherNumber = voucherNo;
                item.CreatedDate = dt;
                item.Date = DateTime.Now;
                item.FiscalId = SiteContext.Current.Fiscal.Id;
            }
            transRepo.Add(trans, true);
        }
        public static void AddAuctionnerTransaction(VehicleAcution va, BaseRepository repo)
        {
            var dt = va.AuctionerFinalizationDate.HasValue ? va.AuctionerFinalizationDate.Value : DateTime.Now;
            var transRepo = new TransactionRepository(repo);
            var saleRepo = new VehicleSaleRepository(repo);
            var vehicleRepo = new VehicleRepository(repo);
            var customerId = saleRepo.GetCustomerId(va.SaleId);
            var branchId = saleRepo.GetBranchId(va.SaleId);
            var voucherNo = transRepo.GetNextVoucherNumber(VoucherType.AuctionnerCharges);
            var vehicle = vehicleRepo.GetVehicleDetailForBLTransaction(new List<int> { va.VehicleId }).FirstOrDefault();
            transRepo.HardDeleteByReferenceIdTransactionType(va.Id, VoucherType.AuctionnerCharges);
            var comments = "Auctionner charges against " + vehicle.Name;
            var trans = new List<Transaction>
            {
                new Transaction
                {
                    InvoiceNumber = voucherNo,
                    VoucherNumber = voucherNo,
                    AccountId = va.AcutionerId,
                    TransactionType = VoucherType.AuctionnerCharges,
                    EntryType = (byte) EntryType.MasterDetail,
                    Quantity = 1,
                    Credit=va.Charges,
                    ReferenceId=va.Id,
                    Comments=comments,
                    BranchId=branchId

                },
                new Transaction
                {
                    InvoiceNumber = voucherNo,
                    VoucherNumber = voucherNo,
                    AccountId = customerId,
                    TransactionType = VoucherType.AuctionnerCharges,
                    EntryType = (byte) EntryType.Item,
                    Quantity = 1,
                    Debit=va.Charges,
                    ReferenceId=va.Id,
                    MainEntityId=va.VehicleId,
                    Comments=comments,
                    BranchId=branchId
                },
            };




            foreach (var item in trans)
            {
                item.VoucherNumber = voucherNo;
                item.CreatedDate = dt;
                item.Date = dt;
                item.FiscalId = SiteContext.Current.Fiscal.Id;
            }
            transRepo.Add(trans);
        }

        public static void PayInstallments(VehicleInstallmentPayment input)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new VehicleSaleDetailRepository();
                var vehiclepaymentRepo = new VehiclePaymentRepository(repo);
                var saleRepo = new VehicleSaleRepository(repo);
                var vehicleRepo = new VehicleRepository(repo);
                var paymentRepo = new VehicleInstallmentPaymentRepository(repo);
                var customerId = saleRepo.GetCustomerId(input.VehicleSaleId);
                var vehicleId = saleRepo.GetVehicleId(input.VehicleSaleId);
                var branchId = vehicleRepo.GetBranchId(vehicleId);
                var installments = repo.GetPendingInstalmentByVehicleSaleId(input.VehicleSaleId).OrderBy(p => p.InstallmentDate);
                var totalAmount = input.Amount;
                var totaldiscount = input.Discount;
                var breakLoop = false;
                input.VehicleId = vehicleId;
                var ListVIP = new List<VehicleInstallmentPayment>();
                var json = JsonConvert.SerializeObject(input);
                var vehiclePayment = JsonConvert.DeserializeObject<VehiclePayment>(json);
                foreach (var dbInstallment in installments)
                {
                    var installmentPayment = input.CloneWithJson();

                    //var totalPaid = paymentRepo.GetTotalPaid(dbInstallment.Id);
                    var pending = dbInstallment.Amount - dbInstallment.RecievedAmount;
                    var installmentAmount = pending;
                    var discount = 0.0;
                    if (totalAmount > pending)
                    {
                        totalAmount = totalAmount - pending;

                    }
                    else
                    {
                        breakLoop = true;
                        installmentAmount = totalAmount;
                    }
                    if (totaldiscount > 0)
                    {
                        totaldiscount = totaldiscount - pending;
                        installmentPayment.Amount = installmentAmount;
                    }


                    installmentPayment.Amount = installmentAmount;
                    installmentPayment.Discount = 0;
                    installmentPayment.VehicleSaleDetailId = dbInstallment.Id;
                    ListVIP.Add(installmentPayment);
                    dbInstallment.RecievedAmount = installmentAmount + dbInstallment.RecievedAmount;
                    if (dbInstallment.Amount - dbInstallment.RecievedAmount <= 0)
                        dbInstallment.IsRecieved = true;

                    if (breakLoop)
                        break;

                }

                vehiclepaymentRepo.Add(vehiclePayment, false, false);
                repo.SaveChanges();
                ListVIP.ForEach(p => p.VehiclePaymentId = vehiclePayment.Id);
                paymentRepo.Add(ListVIP);
                PayInstallmentsTransaction(input, repo, customerId, vehiclePayment.Id, branchId);
                repo.SaveChanges();
                scope.Complete();
            }

        }
        public static ApiResponse DeleteInstallments(int id)
        {
            ApiResponse response;
            var err = ValidateInstallmentDelete(id);
            if (err == "")
            {
                using (var scope = TransactionScopeBuilder.Create())
                {
                    var repo = new VehicleSaleDetailRepository();
                    var vehiclepaymentRepo = new VehiclePaymentRepository(repo);
                    var input = vehiclepaymentRepo.GetById(id);
                    var installmentPaymentRepo = new VehicleInstallmentPaymentRepository(repo);
                    var InstallemtnPayments = installmentPaymentRepo.GetByPaymentId(id);
                    var saleDetailIds = InstallemtnPayments.Select(p => p.VehicleSaleDetailId).ToList();
                    //var saleDetailIds = new List<int>();
                    var installments = repo.GetUnPaidByIds(saleDetailIds).OrderBy(p => p.InstallmentDate);
                    var totalAmount = input.Amount;
                    foreach (var dbInstallment in installments)
                    {
                        var installemtPayment = InstallemtnPayments.FirstOrDefault(p => p.VehicleSaleDetailId == dbInstallment.Id);
                        var installmentAmount = installemtPayment.Amount;
                        dbInstallment.RecievedAmount = dbInstallment.RecievedAmount - installmentAmount;
                        if (dbInstallment.Amount - dbInstallment.RecievedAmount <= 0)
                            dbInstallment.IsRecieved = true;
                        else
                            dbInstallment.IsRecieved = false;
                    }


                    DeleteInstallmentsTransaction(repo, input.Id);
                    vehiclepaymentRepo.Delete(input.Id);
                    installmentPaymentRepo.Delete(input.Id, "VehiclePaymentId");
                    repo.SaveChanges();
                    scope.Complete();
                }
                response = new ApiResponse()
                {
                    Success = true,
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
            return response;
        }
        public static void SaveDocuments(List<SaleDocument> documents)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {

                var repo = new VehicleSaleDocumentRepository();
                repo.Add(documents, false, false);
                repo.SaveChanges();
                repo.SaveChanges();
                scope.Complete();
            }

        }
        public static void PayInstallmentsByRecovery(VehicleInstallmentPayment input)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new VehicleSaleDetailRepository();
                var saleRepo = new VehicleSaleRepository(repo);
                saleRepo.UpdateRecoveryStatus(input.VehicleSaleId, (byte)RecoveryStatus.CustomerReturn);
                var paymentRepo = new VehicleInstallmentPaymentRepository(repo);
                var customerId = saleRepo.GetCustomerId(input.VehicleSaleId);
                var branchId = saleRepo.GetBranchId(input.VehicleSaleId);
                var vehicleId = saleRepo.GetVehicleId(input.VehicleSaleId);
                var installments = repo.GetPendingInstalmentByVehicleSaleId(input.VehicleSaleId).OrderBy(p => p.InstallmentDate);
                var totalAmount = input.Amount + input.Discount;
                var totaldiscount = input.Discount;
                var breakLoop = false;
                input.VehicleId = vehicleId;
                foreach (var dbInstallment in installments)
                {
                    var installmentPayment = input.CloneWithJson();
                    //var totalPaid = paymentRepo.GetTotalPaid(dbInstallment.Id);
                    var pending = dbInstallment.Amount - dbInstallment.RecievedAmount;
                    var installmentAmount = pending;
                    var discount = 0.0;
                    if (totalAmount > pending)
                    {
                        totalAmount = totalAmount - pending;


                    }
                    else
                    {
                        breakLoop = true;
                        installmentAmount = totalAmount;
                    }
                    //if (totaldiscount > 0)
                    //{
                    //    totaldiscount = totaldiscount - pending;
                    //    installmentPayment.Amount = installmentAmount;
                    //}


                    installmentPayment.Amount = installmentAmount;
                    installmentPayment.VehicleSaleDetailId = dbInstallment.Id;
                    paymentRepo.Add(installmentPayment);
                    dbInstallment.RecievedAmount = installmentAmount + dbInstallment.RecievedAmount;
                    if (dbInstallment.Amount - dbInstallment.RecievedAmount <= 0)
                        dbInstallment.IsRecieved = true;

                    if (breakLoop)
                        break;

                }

                PayInstallmentsTransaction(input, repo, customerId, input.VehicleSaleId, branchId);
                repo.SaveChanges();
                scope.Complete();
            }

        }
        public static void PayInstallmentsTransaction(VehicleInstallmentPayment s, BaseRepository repo, int customerId, int referenceId, int branchId)
        {

            var dt = DateTime.Now;
            var transRepo = new TransactionRepository(repo);
            var types = new List<VoucherType>() { VoucherType.VCR, VoucherType.VBR };
            var voucherNo = new TransactionRepository(repo).GetNextVoucherNumber(types);
            var trans = new List<Transaction>
            {
                 new Transaction
                {
                    InvoiceNumber =voucherNo,
                    VoucherNumber = voucherNo,
                    AccountId = customerId,
                    Date=s.RecievedDate,
                     TransactionType = s.PaymentMode == "Bank" ? VoucherType.VBR : VoucherType.VCR,
                    EntryType = (byte) EntryType.Installement,
                    Quantity = 1,
                    Credit=s.Amount-s.Discount,
                    Comments=s.Remarks,
                    ReferenceId=referenceId,
                    MainEntityId=s.VehicleId,
                    BranchId=branchId
                    
                },
                new Transaction
                {
                    InvoiceNumber =voucherNo,
                    VoucherNumber = voucherNo,
                    AccountId = Numerics.GetInt(s.RcvAccountId),
                    Date=s.RecievedDate,
                   TransactionType = s.PaymentMode == "Bank" ? VoucherType.VBR : VoucherType.VCR,
                    EntryType = (byte) EntryType.Installement,
                    Quantity = 1,
                    Debit=s.Amount-s.Discount,
                    Comments=s.Remarks,
                     ReferenceId=referenceId,
                    BranchId=branchId
                },
               
            };
            if (s.Discount > 0)
            {
                trans.Add(
                  new Transaction
                {
                    InvoiceNumber = voucherNo,
                    VoucherNumber = voucherNo,
                    AccountId = customerId,
                    Date = s.RecievedDate,
                    TransactionType = s.PaymentMode == "Bank" ? VoucherType.VBR : VoucherType.VCR,
                    EntryType = (byte)EntryType.Discount,
                    Quantity = 1,
                    Credit = s.Discount,
                    Comments = s.Remarks,
                    ReferenceId = referenceId,
                    MainEntityId = s.VehicleId,
                    BranchId = branchId
                });
                trans.Add(new Transaction
                {
                    InvoiceNumber = voucherNo,
                    VoucherNumber = voucherNo,
                    AccountId = SettingManager.DiscountAccountId,
                    Date = s.RecievedDate,
                    TransactionType = s.PaymentMode == "Bank" ? VoucherType.VBR : VoucherType.VCR,
                    EntryType = (byte)EntryType.Discount,
                    Quantity = 1,
                    Debit = s.Discount,
                    Comments = s.Remarks,
                    ReferenceId = referenceId,
                    BranchId = branchId
                });
            }

            foreach (var item in trans)
            {
                item.CreatedDate = dt;
                item.FiscalId = SiteContext.Current.Fiscal.Id;
            }
            transRepo.Add(trans);
        }
        public static void DeleteInstallmentsTransaction(BaseRepository repo, int referenceId)
        {

            var transRepo = new TransactionRepository(repo);
            var types = new List<VoucherType>() { VoucherType.VCR, VoucherType.VBR };
            new TransactionRepository(repo).HardDeleteByReferenceIdTransactionType(referenceId, types);
        }

        public static ApiResponse Delete(int id)
        {
            ApiResponse response;
            var err = ValidateDelete(id);
            if (err == "")
            {
                var repo = new VehicleSaleRepository();
                var vehicleRepo = new VehicleRepository(repo);
                using (var scope = TransactionScopeBuilder.Create())
                {
                    var sale = repo.GetById(id);
                    if (sale != null)
                    {

                        var vehicle = vehicleRepo.GetById(sale.VehicleId);
                        vehicle.IsSale = false;
                        repo.Delete(sale);
                    }
                    repo.SaveChanges();
                    scope.Complete();
                }

                response = new ApiResponse()
                {
                    Success = true,
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
            return response;
        }


        private static string ValidateInstallmentDelete(int id)
        {
            var err = ",";
            try
            {
                var saleRepo = new VehicleSaleRepository();
                var paymentRepo = new VehiclePaymentRepository(saleRepo);
                var auctionRepo = new VehicleAcutionRepository(saleRepo);







            }
            catch (Exception ex)
            {
                ErrorManager.Log(ex);
                err = "error in data validation";
            }
            err = err.Trim(',');
            return err;


        }
        private static string ValidateDelete(int id)
        {
            var err = ",";
            try
            {
                var saleRepo = new VehicleSaleRepository();
                var paymentRepo = new VehiclePaymentRepository(saleRepo);
                var auctionRepo = new VehicleAcutionRepository(saleRepo);

                if (paymentRepo.IsExistBySaleId(id))
                {
                    err += "Agreement is linked with installment payment and can't be deleted.,";
                }
                if (auctionRepo.IsExistBySaleId(id))
                {
                    err += "Agreement is linked with auction and can't be deleted.,";
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
        public static string ValidateCancelDeal(VehicleSale input, VehicleSale dbSale, Vehicle tradeVehicle)
        {
            var err = ",";
            try
            {
                var saleRepo = new VehicleSaleRepository();

                if (tradeVehicle.IsSale && input.IsTradeUnitReturned)
                {
                    err += "Trade unit sold, It can't be return.,";
                }
                if (dbSale.RecoveryStatus == (byte)RecoveryStatus.SaleReturn)
                {
                    err += "Deal is already cancelled.,";
                }
                if (input.ReturnAccountId == 0)
                {
                    err += "account is required.,";
                }
                if (SettingManager.DepriciationAcccountId == 0)
                {
                    err += "depriciation account is missing.,";
                }

                if (input.ReturnAccountId == 0)
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
