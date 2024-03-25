using AccountEx.CodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Core.Objects;
using AccountEx.Common;
using AccountEx.CodeFirst.Models.CRM;
using AccountEx.CodeFirst.Models.Nexus;
using AccountEx.Common.Nexus;
using System.Data.Entity;

namespace AccountEx.Repositories
{
    public class NexusCaseRepository : GenericRepository<NexusPostedCases>
    {
        public NexusCaseRepository() : base() { }
        public NexusCaseRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public IQueryable<Nexus_Vw_Cases> AsQueryableCaseView()
        {
            return Db.Nexus_Vw_Cases.AsQueryable();
        }
        public NexusPostedCases GetByCaseId(long caseId)
        {
            return Db.NexusPostedCases.FirstOrDefault(p => p.CaseId == caseId);
        }
        public List<NexusPostedCases> GetByCaseIds(List<long> caseIds)
        {
            return Collection.Where(p => caseIds.Contains(p.CaseId)).AsNoTracking().ToList();
        }
        public int GetVoucherNoByCaseId(long caseId)
        {
            if (FiscalCollection.Any(p => p.CaseId == caseId))
                return FiscalCollection.FirstOrDefault(p => p.CaseId == caseId).VoucherNumber;
            else return 0;

        }
        public List<IdName> GetReferences()
        {

            return Db.Nexus_Reference.Select(p => new IdName
            {
                Id = p.ID,
                Name = p.Code + " - " + p.Name,

            }).ToList();
        }
        public Nexus_Vw_Cases GetFromNexusCaseViewByCaseId(long caseId)
        {
            return Db.Nexus_Vw_Cases.FirstOrDefault(p => p.ID == caseId);
        }
        public Nexus_Vw_Cases GetFromNexusCaseViewByCaseNo(string caseNo)
        {
            return Db.Nexus_Vw_Cases.FirstOrDefault(p => p.CaseNumber == caseNo);
        }
        public long GetCaseIdFromNexusCaseViewByCaseNo(string caseNo)
        {
            if (Db.Nexus_Vw_Cases.Any(p => p.CaseNumber == caseNo))
                return Db.Nexus_Vw_Cases.FirstOrDefault(p => p.CaseNumber == caseNo).ID;
            else return 0;

        }
        public List<NexusUnpostedCases> GetUnpostedCases(DateTime fromDate, DateTime toDate, NexusCaseType type, string caseNo = "")
        {
            var query = string.Format("EXEC [DBO].[Nexus_GetUnPostedCases] @COMPANYID = {0},@Fromate = {1},@ToDate = {2},@Type = {3},@CaseNo = {4}",
                 SiteContext.Current.User.CompanyId, "'" + (fromDate != DateTime.MinValue ? fromDate.ToString("yyyy-MM-dd") : "NULL") + "'", "'" + (toDate != DateTime.MinValue ? toDate.ToString("yyyy-MM-dd") : "NULL") + "'", type, (!string.IsNullOrWhiteSpace(caseNo) ? caseNo : "NULL"));
            return Db.Database.SqlQuery<NexusUnpostedCases>(query).ToList();
        }
        public List<NexusUnpostedCases> GetPostedCasesForUpdate(DateTime fromDate, DateTime toDate, NexusCaseType type, string caseNo = "")
        {
            fromDate = SiteContext.Current.Fiscal.FromDate;
            toDate = SiteContext.Current.Fiscal.ToDate;
            var query = string.Format("EXEC [DBO].[Nexus_GetUnPostedCasesForUpdate] @COMPANYID = {0},@Fromate = {1},@ToDate = {2},@Type = {3},@CaseNo = '{4}'",
                 SiteContext.Current.User.CompanyId, "'" + (fromDate != DateTime.MinValue ? fromDate.ToString("yyyy-MM-dd") : "NULL") + "'", "'" + (toDate != DateTime.MinValue ? toDate.ToString("yyyy-MM-dd") : "NULL") + "'", type, (!string.IsNullOrWhiteSpace(caseNo) ? caseNo : "NULL"));
            return Db.Database.SqlQuery<NexusUnpostedCases>(query).ToList();
        }
        public List<NexusTestWithPrice> GetTest()
        {
            var query = string.Format("EXEC [DBO].[GetRateLists] @COMPANYID = {0}",
                 SiteContext.Current.User.CompanyId);
            return Db.Database.SqlQuery<NexusTestWithPrice>(query).ToList();
        }
        public List<NexusInvoicePrinting> GetInvoicePrintingData(int departmentId, DateTime fromdate, DateTime todate)
        {
            var query = string.Format("EXEC [DBO].[GetNexusInvoicePrintingData] @COMPANYID = {0}, @FROMDATE = {1}, @TODATE={2},@DepartmentId={3}",
                 SiteContext.Current.User.CompanyId, "'" + fromdate.ToString("yyyy-MM-dd") + "'", "'" + todate.ToString("yyyy-MM-dd") + "'", departmentId);
            return Db.Database.SqlQuery<NexusInvoicePrinting>(query).ToList();
        }
        public Nexus_Case GetFromNexusTableByCaseId(long caseId)
        {
            return Db.Nexus_Case.FirstOrDefault(p => p.ID == caseId);
        }
        public void UpdateIsPosted(List<long> caseDetailIds)
        {
            if (caseDetailIds.Count() > 0)
                Db.Database.ExecuteSqlCommand("UPDATE dbo.Nexus_CaseDetail SET IsPosted='1' WHERE ID IN(" + string.Join(",", caseDetailIds) + ")");
        }
        public void UpdateIsPostedByNexusCaseId(long caseId, bool isPosted)
        {
            if (caseId > 0)
                Db.Database.ExecuteSqlCommand("UPDATE dbo.Nexus_CaseDetail SET IsPosted='" + (isPosted ? 1 + "" : 0 + "") + "' WHERE CaseID IN(" + string.Join(",", caseId) + ")");
        }
       
