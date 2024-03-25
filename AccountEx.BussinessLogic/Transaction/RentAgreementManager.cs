using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.COA;
using AccountEx.Common;
using AccountEx.DbMapping;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AccountEx.BussinessLogic
{
    public static class RentAgreementManager
    {
        public static void Update(RentAgreementExtra input)
        {

            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new RentAgreementRepository();
                var repoRentScheduleItem = new RentAgreementScheduleRepository(repo);
                var transactionRepo = new TransactionRepository(repo);
                var scheduleRepo = new RentAgreementScheduleRepository(repo);
                var challanItemRepo = new ChallanItemRepository(repo);
                var agreement = repo.GetById(input.RentAgreements.Id, true);

                agreement.SecurityMoneyAmount = input.RentAgreements.SecurityMoneyAmount;
                agreement.ReceivedSecurityAmount = input.RentAgreements.ReceivedSecurityAmount;
                agreement.SecurityInstallment = input.RentAgreements.SecurityInstallment;
                agreement.SecurityPerInstallment = input.RentAgreements.SecurityPerInstallment;
                agreement.SecurityBalance = input.RentAgreements.SecurityBalance;
                agreement.IsActive = input.RentAgreements.IsActive;


                agreement.AlreadyPaidPossessionAmount = input.RentAgreements.AlreadyPaidPossessionAmount;
                agreement.NotPaidPossessionAmount = input.RentAgreements.NotPaidPossessionAmount;
                


                
                agreement.TotalPossessionAmount = input.RentAgreements.TotalPossessionAmount;
                agreement.PossessionInstallment = input.RentAgreements.PossessionInstallment;
                agreement.PossessionPerInstallment = input.RentAgreements.PossessionPerInstallment;

                agreement.PossessionReceived = input.RentAgreements.PossessionReceived;
                agreement.PossessionBalance = input.RentAgreements.PossessionBalance;




                var Ids = input.RentAgreements.RentAgreementSchedules.Select(p => p.Id).ToList();
                var deletedIds = agreement.RentAgreementSchedules.Where(p => !Ids.Contains(p.Id)).Select(p => p.Id).ToList();
                repoRentScheduleItem.Delete(deletedIds);
                repoRentScheduleItem.Save(input.RentAgreements.RentAgreementSchedules.ToList());
                if (input.IsRenew)
                {
                    scheduleRepo.RenewSchedules(input.RentAgreements);
                }
                else
                    scheduleRepo.UpdateSchedules(input.RentAgreements);

                //var challanItem = challanItemRepo.FirstOrDefault(p => p.RentAgreementId == agreement.Id
                //                        && p.TransactionType == VoucherType.RentAgreement, false);
                ////challanItem.Id = 0;
                //challanItem.NetAmount = input.ExtraSecurityAmount;

                //challanItemRepo.Save(challanItem);

                var trans = new List<Transaction>();

                var dbAgreement = new vw_RentAgreementsRepository().GetById(agreement.Id);
                //var comment = " for tenant " + dbAgreement.TenantName + " (" + dbAgreement.ShopCode + "-" + dbAgreement.ShopNo + "-" + dbAgreement.Block + ")";
                //var transactionComment = input.IsRenew ? "renew" : "editing" + comment;
                //if (input.ExtraSecurityAmount > 0)
                //{
                //    trans.Add(new Transaction
                //            {
                //                ReferenceId = agreement.Id,
                //                InvoiceNumber = agreement.VoucherNumber,
                //                VoucherNumber = agreement.VoucherNumber,
                //                AccountId = SettingManager.SecurityMoneyId,
                //                TransactionType = VoucherType.RentAgreement,
                //                EntryType = (byte)EntryType.Automatic,
                //                Quantity = 1,
                //                Debit = input.ExtraSecurityType == (byte)AdjustmentType.Less ? input.ExtraSecurityAmount : 0,
                //                Credit = input.ExtraSecurityType == (byte)AdjustmentType.Increase ? input.ExtraSecurityAmount : 0,
                //                Comments = "Security money adjustment after " + transactionComment
                //            });
                //    trans.Add(new Transaction
                //            {
                //                ReferenceId = agreement.Id,
                //                InvoiceNumber = agreement.VoucherNumber,
                //                VoucherNumber = agreement.VoucherNumber,
                //                AccountId = agreement.TenantAccountId,
                //                TransactionType = VoucherType.RentAgreement,
                //                EntryType = (byte)EntryType.Automatic,
                //                Quantity = 1,
                //                Credit = input.ExtraSecurityType == (byte)AdjustmentType.Less ? input.ExtraSecurityAmount : 0,
                //                Debit = input.ExtraSecurityType == (byte)AdjustmentType.Increase ? input.ExtraSecurityAmount : 0,
                //                Comments = "Security money adjustment after " + transactionComment
                //            });
                //}
                //if (input.ExtraPossessionAmount > 0)
                //{
                //    trans.Add(new Transaction
                //        {
                //            ReferenceId = agreement.Id,
                //            InvoiceNumber = agreement.VoucherNumber,
                //            VoucherNumber = agreement.VoucherNumber,
                //            AccountId = SettingManager.PossessionChargesId,
                //            TransactionType = VoucherType.RentAgreement,
                //            EntryType = (byte)EntryType.Automatic,
                //            Quantity = 1,
                //            Debit = input.ExtraPossessionType == (byte)AdjustmentType.Less ? input.ExtraPossessionAmount : 0,
                //            Credit = input.ExtraPossessionType == (byte)AdjustmentType.Increase ? input.ExtraPossessionAmount : 0,
                //            Comments = "Possession charges adjustment after " + transactionComment
                //        });
                //    trans.Add(new Transaction
                //        {
                //            ReferenceId = agreement.Id,
                //            InvoiceNumber = agreement.VoucherNumber,
                //            VoucherNumber = agreement.VoucherNumber,
                //            AccountId = agreement.TenantAccountId,
                //            TransactionType = VoucherType.RentAgreement,
                //            EntryType = (byte)EntryType.Automatic,
                //            Quantity = 1,
                //            Credit = input.ExtraPossessionType == (byte)AdjustmentType.Less ? input.ExtraPossessionAmount : 0,
                //            Debit = input.ExtraPossessionType == (byte)AdjustmentType.Increase ? input.ExtraPossessionAmount : 0,
                //            Comments = "Extra Possession charges after " + transactionComment
                //        });
                //}
                //var dt = DateTime.Now;
                //foreach (var item in trans)
                //{
                //    item.CreatedDate = dt;
                //    item.Date = dt.Date;
                //    item.FiscalId = SiteContext.Current.Fiscal.Id;
                //}
                repo.Update(agreement, true, false);
                AddTransaction(agreement, repo);
                //transactionRepo.Add(trans, true);
                repo.SaveChanges();
                scope.Complete();
            }
        }

        public static void Save(RentAgreement input)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new RentAgreementRepository();
                var challanItemRepo = new ChallanItemRepository(repo);
                var challanRepo = new ChallanRepository(repo);
                if (input.Id == 0)
                {
                    repo.Add(input, true, false);
                }

                repo.SaveChanges();
                AddTransaction(input, repo);
                repo.SaveChanges();
                scope.Complete();
            }

        }
        public static ApiResponse Transfeer(RentAgreement input)
        {

            ApiResponse response;
            try
            {


                using (var scope = TransactionScopeBuilder.Create())
                {
                    var repo = new RentAgreementRepository();
                    var dbAgreement = repo.GetById(input.Id);
                    var agreementState = new AgreementEditingState();
                    agreementState = repo.GetAgreementEditingState(dbAgreement.Id);
                    var err = ValidateTransfeer(input, dbAgreement);

                    if (err == "")
                    {


                        dbAgreement.TransfeerFee = input.TransfeerFee;
                        dbAgreement.ProcessingFee = input.ProcessingFee;
                        dbAgreement.TransfeerDate = input.TransfeerDate;
                        dbAgreement.TransfeerRemarks = input.TransfeerRemarks;
                        dbAgreement.Status = (byte)AgreementStatus.Transfeer;

                        ///Create clone of current agreement
                        var newAgreement = dbAgreement.CloneWithJson();
                        newAgreement.Id = 0;
                        newAgreement.TransfeerDate = null;
                        newAgreement.TransfeerRemarks = null;
                        newAgreement.VoucherNumber = repo.GetNextVoucherNumber();

                        newAgreement.TenantAccountId = input.TenantAccountId;
                        newAgreement.TransfeerAgreementId = dbAgreement.Id;
                        newAgreement.ReceivedSecurityAmount += agreementState.SecurityAmountReceived;
                        newAgreement.SecurityBalance = newAgreement.SecurityMoneyAmount - newAgreement.ReceivedSecurityAmount;

                        newAgreement.PossessionReceived += agreementState.PossessionAmountReceived;
                        newAgreement.PossessionBalance = newAgreement.TotalPossessionAmount - newAgreement.PossessionReceived;
                        newAgreement.Status = (byte)AgreementStatus.Default;
                        repo.Add(newAgreement);
                        repo.SaveChanges();
                        AddTransfeerTransaction(newAgreement, dbAgreement, repo);
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
        public static string ValidateTransfeer(RentAgreement input, RentAgreement dbAgreement)
        {
            var err = ",";
            try
            {
                var saleRepo = new VehicleSaleRepository();



                if (input.TenantAccountId == 0)
                {
                    err += "Tenant is required.,";
                }
                if (input.TenantAccountId == dbAgreement.TenantAccountId)
                {
                    err += "Transfeer and current tenant can't be same,";
                }


                if (input.TransfeerFee == 0)
                {
                    err += "Transfeer fee should be greater than zero.,";
                }
                if (input.ProcessingFee == 0)
                {
                    err += "Processing fee should be greater than zero.,";
                }
                if (dbAgreement.Status == (byte)AgreementStatus.Transfeer)
                {
                    err += "This agreement has alfreday been transfeered.,";
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
        public static void AddTransaction(RentAgreement s, BaseRepository baseRepo)
        {
            var agreement = new vw_RentAgreementsRepository().GetById(s.Id);
            var dt = DateTime.Now;
            var repo = new TransactionRepository(baseRepo);
            var possessionVoucherNo = repo.GetNextVoucherNumber(VoucherType.PossessionCharges);
            var securityVoucherNo = repo.GetNextVoucherNumber(VoucherType.SecurityMoney);
            var comment = "for tenant " + agreement.TenantName + " (" + agreement.ShopCode + "-" + agreement.ShopNo + "-" + agreement.Block + ")";
            var trans = new List<Transaction>
            {
                new Transaction
                {
                   
                    InvoiceNumber = possessionVoucherNo,
                    VoucherNumber = possessionVoucherNo,
                    AccountId = SettingManager.PossessionChargesId,
                    TransactionType = VoucherType.PossessionCharges,
                    EntryType = (byte) EntryType.Automatic,
                    Quantity = 1,
                    Debit =0,
                    Credit =s.PossessionBalance,
                    ReferenceId=s.Id,
                    Comments="Possession charges liability on agreement creation "+comment,
                },
                new Transaction
                {
                    InvoiceNumber = possessionVoucherNo,
                    VoucherNumber = possessionVoucherNo,
                    AccountId = s.TenantAccountId,
                    TransactionType = VoucherType.PossessionCharges,
                    EntryType = (byte) EntryType.Automatic,
                    Quantity = 1,
                    Debit =s.PossessionBalance,
                    Credit =0,
                    ReferenceId=s.Id,
                    Comments="Possession charges liability on agreement creation "+comment,
                },

                 new Transaction
                {
                    InvoiceNumber = securityVoucherNo,
                    VoucherNumber = securityVoucherNo,
                    AccountId = SettingManager.SecurityMoneyId,
                    TransactionType = VoucherType.SecurityMoney,
                    EntryType = (byte) EntryType.Automatic,
                    Quantity = 1,
                    Debit =0,
                    Credit =s.SecurityBalance,
                    ReferenceId=s.Id,
                    Comments="Security money liability on agreement creation "+comment,
                },
                new Transaction
                {
                    InvoiceNumber =securityVoucherNo,
                    VoucherNumber = securityVoucherNo,
                    AccountId = s.TenantAccountId,
                    TransactionType = VoucherType.SecurityMoney,
                    EntryType = (byte) EntryType.Automatic,
                    Quantity = 1,
                    Debit =s.SecurityBalance,
                    Credit =0,
                     ReferenceId=s.Id,
                    Comments="Security money liability on agreement creation "+comment,
                }
            };

            ////  For Receiving(Possession) Transactions, if exists
            //if (s.PossessionReceived > 0)
            //{
            //    repo.HardDelete(s.VoucherNumber, VoucherType.FortressBankReceipt);
            //    trans.Add(new Transaction()
            //     {
            //         ReferenceId = s.Id,
            //         InvoiceNumber = s.VoucherNumber,
            //         VoucherNumber = s.VoucherNumber,
            //         AccountId = s.TenantAccountId,
            //         TransactionType = VoucherType.FortressBankReceipt,
            //         EntryType = (byte)EntryType.Automatic,
            //         Debit = s.PossessionReceived,
            //         Comments = "Possession's money received from",
            //     });
            //    trans.Add(new Transaction()
            //    {
            //        ReferenceId = s.Id,
            //        InvoiceNumber = s.VoucherNumber,
            //        VoucherNumber = s.VoucherNumber,
            //        AccountId = SettingManager.BankHeadId,
            //        TransactionType = VoucherType.FortressBankReceipt,
            //        EntryType = (byte)EntryType.Automatic,
            //        Credit = s.PossessionReceived,
            //        Comments = "Possession's money received from",
            //    });
            //}
            ////  For Receiving(Security) Transactions, if exists
            //if (s.ReceivedSecurityAmount > 0)
            //{
            //    repo.HardDelete(s.VoucherNumber, VoucherType.FortressBankReceipt);
            //    trans.Add(new Transaction()
            //    {
            //        ReferenceId = s.Id,
            //        InvoiceNumber = s.VoucherNumber,
            //        VoucherNumber = s.VoucherNumber,
            //        AccountId = s.TenantAccountId,
            //        TransactionType = VoucherType.FortressBankReceipt,
            //        EntryType = (byte)EntryType.Automatic,
            //        Debit = s.ReceivedSecurityAmount,
            //        Comments = "Security's money received from",
            //    });
            //    trans.Add(new Transaction()
            //    {
            //        ReferenceId = s.Id,
            //        InvoiceNumber = s.VoucherNumber,
            //        VoucherNumber = s.VoucherNumber,
            //        AccountId = SettingManager.BankHeadId,
            //        TransactionType = VoucherType.FortressBankReceipt,
            //        EntryType = (byte)EntryType.Automatic,
            //        Credit = s.ReceivedSecurityAmount,
            //        Comments = "Security's money received from",
            //    });
            //}

            s.VoucherNumber = s.VoucherNumber;
            foreach (var item in trans)
            {
                item.CreatedDate = dt;
                item.Date = dt.Date;
                item.FiscalId = SiteContext.Current.Fiscal.Id;
            }
            repo.Add(trans, true);
        }

        public static void AddTransfeerTransaction(RentAgreement s, RentAgreement oldAgreement, BaseRepository baseRepo)
        {
            var agreement = new vw_RentAgreementsRepository().GetById(s.Id);
            var dt = DateTime.Now;
            var repo = new TransactionRepository(baseRepo);
            var possessionVoucherNo = repo.GetNextVoucherNumber(VoucherType.PossessionCharges);
            var securityVoucherNo = repo.GetNextVoucherNumber(VoucherType.SecurityMoney);
            var comment = "for tenant " + agreement.TenantName + " (" + agreement.ShopCode + "-" + agreement.ShopNo + "-" + agreement.Block + ")";
            var comment1 = "to tenant " + agreement.TenantName + " (" + agreement.ShopCode + "-" + agreement.ShopNo + "-" + agreement.Block + ")";
            var trans = new List<Transaction>
            {
                new Transaction
                {
                   
                    InvoiceNumber = possessionVoucherNo,
                    VoucherNumber = possessionVoucherNo,
                    AccountId = oldAgreement.TenantAccountId,
                    TransactionType = VoucherType.PossessionCharges,
                    EntryType = (byte) EntryType.Automatic,
                    Quantity = 1,
                    Debit =0,
                    Credit =s.PossessionBalance,
                    ReferenceId=s.Id,
                    Comments="Possession charges transffer "+comment1,
                    
                    
                },
                new Transaction
                {
                    InvoiceNumber = possessionVoucherNo,
                    VoucherNumber = possessionVoucherNo,
                    AccountId = s.TenantAccountId,
                    TransactionType = VoucherType.PossessionCharges,
                    EntryType = (byte) EntryType.Automatic,
                    Quantity = 1,
                    Debit =s.PossessionBalance,
                    Credit =0,
                    ReferenceId=s.Id,
                    Comments="Possession charges liability on agreement creation "+comment,
                },

                 new Transaction
                {
                    InvoiceNumber = securityVoucherNo,
                    VoucherNumber = securityVoucherNo,
                    AccountId = oldAgreement.TenantAccountId,
                    TransactionType = VoucherType.SecurityMoney,
                    EntryType = (byte) EntryType.Automatic,
                    Quantity = 1,
                    Debit =0,
                    Credit =s.SecurityBalance,
                    ReferenceId=s.Id,
                    Comments="Security money transfeer "+comment1,
                },
                new Transaction
                {
                    InvoiceNumber =securityVoucherNo,
                    VoucherNumber = securityVoucherNo,
                    AccountId = s.TenantAccountId,
                    TransactionType = VoucherType.SecurityMoney,
                    EntryType = (byte) EntryType.Automatic,
                    Quantity = 1,
                    Debit =s.SecurityBalance,
                    Credit =0,
                     ReferenceId=s.Id,
                    Comments="Security money liability on agreement creation "+comment,
                }
            };

            ////  For Receiving(Possession) Transactions, if exists
            //if (s.PossessionReceived > 0)
            //{
            //    repo.HardDelete(s.VoucherNumber, VoucherType.FortressBankReceipt);
            //    trans.Add(new Transaction()
            //     {
            //         ReferenceId = s.Id,
            //         InvoiceNumber = s.VoucherNumber,
            //         VoucherNumber = s.VoucherNumber,
            //         AccountId = s.TenantAccountId,
            //         TransactionType = VoucherType.FortressBankReceipt,
            //         EntryType = (byte)EntryType.Automatic,
            //         Debit = s.PossessionReceived,
            //         Comments = "Possession's money received from",
            //     });
            //    trans.Add(new Transaction()
            //    {
            //        ReferenceId = s.Id,
            //        InvoiceNumber = s.VoucherNumber,
            //        VoucherNumber = s.VoucherNumber,
            //        AccountId = SettingManager.BankHeadId,
            //        TransactionType = VoucherType.FortressBankReceipt,
            //        EntryType = (byte)EntryType.Automatic,
            //        Credit = s.PossessionReceived,
            //        Comments = "Possession's money received from",
            //    });
            //}
            ////  For Receiving(Security) Transactions, if exists
            //if (s.ReceivedSecurityAmount > 0)
            //{
            //    repo.HardDelete(s.VoucherNumber, VoucherType.FortressBankReceipt);
            //    trans.Add(new Transaction()
            //    {
            //        ReferenceId = s.Id,
            //        InvoiceNumber = s.VoucherNumber,
            //        VoucherNumber = s.VoucherNumber,
            //        AccountId = s.TenantAccountId,
            //        TransactionType = VoucherType.FortressBankReceipt,
            //        EntryType = (byte)EntryType.Automatic,
            //        Debit = s.ReceivedSecurityAmount,
            //        Comments = "Security's money received from",
            //    });
            //    trans.Add(new Transaction()
            //    {
            //        ReferenceId = s.Id,
            //        InvoiceNumber = s.VoucherNumber,
            //        VoucherNumber = s.VoucherNumber,
            //        AccountId = SettingManager.BankHeadId,
            //        TransactionType = VoucherType.FortressBankReceipt,
            //        EntryType = (byte)EntryType.Automatic,
            //        Credit = s.ReceivedSecurityAmount,
            //        Comments = "Security's money received from",
            //    });
            //}

            s.VoucherNumber = s.VoucherNumber;
            foreach (var item in trans)
            {
                item.CreatedDate = dt;
                item.Date = dt.Date;
                item.FiscalId = SiteContext.Current.Fiscal.Id;
            }
            repo.Add(trans, true);
        }


        public static void Delete(int id, int voucherNumber)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new RentAgreementRepository();
                var transactionRepo = new TransactionRepository(repo);
                var challanItemRepo = new ChallanItemRepository(repo);
                var challanRepo = new ChallanRepository(repo);
                transactionRepo.HardDeleteByReferenceIdTransactionType(id, VoucherType.PossessionCharges);
                transactionRepo.HardDeleteByReferenceIdTransactionType(id, VoucherType.SecurityMoney);
                var electricityUnitIds = new GenericRepository<ElectricityUnitItem>().AsQueryable().Where(p => p.RentAgreementId == id).Select(p => p.ElectricityUnitId).ToList();
                new GenericRepository<ElectricityUnit>().Delete(electricityUnitIds);
                repo.Delete(id);
                repo.SaveChanges();
                scope.Complete();
            }
        }

    }
}
