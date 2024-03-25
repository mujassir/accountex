using AccountEx.CodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Core.Objects;
using AccountEx.Common;

namespace AccountEx.Repositories
{
    public class MiscChargesRepository : GenericRepository<MiscCharge>
    {
        public int GetNextVoucherNumber()
        {
            var maxnumber = 1001;
            if (!FiscalCollection.Any())
                return maxnumber;
            return FiscalCollection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
        }
        public MiscCharge GetByVoucherNumber(int voucherno, VoucherType vtype)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno );
        }
        public bool IsVoucherExits(int voucherno, int id)
        {
            return FiscalCollection.Any(p => p.VoucherNumber == voucherno && p.Id != id);
        }
        public MiscCharge GetByVoucherNumber(int voucherno, int id)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno && p.Id != id);
        }
        public MiscCharge GetByVoucherNumber(int voucherno,string key, out bool next, out bool previous)
        {
            MiscCharge v = null;
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
                v = new MiscCharge();
                v.VoucherNumber = ConfigurationReader.GetConfigKeyValue<int>("VoucherStartNumber", 1001);
                v.CreatedAt = DateTime.Now;
            }
            next = FiscalCollection.Any(p => p.VoucherNumber > voucherno);
            previous = FiscalCollection.Any(p => p.VoucherNumber < voucherno);
            return v;
        }
        public void DeleteByVoucherNo(int voucherno)
        {
            var misccharge = FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno);
            Db.MiscCharges.Remove(misccharge);
            SaveLog(misccharge, ActionType.Deleted);
            Db.SaveChanges();
        }
        public override void Update(MiscCharge input)
        {

            var dbMisc = FiscalCollection.FirstOrDefault(p => p.Id == input.Id);
            var query = "Delete from MiscChargeItems where MiscChargeId=" + input.Id + " and CompanyId=" + SiteContext.Current.User.CompanyId;
            Db.Database.ExecuteSqlCommand(query);
            foreach (var item in input.MiscChargeItems)
            {
                Db.MiscChargeItems.Add(item);
            }
            SaveChanges();
            input.MiscChargeItems = null;

            Db.Entry(dbMisc).CurrentValues.SetValues(input);
            SaveChanges();
        }


    }
}