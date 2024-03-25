using AccountEx.CodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Core.Objects;
using AccountEx.Common;
using AccountEx.CodeFirst.Models.CRM;
using AccountEx.Common.CRM;

namespace AccountEx.Repositories
{
    public class CRMImportRequisitionRepository : GenericRepository<CRMImportRequisition>
    {
        public CRMImportRequisitionRepository() : base() { }
        public CRMImportRequisitionRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }

        public int GetNextVoucherNumber()
        {
            var statuses = new List<CRMImportRequisitionStatus>() { CRMImportRequisitionStatus.Archive };
            var maxnumber = 1;
            if (!FiscalCollection.Any())
                return maxnumber;
            return FiscalCollection.Where(p => !statuses.Contains(p.Status)).OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
        }
        public int GetNextBookNumber(VoucherType vouchertype)
        {
            var maxnumber = 1001;
            if (!FiscalCollection.Any())
                return maxnumber;
            return FiscalCollection.OrderByDescending(p => p.InvoiceNumber).FirstOrDefault().InvoiceNumber + 1;
        }


        public CRMImportRequisition GetByVoucherNumber(int voucherno, VoucherType vtype)
        {
            var statuses = new List<CRMImportRequisitionStatus>() { CRMImportRequisitionStatus.Archive };
            return FiscalCollection.FirstOrDefault(p => !statuses.Contains(p.Status) && p.VoucherNumber == voucherno);
        }
        public List<int> GetImportIdsByDivisionId(int divisionId)
        {
            var products = AsQueryable<vw_CRMPRoducts>();
            var ids = (from fc in FiscalCollection
                       join IRI in Db.ImportRequisitionItems on fc.Id equals IRI.CRMImportRequisitionId
                       join p in products on IRI.ItemId equals p.Id
                       where IRI.CompanyId.Value == SiteContext.Current.User.CompanyId && p.DivisionId == divisionId
                       && p.CompanyId == SiteContext.Current.User.CompanyId
                       select new
                       {
                           fc.Id,

                       }).Distinct().Select(p => p.Id).ToList();
            return ids;
        }
        public List<CRMImportRequisition> GetByVoucherNumber(VoucherType vtype, int voucherno)
        {
            var statuses = new List<CRMImportRequisitionStatus>() { CRMImportRequisitionStatus.Archive };
            return FiscalCollection.Where(p => !statuses.Contains(p.Status) && p.VoucherNumber == voucherno).ToList();
        }
        public CRMImportRequisition GetByVoucherNumber(int voucherno, VoucherType type, int id)
        {
            var statuses = new List<CRMImportRequisitionStatus>() { CRMImportRequisitionStatus.Archive };
            return FiscalCollection.FirstOrDefault(p => !statuses.Contains(p.Status) && p.VoucherNumber == voucherno && p.Id != id);
        }
        public bool IsVoucherExits(int voucherno, int id)
        {
            var statuses = new List<CRMImportRequisitionStatus>() { CRMImportRequisitionStatus.Archive };
            return FiscalCollection.Any(p => !statuses.Contains(p.Status) && p.VoucherNumber == voucherno && p.Id != id);
        }
        public CRMImportRequisition GetByBookNumber(int bookNo, VoucherType type, int id)
        {
            return FiscalCollection.FirstOrDefault(p => p.InvoiceNumber == bookNo && p.Id != id);
        }
        public bool IsBookNoExits(int bookNo, VoucherType type, int id)
        {
            return FiscalCollection.Any(p => p.InvoiceNumber == bookNo && p.Id != id);
        }
        public List<ImportRequisitionHisotryExtra> GetHistoryByVoucherNo(int voucherno)
        {
            var statuses = new List<CRMImportRequisitionStatus>() { CRMImportRequisitionStatus.Archive };
            return FiscalCollection.Where((p => statuses.Contains(p.Status) && p.VoucherNumber == voucherno)).Select(p => new ImportRequisitionHisotryExtra()
            {
                RevisionNo = p.RevisionNo,
                VoucherNumber = p.VoucherNumber,
                ReviseDate = p.ReviseDate
            }).ToList();
        }



