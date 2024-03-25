using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.COA;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AccountEx.BussinessLogic
{
    public static class MiscChargesManager
    {
        public static void Save(MiscCharge input)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new MiscChargesRepository();
                if (input.Id == 0)
                {
                    repo.Add(false, input);
                    AddTransaction(input, repo);
                }
                else
                {
                    AddTransaction(input, repo);
                    repo.Update(input);
                }
                
                repo.SaveChanges();
                scope.Complete();
            }
        }

        public static void AddTransaction(MiscCharge s, BaseRepository baseRepo)
        {
            var dt = DateTime.Now;
            var repo = new TransactionRepository(baseRepo);
            repo.HardDelete(s.VoucherNumber, VoucherType.MiscCharges);
            var trans = new List<Transaction>();
            foreach (var item in s.MiscChargeItems)
            {
                trans.Add(
                   new Transaction
                   {
                       ReferenceId = s.Id,
                       InvoiceNumber = s.VoucherNumber,
                       VoucherNumber = s.VoucherNumber,
                       AccountId = SettingManager.MiscChargesHeadId,
                       TransactionType = VoucherType.MiscCharges,
                       EntryType = (byte)EntryType.MasterDetail,
                       Quantity = 1,
                       Debit = 0,
                       Credit = item.Amount,
                   });
                trans.Add(
                new Transaction
                {
                    ReferenceId = s.Id,
                    InvoiceNumber = s.VoucherNumber,
                    VoucherNumber = s.VoucherNumber,
                    AccountId = item.TenantAccountId,
                    TransactionType = VoucherType.MiscCharges,
                    EntryType = (byte)EntryType.MasterDetail,
                    Quantity = 1,
                    Debit = item.Amount,
                    Credit = 0,
                });
            }
            s.VoucherNumber = s.VoucherNumber;
            foreach (var item in trans)
            {
                item.VoucherNumber = s.VoucherNumber;
                item.CreatedDate = dt;
                item.Date = dt.Date;
                //item.Comments = s.Comments;
                item.FiscalId = SiteContext.Current.Fiscal.Id;
            }
            repo.Add(trans);
        }

        public static void Delete(int voucherNumber)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new MiscChargesRepository();
                var transactionRepo = new TransactionRepository(repo);
                transactionRepo.HardDelete(voucherNumber, VoucherType.MiscCharges);
                repo.DeleteByVoucherNo(voucherNumber);
                repo.SaveChanges();
                scope.Complete();
            }
        }

    }
}
