using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using System;
using System.Linq;
using EntityFramework.Extensions;
using System.Data;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;

namespace AccountEx.Repositories
{
    public class SaleDiscountRepository : GenericRepository<SaleDiscount>
    {
        public override void Update(SaleDiscount saleDiscount)
        {
            //var txOptions = new TransactionOptions();
            //txOptions.IsolationLevel = IsolationLevel.ReadUncommitted;
            //using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, txOptions))
            //{
            //  var query = "Delete from VoucherItems where VoucherId=" + v.Id;
            // Db.Database.ExecuteSqlCommand(query);
            //var dbitems = FiscalCollection.AsNoTracking().FirstOrDefault(p => p.Id == v.Id).VoucherItems.ToList();
            //dbitems.ForEach(p => p.IsDeleted = true);
            //new GenericRepository<VoucherItem>().Save(dbitems);


            Db.SaleDiscountItems.Delete(p => p.CompanyId == SiteContext.Current.User.CompanyId && p.SaleDiscountId == saleDiscount.Id);
            //Db.Vouchers.Attach(v);
            //Db.Entry(v).State = EntityState.Modified;
            //Db.Entry(v.VoucherItems).State = EntityState.Added;
            // etc
            //var originalItem = Db.Vouchers.Find(v.Id);
            ////Db.Entry(v).State = EntityState.Modified;
            //Db.Entry(originalItem).CurrentValues.SetValues(v);
            //Db.Entry(originalItem.VoucherItems).CurrentValues.SetValues(v.VoucherItems);
            //Db.SaveChanges();
            foreach (var item in saleDiscount.SaleDiscountItems)
            {
                //var entry = Db.Entry(item);
                //entry.State = EntityState.Added;
                //Db.Entry(v.VoucherItems).State = EntityState.Added;
                Db.SaleDiscountItems.Add(item);
                Db.SaveChanges();
            }
            saleDiscount.SaleDiscountItems = null;
            base.Update(saleDiscount);

            SaveChanges();
            //}
        }

        public SaleDiscount GetByVoucherNumber(int voucherno, VoucherType vtype, string key, out bool next, out bool previous)
        {
            SaleDiscount saleDiscount = null;
            switch (key)
            {
                case "first":
                    saleDiscount = FiscalCollection.Where(p => p.TransactionType == vtype).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "last":
                    saleDiscount = FiscalCollection.Where(p => p.TransactionType == vtype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "next":
                    saleDiscount = FiscalCollection.Where(p => p.TransactionType == vtype && p.VoucherNumber > voucherno).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "previous":
                    saleDiscount = FiscalCollection.Where(p => p.TransactionType == vtype && p.VoucherNumber < voucherno).OrderByDescending(p => p.VoucherNumber).FirstOrDefault(); ;
                    break;
                case "same":
                    saleDiscount = FiscalCollection.FirstOrDefault(p => p.TransactionType == vtype && p.VoucherNumber == voucherno);
                    break;

            }

            if (saleDiscount != null)
                voucherno = saleDiscount.VoucherNumber;
            else if (key != "nextvouchernumber")
            {
                saleDiscount = FiscalCollection.Where(p => p.TransactionType == vtype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
            }
            if (saleDiscount == null && !FiscalCollection.Any(p => p.TransactionType == vtype))
            {
                saleDiscount = new SaleDiscount
                {
                    VoucherNumber = 1001,
                    InvoiceNumber = 1,
                    Date = DateTime.Now,
                };
            }
            next = FiscalCollection.Any(p => p.TransactionType == vtype && p.VoucherNumber > voucherno);
            previous = FiscalCollection.Any(p => p.TransactionType == vtype && p.VoucherNumber < voucherno);
            return saleDiscount;
        }
        public SaleDiscount GetVocuherNumber(VoucherType vtype, string key)
        {
            if (key == "first")
                return FiscalCollection.Where(p => p.TransactionType == vtype).OrderBy(p => p.VoucherNumber).FirstOrDefault();
            return FiscalCollection.Where(p => p.TransactionType == vtype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
        }

        public void DeleteByVoucherNumber(int voucherno,VoucherType trtype)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                //string query = "Delete from Vouchers where VoucherNumber='" + voucherno + "' AND TransactionType='" + trtype + "'";
                //Db.Database.ExecuteSqlCommand(query);
                var record = FiscalCollection.Where(p => p.VoucherNumber == voucherno && p.TransactionType == trtype).FirstOrDefault();
                //foreach (var item in FiscalCollection.Where(p => p.VoucherNumber == voucherno && p.TransactionType == trtype))
                //{
                //    item.IsDeleted = true;
                //}

                if (record != null) 
                {
                    Db.SaleDiscounts.Remove(record);
                }
                SaveLog(record, ActionType.Deleted);
                Db.SaveChanges();
                scope.Complete();
            }
        }
        public SaleDiscount GetByVoucherNo(int voucherno, int id, VoucherType transtype)
        {

            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno && p.TransactionType == transtype && p.Id != id);

        }

        public List<SaleDiscount> GetByDates(DateTime fromDate, DateTime toDate, List<VoucherType> transactionTypes)
        {

            var query = FiscalCollection.Where(p => EntityFunctions.TruncateTime(p.Date) >= EntityFunctions.TruncateTime(fromDate) && EntityFunctions.TruncateTime(p.Date) <= EntityFunctions.TruncateTime(toDate));
            if (transactionTypes != null && transactionTypes.Count > 0)
                query = query.Where(p => transactionTypes.Contains(p.TransactionType));
            return query.OrderBy(p => p.Date).ToList();
        }
    }
}