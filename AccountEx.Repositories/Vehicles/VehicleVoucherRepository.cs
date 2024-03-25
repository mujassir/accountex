using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.Vehicles;
using AccountEx.Common;
using AccountEx.Common.VehicleSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Repositories.Vehicles
{
    public class VehicleVoucherRepository : GenericRepository<VehicleVoucher>
    {
        public VehicleVoucherRepository() : base() { }
        public VehicleVoucherRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public int GetNextVoucherNumber(VoucherType vouchertype)
        {
            var maxnumber = ConfigurationReader.GetConfigKeyValue<int>("VoucherStartNumber", 1);
            if (!FiscalCollection.Any(p => p.TransactionType == vouchertype))
                return maxnumber;
            return FiscalCollection.Where(p => p.TransactionType == vouchertype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
        }

        public decimal GetTotalPayments(int voucherId, VoucherType type)
        {
            if (FiscalCollection.Any(p => p.TransactionType == type && p.SaleId == voucherId))
                return FiscalCollection.Where(p => p.TransactionType == type && p.SaleId == voucherId).Sum(p => p.Amount);
            else return 0.0M;
        }

        public VehicleVoucher GetByVoucherNumber(int voucherno, VoucherType vtype, string key, out bool next, out bool previous)
        {
            VehicleVoucher v = null;
            switch (key)
            {
                case "first":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "last":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "next":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype && p.VoucherNumber > voucherno).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "previous":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype && p.VoucherNumber < voucherno).OrderByDescending(p => p.VoucherNumber).FirstOrDefault(); ;
                    break;
                case "same":
                    v = FiscalCollection.FirstOrDefault(p => p.TransactionType == vtype && p.VoucherNumber == voucherno);
                    break;

            }

            if (v != null)
                voucherno = v.VoucherNumber;
            else if (key != "nextvouchernumber")
            {
                v = FiscalCollection.Where(p => p.TransactionType == vtype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
            }
            if (v == null && !FiscalCollection.Any(p => p.TransactionType == vtype))
            {
                v = new VehicleVoucher
                {
                    VoucherNumber = 1001,
                    InvoiceNumber = 1,
                    // Date = DateTime.Now,

                };
            }
            next = FiscalCollection.Any(p => p.TransactionType == vtype && p.VoucherNumber > voucherno);
            previous = FiscalCollection.Any(p => p.TransactionType == vtype && p.VoucherNumber < voucherno);
            return v;
        }
        public VehicleVoucher GetVocuherNumber(VoucherType vtype, string key)
        {
            if (key == "first")
                return FiscalCollection.Where(p => p.TransactionType == vtype).OrderBy(p => p.VoucherNumber).FirstOrDefault();
            return FiscalCollection.Where(p => p.TransactionType == vtype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
        }
        public void DeleteByVoucherNumber(int voucherno,VoucherType trtype)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var record = FiscalCollection.Where(p => p.VoucherNumber == voucherno && p.TransactionType == trtype).FirstOrDefault();
                if (record != null)
                {
                    Db.VehicleVouchers.Remove(record);
                }
                SaveLog(record, ActionType.Deleted);
                Db.SaveChanges();
                scope.Complete();
            }
        }
        public List<PrintVehicleVoucherById> PrintVehicleVoucherById(int VoucherId)
        {
            var query = string.Format("EXEC [DBO].[PrintVehicleVoucherById] @CompanyId = {0},@VoucherId={1}", SiteContext.Current.User.CompanyId, VoucherId);
            return Db.Database.SqlQuery<PrintVehicleVoucherById>(query).ToList();

        }
    }
}
