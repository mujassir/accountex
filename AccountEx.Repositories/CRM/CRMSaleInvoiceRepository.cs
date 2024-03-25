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
    public class CRMSaleInvoiceRepository : GenericRepository<CRMSaleInvoice>
    {
        public CRMSaleInvoiceRepository() : base() { }
        public CRMSaleInvoiceRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public List<CRMInvoiceListing> GetListing(int startRow, int pageSize, string sortColumn, string sortDirection, string textToSearch)
        {
            var query = string.Format("EXEC [DBO].[CRM_InvoiceListing] @COMPANYID = {0}, @FiscalId = {1},@UserId={2}, @StartRow = {3}, @PageSize={4},@SortCol='{5}',@SortDir='{6}',@TextToSearch='{7}'",
                 SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, SiteContext.Current.User.Id, startRow, pageSize, sortColumn, sortDirection, textToSearch);
            return Db.Database.SqlQuery<CRMInvoiceListing>(query).ToList();
        }
        public int GetNextVoucherNumber()
        {
            var maxnumber = 1;
            if (!FiscalCollection.Any())
                return maxnumber;
            return FiscalCollection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
        }
        public int GetNextBookNumber()
        {
            var maxnumber = 1001;
            if (!FiscalCollection.Any())
                return maxnumber;
            return FiscalCollection.OrderByDescending(p => p.InvoiceNumber).FirstOrDefault().InvoiceNumber + 1;
        }
        public int GetNGSTNextBookNumber()
        {
            var maxnumber = 1;
            if (!FiscalCollection.Any())
                return maxnumber;
            return FiscalCollection.Where(p => p.SaleType == CRMInvoiceType.NonGST).OrderByDescending(p => p.InvoiceNumber).FirstOrDefault().InvoiceNumber + 1;
        }
        public void ChangeCurrentToProjectSale(int projectId, int customerId, int productId)
        {
            if (FiscalCollection.Where(p => p.CustomerId == customerId).Any(p => p.CRMSaleInvoiceItems.Any(q => q.ItemId == productId)))
            {
                var invoiceItem = FiscalCollection.Where(p => p.CustomerId == customerId).SelectMany(p => p.CRMSaleInvoiceItems).FirstOrDefault(p => p.ItemId == productId);
                var projectSaleType = CRMSaleType.Project;
                if (projectSaleType != null && invoiceItem != null)
                {
                    invoiceItem.SaleType = CRMSaleType.Project;
                    invoiceItem.ProjectId = projectId;
                    SaveChanges();
                }
            }
        }
        public void ChangeAllToRegulerSale(int projectId)
        {
            var igrnQuery = AsQueryable<CRMImportGRN>(true);
            var saleType = CRMSaleType.Reguler;
            if (FiscalCollection.Any(p => p.CRMSaleInvoiceItems.Any(q => q.ProjectId == projectId)))
            {
                var invoiceItem = FiscalCollection.SelectMany(p => p.CRMSaleInvoiceItems.Where(q => q.ProjectId == projectId)).ToList();

                foreach (var item in invoiceItem)
                {
                    item.SaleType = saleType;
                    item.ProjectId = null;
                }
                SaveChanges();
            }
            if (igrnQuery.Any(p => p.ProjectId == projectId))
            {
                var igrnItems = igrnQuery.Where(p => p.ProjectId == projectId).ToList();
                foreach (var item in igrnItems)
                {
                    item.SaleType = saleType;
                    item.ProjectId = null;
                }
                SaveChanges();
            }
        }
        public void ChangeAllToProjectSale(int projectId, int customerId, int productId)
        {
            var igrnQuery = AsQueryable<CRMImportGRN>(true);
            var saleType = CRMSaleType.Project;
            if (FiscalCollection.Where(p => p.CustomerId == customerId).Any(p => p.CRMSaleInvoiceItems.Any(q => q.ItemId == productId)))
            {
                var invoiceItems = FiscalCollection.Where(p => p.CustomerId == customerId).SelectMany(p => p.CRMSaleInvoiceItems.Where(q => q.ItemId == productId)).ToList();

                foreach (var item in invoiceItems)
                {
                    item.SaleType = saleType;
                    item.ProjectId = projectId;
                }
                SaveChanges();
            }
            if (igrnQuery.Any(p => p.CustomerId == customerId && p.ProductId == productId))
            {
                var igrnItems = igrnQuery.Where(p => p.CustomerId == customerId && p.ProductId == productId).ToList();
                foreach (var item in igrnItems)
                {
                    item.SaleType = saleType;
                    item.ProjectId = projectId;
                }
                SaveChanges();
            }
        }


        public CRMSaleInvoice GetByVoucherNumber(int voucherno, VoucherType vtype)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno);
        }
        public List<CRMSaleInvoice> GetByVoucherNumber(VoucherType vtype, int voucherno)
        {
            return FiscalCollection.Where(p => p.VoucherNumber == voucherno).ToList();
        }
        public CRMSaleInvoice GetByVoucherNumber(int voucherno, VoucherType type, int id)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno && p.Id != id);
        }
        public bool IsVoucherExits(int voucherno, int id)
        {
            return FiscalCollection.Any(p => p.VoucherNumber == voucherno && p.Id != id);
        }
        public CRMSaleInvoice GetByBookNumber(int bookNo, VoucherType type, int id)
        {
            return FiscalCollection.FirstOrDefault(p => p.InvoiceNumber == bookNo && p.Id != id);
        }
        public bool IsInvoiceExist(int customerId, int productId)
        {
            return FiscalCollection.Any(p => p.CustomerId == customerId && p.CRMSaleInvoiceItems.Any(q => q.ItemId == productId && q.SaleType == CRMSaleType.Project));
        }
        public bool IsBookNoExits(int bookNo, int id)
        {
            return FiscalCollection.Any(p => p.InvoiceNumber == bookNo && p.Id != id);
        }
        public bool IsBookNoExits(int bookNo, int id, int customerId)
        {
            return FiscalCollection.Any(p => p.InvoiceNumber == bookNo && p.CustomerId == customerId && p.Id != id);
        }



        public CRMSaleInvoice GetByVoucherNumber(int voucherno, VoucherType vtype, string key, out bool next, out bool previous)
        {
            CRMSaleInvoice v = null;
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
                v = new CRMSaleInvoice();
                v.VoucherNumber = ConfigurationReader.GetConfigKeyValue<int>("VoucherStartNumber", 1001);
                v.InvoiceNumber = ConfigurationReader.GetConfigKeyValue<int>("InvoiceStartNumber", 1001);
                v.Date = DateTime.Now;
            }
            next = FiscalCollection.Any(p => p.VoucherNumber > voucherno);
            previous = FiscalCollection.Any(p => p.VoucherNumber < voucherno);
            return v;
        }

        //overload for service order

        public void DeleteByVoucherNumber(int voucherno)
        {
            var CRMSaleInvoice = FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno);
            Delete(CRMSaleInvoice);
        }


        public override void Delete(int id)
        {
            var CRMSaleInvoice = FiscalCollection.FirstOrDefault(p => p.Id == id);
            Delete(CRMSaleInvoice);
        }
        public void Delete(CRMSaleInvoice CRMSaleInvoice)
        {

            base.Delete(CRMSaleInvoice.Id);
        }

        public override void Save(CRMSaleInvoice CRMSaleInvoice)
        {
            var repo = new CRMSaleInvoiceRepository();
            if (CRMSaleInvoice.Id == 0)
            {
                repo.Add(CRMSaleInvoice);
            }
            else
            {
                repo.Update(CRMSaleInvoice);
            }
        }

        public override void Add(CRMSaleInvoice CRMSaleInvoice)
        {
            base.Add(CRMSaleInvoice, true, false);
        }
        public override void Update(CRMSaleInvoice CRMSaleInvoice)
        {
            var CRMSaleInvoiceItemsRepo = new CRMSaleInvoiceItemRepository(this);
            var dbCRMSaleInvoice = GetById(CRMSaleInvoice.Id, true);

            //add,update & remove services items
            var Ids = CRMSaleInvoice.CRMSaleInvoiceItems.Select(p => p.Id).ToList();


            //add,update & remove services items

            if (SiteContext.Current.UserTypeId == CRMUserType.SalesExecutive)
            {
                Ids.AddRange(dbCRMSaleInvoice.CRMSaleInvoiceItems.Where(p => p.SalePersonId != SiteContext.Current.User.Id).Select(p => p.Id).ToList());
            }
            else if (SiteContext.Current.UserTypeId == CRMUserType.DivisionalHead)
            {
                var dbProductIds = dbCRMSaleInvoice.CRMSaleInvoiceItems.Select(p => p.ItemId).ToList();
                var DHproductIds = new CRMProductRepository().GetProductIdsByDivision(dbProductIds, SiteContext.Current.DivisionId);
                Ids.AddRange(dbCRMSaleInvoice.CRMSaleInvoiceItems.Where(p => !DHproductIds.Contains(p.ItemId)).Select(p => p.Id).ToList());
            }
            var deletedIds = dbCRMSaleInvoice.CRMSaleInvoiceItems.Where(p => !Ids.Contains(p.Id)).Select(p => p.Id).ToList();
            CRMSaleInvoiceItemsRepo.Delete(deletedIds);
            CRMSaleInvoiceItemsRepo.Save(CRMSaleInvoice.CRMSaleInvoiceItems.ToList());


            //SaveChanges();
            CRMSaleInvoice.CRMSaleInvoiceItems = null;
            base.Update(CRMSaleInvoice, true, false);
        }
        public List<CRMSaleByProductIdExtra> GetSaleByProductIds(int salePersonId, int month, int year, int productId, byte type)
        {
            var query = string.Format("EXEC [dbo].[GetCRMSaleByProductIds] @COMPANYID={0}, @Month={1}, @Year={2}, @SalePersonId={3}, @ProductId={4}, @Type={5}", SiteContext.Current.User.CompanyId, month, year, salePersonId, productId, type);
            return Db.Database.SqlQuery<CRMSaleByProductIdExtra>(query).ToList();
        }
        public List<CRMSaleByProductExtra> GetSaleByType(int salePersonId, int month, int year, int? rsmId, byte type, int productId)
        {
            var query = string.Format("EXEC [dbo].[GetCRMSaleForecastByType] @COMPANYID={0}, @Month={1}, @Year={2}, @SalePersonId={3}, @RSMId={4}, @Type={5},@ProductId={6}", SiteContext.Current.User.CompanyId, month, year, salePersonId, rsmId, type, productId);
            return Db.Database.SqlQuery<CRMSaleByProductExtra>(query).ToList();
        }
        public List<CRMSaleByProductExtra> GetSaleForRSM(int? rsmId, int month, int year, int productId)
        {
            var query = string.Format("EXEC [dbo].[GetCRMSaleForecastForRSM] @RSMId={0},@COMPANYID={1}, @Month={2}, @Year={3},@ProductId={4}", SiteContext.Current.User.Id, SiteContext.Current.User.CompanyId, month, year, productId);
            return Db.Database.SqlQuery<CRMSaleByProductExtra>(query).ToList();
        }
        public List<CRMSaleByProductExtra> GetSaleForSalePerson(int salePersonId, int month, int year, int customerId, int productId)
        {
            var query = string.Format("EXEC [dbo].[GetCRMSaleForecastByForSalePerson] @COMPANYID={0}, @Month={1}, @Year={2}, @SalePersonId={3},@CustomerId={4},@ProductId={5}", SiteContext.Current.User.CompanyId, month, year, salePersonId, customerId, productId);
            return Db.Database.SqlQuery<CRMSaleByProductExtra>(query).ToList();
        }
        public List<CRMSaleByProductExtra> GetSaleForDH(int userId, int month, int year, int customerId, int productId)
        {
            var query = string.Format("EXEC [dbo].[GetCRMSaleForecastByForDH] @COMPANYID={0}, @Month={1}, @Year={2}, @UserId={3},@CustomerId={4},@ProductId={5}", SiteContext.Current.User.CompanyId, month, year, userId, customerId, productId);
            return Db.Database.SqlQuery<CRMSaleByProductExtra>(query).ToList();
        }




    }
}