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
    public class VoucherTransRepository : GenericRepository<Voucher>
    {
        public VoucherTransRepository() : base() { }
        public VoucherTransRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public int GetNextVoucherNumber(VoucherType vouchertype, int locationId = 0)
        {
            var maxnumber = ConfigurationReader.GetConfigKeyValue<int>("VoucherStartNumber", 1001);
            if (!FiscalCollection.Any(p => p.TransactionType == vouchertype && p.AuthLocationId == locationId))
                return maxnumber;
            return FiscalCollection.Where(p => p.TransactionType == vouchertype && p.AuthLocationId == locationId).OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
        }
        public decimal GetVehicleExpenses(int vehicleId)
        {
            var types = new List<VoucherType> { VoucherType.CashPayments, VoucherType.BankPayments };
            if (Collection.Any(p => types.Contains(p.TransactionType) && p.VehicleId == vehicleId && p.VoucherItems.Any(q => q.Debit > 0 && q.Debit.HasValue)))
                return Collection.Where(p => types.Contains(p.TransactionType) && p.VehicleId == vehicleId)
                    .SelectMany(p => p.VoucherItems).Where(p => p.Debit > 0 && p.Debit.HasValue).Sum(p => p.Debit.Value);
            else return 0;
        }
        public override void Update(Voucher v)
        {
            var voucherItemRepo = new VoucherItemRepository(this);
            var dbVoucher = GetById(v.Id, true);

            //add,update & remove voucher items
            var Ids = v.VoucherItems.Select(p => p.Id).ToList();
            var deletedIds = dbVoucher.VoucherItems.Where(p => !Ids.Contains(p.Id)).Select(p => p.Id).ToList();
            voucherItemRepo.Delete(deletedIds);
            voucherItemRepo.Save(v.VoucherItems.ToList());
            //var v1 = v.CloneWithJson();
            //v1.VoucherItems = null;
            base.Update(v, true, false);
        }
        public void MarkFinal(Voucher v)
        {
            base.Update(v, true, false);
        }
        public int GetBranchId(int Id)
        {
            if (Collection.Any(p => p.Id == Id))
                return Collection.FirstOrDefault(p => p.Id == Id).BranchId ?? 0;
            else return 0;
        }

        public Voucher GetByVoucherNumber(int voucherno, VoucherType vtype, string key, int locationId, out bool next, out bool previous)
        {
            Voucher v = null;
            switch (key)
            {
                case "first":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype && p.AuthLocationId == locationId).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "last":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype && p.AuthLocationId == locationId).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "next":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype && p.VoucherNumber > voucherno && p.AuthLocationId == locationId).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "previous":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype && p.VoucherNumber < voucherno && p.AuthLocationId == locationId).OrderByDescending(p => p.VoucherNumber).FirstOrDefault(); ;
                    break;
                case "same":
                    v = FiscalCollection.FirstOrDefault(p => p.TransactionType == vtype && p.VoucherNumber == voucherno && p.AuthLocationId == locationId);
                    break;

            }

            if (v != null)
                voucherno = v.VoucherNumber;
            else if (key != "nextvouchernumber")
            {
                v = FiscalCollection.Where(p => p.TransactionType == vtype && p.AuthLocationId == locationId).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
            }
            if (v == null && !FiscalCollection.Any(p => p.TransactionType == vtype && p.AuthLocationId == locationId))
            {
                v = new Voucher
                {
                    VoucherNumber = 1001,
                    InvoiceNumber = 1,
                    Date = DateTime.Now,
                    CreatedDate = DateTime.Now
                };
            }
            next = FiscalCollection.Any(p => p.TransactionType == vtype && p.VoucherNumber > voucherno && p.AuthLocationId == locationId);
            previous = FiscalCollection.Any(p => p.TransactionType == vtype && p.VoucherNumber < voucherno && p.AuthLocationId == locationId);
            return v;
        }
        public Voucher GetVocuherNumber(VoucherType vtype, string key)
        {
            if (key == "first")
                return FiscalCollection.Where(p => p.TransactionType == vtype).OrderBy(p => p.VoucherNumber).FirstOrDefault();
            return FiscalCollection.Where(p => p.TransactionType == vtype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
        }

        public int GetVocuherId(int voucherno,VoucherType trtype)
        {
            if (FiscalCollection.Any(p => p.VoucherNumber == voucherno && p.TransactionType == trtype))
                return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno && p.TransactionType == trtype).Id;
            else return 0;
        }
        public void DeleteByVoucherNumber(int voucherno,VoucherType trtype, int locationId = 0)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                //string query = "Delete from Vouchers where VoucherNumber='" + voucherno + "' AND TransactionType='" + trtype + "'";
                //Db.Database.ExecuteSqlCommand(query);
                var record = FiscalCollection.Where(p => p.VoucherNumber == voucherno && p.TransactionType == trtype && p.AuthLocationId == locationId).FirstOrDefault();
                //foreach (var item in FiscalCollection.Where(p => p.VoucherNumber == voucherno && p.TransactionType == trtype))
                //{
                //    item.IsDeleted = true;
                //}

                if (record != null)
                {
                    Db.Vouchers.Remove(record);
                }
                SaveLog(record, ActionType.Deleted);
                Db.SaveChanges();
                scope.Complete();
            }
        }
        public Voucher GetByVoucherNo(int voucherno, int id, VoucherType transtype)
        {

            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno && p.TransactionType == transtype && p.Id != id);

        }
        public List<Voucher> GetByIds(List<int> ids)
        {

            return FiscalCollection.Where(p => ids.Contains(p.Id)).ToList(); ;

        }
        public bool IsVoucherExistByVoucherNo(int voucherno, int id, VoucherType transtype, int locationId = 0)
        {

            return FiscalCollection.Any(p => p.VoucherNumber == voucherno && p.TransactionType == transtype && p.Id != id && p.AuthLocationId == locationId);

        }
        public bool IsExpenseExistByVehicleId(int vehicleId, int expenseId, VoucherType transType, int id)
        {
            return Collection.Any(p => p.VehicleId == vehicleId && p.VoucherItems.Any(q => q.AccountId == expenseId) && p.Id != id);
        }

        public List<Voucher> GetByDates(DateTime fromDate, DateTime toDate, List<VoucherType> transactionTypes)
        {

            var query = FiscalCollection.Where(p => EntityFunctions.TruncateTime(p.Date) >= EntityFunctions.TruncateTime(fromDate) && EntityFunctions.TruncateTime(p.Date) <= EntityFunctions.TruncateTime(toDate));
            if (transactionTypes != null && transactionTypes.Count > 0)
                query = query.Where(p => transactionTypes.Contains(p.TransactionType));
            return query.OrderBy(p => p.Date).ToList();
        }
        public List<Voucher> GetUnPostedByDates(DateTime fromDate, DateTime toDate, List<VoucherType> transactionTypes)
        {

            var query = FiscalCollection.Where(p => EntityFunctions.TruncateTime(p.Date) >= EntityFunctions.TruncateTime(fromDate) && EntityFunctions.TruncateTime(p.Date) <= EntityFunctions.TruncateTime(toDate));
            if (transactionTypes != null && transactionTypes.Count > 0)
                query = query.Where(p => transactionTypes.Contains(p.TransactionType) && !p.IsFinal);
            return query.OrderBy(p => p.Date).ToList();
        }
    }
}