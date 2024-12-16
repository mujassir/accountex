using AccountEx.CodeFirst.Models.Production;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using System.Linq;
using System.Linq.Expressions;

namespace AccountEx.Repositories.Production
{
    public class ProductionUnitRepository : GenericRepository<ProductionUnit>
    {
        public ProductionUnitRepository() : base() { }
        public ProductionUnitRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }

        public int GetNextVoucherNumber(int locationId)
        {
            const int maxNumber = 1;
            if (!FiscalCollection.Any())
                return maxNumber;
            var voucher = FiscalCollection.Where(p => p.LocationId == locationId).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
            if (voucher != null)
            {
                var vno = voucher.VoucherNumber;
                vno = vno + 1;
                return vno;
            }
            return maxNumber;
        }

        public ProductionUnit GetByVoucherNumber(int locationId, int voucherNo)
        {
            return FiscalCollection.FirstOrDefault(p => p.LocationId == locationId && p.VoucherNumber == voucherNo);
        }

        public bool IsVoucherExists(int id)
        {
            return FiscalCollection.Any(p => p.Id != id);
        }

        public ProductionUnit GetByVoucherNumber(int voucherno, string key, int locationId, out bool next, out bool previous)
        {
            ProductionUnit v = null;
            switch (key)
            {
                case "first":
                    v = FiscalCollection.Where(p => p.LocationId == locationId).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "last":
                    v = FiscalCollection.Where(p => p.LocationId == locationId).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "next":
                    v = FiscalCollection.Where(p => p.LocationId == locationId && p.VoucherNumber > voucherno).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "previous":
                    v = FiscalCollection.Where(p => p.LocationId == locationId && p.VoucherNumber < voucherno).OrderByDescending(p => p.VoucherNumber).FirstOrDefault(); ;
                    break;
                case "same":
                    v = FiscalCollection.FirstOrDefault(p => p.LocationId == locationId && p.VoucherNumber == voucherno);
                    break;
                case "challan":
                    v = FiscalCollection.FirstOrDefault(p => p.LocationId == locationId && p.VoucherNumber == voucherno);
                    break;

            }

            if (v != null)
                voucherno = v.VoucherNumber;
            else if (key != "nextvouchernumber" && key != "challan")
            {
                v = FiscalCollection.Where(p => p.LocationId == locationId).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();

            }
            next = FiscalCollection.Any(p => p.LocationId == locationId && p.VoucherNumber > voucherno);
            previous = FiscalCollection.Any(p => p.LocationId == locationId && p.VoucherNumber < voucherno);
            return v;
        }
        public void DeleteByVoucherNumber(int id)
        {
            var rec = FiscalCollection.FirstOrDefault(p => p.Id == id);
            if (rec != null)
            {
                ObjectSet.Remove(rec);
                Db.SaveChanges();
            }
        }
        public override void Update(ProductionUnit p)
        {
            var record = GetById(p.Id, true);
            var ids = p.Items.Select(q => q.Id).ToList();
            var deletedIds = record.Items.Where(q => !ids.Contains(q.Id)).Select(q => q.Id).ToList();
            new ProductionUnitItemRepository(this).Delete(deletedIds);
            new ProductionUnitItemRepository(this).Save(p.Items.ToList());
            base.Update(p, true, false);
        }
    }
}
