using System.Collections.Generic;
using System.Linq;
using AccountEx.CodeFirst.Models;

namespace AccountEx.Repositories
{
    public class ReportColumnRepository : GenericRepository<ReportColumn>
    {
        public List<ReportColumn> GetByReportId(int reportId)
        {

            return Collection.Where(p => p.ReportId == reportId).ToList();
        }
    }
}