        public int GetNextVoucherNumber()
        {
            var maxnumber = 1;
            if (!FiscalCollection.Any())
                return maxnumber;
            return FiscalCollection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
        }


        public NexusPostedCases GetByVoucherNumber(int voucherno, VoucherType vtype)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno);
        }
        public List<NexusPostedCases> GetByVoucherNumber(VoucherType vtype, int voucherno)
        {
            return FiscalCollection.Where(p => p.VoucherNumber == voucherno).ToList();
        }
        public NexusPostedCases GetByVoucherNumber(int voucherno, VoucherType type, int id)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno && p.Id != id);
        }
        public bool IsVoucherExits(int voucherno, int id)
        {
            return FiscalCollection.Any(p => p.VoucherNumber == voucherno && p.Id != id);
        }




        public NexusPostedCases GetByVoucherNumber(int voucherno, VoucherType vtype, string key, out bool next, out bool previous)
        {
            NexusPostedCases v = null;
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
                v = new NexusPostedCases();
                v.VoucherNumber = ConfigurationReader.GetConfigKeyValue<int>("VoucherStartNumber", 1001);
            }
            next = FiscalCollection.Any(p => p.VoucherNumber > voucherno);
            previous = FiscalCollection.Any(p => p.VoucherNumber < voucherno);
            return v;
        }

        //overload for service order

        public void DeleteByVoucherNumber(int voucherno)
        {
            var Nexus_PostedCases = FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno);
            if (Nexus_PostedCases != null)
                Delete(Nexus_PostedCases);
        }


        public override void Delete(int id)
        {
            var r = FiscalCollection.FirstOrDefault(p => p.Id == id);
            Delete(r);
        }
        public void Delete(NexusPostedCases data)
        {

            base.Delete(data.Id);
        }

        public override void Save(NexusPostedCases data)
        {
            var repo = new NexusCaseRepository();
            if (data.Id == 0)
            {
                repo.Add(data);
            }
            else
            {
                repo.Update(data);
            }
        }

        public override void Add(NexusPostedCases data)
        {
            base.Add(data, true, false);
        }
        public override void Update(NexusPostedCases postedCase)
        {
            var ItemsRepo = new PostedCaseItemRepository(this);
            var dbCase = GetById(postedCase.Id, true);
            //add,update & remove services items
            var Ids = postedCase.NexusPostedCasesItems.Select(p => p.Id).ToList();
            var deletedIds = dbCase.NexusPostedCasesItems.Where(p => !Ids.Contains(p.Id)).Select(p => p.Id).ToList();
            ItemsRepo.Delete(deletedIds);
            ItemsRepo.Save(postedCase.NexusPostedCasesItems.ToList());
            //SaveChanges();
            postedCase.NexusPostedCasesItems = null;
            base.Update(postedCase, true, false);
        }




    }
}