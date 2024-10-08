﻿using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AccountEx.Repositories
{
    public class StockRequisitionRepository : GenericRepository<StockRequisition>
    {
        public StockRequisitionRepository() : base() { }
        public StockRequisitionRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }

        public StockRequisition GetByVoucherNo(int voucherno)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno);
        }
        public StockRequisition GetByVoucherNo(int voucherno, int id)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno && p.Id != id);
        }
        public bool CheckIsVoucherNoExist(int voucherno, int id)
        {
            return FiscalCollection.Any(p => p.VoucherNumber == voucherno && p.Id != id);
        }
        public StockRequisition GetByInvoiceNo(int bookno, int id)
        {
            return FiscalCollection.FirstOrDefault(p => p.InvoiceNumber == bookno && p.Id != id);
        }
        public bool CheckIdInvoiceNoExist(int bookno, int id)
        {
            return FiscalCollection.Any(p => p.InvoiceNumber == bookno && p.Id != id);
        }
        public int GetNextVoucherNumber()
        {
            var maxnumber = 1;
            if (!FiscalCollection.Any())
                return maxnumber;
            //return FiscalCollection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
            return FiscalCollection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
        }
        public override void Update(StockRequisition r)
        {
            var dbStockRequisition = GetById(r.Id,true);
            var ids = r.StockRequisitionItems.Select(p => p.Id).ToList();
            var deletedIds = dbStockRequisition.StockRequisitionItems.Where(p => !ids.Contains(p.Id)).Select(p => p.Id).ToList();
            new StockRequisitionItemRepository(this).Delete(deletedIds);
            new StockRequisitionItemRepository(this).Save(r.StockRequisitionItems.ToList());
            r.StockRequisitionItems = null;
            r.Status = dbStockRequisition.Status;
            base.Update(r, true, false);
        }
        public StockRequisition GetByVoucherNumber(int voucherno, string key, out bool next, out bool previous)
        {
            StockRequisition v = null;
            switch (key)
            {
                case "first":
                    v = FiscalCollection.OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "last":
                    v = FiscalCollection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "next":
                    v = FiscalCollection.Where(p => p.VoucherNumber > voucherno).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "previous":
                    v = FiscalCollection.Where(p => p.VoucherNumber < voucherno).OrderByDescending(p => p.VoucherNumber).FirstOrDefault(); ;
                    break;
                case "same":
                    v = FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno);
                    break;
                case "challan":
                    v = FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno);
                    break;

            }

            if (v != null)
                voucherno = v.VoucherNumber;
            else if (key != "nextvouchernumber" && key != "challan")
            {
                v = FiscalCollection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault();

            }
            if (v == null && !FiscalCollection.Any())
            {
                v = new StockRequisition();
                v.VoucherNumber = 1001;
                v.InvoiceNumber = 1;
                v.Date = DateTime.Now;
                v.CreatedAt = DateTime.Now;
            }
            next = FiscalCollection.Any(p => p.VoucherNumber > voucherno);
            previous = FiscalCollection.Any(p => p.VoucherNumber < voucherno);
            return v;
        }
        public void DeleteByVoucherNumber(int voucherno)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {

                var record = FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno);
                if (record != null)
                {
                    Db.StockRequisitions.Remove(record);
                    SaveLog(record, ActionType.Deleted);
                    Db.SaveChanges();
                }
                scope.Complete();
            }
        }
        public override void Delete(int id)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {

                var record = FiscalCollection.FirstOrDefault(p => p.Id == id);
                if (record != null)
                {
                    Db.StockRequisitions.Remove(record);
                    //string query = "Delete from OrderItems where OrderId='" + id + "'";
                    //Db.Database.ExecuteSqlCommand(query);
                    //record.IsDeleted = true;
                }
                Db.SaveChanges();
                scope.Complete();
            }
        }
        public void Update(int srnno)
        {

            var srn = FiscalCollection.FirstOrDefault(p => p.VoucherNumber == srnno);
            if (srn != null)
            {
                srn.Status = (byte)AccountEx.Common.TransactionStatus.Delivered;
            }
        }

    }
}