        public CRMImportRequisition GetByVoucherNumber(int voucherno, CRMImportRequisitionType requisitionType, string key, out bool next, out bool previous)
        {
            CRMImportRequisition v = null;

            var statuses = new List<CRMImportRequisitionStatus>() { CRMImportRequisitionStatus.Pending, CRMImportRequisitionStatus.Review };
            if (requisitionType == CRMImportRequisitionType.DH)
            {
                statuses = new List<CRMImportRequisitionStatus>() { CRMImportRequisitionStatus.Pending, CRMImportRequisitionStatus.Revised };
            }
            else if (requisitionType == CRMImportRequisitionType.RSM)
            {
                statuses = new List<CRMImportRequisitionStatus>() { CRMImportRequisitionStatus.Approved };
            }

            switch (key)
            {
                case "first":
                    v = FiscalCollection.OrderBy(p => p.VoucherNumber).FirstOrDefault(p => statuses.Contains(p.Status));
                    break;
                case "last":
                    v = FiscalCollection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault(p => statuses.Contains(p.Status));
                    break;
                case "next":
                    v = FiscalCollection.Where(p => statuses.Contains(p.Status) && p.VoucherNumber > voucherno).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "previous":
                    v = FiscalCollection.Where(p => statuses.Contains(p.Status) && p.VoucherNumber < voucherno).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "same":
                    v = FiscalCollection.FirstOrDefault(p => statuses.Contains(p.Status) && p.VoucherNumber == voucherno);
                    break;
                case "challan":
                    v = FiscalCollection.FirstOrDefault(p => statuses.Contains(p.Status) && p.VoucherNumber == voucherno);
                    break;

            }

            if (v != null)
                voucherno = v.VoucherNumber;
            else if (key != "nextvouchernumber" && key != "challan")
            {
                v = FiscalCollection.Where(p => statuses.Contains(p.Status)).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();

            }
            if (v == null && !FiscalCollection.Any(p => statuses.Contains(p.Status)))
            {
                v = new CRMImportRequisition();
                v.VoucherNumber = ConfigurationReader.GetConfigKeyValue<int>("VoucherStartNumber", 1001);
                v.InvoiceNumber = ConfigurationReader.GetConfigKeyValue<int>("InvoiceStartNumber", 1001);
                v.Date = DateTime.Now;
            }
            next = FiscalCollection.Any(p => statuses.Contains(p.Status) && p.VoucherNumber > voucherno);
            previous = FiscalCollection.Any(p => statuses.Contains(p.Status) && p.VoucherNumber < voucherno);
            return v;
        }

        //overload for service order

        public void DeleteByVoucherNumber(int voucherno)
        {
            var CRMImportRequisition = FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno);
            Delete(CRMImportRequisition);
        }


        public override void Delete(int id)
        {
            var CRMImportRequisition = FiscalCollection.FirstOrDefault(p => p.Id == id);
            Delete(CRMImportRequisition);
        }
        public void Delete(CRMImportRequisition recrod)
        {

            base.Delete(recrod.Id);
        }

        public override void Save(CRMImportRequisition record)
        {
            var repo = new CRMImportRequisitionRepository();
            if (record.Id == 0)
            {
                repo.Add(record);
            }
            else
            {
                repo.Update(record);
            }
        }

        public override void Add(CRMImportRequisition record)
        {
            base.Add(record, true, false);
        }
        public override void Update(CRMImportRequisition record)
        {
            var CRMImportRequisitionItemsRepo = new CRMImportRequisitionItemRepository(this);
            var dbCRMImportRequisition = GetById(record.Id, true);

            var Ids = record.CRMImportRequisitionItems.Select(p => p.Id).ToList();
            if (SiteContext.Current.UserTypeId == CRMUserType.DivisionalHead)
            {
                var dbProductIds = dbCRMImportRequisition.CRMImportRequisitionItems.Select(p => p.ItemId).ToList();
                var DHproductIds = new CRMProductRepository().GetProductIdsByDivision(dbProductIds, SiteContext.Current.DivisionId);
                Ids.AddRange(dbCRMImportRequisition.CRMImportRequisitionItems.Where(p => !DHproductIds.Contains(p.ItemId)).Select(p => p.Id).ToList());
            }

            //add,update & remove services items
            
            var deletedIds = dbCRMImportRequisition.CRMImportRequisitionItems.Where(p => !Ids.Contains(p.Id)).Select(p => p.Id).ToList();
            CRMImportRequisitionItemsRepo.Delete(deletedIds);
            CRMImportRequisitionItemsRepo.Save(record.CRMImportRequisitionItems.ToList());


            //SaveChanges();
            record.CRMImportRequisitionItems = null;
            base.Update(record, true, false);
        }




    }
}