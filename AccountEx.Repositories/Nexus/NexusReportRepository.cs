using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Common.Nexus;
using AccountEx.DbMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Repositories.Nexus
{
    public class NexusReportRepository : GlobalGenericRepository<Report>
    {

        public List<NexusDepartmentSummary> GetSummaryOfDepartmentBilling(DateTime fromdate, DateTime todate)
        {
            var query = string.Format("EXEC [DBO].[NexusDepartmentBillingSummary] @COMPANYID = {0},@FISCALID={1}, @FROMDATE = {2}, @TODATE={3}",
                 SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, "'" + fromdate.ToString("yyyy-MM-dd") + "'", "'" + todate.ToString("yyyy-MM-dd") + "'");
            return Db.Database.SqlQuery<NexusDepartmentSummary>(query).ToList();
        }
        public List<NexusDepartmentSummary> GetPendingSummary(DateTime fromdate, DateTime todate)
        {
            var query = string.Format("EXEC [DBO].[NexusDepartmentPendingSummary] @COMPANYID = {0},@FISCALID={1}, @FROMDATE = {2}, @TODATE={3}",
                 SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, "'" + fromdate.ToString("yyyy-MM-dd") + "'", "'" + todate.ToString("yyyy-MM-dd") + "'");
            return Db.Database.SqlQuery<NexusDepartmentSummary>(query).ToList();
        }
        public List<NexusDepartmentSummaryByPatient> GetSummaryOfDepartmentBillingByPatient(DateTime fromdate, DateTime todate)
        {
            var query = string.Format("EXEC [DBO].[NexusDepartmentBillingSummaryByPatient] @COMPANYID = {0},@FISCALID={1}, @FROMDATE = {2}, @TODATE={3}",
                 SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, "'" + fromdate.ToString("yyyy-MM-dd") + "'", "'" + todate.ToString("yyyy-MM-dd") + "'");
            return Db.Database.SqlQuery<NexusDepartmentSummaryByPatient>(query).ToList();
        }
        public List<NexusBillingByPatient> GetBillingByPatient(DateTime fromdate, DateTime todate, int departmentId)
        {
            var query = string.Format("EXEC [DBO].[NexusBillingByPatient] @COMPANYID = {0},@FISCALID={1}, @FROMDATE = {2}, @TODATE={3},@DepartmentId={4}",
                 SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, "'" + fromdate.ToString("yyyy-MM-dd") + "'", "'" + todate.ToString("yyyy-MM-dd") + "'", departmentId);
            return Db.Database.SqlQuery<NexusBillingByPatient>(query).ToList();
        }
        public List<NexusReferralSummary> GetReferralSummary(DateTime fromdate, DateTime todate)
        {
            var query = string.Format("EXEC [DBO].[NexusReferralSummary] @COMPANYID = {0},@FISCALID={1}, @FROMDATE = {2}, @TODATE={3}",
                 SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, "'" + fromdate.ToString("yyyy-MM-dd") + "'", "'" + todate.ToString("yyyy-MM-dd") + "'");
            return Db.Database.SqlQuery<NexusReferralSummary>(query).ToList();
        }

        public List<NexusReceivablesSummary> GetReceivablesSummary(DateTime fromdate, DateTime todate)
        {
            var query = string.Format("EXEC [DBO].[NexusMonthlyReceiptSummary] @COMPANYID = {0},@FISCALID={1}, @FROMDATE = {2}, @TODATE={3}",
                 SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, "'" + fromdate.ToString("yyyy-MM-dd") + "'", "'" + todate.ToString("yyyy-MM-dd") + "'");
            return Db.Database.SqlQuery<NexusReceivablesSummary>(query).ToList();
        }
        public List<NexusReceivablesSummaryByDepartment> GetReceivablesSummaryByDepartment(DateTime fromdate, DateTime todate, int departmentId)
        {
            var query = string.Format("EXEC [DBO].[NexusReceiptSummaryByDepartment] @COMPANYID = {0},@FISCALID={1}, @FROMDATE = {2}, @TODATE={3},@DepartmentId={4}",
                 SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, "'" + fromdate.ToString("yyyy-MM-dd") + "'", "'" + todate.ToString("yyyy-MM-dd") + "'",departmentId);
            return Db.Database.SqlQuery<NexusReceivablesSummaryByDepartment>(query).ToList();
        }
        public List<NexusMonthlyReceiptSummary> GetMonthlyReceiptSummary(DateTime fromdate, DateTime todate)
        {
            var query = string.Format("EXEC [DBO].[NexusMonthlyReceiptSummaryWithDetail] @COMPANYID = {0},@FISCALID={1}, @FROMDATE = {2}, @TODATE={3}",
                 SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, "'" + fromdate.ToString("yyyy-MM-dd") + "'", "'" + todate.ToString("yyyy-MM-dd") + "'");
            return Db.Database.SqlQuery<NexusMonthlyReceiptSummary>(query).ToList();
        }

        

    }
}
