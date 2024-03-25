using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.COA;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Transaction = AccountEx.CodeFirst.Models.Transaction;

namespace AccountEx.BussinessLogic
{
    public static class ChallanManager
    {




        public static void CreateRentChallan(Challan input)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new ChallanRepository();
                var transRepo = new TransactionRepository(repo);
                input.TransactionType = VoucherType.RC;


                //input.DueDate = DateConverter.ConvertFromDmy(SettingManager.RentDueDate);
                input.VoucherNumber = repo.GetNextVoucherNumber(VoucherType.RC);
                repo.Add(input);
                repo.SaveChanges();
                AddRentTransaction(input, repo, true);
                repo.SaveChanges();

                if (!input.IsOpening)
                {
                    var data = new RentMonthlyLiabilityRepository().GetByMonthYearTenant(input.Month, input.Year, input.ToMonth, input.ToYear, Numerics.GetInt(input.TenantAccountId), Numerics.GetInt(input.RentAgreementId));


                    var rd = data.RentDetail;
                    var rentTotal = rd.MonthlyRent + rd.RentArrears + rd.UCPercent + rd.UCPercentArears + rd.SurCharge;
                    var paidTotal = data.AllChallans.Sum(p => p.MonthlyRent + p.RentArrears + p.UCPercent + p.UCPercentArears + p.SurCharge);
                    if (paidTotal < rentTotal)
                    {

                        var alreadyChallaned = new
                        {

                            MonthlyRent = data.AllChallans.Sum(p => p.MonthlyRent),
                            UCPercent = data.AllChallans.Sum(p => p.UCPercent),
                            RentArrears = data.AllChallans.Sum(p => p.RentArrears),
                            UCPercentArears = data.AllChallans.Sum(p => p.UCPercentArears),
                            SurCharge = data.AllChallans.Sum(p => p.SurCharge),
                            NetTotal = rentTotal - paidTotal
                        };
                        var autoChallan = input.CloneWithJson();
                        autoChallan.IsAuto = true;
                        var item = autoChallan.ChallanItems.FirstOrDefault();
                        item.MonthlyRent = rd.MonthlyRent - alreadyChallaned.MonthlyRent;
                        item.RentArrears = rd.RentArrears - alreadyChallaned.RentArrears;
                        item.UCPercent = rd.UCPercent - alreadyChallaned.UCPercent;
                        item.UCPercentArears = rd.UCPercentArears - alreadyChallaned.UCPercentArears;
                        item.SurCharge = rd.SurCharge - alreadyChallaned.SurCharge;
                        autoChallan.VoucherNumber = repo.GetNextVoucherNumber(VoucherType.RC);
                        autoChallan.NetAmount = autoChallan.GrandTotal = alreadyChallaned.NetTotal;
                        autoChallan.AdjustmentAmount = 0;
                        autoChallan.AdjustmentType = 0;
                        repo.Add(autoChallan);
                        repo.SaveChanges();
                        AddRentTransaction(autoChallan, repo, true);
                    }
                }
                repo.SaveChanges();
                scope.Complete();
            }
        }

        public static void CreateElectricityChallan(Challan input)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new ChallanRepository();
                var transRepo = new TransactionRepository(repo);
                input.TransactionType = VoucherType.ElectictyChallan;
                //input.DueDate = DateConverter.ConvertFromDmy(SettingManager.RentDueDate);
                input.VoucherNumber = transRepo.GetNextVoucherNumber(VoucherType.ElectictyChallan);
                repo.Add(input);
                repo.SaveChanges();
                AddElectricityTransaction(input, repo, true);
                repo.SaveChanges();


                if (!input.IsOpening)
                {
                    var data = new RentMonthlyLiabilityRepository().GetElectrictyByMonthYearTenant(input.Month, input.Year, Numerics.GetInt(input.TenantAccountId), Numerics.GetInt(input.RentAgreementId));


                    var rd = data.ElectricityChallan;
                    var rentTotal = rd.ElectricityCharges + rd.ElectricityArrears + rd.SurCharge;
                    var paidTotal = data.AllChallans.Sum(p => p.ElectricityCharges + p.ElectricityArrears + p.SurCharge);
                    if (paidTotal < rentTotal)
                    {

                        var alreadyChallaned = new
                        {

                            ElectricityCharges = data.AllChallans.Sum(p => p.ElectricityCharges),
                            ElectricityArrears = data.AllChallans.Sum(p => p.ElectricityArrears),
                            SurCharge = data.AllChallans.Sum(p => p.SurCharge),
                            NetTotal = rentTotal - paidTotal
                        };
                        var autoChallan = input.CloneWithJson();
                        autoChallan.IsAuto = true;
                        var item = autoChallan.ChallanItems.FirstOrDefault();
                        item.ElectricityCharges = rd.ElectricityCharges - alreadyChallaned.ElectricityCharges;
                        item.ElectricityArrears = rd.ElectricityArrears - alreadyChallaned.ElectricityArrears;
                        item.SurCharge = rd.SurCharge - alreadyChallaned.SurCharge;
                        autoChallan.VoucherNumber = repo.GetNextVoucherNumber(VoucherType.ElectictyChallan);
                        autoChallan.NetAmount = autoChallan.GrandTotal = alreadyChallaned.NetTotal;
                        autoChallan.AdjustmentAmount = 0;
                        autoChallan.AdjustmentType = 0;
                        repo.Add(autoChallan);
                        repo.SaveChanges();
                        AddElectricityTransaction(autoChallan, repo, true);
                    }
                }
                repo.SaveChanges();
                scope.Complete();
            }
        }
        public static void CreateMiscChallan(Challan input)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new ChallanRepository();
                var transRepo = new TransactionRepository(repo);
                input.TransactionType = VoucherType.MiscCharges;
                input.VoucherNumber = transRepo.GetNextVoucherNumber(VoucherType.MiscCharges);
                repo.Add(input);
                AddMiscChargeTransaction(input, repo);
                repo.SaveChanges();
                scope.Complete();
            }
        }
        public static void ReceiveRentalChallan(List<ReceiveRentChallanExtra> challans)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new ChallanRepository();
                var transRepo = new TransactionRepository(repo);
                var Ids = challans.Select(p => p.Id).ToList();
                var dbchallans = repo.GetByIds(Ids);
                var voucherNo = transRepo.GetNextVoucherNumber(VoucherType.FortressBankReceipt);
                foreach (var dbChallan in dbchallans)
                {
                    var challan = challans.FirstOrDefault(p => p.Id == dbChallan.Id);
                    dbChallan.IsReceived = true;
                    dbChallan.ReceiveDate = challan.ReceiveDate;
                    dbChallan.RcvNo = voucherNo;
                    if (dbChallan.ChallanItems.Any())
                    {
                        var item = dbChallan.ChallanItems.FirstOrDefault();
                        item.IsReceived = true;
                        item.NetAmount = challan.NetAmount;
                        item.LateSurCharge = challan.LateSurCharge;
                    }
                }
                AddRentalReceivingTransaction(dbchallans, repo, voucherNo);
                repo.SaveChanges();
                scope.Complete();
            }
        }
        public static void AddRentalReceivingTransaction(List<Challan> challans, BaseRepository baseRepo, int voucherNo)
        {
            var transRepo = new TransactionRepository(baseRepo);
            var dt = DateTime.Now;
            var trans = new List<Transaction>();
            foreach (var challan in challans)
            {
                dt = challan.ReceiveDate.HasValue ? challan.ReceiveDate.Value : dt;
                var item = challan.ChallanItems.FirstOrDefault();

                if (challan.TransactionType  == VoucherType.RC)
                {
                    string monthName = new DateTime(challan.Year, challan.Month, 1).ToString("MMM, yyyy", CultureInfo.InvariantCulture);
                    var comment = "rent receiving  for tenant " + item.TenantAccountName + " for the month of " + monthName;
                    //1. Monthly Rent Credit to TenantAccount
                    trans.Add(new Transaction()
                    {
                        AccountId = item.TenantAccountId,
                        CompanyId = SiteContext.Current.User.CompanyId,
                        FiscalId = SiteContext.Current.Fiscal.Id,
                        Date = dt.Date,
                        TransactionType = VoucherType.FortressBankReceipt,
                        EntryType = (byte)EntryType.MonthlyRent,
                        VoucherNumber = voucherNo,
                        Credit = item.MonthlyRent + item.RentArrears,
                        CreatedDate = dt,
                        Comments = "Monthly Rent " + comment,
                        ReferenceId = challan.Id

                    });

                    //3. Utliliy charges Credit to TenantAccount
                    trans.Add(new Transaction()
                    {
                        AccountId = item.TenantAccountId,
                        CompanyId = SiteContext.Current.User.CompanyId,
                        FiscalId = SiteContext.Current.Fiscal.Id,
                        Date = dt.Date,
                        TransactionType = VoucherType.FortressBankReceipt,
                        EntryType = (byte)EntryType.UCPercent,
                        VoucherNumber = voucherNo,
                        Credit = item.UCPercent + item.UCPercentArears,
                        CreatedDate = dt,
                        Comments = "Utliliy charges" + comment,
                        ReferenceId = challan.Id

                    });
                    //4. Surcahrge Credit to TenantAccount
                    trans.Add(new Transaction()
                    {
                        AccountId = item.TenantAccountId,
                        CompanyId = SiteContext.Current.User.CompanyId,
                        FiscalId = SiteContext.Current.Fiscal.Id,
                        Date = dt.Date,
                        TransactionType = VoucherType.FortressBankReceipt,
                        EntryType = (byte)EntryType.Surcharge,
                        VoucherNumber = voucherNo,
                        Credit = item.LateSurCharge + item.SurCharge,
                        CreatedDate = dt,
                        Comments = "Surcharge " + comment,
                        ReferenceId = challan.Id

                    });


                    //5. Monthly Rent Debit to Bank
                    trans.Add(new Transaction()
                    {
                        AccountId = SettingManager.BankAccountId,
                        CompanyId = SiteContext.Current.User.CompanyId,
                        FiscalId = SiteContext.Current.Fiscal.Id,
                        Date = dt.Date,
                        TransactionType = VoucherType.FortressBankReceipt,
                        EntryType = (byte)EntryType.MonthlyRent,
                        VoucherNumber = voucherNo,
                        Debit = item.MonthlyRent + item.RentArrears,
                        CreatedDate = dt,
                        Comments = "Monthly Rent " + comment,
                        ReferenceId = challan.Id

                    });

                    //7. Utliliy charges Debit to Bank
                    trans.Add(new Transaction()
                    {
                        AccountId = SettingManager.BankAccountId,
                        CompanyId = SiteContext.Current.User.CompanyId,
                        FiscalId = SiteContext.Current.Fiscal.Id,
                        Date = dt.Date,
                        TransactionType = VoucherType.FortressBankReceipt,
                        EntryType = (byte)EntryType.UCPercent,
                        VoucherNumber = voucherNo,
                        Debit = item.UCPercent + item.UCPercentArears,
                        CreatedDate = dt,
                        Comments = "Utliliy charges " + comment,
                        ReferenceId = challan.Id
                    });
                    //4. Surcahrge Debit to Bank Account
                    trans.Add(new Transaction()
                    {
                        AccountId = SettingManager.BankAccountId,
                        CompanyId = SiteContext.Current.User.CompanyId,
                        FiscalId = SiteContext.Current.Fiscal.Id,
                        Date = dt.Date,
                        TransactionType = VoucherType.FortressBankReceipt,
                        EntryType = (byte)EntryType.Surcharge,
                        VoucherNumber = voucherNo,
                        Debit = item.SurCharge + item.LateSurCharge,
                        CreatedDate = dt,
                        Comments = "Surcharge " + comment,
                        ReferenceId = challan.Id

                    });
                    if (item.LateSurCharge > 0)
                    {
                        //1. Surcahrge Debit to TenantAccount
                        trans.Add(new Transaction()
                        {
                            AccountId = item.TenantAccountId,
                            CompanyId = SiteContext.Current.User.CompanyId,
                            FiscalId = SiteContext.Current.Fiscal.Id,
                            Date = dt.Date,
                            TransactionType = VoucherType.FortressBankReceipt,
                            EntryType = (byte)EntryType.Surcharge,
                            VoucherNumber = voucherNo,
                            Debit = item.LateSurCharge,
                            CreatedDate = dt,
                            Comments = "Surcharge " + comment,
                            ReferenceId = challan.Id

                        });
                        //1. Surcahrge Credit to Surcharge Account
                        trans.Add(new Transaction()
                        {
                            AccountId = item.TenantAccountId,
                            CompanyId = SiteContext.Current.User.CompanyId,
                            FiscalId = SiteContext.Current.Fiscal.Id,
                            Date = dt.Date,
                            TransactionType = VoucherType.FortressBankReceipt,
                            EntryType = (byte)EntryType.Surcharge,
                            VoucherNumber = voucherNo,
                            Credit = item.LateSurCharge,
                            CreatedDate = dt,
                            Comments = "Surcharge " + comment,
                            ReferenceId = challan.Id

                        });
                    }
                    if (challan.AdjustmentType > 0)
                    {
                        //1. Adjustment Credit/Debit to Tenant Account
                        comment = "adjustment of tenant " + item.TenantAccountName + " for the month of " + monthName;
                        trans.Add(new Transaction()
                        {
                            AccountId = item.TenantAccountId,
                            CompanyId = SiteContext.Current.User.CompanyId,
                            FiscalId = SiteContext.Current.Fiscal.Id,
                            Date = dt.Date,
                            TransactionType = VoucherType.FortressBankReceipt,
                            EntryType = (byte)EntryType.Surcharge,
                            VoucherNumber = voucherNo,
                            Credit = challan.AdjustmentType == (byte)AdjustmentType.Less ? challan.AdjustmentAmount : 0,
                            Debit = challan.AdjustmentType == (byte)AdjustmentType.Increase ? challan.AdjustmentAmount : 0,
                            CreatedDate = dt,
                            Comments = comment,
                            ReferenceId = challan.Id

                        });
                        //1. Adjustment Credit/Debit to Adjustment Account
                        trans.Add(new Transaction()
                        {
                            AccountId = SettingManager.AdjustmentHeadId,
                            CompanyId = SiteContext.Current.User.CompanyId,
                            FiscalId = SiteContext.Current.Fiscal.Id,
                            Date = dt.Date,
                            TransactionType = VoucherType.FortressBankReceipt,
                            EntryType = (byte)EntryType.Surcharge,
                            VoucherNumber = voucherNo,
                            Debit = challan.AdjustmentType == (byte)AdjustmentType.Less ? challan.AdjustmentAmount : 0,
                            Credit = challan.AdjustmentType == (byte)AdjustmentType.Increase ? challan.AdjustmentAmount : 0,
                            CreatedDate = dt,
                            Comments = comment,
                            ReferenceId = challan.Id

                        });
                    }

                }
                if (challan.TransactionType  == VoucherType.ElectictyChallan)
                {
                    string monthName = new DateTime(challan.Year, challan.Month, 1).ToString("MMM, yyyy", CultureInfo.InvariantCulture);
                    var comment = "electricity receiving  for tenant " + item.TenantAccountName + " for the month of " + monthName;
                    //2. Elecricity charges Credit to TenantAccount
                    trans.Add(new Transaction()
                    {
                        AccountId = item.TenantAccountId,
                        CompanyId = SiteContext.Current.User.CompanyId,
                        FiscalId = SiteContext.Current.Fiscal.Id,
                        Date = dt.Date,
                        TransactionType = VoucherType.FortressBankReceipt,
                        EntryType = (byte)EntryType.ElectricityCharges,
                        VoucherNumber = voucherNo,
                        Credit = item.ElectricityCharges + item.ElectricityArrears,
                        CreatedDate = dt,
                        Comments = "Elecricity charges " + comment,
                        ReferenceId = challan.Id

                    });
                    //6. Elecricity charges Debit to Bank
                    trans.Add(new Transaction()
                    {
                        AccountId = SettingManager.BankAccountId,
                        CompanyId = SiteContext.Current.User.CompanyId,
                        FiscalId = SiteContext.Current.Fiscal.Id,
                        Date = dt.Date,
                        TransactionType = VoucherType.FortressBankReceipt,
                        EntryType = (byte)EntryType.ElectricityCharges,
                        VoucherNumber = voucherNo,
                        Debit = item.ElectricityCharges + item.ElectricityArrears,
                        CreatedDate = dt,
                        Comments = "Elecricity charges " + comment,
                        ReferenceId = challan.Id

                    });

                }
                else if (challan.TransactionType  == VoucherType.PossessionCharges || challan.TransactionType  == VoucherType.SecurityMoney || challan.TransactionType  == VoucherType.MiscCharges)
                {
                    var comment = "";
                    byte entryType = (byte)EntryType.MasterDetail;
                    if (challan.TransactionType  == VoucherType.PossessionCharges)
                    {

                        comment = "Possession receiving for tenant " + item.TenantAccountName;
                        entryType = (byte)EntryType.Possession;
                    }
                    else if (challan.TransactionType  == VoucherType.SecurityMoney)
                    {

                        comment = "Security Money receiving for tenant " + item.TenantAccountName;
                        entryType = (byte)EntryType.Security;
                    }
                    else if (challan.TransactionType  == VoucherType.MiscCharges)
                    {
                        string monthName = new DateTime(challan.Year, challan.Month, 1).ToString("MMM, yyyy", CultureInfo.InvariantCulture);
                        comment = "Misc charges for tenant " + item.TenantAccountName + " for the month of " + monthName; ;
                        entryType = (byte)EntryType.Misc;
                    }
                    //1. Amount Credit to TenantAccount
                    trans.Add(new Transaction()
                    {
                        AccountId = item.TenantAccountId,
                        AccountTitle = "",
                        CompanyId = SiteContext.Current.User.CompanyId,
                        FiscalId = SiteContext.Current.Fiscal.Id,
                        Date = dt.Date,
                        TransactionType = VoucherType.FortressBankReceipt,
                        EntryType = entryType,
                        VoucherNumber = voucherNo,
                        Credit = challan.NetAmount,
                        CreatedDate = dt,
                        Comments = comment,
                        ReferenceId = challan.Id

                    });
                    //2. Amount Debit to Bank
                    trans.Add(new Transaction()
                    {
                        AccountId = SettingManager.BankAccountId,
                        AccountTitle = "",
                        CompanyId = SiteContext.Current.User.CompanyId,
                        FiscalId = SiteContext.Current.Fiscal.Id,
                        Date = dt.Date,
                        TransactionType = VoucherType.FortressBankReceipt,
                        EntryType = entryType,
                        VoucherNumber = voucherNo,
                        Debit = challan.NetAmount,
                        CreatedDate = dt,
                        Comments = comment,
                        ReferenceId = challan.Id

                    });

                }

            }
            transRepo.Add(trans);
        }
        public static void AddMiscChargeTransaction(Challan challan, BaseRepository baseRepo)
        {
            var transRepo = new TransactionRepository(baseRepo);
            var voucherNo = challan.VoucherNumber;
            var dt = DateTime.Now;
            var trans = new List<Transaction>();
            var item = challan.ChallanItems.FirstOrDefault();
            byte entryType = (byte)EntryType.MasterDetail;
            var comment = "Misc charges against tenant " + item.TenantAccountName + "." + (!string.IsNullOrWhiteSpace(item.Remarks) ? item.Remarks : "");
            entryType = (byte)EntryType.Possession;
            //1. Amount Debit to TenantAccount
            trans.Add(new Transaction()
            {
                AccountId = item.TenantAccountId,
                CompanyId = SiteContext.Current.User.CompanyId,
                FiscalId = SiteContext.Current.Fiscal.Id,
                Date = dt.Date,
                TransactionType = challan.TransactionType,
                EntryType = entryType,
                VoucherNumber = voucherNo,
                Debit = challan.NetAmount,
                CreatedDate = dt,
                Comments = comment,
                ReferenceId = challan.Id

            });
            //2. Amount Credit to Misc Charge Account
            trans.Add(new Transaction()
            {
                AccountId = Numerics.GetInt(challan.ChargeId),
                CompanyId = SiteContext.Current.User.CompanyId,
                FiscalId = SiteContext.Current.Fiscal.Id,
                Date = dt.Date,
                TransactionType = challan.TransactionType,
                EntryType = entryType,
                VoucherNumber = voucherNo,
                Credit = challan.NetAmount,
                CreatedDate = dt,
                Comments = comment,
                ReferenceId = challan.Id

            });
            transRepo.Add(trans);
        }
        public static string ValidateSave(Challan input)
        {
            return ValidateSave(input, false);
        }
        public static string ValidateSave(Challan input, bool allowDupliateItem)
        {
            return ValidateSave(input, allowDupliateItem, false);
        }
        public static string ValidateSave(Challan input, bool allowDupliateItem, bool allowDupliateBookNo)
        {
            return ValidateSave(input, allowDupliateItem, allowDupliateBookNo, true);
        }

        public static string ValidateSave(Challan input, bool allowDupliateItem, bool allowDupliateBookNo, bool isAccountRequired)
        {
            var err = ",";
            try
            {
                var challanRepo = new ChallanRepository();
                var dcRepo = new DeliveryChallanRepository(challanRepo);
                var invoiceDCRepo = new InvoiceDcRepository(challanRepo);
                var GNType = input.TransactionType  == VoucherType.Sale || input.TransactionType  == VoucherType.GstSale ? VoucherType.GoodIssue : VoucherType.GoodReceive;
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
                //if (isAccountRequired)
                //{
                //    if (input.r == 0)
                //    {
                //        err += "Account is not valid to process the request.,";
                //    }
                //}
                if (SiteContext.Current.Fiscal == null)
                {
                    err += "No fiscal year found.,";

                }
                if (SiteContext.Current.Fiscal.IsClosed)
                {
                    err += "Fiscal year is closed No action can be done.,";
                }
                //if (!FiscalYearManager.IsValidFiscalDate(input.dat))
                //{
                //    err += "Voucher date should be within current fiscal year.,";
                //}

                var isExist = challanRepo.IsVoucherExits(input.VoucherNumber, input.TransactionType, input.Id);
                if (isExist)
                {
                    err += "Voucher no already exist.,";
                }
                if (!allowDupliateBookNo)
                {
                    isExist = challanRepo.IsBookNoExits(input.InvoiceNumber, input.TransactionType, input.Id);

                    if (isExist)
                    {
                        err += "invoice no already exist.,";
                    }
                }
                if (input.Id > 0)
                {
                    var dbSale = challanRepo.GetById(input.Id);
                    if (dbSale.VoucherNumber != input.VoucherNumber)
                    {
                        err += "can't change voucher no.please use previous voucher no.(" + dbSale.VoucherNumber + "),";
                    }

                    if (challanRepo.IsChallanReceived(input.Id))
                    {
                        err += "Challan is received & could not be updated.,";
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

        public static void Delete(int id, VoucherType type)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var challanRepo = new ChallanRepository();
                var tranRepo = new TransactionRepository(challanRepo);
                tranRepo.HardDeleteByReferenceIdTransactionType(id, type);
                challanRepo.Delete(id);
                challanRepo.SaveChanges();
                scope.Complete();
            }

        }

        public static void AddRentTransaction(Challan rentDetail, BaseRepository repo, bool skipZeroCheck)
        {


            var transRepo = new TransactionRepository(repo);
            // transRepo.HardDelete(rentDetail.VoucherNumber, VoucherType.RentMonthlyLiability);
            var item = rentDetail.ChallanItems.FirstOrDefault();
            var agreement = new vw_RentAgreementsRepository(repo).GetById(rentDetail.RentAgreementId.Value);
            var dt = DateTime.Now;
            var comment = "for tenant " + agreement.TenantName + " (Shop:" + agreement.ShopCode + "-" + agreement.ShopNo + " Block:" + agreement.Block + ")";
            var trans = new List<Transaction>();

            //1. Monthly Rent Debit to TenantAccount
            trans.Add(new Transaction()
            {
                AccountId = item.TenantAccountId,
                AccountTitle = "",
                CompanyId = SiteContext.Current.User.CompanyId,
                FiscalId = SiteContext.Current.Fiscal.Id,
                Date = dt.Date,
                TransactionType = VoucherType.RC,
                EntryType = (byte)EntryType.MonthlyRent,
                VoucherNumber = item.VoucherNumber,
                Debit = item.MonthlyRent,
                CreatedDate = dt,
                Comments = "Monthly Rent " + comment,
                ReferenceId = item.Id,

            });
            //2. Monthly Rent Credit to TenantAccount
            trans.Add(new Transaction()
            {
                AccountId = SettingManager.MonthlyRentAcId,
                AccountTitle = null,
                CompanyId = SiteContext.Current.User.CompanyId,
                FiscalId = SiteContext.Current.Fiscal.Id,
                Date = dt.Date,
                TransactionType = VoucherType.RC,
                EntryType = (byte)EntryType.MonthlyRent,
                VoucherNumber = item.VoucherNumber,
                Credit = item.MonthlyRent,
                CreatedDate = dt,
                Comments = "Monthly Rent " + comment,
                ReferenceId = item.Id,

            });

            //1. Surcharge Debit to TenantAccount
            trans.Add(new Transaction()
            {
                AccountId = item.TenantAccountId,
                AccountTitle = "",
                CompanyId = SiteContext.Current.User.CompanyId,
                FiscalId = SiteContext.Current.Fiscal.Id,
                Date = dt.Date,
                TransactionType = VoucherType.RC,
                EntryType = (byte)EntryType.Surcharge,
                VoucherNumber = item.VoucherNumber,
                Debit = item.SurCharge,
                CreatedDate = dt,
                Comments = "Surcharge " + comment,
                ReferenceId = item.Id,

            });
            //2.Surcharge Credit to System Account
            trans.Add(new Transaction()
            {
                AccountId = SettingManager.SurchargeAccountId,
                AccountTitle = null,
                CompanyId = SiteContext.Current.User.CompanyId,
                FiscalId = SiteContext.Current.Fiscal.Id,
                Date = dt.Date,
                TransactionType = VoucherType.RC,
                EntryType = (byte)EntryType.Surcharge,
                VoucherNumber = item.VoucherNumber,
                Credit = item.SurCharge,
                CreatedDate = dt,
                Comments = "Surcharge " + comment,
                ReferenceId = item.Id,

            });


            //3. Utliliy charges Debit to TenantAccount
            trans.Add(new Transaction()
            {
                AccountId = item.TenantAccountId,
                AccountTitle = "",
                CompanyId = SiteContext.Current.User.CompanyId,
                FiscalId = SiteContext.Current.Fiscal.Id,
                Date = dt.Date,
                TransactionType = VoucherType.RC,
                EntryType = (byte)EntryType.UCPercent,
                VoucherNumber = item.VoucherNumber,
                Debit = item.UCPercent,
                CreatedDate = dt,
                Comments = "Utliliy charges " + comment,
                ReferenceId = item.Id,
            });
            //4. Monthly Rent Credit to Utliliy charge Acount
            trans.Add(new Transaction()
            {
                AccountId = SettingManager.UtliliychargeAcId,
                AccountTitle = null,
                CompanyId = SiteContext.Current.User.CompanyId,
                FiscalId = SiteContext.Current.Fiscal.Id,
                Date = dt.Date,
                TransactionType = VoucherType.RC,
                EntryType = (byte)EntryType.UCPercent,
                VoucherNumber = item.VoucherNumber,
                Credit = item.UCPercent,
                CreatedDate = dt,
                Comments = "Utliliy charges " + comment,
                ReferenceId = item.Id,

            });

            trans = trans.Where(p => p.Debit > 0 || p.Credit > 0).ToList();
            transRepo.Add(trans, skipZeroCheck);
        }
        public static void AddElectricityTransaction(Challan rentDetail, BaseRepository repo, bool skipZeroCheck = false)
        {


            var transRepo = new TransactionRepository(repo);
            // transRepo.HardDelete(rentDetail.VoucherNumber, VoucherType.RentMonthlyLiability);
            var item = rentDetail.ChallanItems.FirstOrDefault();
            var trans = new List<Transaction>();
            var agreement = new vw_RentAgreementsRepository(repo).GetById(rentDetail.RentAgreementId.Value);
            var dt = DateTime.Now;
            var comment = "for tenant " + agreement.TenantName + " (Shop:" + agreement.ShopCode + "-" + agreement.ShopNo + " Block:" + agreement.Block + ")";


            //2. Elecricity charges Debit to TenantAccount
            trans.Add(new Transaction()
            {
                AccountId = item.TenantAccountId,
                AccountTitle = "",
                CompanyId = SiteContext.Current.User.CompanyId,
                FiscalId = SiteContext.Current.Fiscal.Id,
                Date = dt.Date,
                TransactionType = VoucherType.ElectictyChallan,
                EntryType = (byte)EntryType.ElectricityCharges,
                VoucherNumber = rentDetail.VoucherNumber,
                InvoiceNumber = rentDetail.VoucherNumber,
                Debit = item.ElectricityCharges,
                CreatedDate = dt,
                Comments = "Elecricity charges  " + comment,
                ReferenceId = rentDetail.Id,

            });
            //2. Electricity charges Credit to Electricity charge Account
            trans.Add(new Transaction()
            {
                AccountId = SettingManager.ElectricitychargeAcId,
                AccountTitle = "",
                CompanyId = SiteContext.Current.User.CompanyId,
                FiscalId = SiteContext.Current.Fiscal.Id,
                Date = dt.Date,
                TransactionType = VoucherType.ElectictyChallan,
                EntryType = (byte)EntryType.ElectricityCharges,
                VoucherNumber = rentDetail.VoucherNumber,
                InvoiceNumber = rentDetail.VoucherNumber,
                Credit = item.ElectricityCharges,
                CreatedDate = dt,
                Comments = "Elecricity charges  " + comment,
                ReferenceId = rentDetail.Id,

            });

            //1. Surcharge Debit to TenantAccount
            trans.Add(new Transaction()
            {
                AccountId = item.TenantAccountId,
                AccountTitle = "",
                CompanyId = SiteContext.Current.User.CompanyId,
                FiscalId = SiteContext.Current.Fiscal.Id,
                Date = dt.Date,
                TransactionType = VoucherType.ElectictyChallan,
                EntryType = (byte)EntryType.Surcharge,
                VoucherNumber = item.VoucherNumber,
                Debit = item.SurCharge,
                CreatedDate = dt,
                Comments = "Surcharge " + comment,
                ReferenceId = item.Id,

            });
            //2.Surcharge Credit to System Account
            trans.Add(new Transaction()
            {
                AccountId = SettingManager.SurchargeAccountId,
                AccountTitle = null,
                CompanyId = SiteContext.Current.User.CompanyId,
                FiscalId = SiteContext.Current.Fiscal.Id,
                Date = dt.Date,
                TransactionType = VoucherType.ElectictyChallan,
                EntryType = (byte)EntryType.Surcharge,
                VoucherNumber = item.VoucherNumber,
                Credit = item.SurCharge,
                CreatedDate = dt,
                Comments = "Surcharge " + comment,
                ReferenceId = item.Id,

            });



            trans = trans.Where(p => p.Debit > 0 || p.Credit > 0).ToList();
            transRepo.Add(trans, skipZeroCheck);
        }
    }
}
