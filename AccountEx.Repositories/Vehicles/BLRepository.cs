using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Core.Objects;
using Transaction = AccountEx.CodeFirst.Models.Transaction;
using System.Configuration;
using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.Transactions;
using AccountEx.Common.VehicleSystem;
using AccountEx.CodeFirst.Models.Vehicles;
using AccountEx.DbMapping;

namespace AccountEx.Repositories
{
    public class BLRepository : GenericRepository<BL>
    {
        public BLRepository() : base() { }
        public BLRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public BL GetByVoucherNumber(int voucherNo)
        {
            return Collection.FirstOrDefault(p => p.VoucherNumber == voucherNo);
        }
        public BL GetByVoucherNumber(int voucherNo, int id)
        {
            return Collection.FirstOrDefault(p => p.VoucherNumber == voucherNo && p.Id != id);
        }
        public bool IsVoucherExists(int voucherNo, int id)
        {
            return Collection.Any(p => p.VoucherNumber == voucherNo && p.Id != id);
        }
        public int GetNextVoucherNumber(int voucherNo)
        {
            var maxnumber = 1;
            if (!Collection.Any())
                return maxnumber;
            return Collection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
        }
        public override void Update(BL entity)
        {


            var blItemRepo = new BLItemRepository(this);
            var blChargeRepo = new BLChargeRepository(this);
            var dbBl = GetById(entity.Id, true);

            //add,update & remove BL items
            var Ids = entity.BLItems.Select(p => p.Id).ToList();
            var deletedIds = dbBl.BLItems.Where(p => !Ids.Contains(p.Id)).Select(p => p.Id).ToList();
            blItemRepo.Delete(deletedIds);
            blItemRepo.Save(entity.BLItems.ToList());

            //add,update & remove BL Charges
            Ids = entity.BLCharges.Select(p => p.Id).ToList();
            deletedIds = dbBl.BLCharges.Where(p => !Ids.Contains(p.Id)).Select(p => p.Id).ToList();
            blChargeRepo.Delete(deletedIds);
            blChargeRepo.Save(entity.BLCharges.ToList());
            entity.BLItems = null;
            entity.BLCharges = null;
            base.Update(entity, true, false);
        }

        public List<BLStatusExtra> GetBlStatuses()
        {
            var query = string.Format("EXEC [DBO].[GetBlStatuses] @COMPANYID={0}", SiteContext.Current.User.CompanyId);
            return Db.Database.SqlQuery<BLStatusExtra>(query).ToList();
        }
        public List<BLPaymentExtra> GetPendingBLForPayment(int supplierId)
        {
            return (from BLC in AsQueryable<BLCharge>()
                    join bl in Collection on BLC.BLId equals bl.Id
                    join a in AsQueryable<Account>() on BLC.ChargeId equals a.Id
                    where BLC.SupplierId == supplierId && !BLC.IsPaid
                    select new BLPaymentExtra()
                    {
                        Id = BLC.Id,
                        BLId = bl.Id,
                        BLNumber = bl.BLNumber,
                        InvoiceNo = BLC.InvoiceNo,
                        Amount = BLC.Amount,
                        Charge = a.Name

                    }).ToList();

        }
        public List<BLDetailForPayment> GetBLDetailForPayment(List<int> blIds)
        {

            return Collection.Where(p => blIds.Contains(p.Id)).Select(p => new BLDetailForPayment()
                    {
                        Id = p.Id,
                        BLNumber = p.BLNumber,
                        TotalUnit = p.BLItems.Count()

                    }).ToList();

        }
        public List<BL> GetByIds(List<int> Ids)
        {

            return Collection.Where(p => Ids.Contains(p.Id)).ToList();



        }
        public BL GetByVoucherNumber(int voucherNo, string key, out bool next, out bool previous)
        {
            BL v = null;
            switch (key)
            {
                case "first":
                    v = Collection.OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "last":
                    v = Collection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "next":
                    v = Collection.Where(p => p.VoucherNumber > voucherNo).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "previous":
                    v = Collection.Where(p => p.VoucherNumber < voucherNo).OrderByDescending(p => p.VoucherNumber).FirstOrDefault(); ;
                    break;
                case "same":
                    v = Collection.FirstOrDefault(p => p.VoucherNumber == voucherNo);
                    break;
                case "challan":
                    v = Collection.FirstOrDefault(p => p.VoucherNumber == voucherNo);
                    break;
            }

            if (v != null)
                voucherNo = v.VoucherNumber;
            else if (key != "nextvouchernumber" && key != "challan")
            {
                v = Collection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault();

            }
            if (v == null && !Collection.Any())
            {
                v = new BL();
                v.VoucherNumber = 1001;
                v.Date = DateTime.Now;
            }
            next = Collection.Any(p => p.VoucherNumber > voucherNo);
            previous = Collection.Any(p => p.VoucherNumber < voucherNo);
            return v;
        }
        public void DeleteByVoucherNumber(int voucherNo)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {

                var record = Collection.FirstOrDefault(p => p.VoucherNumber == voucherNo);
                if (record != null)
                {
                    Db.BLs.Remove(record);
                }
                Db.SaveChanges();
                scope.Complete();
            }
        }
        public override void Delete(int id)
        {
            var record = Collection.FirstOrDefault(p => p.Id == id);
            if (record != null)
            {
                Db.BLs.Remove(record);
            }
        }
        public bool CheckIfShipIdExists(int shipId)
        {
            return Collection.Any(p => p.ShipId == shipId);
        }
    }
}