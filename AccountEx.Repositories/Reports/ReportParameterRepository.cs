using System.Collections.Generic;
using System.Linq;
using AccountEx.CodeFirst.Models;

namespace AccountEx.Repositories
{
    public class ReportParameterRepository : GenericRepository<ReportParameter>
    {
        public List<ReportParameter> GetByReportId(int reportId)
        {
            return Db.ReportParameters.Where(p => p.ReportId == reportId).ToList();
        }
    }
}