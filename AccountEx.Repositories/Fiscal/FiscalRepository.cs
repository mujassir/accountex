using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.CodeFirst;
using AccountEx.Common;
using AccountEx.CodeFirst.Models;
using System.Data.Entity;

namespace AccountEx.Repositories
{
    public class FiscalRepository : GenericRepository<Fiscal>
    {
        public Fiscal GetNextFiscal(DateTime currentFiscalToDate, int companyId, bool createIfNotExist)
        {
            var fiscal = Db.Fiscals.Where(p => p.CompanyId == companyId && currentFiscalToDate < p.FromDate)
                        .OrderBy(p => p.FromDate).FirstOrDefault();
            if (fiscal == null && createIfNotExist)
            {
                fiscal = new Fiscal()
                {
                    CompanyId = companyId,
                    FromDate = currentFiscalToDate.AddDays(1),
                    IsDefault = true,
                };
                fiscal.ToDate = fiscal.FromDate.AddYears(1);
                fiscal.Name = fiscal.FromDate.ToString("MMMM yyyy") + " to " + fiscal.ToDate.ToString("MMMM yyyy");
                Db.Fiscals.Add(fiscal);
                Db.SaveChanges();
            }
            return fiscal;
        }
        public Fiscal GetPreviousFiscal(DateTime currentFiscalFromDate, int companyId)
        {
            var fiscal = Db.Fiscals.Where(p => p.CompanyId == companyId && p.ToDate < currentFiscalFromDate)
                        .OrderByDescending(p => p.ToDate).FirstOrDefault();
            return fiscal;
        }


        public bool IsAlreadyExistInDates(int id, DateTime fromDate, DateTime toDate, out string configName)
        {
            configName = "";
            if (id != 0) return false;
            var config = Collection.FirstOrDefault(p =>
                    (fromDate >= p.FromDate && fromDate <= p.ToDate)
                || (toDate >= p.FromDate && toDate <= p.ToDate)
                );
            if (config != null)
            {
                configName = config.Name;
                return true;
            }
            return false;

        }
        public List<IdName> GetNames(int companyId)
        {
            return Db.Fiscals.Where(p => p.CompanyId == companyId).OrderBy(p => p.FromDate)
                .ThenByDescending(p => p.IsDefault).Select(p => new IdName() { Id = p.Id, Name = p.Name }).ToList();
        }
        public List<IdName> GetShortNames()
        {
            return Collection.OrderByDescending(p => p.FromDate)
                .ThenByDescending(p => p.IsDefault).Select(p => new IdName() { Id = p.Id, Name = p.ShortName }).ToList();
        }
        public Fiscal GetDefaultFiscal()
        {
            if (Collection.Any(p => p.IsDefault))
                return Collection.AsNoTracking().FirstOrDefault(p => p.IsDefault);
            else
                return Collection.AsNoTracking().FirstOrDefault();
        }
        public int GetTotalFiscal(int companyId)
        {
            return Db.Fiscals.Count(p => p.CompanyId == companyId);
        }
    }
}
