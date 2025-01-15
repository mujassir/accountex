using System.Linq;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using Transaction = AccountEx.CodeFirst.Models.Transaction;
using AccountEx.CodeFirst.Models.Transactions;
using AccountEx.Repositories.Transactions;
using AccountEx.CodeFirst.Models.Vehicles;
using AccountEx.Repositories.Vehicles;

namespace AccountEx.BussinessLogic
{
    public static class VoucherManager
    {
        public static void Save(Voucher voucher)
        {

            var repo = new VoucherTransRepository();
            var accountRepo = new AccountRepository(repo);
            using (var scope = TransactionScopeBuilder.Create())
            {
                var isFinal = false;
                foreach (var item in voucher.VoucherItems)
                {
                    item.CompanyId = SiteContext.Current.User.CompanyId;
                    switch (voucher.TransactionType)
                    {
                        case VoucherType.CashPayments:
                            item.Debit = item.Amount;
                            isFinal = !SettingManager.IsCashPaymentAllowFinalization;
                            break;
                        case VoucherType.BankPayments:
                            item.Debit = item.Amount;
                            isFinal = !SettingManager.IsBankPaymentAllowFinalization;
                            break;
                        case VoucherType.CashReceipts:
                            item.Credit = item.Amount;
                            isFinal = !SettingManager.IsCashReceiptAllowFinalization;
                            break;
                        case VoucherType.BankReceipts:
                            item.Credit = item.Amount;
                            isFinal = !SettingManager.IsBankReceiptAllowFinalization;
                            break;

                        case VoucherType.TransferVoucher:
                            isFinal = !SettingManager.IsJvAllowFinalization;
                            break;
                        case VoucherType.VehiclePayable:
                            item.Debit = item.Amount;
                            isFinal = true;
                            break;
                        case VoucherType.AdvanceReceipts:
                            item.Debit = item.Amount;
                            isFinal = true;
                            break;

                    }
                }
                voucher.IsFinal = isFinal;

                if (voucher.Id == 0)
                {
                    repo.Add(voucher, true, false);
                }
                else
                {
                    repo.Update(voucher);
                }
                ConfigTransaction(voucher, repo);
                repo.SaveChanges();
                scope.Complete();
            }
        }


