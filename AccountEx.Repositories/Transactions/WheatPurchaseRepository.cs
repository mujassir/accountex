using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using AccountEx.CodeFirst.Models;


namespace AccountEx.Repositories
{
    public class WheatPurchaseRepository : GenericRepository<WheatPurchase>
    {
        public int GetNextVoucherNumber()
        {
            var maxnumber = ConfigurationReader.GetConfigKeyValue<int>("VoucherStartNumber",1);
            if (!FiscalCollection.Any())
                return maxnumber;
            var voucher = FiscalCollection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
            if (voucher != null)
                return voucher.VoucherNumber + 1;
            return maxnumber;
        }

        public WheatPurchase GetByVoucherNumber(int voucherno, string key, out bool next, out bool previous)
        {
            WheatPurchase v = null;
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
                v = new WheatPurchase();
                v.VoucherNumber = ConfigurationReader.GetConfigKeyValue<int>("VoucherStartNumber", 1001);
                v.InvoiceNumber = ConfigurationReader.GetConfigKeyValue<int>("InvoiceStartNumber", 1001);
                v.Date = DateTime.Now;
                v.CreatedAt = DateTime.Now;
            }
            next = FiscalCollection.Any(p => p.VoucherNumber > voucherno);
            previous = FiscalCollection.Any(p => p.VoucherNumber < voucherno);
            return v;
        }
        public void DeleteByVoucherNumber(int voucherno)
        {


            //Db.WheatPurchases.Delete(p => p.CompanyId == SiteContext.Current.User.CompanyId && p.VoucherNumber == voucherno);
            if (FiscalCollection.Any(p => p.VoucherNumber == voucherno))
            {
                var record = FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno);
                ObjectSet.Remove(record);
                SaveLog(record, ActionType.Deleted);
                Db.SaveChanges();
            }
        }
        public override void Save(WheatPurchase wp)
        {
            var repo = new WheatPurchaseRepository();

            if (wp.Id == 0)
            {
                new WheatPurchaseRepository().Add(wp);
            }
            else
            {
                repo.Update(wp);

            }


        }
        public override void Update(WheatPurchase p)
        {
            //Db.WheatPurchaseItems.Delete(q => q.PurchaseId == p.Id && p.CompanyId == SiteContext.Current.User.CompanyId);

            var query = "Delete from WheatPurchaseItems where PurchaseId=" + p.Id + " AND CompanyId=" + SiteContext.Current.User.CompanyId;
            Db.Database.ExecuteSqlCommand(query);
            foreach (var item in p.WheatPurchaseItems)
            {
                Db.WheatPurchaseItems.Add(item);
                Db.SaveChanges();
            }
            p.WheatPurchaseItems = null;
            base.Update(p);
            SaveChanges();


        }

        public List<LatestSoldItemExtra> GetLatestSoldRates()
        {
            var query = "";
            query = string.Format("EXEC [dbo].[GetSJFMLatestSoldRates] @CompanyId={0}", SiteContext.Current.User.CompanyId);
            var data= Db.Database.SqlQuery<LatestSoldItemExtra>(query).ToList();
            return data;
        }
      
        public WheatPurchase GetByVoucherNo(int voucherno, int id)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno && p.Id != id);
        }
        public int GetNextInvoiceNumber(bool isGovt)
        {

            var maxnumber = ConfigurationReader.GetConfigKeyValue<int>("InvoiceStartNumber",1);
            if (!isGovt)
            {
                if (!FiscalCollection.Any(p => !p.IsGovt))
                    return maxnumber;
                else
                    return FiscalCollection.Where(p => !p.IsGovt).OrderByDescending(p => p.InvoiceNumber).FirstOrDefault().InvoiceNumber + 1;
            }
            else
            {
                if (!FiscalCollection.Any(p => p.IsGovt))
                    return maxnumber;
                else
                    return FiscalCollection.Where(p => p.IsGovt).OrderByDescending(p => p.InvoiceNumber).FirstOrDefault().InvoiceNumber + 1;
            }
        }

    }
}