using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using AccountEx.CodeFirst.Models;

namespace AccountEx.BussinessLogic
{
    public static class ProjectManager
    {
        public static void Save(Project entity)
        {
            //if (entity.Id > 0)
            //{
            //    new TransactionRepository().HardDelete(entity.Number, VoucherType.Project);
            //}
            //var dt = DateTime.Now;
            //var trans = new List<Transaction>
            //{
            //    new Transaction()
            //    {
            //        VoucherNumber = entity.Number,
            //        AccountId = AccountManager.GetLeafAccountId("Projects"),
            //        AccountTitle = "Projects",
            //        TransactionType = (int) VoucherType.Project,
            //        Credit = entity.GrossCost,
            //        EntryType = (byte) EntryType.MasterDetail,
            //        InvoiceNumber = entity.Number,
            //        Date = dt,
            //        CreatedDate = dt,
            //        Comments = entity.Title,
            //    },
            //    new Transaction()
            //    {
            //        VoucherNumber = entity.Number,
            //        AccountId = entity.AccountId,
            //        AccountTitle = entity.AccountTitle,
            //        TransactionType = (int) VoucherType.Project,
            //        Debit = entity.GrossCost,
            //        EntryType = (byte) EntryType.Item,
            //        InvoiceNumber = entity.Number,
            //        Date = dt,
            //        CreatedDate = dt,
            //        Comments = entity.Title,
            //    }
            //};
            //new TransactionRepository().Add(trans);
            new ProjectRepository().Save(entity);
        }

        public static void AddReceipt(ProjectReceipt pr)
        {
            pr.FiscalId = SiteContext.Current.Fiscal.Id;
            new ProjectReceiptRepository().Add(pr);
        }
    }
}