        public static void PostBulkVoucher(List<VoucherPostingExtra> records)
        {

            var repo = new VoucherTransRepository();
            var accountRepo = new AccountRepository(repo);
            using (var scope = TransactionScopeBuilder.Create())
            {
                var Ids = records.Select(p => p.Id).ToList();
                var vouchers = repo.GetByIds(Ids);
                foreach (var voucher in vouchers)
                {
                    voucher.IsFinal = true;
                    ConfigTransaction(voucher, repo, true);

                }
                repo.SaveChanges();
                scope.Complete();
            }
        }
        public static void SavePayablePayment(PayablePayment voucher)
        {

            var repo = new PayablePaymentRepository();
            var voucherRepo = new VoucherTransRepository(repo);
            var accountRepo = new AccountRepository(repo);
            using (var scope = TransactionScopeBuilder.Create())
            {
                if (voucher.Id == 0)
                {
                    voucher.VoucherNumber = repo.GetNextVoucherNumber((VoucherType)Convert.ToByte(VoucherType.PayablePayment));
                    repo.Add(voucher, true, false);
                }
                else
                {
                    repo.Update(voucher);
                }
                repo.SaveChanges();
                AddPayablePaymentTransaction(voucher, repo);
                var totalPayments = repo.GetTotalPayments(voucher.VoucherId, voucher.TransactionType);
                var payableVoucher = voucherRepo.GetById(voucher.VoucherId);
                if (totalPayments >= payableVoucher.NetTotal)
                {
                    payableVoucher.IsPaid = true;
                }
                payableVoucher.TotalPaid = totalPayments;
                repo.SaveChanges();
                scope.Complete();
            }
        }
        public static void SaveVehicleVoucher(VehicleVoucher voucher)
        {

            var repo = new VehicleVoucherRepository();
            var voucherRepo = new VoucherTransRepository(repo);
            var accountRepo = new AccountRepository(repo);
            using (var scope = TransactionScopeBuilder.Create())
            {
                if (voucher.Id == 0)
                {
                    voucher.VoucherNumber = repo.GetNextVoucherNumber(voucher.TransactionType);
                    repo.Add(voucher, true, false);
                }
                else
                {
                    repo.Update(voucher);
                }
                AddVehicleVoucherTransaction(voucher, repo);
                repo.SaveChanges();
                scope.Complete();
            }
        }
        public static void SaveForexVoucher(VehicleVoucher voucher)
        {

            var repo = new VehicleVoucherRepository();
            var voucherRepo = new VoucherTransRepository(repo);
            var accountRepo = new AccountRepository(repo);
            voucher.TransactionType = VoucherType.ForexVoucher;
            using (var scope = TransactionScopeBuilder.Create())
            {
                if (voucher.Id == 0)
                {
                    voucher.VoucherNumber = repo.GetNextVoucherNumber(voucher.TransactionType);
                    repo.Add(voucher, true, false);
                }
                else
                {
                    repo.Update(voucher);
                }
                AddForexVoucherTransaction(voucher, repo);
                repo.SaveChanges();
                scope.Complete();
            }
        }
        private static void ConfigTransaction(Voucher voucher, BaseRepository baseRepo)
        {
            ConfigTransaction(voucher, baseRepo, false);
        }
        private static void ConfigTransaction(Voucher voucher, BaseRepository baseRepo, bool forcePost)
        {
            var repo = baseRepo;
            var accountRepo = new AccountRepository(repo);
            var bankId = voucher.AccountId.HasValue ? voucher.AccountId.Value : 0;
            switch (voucher.TransactionType)
            {
                case VoucherType.CashPayments:
                    if (!SettingManager.IsCashPaymentAllowFinalization || (forcePost))
                    {
                        AddTransaction(voucher, repo);
                    }
                    break;
                case VoucherType.BankPayments:
                    if (!SettingManager.IsBankPaymentAllowFinalization || (forcePost))
                    {
                        if (voucher.AccountId == null || voucher.AccountId == 0)
                        {
                            AddJvTransaction(voucher, repo);
                        }
                        else
                        {
                            AddTransaction(voucher, repo);
                        }

                    }
                    else
                    {
                        if (SettingManager.IsUnpresentedTransactionAllowed)
                        {
                            var accountIds = voucher.VoucherItems.Select(p => p.AccountId).ToList();
                            var voucherClone = voucher.CloneWithJson<Voucher>();

                            if (voucherClone.AccountId == null || voucherClone.AccountId == 0)
                            {
                                bankId = accountRepo.GetBankId(accountIds, SettingManager.BankHeadId);
                                voucherClone.VoucherItems.FirstOrDefault(p => p.AccountId == bankId).AccountId = SettingManager.UnpresentedChequeHeadId;
                                AddJvTransaction(voucherClone, repo);
                            }
                            else
                            {
                                voucherClone.AccountId = SettingManager.UnpresentedChequeHeadId;
                                AddTransaction(voucherClone, repo);
                            }
                        }
                    };
                    break;
                case VoucherType.CashReceipts:
                    if (!SettingManager.IsCashReceiptAllowFinalization || (forcePost))
                    {
                        AddTransaction(voucher, repo);
                    }
                    break;
                case VoucherType.BankReceipts:
                    if (!SettingManager.IsBankReceiptAllowFinalization || (forcePost))
                    {
                        if (voucher.AccountId == null || voucher.AccountId == 0)
                        {
                            AddJvTransaction(voucher, repo);
                        }
                        else
                        {
                            AddTransaction(voucher, repo);
                        }
                    }
                    else
                    {
                        var accountIds = voucher.VoucherItems.Select(p => p.AccountId).ToList();
                        var voucherClone = voucher.CloneWithJson<Voucher>();
                        if (SettingManager.IsUnpresentedTransactionAllowed)
                        {

                            if (voucherClone.AccountId == null || voucherClone.AccountId == 0)
                            {
                                bankId = accountRepo.GetBankId(accountIds, SettingManager.BankHeadId);
                                voucherClone.VoucherItems.FirstOrDefault(p => p.AccountId == bankId).AccountId = SettingManager.UnpresentedChequeHeadId;
                                AddJvTransaction(voucherClone, repo);
                            }
                            else
                            {
                                voucherClone.AccountId = SettingManager.UnpresentedChequeHeadId;
                                AddTransaction(voucherClone, repo);
                            }
                        }
                    };
                    break;

                case VoucherType.TransferVoucher:
                    if (!SettingManager.IsJvAllowFinalization || (forcePost))
                    {
                        AddJvTransaction(voucher, repo);
                    }
                    break;
                case VoucherType.VehiclePayable:
                    AddTransaction(voucher, repo);
                    break;
                case VoucherType.AdvanceReceipts:
                    if (true)
                    {
                        AddAdvanceTransaction(voucher, repo);
                    }
                    break;

            }
        }
        public static void SaveBankVoucher(Voucher voucher)
        {
            List<int> AccountIds = new List<int>();
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new VoucherTransRepository();
                var accountRepo = new AccountRepository(repo);
                foreach (var item in voucher.VoucherItems)
                {
                    AccountIds.Add(item.AccountId);

                    item.CompanyId = SiteContext.Current.User.CompanyId;
                    switch (voucher.TransactionType)
                    {
                        case VoucherType.CashPayments:

                            item.Debit = item.Amount;
                            break;
                        case VoucherType.CashReceipts:
                            item.Credit = item.Amount;
                            break;
                    }
                }
                if (!SettingManager.IsBankReceiptAllowFinalization && voucher.TransactionType == VoucherType.BankReceipts)
                {
                    voucher.IsFinal = true;
                }
                else if (!SettingManager.IsBankPaymentAllowFinalization && voucher.TransactionType == VoucherType.BankPayments)
                {
                    voucher.IsFinal = true;
                }
                if (voucher.Id == 0)
                {
                    repo.Add(voucher, true, false);
                }
                else
                {
                    repo.Update(voucher);
                }
                if (voucher.TransactionType == VoucherType.TransferVoucher)
                {
                    AddJvTransaction(voucher, repo);
                }
                else if (voucher.TransactionType == VoucherType.BankReceipts)
                {
                    if (!SettingManager.IsBankReceiptAllowFinalization)
                    {
                        AddJvTransaction(voucher, repo);
                    }
                    else
                    {
                        if (SettingManager.IsUnpresentedTransactionAllowed)
                        {
                            int bankId = accountRepo.GetBankId(AccountIds, SettingManager.BankHeadId);
                            var voucherClone = voucher.CloneWithJson<Voucher>();
                            voucherClone.VoucherItems.FirstOrDefault(p => p.AccountId == bankId).AccountId = SettingManager.UnpresentedChequeHeadId;
                            AddJvTransaction(voucherClone, repo);
                        }
                    };
                }
                else if (voucher.TransactionType == VoucherType.BankPayments)
                {
                    if (!SettingManager.IsBankPaymentAllowFinalization)
                    {
                        AddJvTransaction(voucher, repo);
                    }
                    else
                    {

                        if (SettingManager.IsUnpresentedTransactionAllowed)
                        {
                            int bankId = accountRepo.GetBankId(AccountIds, SettingManager.BankHeadId);
                            var voucherClone = voucher.CloneWithJson<Voucher>();
                            voucherClone.VoucherItems.FirstOrDefault(p => p.AccountId == bankId).AccountId = SettingManager.UnpresentedChequeHeadId;
                            AddJvTransaction(voucherClone, repo);
                        }
                    };
                }

                else
                    AddTransaction(voucher, repo);
                repo.SaveChanges();
                scope.Complete();
            }
        }

        public static void FinalUnfinalAllVoucher(Voucher voucher, bool isfinal)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new VoucherTransRepository();
                var tranRepo = new TransactionRepository(repo);
                var accountRepo = new AccountRepository(repo);
                voucher = repo.GetById(voucher.Id);
                var bankId = voucher.AccountId.HasValue ? voucher.AccountId.Value : 0;
                if (isfinal)
                {
                    voucher.IsFinal = true;
                    ConfigTransaction(voucher, repo, true);
                }
                else if (!isfinal)
                {
                    voucher.IsFinal = false;
                    tranRepo.HardDelete(voucher.VoucherNumber, voucher.TransactionType);
                    if (voucher.TransactionType == VoucherType.BankReceipts)
                    {
                        if (SettingManager.IsUnpresentedTransactionAllowed)
                        {
                            var voucherClone = voucher.CloneWithJson<Voucher>();
                            voucherClone.AccountId = SettingManager.UnpresentedChequeHeadId;
                            AddTransaction(voucherClone, repo);
                        }

                    }
                    else if (voucher.TransactionType == VoucherType.BankPayments)
                    {
                        if (SettingManager.IsUnpresentedTransactionAllowed)
                        {
                            var voucherClone = voucher.CloneWithJson<Voucher>();
                            voucherClone.AccountId = SettingManager.UnpresentedChequeHeadId;
                            AddTransaction(voucherClone, repo);
                        }

                    }



                }
                repo.SaveChanges();
                scope.Complete();
            }
        }
        public static void FinalUnfinalBankVoucher(Voucher voucher, bool isfinal)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new VoucherTransRepository();
                var tranRepo = new TransactionRepository(repo);
                var accountRepo = new AccountRepository(repo);
                voucher = repo.GetById(voucher.Id);
                if (isfinal)
                {
                    voucher.IsFinal = true;
                    if (SettingManager.IsBankReceiptAllowFinalization)
                    {

                        AddJvTransaction(voucher, repo);
                    }
                    else if (SettingManager.IsBankPaymentAllowFinalization)
                    {

                        AddJvTransaction(voucher, repo);
                    }
                    repo.MarkFinal(voucher);

                }
                else if (!isfinal)
                {
                    tranRepo.HardDelete(voucher.VoucherNumber, voucher.TransactionType);
                    var accountIds = voucher.VoucherItems.Select(p => p.AccountId).ToList();
                    voucher.IsFinal = false;
                    if (voucher.TransactionType == VoucherType.BankReceipts)
                    {
                        if (!SettingManager.IsBankReceiptAllowFinalization)
                        {
                            AddJvTransaction(voucher, repo);
                        }
                        else
                        {
                            if (SettingManager.IsUnpresentedTransactionAllowed)
                            {
                                int bankId = accountRepo.GetBankId(accountIds, SettingManager.BankHeadId);
                                var voucherClone = voucher.CloneWithJson<Voucher>();
                                voucherClone.VoucherItems.FirstOrDefault(p => p.AccountId == bankId).AccountId = SettingManager.UnpresentedChequeHeadId;
                                AddJvTransaction(voucherClone, repo);
                            }
                        };
                    }
                    else if (voucher.TransactionType == VoucherType.BankPayments)
                    {
                        if (!SettingManager.IsBankPaymentAllowFinalization)
                        {
                            AddJvTransaction(voucher, repo);
                        }
                        else
                        {
                            if (SettingManager.IsUnpresentedTransactionAllowed)
                            {
                                int bankId = accountRepo.GetBankId(accountIds, SettingManager.BankHeadId);
                                var voucherClone = voucher.CloneWithJson<Voucher>();
                                voucherClone.VoucherItems.FirstOrDefault(p => p.AccountId == bankId).AccountId = SettingManager.UnpresentedChequeHeadId;
                                AddJvTransaction(voucherClone, repo);
                            }
                        };
                    }
                    repo.MarkFinal(voucher);

                }
                repo.SaveChanges();
                scope.Complete();
            }
        }
        public static void AddPayablePaymentTransaction(PayablePayment v, BaseRepository baseRepo)
        {
            var dt = DateTime.Now;
            var transRepo = new TransactionRepository(baseRepo);
            var voucherRepo = new VoucherTransRepository(baseRepo);
            var branchId = voucherRepo.GetBranchId(v.VoucherId);
            transRepo.HardDelete(v.VoucherNumber, v.TransactionType);
            var trans = new List<Transaction>()
            {
            new Transaction
            {
                ReferenceId = v.Id,
                AccountId =v.AccountId,
                InvoiceNumber = v.InvoiceNumber,
                VoucherNumber = v.VoucherNumber,
                TransactionType = v.TransactionType,
                EntryType = (byte)EntryType.Item,
                Comments = v.Comments,
                Credit =v.Amount,
                BranchId=branchId

                      },
             new Transaction
            {
                ReferenceId = v.Id,
                AccountId =v.VendorId,
                InvoiceNumber = v.InvoiceNumber,
                VoucherNumber = v.VoucherNumber,
                TransactionType = v.TransactionType,
                EntryType = (byte)EntryType.Item,
                Comments = v.Comments,
                Debit =v.Amount,
                BranchId=branchId
                    }
            };




            foreach (var item in trans)
            {
                item.VoucherNumber = v.VoucherNumber;
                item.CreatedDate = v.CreatedDate ?? dt; ;
                item.Date = v.Date;
                item.FiscalId = SiteContext.Current.Fiscal.Id;
            }

            transRepo.Add(trans);
        }
        public static void AddVehicleVoucherTransaction(VehicleVoucher v, BaseRepository baseRepo)
        {
            var transRepo = new TransactionRepository(baseRepo);
            var vehicleRepo = new VehicleRepository(baseRepo);
            var branchId = vehicleRepo.GetBranchId(v.VehicleId);
            transRepo.HardDelete(v.VoucherNumber, v.TransactionType);
            var trans = new List<Transaction>()
            {
            new Transaction
            {
                ReferenceId = v.Id,
                AccountId =v.AccountId,
                 ToAccountId =v.AccountId1,
                InvoiceNumber = v.InvoiceNumber,
                VoucherNumber = v.VoucherNumber,
                TransactionType = v.TransactionType,
                EntryType = (byte)EntryType.MasterDetail,
                Comments = v.Comments,
                Debit =v.Amount,
                BranchId=branchId
              },
             new Transaction
            {
                ReferenceId = v.Id,
                MainEntityId=v.VehicleId,
                AccountId =v.AccountId1,
                ToAccountId =v.AccountId,
                InvoiceNumber = v.InvoiceNumber,
                VoucherNumber = v.VoucherNumber,
                TransactionType =v.TransactionType,
                EntryType = (byte)EntryType.Item,
                Comments = v.Comments,
                Credit =v.Amount,
                BranchId=branchId

                    }
            };




            foreach (var item in trans)
            {
                item.VoucherNumber = v.VoucherNumber;
                item.CreatedDate = v.Date;
                item.Date = v.Date;
                item.FiscalId = SiteContext.Current.Fiscal.Id;
            }

            transRepo.Add(trans);
        }
        public static void AddForexVoucherTransaction(VehicleVoucher v, BaseRepository baseRepo)
        {
            var transRepo = new TransactionRepository(baseRepo);
            var vehicleRepo = new VehicleRepository(baseRepo);
            var branchId = new VehicleBranchRepository(baseRepo).GetHeadOfficeId();
            transRepo.HardDelete(v.VoucherNumber, v.TransactionType);
            var trans = new List<Transaction>()
            {
            new Transaction
            {
                ReferenceId = v.Id,
                AccountId =v.AccountId,
                 ToAccountId =v.AccountId1,
                InvoiceNumber = v.InvoiceNumber,
                VoucherNumber = v.VoucherNumber,
                TransactionType = VoucherType.ForexVoucher,
                EntryType = (byte)EntryType.MasterDetail,
                Comments = v.Comments,
                Credit =v.Amount,
                BranchId=branchId
              },
             new Transaction
            {
                ReferenceId = v.Id,
                AccountId =v.AccountId1,
                ToAccountId =v.AccountId,
                InvoiceNumber = v.InvoiceNumber,
                VoucherNumber = v.VoucherNumber,
                TransactionType =VoucherType.ForexVoucher,
                EntryType = (byte)EntryType.Item,
                Comments = v.Comments,
                Debit =v.Amount-v.AdjustmentAmount,
                BranchId=branchId

                    }
            };
            if (v.AdjustmentAmount > 0)
            {
                trans.Add(new Transaction
                {
                    ReferenceId = v.Id,
                    AccountId = SettingManager.CurrencyAdjustmentAccountId,
                    ToAccountId = v.AccountId,
                    InvoiceNumber = v.InvoiceNumber,
                    VoucherNumber = v.VoucherNumber,
                    TransactionType = v.TransactionType,
                    EntryType = (byte)VoucherType.ForexVoucher,
                    Comments = v.Comments,
                    Debit = v.AdjustmentAmount,
                    BranchId = branchId

                });
            }




            foreach (var item in trans)
            {
                item.VoucherNumber = v.VoucherNumber;
                item.CreatedDate = v.Date;
                item.Date = v.Date;
                item.FiscalId = SiteContext.Current.Fiscal.Id;
            }

            transRepo.Add(trans);
        }
        public static void AddTransaction(Voucher v, BaseRepository baseRepo)
        {
            var dt = DateTime.Now;
            var transRepo = new TransactionRepository(baseRepo);
            transRepo.HardDelete(v.VoucherNumber, v.TransactionType);
            var trans = v.VoucherItems.Select(item => new Transaction
            {
                ReferenceId = v.Id,
                BranchId = v.BranchId,
                MainEntityId = v.VehicleId,
                AccountId = item.AccountId,
                ToAccountId = v.AccountId,
                InvoiceNumber = v.InvoiceNumber,
                VoucherNumber = v.VoucherNumber,
                TransactionType = v.TransactionType,
                EntryType = (byte)EntryType.Item,
                Comments = item.Description,
                Credit = v.TransactionType == VoucherType.CashReceipts || v.TransactionType == VoucherType.BankReceipts
                        ? Numerics.GetInt(item.Amount)
                        : 0,
                Debit =
                    v.TransactionType == VoucherType.CashPayments || v.TransactionType == VoucherType.BankPayments || v.TransactionType == VoucherType.VehiclePayable
                        ? Numerics.GetInt(item.Amount)
                        : 0,
            }).ToList();

            trans.AddRange(

                  v.VoucherItems.Select(item => new Transaction
                  {
                      ReferenceId = v.Id,
                      BranchId = v.BranchId,
                      AccountId = Numerics.GetInt(v.AccountId),
                      ToAccountId = item.AccountId,
                      InvoiceNumber = v.InvoiceNumber,
                      VoucherNumber = v.VoucherNumber,
                      TransactionType = v.TransactionType,
                      EntryType = (byte)EntryType.MasterDetail,
                      Comments = item.Description,
                      Debit = v.TransactionType == VoucherType.CashReceipts || v.TransactionType == VoucherType.BankReceipts
                          ? Numerics.GetInt(item.Amount)
                          : 0,
                      Credit =
                      v.TransactionType == VoucherType.CashPayments || v.TransactionType == VoucherType.BankPayments || v.TransactionType == VoucherType.VehiclePayable
                          ? Numerics.GetInt(item.Amount)
                          : 0
                  }).ToList()

                  );
            foreach (var item in trans)
            {
                item.VoucherNumber = v.VoucherNumber;
                item.CreatedDate = v.CreatedDate ?? dt; ;
                item.Date = v.Date;
                item.FiscalId = SiteContext.Current.Fiscal.Id;
            }

            transRepo.Add(trans);
        }
        public static void AddAdvanceTransaction(Voucher v, BaseRepository baseRepo)
        {
            var dt = DateTime.Now;
            var transRepo = new TransactionRepository(baseRepo);
            transRepo.HardDelete(v.VoucherNumber, v.TransactionType);
            var trans = v.VoucherItems.Select(item => new Transaction
            {
                ReferenceId = v.Id,
                BranchId = v.BranchId,
                MainEntityId = v.VehicleId,
                AccountId = item.AccountId,
                ToAccountId = v.AccountId,
                InvoiceNumber = v.InvoiceNumber,
                VoucherNumber = v.VoucherNumber,
                TransactionType = v.TransactionType,
                EntryType = (byte)EntryType.Item,
                Comments = item.Description,
                Credit = Numerics.GetInt(item.Amount),
            }).ToList();

            trans.AddRange(

                  v.VoucherItems.Select(item => new Transaction
                  {
                      ReferenceId = v.Id,
                      BranchId = v.BranchId,
                      AccountId = Numerics.GetInt(v.AccountId),
                      ToAccountId = item.AccountId,
                      InvoiceNumber = v.InvoiceNumber,
                      VoucherNumber = v.VoucherNumber,
                      TransactionType = v.TransactionType,
                      EntryType = (byte)EntryType.MasterDetail,
                      Comments = item.Description,
                      Debit = Numerics.GetInt(item.Amount)
                  }).ToList()

                  );
            foreach (var item in trans)
            {
                item.VoucherNumber = v.VoucherNumber;
                item.CreatedDate = v.CreatedDate ?? dt; ;
                item.Date = v.Date;
                item.FiscalId = SiteContext.Current.Fiscal.Id;
            }

            transRepo.Add(trans);
        }
        public static void AddJvTransaction(Voucher v, BaseRepository baseRepo)
        {
            var dt = DateTime.Now;
            var transRepo = new TransactionRepository(baseRepo);
            transRepo.HardDelete(v.VoucherNumber, v.TransactionType);
            var trans = v.VoucherItems.Select(item => new Transaction
            {
                ReferenceId = v.Id,
                BranchId = v.BranchId,
                AccountId = item.AccountId,
                InvoiceNumber = v.InvoiceNumber,
                VoucherNumber = v.VoucherNumber,
                TransactionType = v.TransactionType,
                EntryType = (byte)EntryType.Item,
                Credit = Numerics.GetDecimal(item.Credit),
                Debit = Numerics.GetDecimal(item.Debit),
                ChequeNumber = item.ChequeNumber,
                ChequeDate = item.Dated,
                Comments = AddChqNumberWithComments(item),
            }).ToList();
            foreach (var item in trans)
            {
                item.VoucherNumber = v.VoucherNumber;
                item.CreatedDate = dt;
                item.Date = v.Date;
                item.FiscalId = SiteContext.Current.Fiscal.Id;

            }
            transRepo.Add(trans);
        }
        private static string AddChqNumberWithComments(VoucherItem vi)
        {
            var comments = vi.Description;
            if (!string.IsNullOrWhiteSpace(vi.ChequeNumber))
                comments += " cheque number:" + vi.ChequeNumber;
            if (vi.Dated.HasValue)
                comments += " cheque date:" + vi.Dated.Value.ToString("dd-MM-yyyy");

            return comments;

        }

        public static void Delete(int voucherno, VoucherType transactiontype, int locationId = 0)
        {
            var repo = new VoucherTransRepository();
            var transRepo = new TransactionRepository(repo);
            using (var scope = TransactionScopeBuilder.Create())
            {
                transRepo.HardDelete(voucherno, transactiontype, locationId);
                repo.DeleteByVoucherNumber(voucherno, transactiontype, locationId);
                repo.SaveChanges();
                scope.Complete();
            }

        }
        public static void DeleteVeicleVoucher(int voucherno, VoucherType transactiontype)
        {
            var repo = new VehicleVoucherRepository();
            var transRepo = new TransactionRepository(repo);
            using (var scope = TransactionScopeBuilder.Create())
            {
                transRepo.HardDelete(voucherno, transactiontype);
                repo.DeleteByVoucherNumber(voucherno, transactiontype);
                repo.SaveChanges();
                scope.Complete();
            }

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

        public static List<VoucherDetail> GetDetailedVouchers(DateTime d1, DateTime d2, bool isDebit)
        {
            var banks = new AccountRepository().GetLeafAccounts(SettingManager.BankHeadId);
            var bankId = 0;
            if (banks != null && banks.Count > 0)
            {
                var setting = banks.FirstOrDefault();
                if (setting != null) bankId = setting.Id;
            }
            var cashId = SettingManager.CashAccountId;
            var rentId = SettingManager.RentId;
            var utitlityChargesId = SettingManager.UtitlityChargesId;
            var electricityId = SettingManager.ElectricityId;
            var possessionChargesId = SettingManager.PossessionChargesId;
            var tfrFeeId = SettingManager.TfrFeeId;
            var securityMoneyId = SettingManager.SecurityMoneyId;
            var promoActivityId = SettingManager.PromoActivityId;
            var carParkingId = SettingManager.CarParkingId;
            var bankProfitId = SettingManager.BankProfitId;
            var surchargeId = SettingManager.SurchargeId;
            var miscId = SettingManager.MiscId;
            var crsId = SettingManager.CrsId;
            var drsId = SettingManager.DrsId;

            var list = new List<VoucherDetail>();
            List<VoucherItem> vItems;
            var vouchers = new VoucherRepository().GetByTypes(d1, d2, isDebit, out vItems);

            foreach (var item in vouchers)
            {
                //var collection = vItems.Where (p=>p.).ToDictionary(p => p.AccountId, p => (p.Debit.HasValue ? p.Debit.Value : 0) + (p.Credit.HasValue ? p.Credit.Value : 0));
                var firstItem = vItems.FirstOrDefault(p => p.VoucherId == item.Id);
                var collection = vItems.Where(p => p.VoucherId == item.Id).GroupBy(p => p.AccountId).Select(p => new { AccountId = p.Key, Amount = p.Sum(q => q.Debit ?? 0) - p.Sum(q => q.Credit ?? 0) }).ToDictionary(p => p.AccountId, p => p.Amount);
                if (firstItem == null) continue;
                var vDetail = new VoucherDetail()
                {
                    VoucherNumber = item.VoucherNumber,
                    Date = item.Date,
                    Remarks = firstItem.Description,
                    Cash = GetValue(collection, cashId),
                    Bank = GetValue(collection, bankId),
                    Rent = GetValue(collection, rentId),
                    UtitlityCharges = GetValue(collection, utitlityChargesId),
                    Electricity = GetValue(collection, electricityId),
                    PossessionCharges = GetValue(collection, possessionChargesId),
                    TfrFee = GetValue(collection, tfrFeeId),
                    SecurityMoney = GetValue(collection, securityMoneyId),
                    PromoActivity = GetValue(collection, promoActivityId),
                    CarParking = GetValue(collection, carParkingId),
                    BankProfit = GetValue(collection, bankProfitId),
                    Surcharge = GetValue(collection, surchargeId),
                    Misc = GetValue(collection, miscId),
                    Crs = GetValue(collection, crsId),
                    Drs = GetValue(collection, drsId),
                    VoucherType = item.TransactionType
                };
                switch ((VoucherType)item.TransactionType)
                {
                    case VoucherType.CashPayments:
                        vDetail.Cash = 0 - item.NetTotal;
                        break;
                    case VoucherType.CashReceipts:
                        vDetail.Cash = item.NetTotal;
                        break;
                    case VoucherType.BankPayments:
                        vDetail.Bank = 0 - item.NetTotal;
                        break;
                    case VoucherType.BankReceipts:
                        vDetail.Bank = item.NetTotal;
                        break;
                }
                list.Add(vDetail);
            }
            return list;
        }

        public static decimal GetValue(Dictionary<int, decimal> collection, int key)
        {
            return key > 0 && collection.ContainsKey(key) ? collection[key] : 0;
        }

    }
}
