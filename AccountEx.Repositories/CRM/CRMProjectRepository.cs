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
    public class CRMProjectRepository : GenericRepository<CRMProject>
    {
        public CRMProjectRepository() : base() { }
        public CRMProjectRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public List<CRMCustomerListing> GetListing(int startRow, int pageSize, string sortColumn, string sortDirection, string textToSearch)
        {
            var query = string.Format("EXEC [DBO].[CRM_InvoiceListing] @COMPANYID = {0}, @COMPANYID = {1},@UserId={2}, @StartRow = {3}, @PageSize={4},@SortCol='{5}',@SortDir='{6}',@TextToSearch='{7}'",
                 SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, SiteContext.Current.User.Id, startRow, pageSize, sortColumn, sortDirection, textToSearch);
            return Db.Database.SqlQuery<CRMCustomerListing>(query).ToList();
        }
        public int GetNextVoucherNumber()
        {
            var maxnumber = 1;
            if (!FiscalCollection.Any())
                return maxnumber;
            return FiscalCollection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
        }
        public List<CRMCustomerProjectExtra> GetProjects(int customerId)
        {
            return AsQueryable<vw_CRMProjects>(true).Where(p => p.CustomerId == customerId).Select(p => new CRMCustomerProjectExtra
            {
                Id = p.Id,
                Name = p.VoucherNumber + "-" + p.Product,
                Product = p.Product,
                ProductId = p.ProductId
            }).ToList();
        }
        public List<IdName> GetProjectsProduct(int customerId)
        {
            return AsQueryable<vw_CRMProjects>(true).Where(p => p.CustomerId == customerId).Select(p => new IdName
            {
                Id = p.ProductId,
                Name = p.Product,
            }).ToList();
        }

        public List<CRMCustomerProjectWithPMCExtra> GetProjectsWithPMCDetail(int customerId)
        {
            return AsQueryable<vw_CRMProjects>().Where(p => p.CustomerId == customerId).Select(p => new CRMCustomerProjectWithPMCExtra
            {
                Id = p.Id,
                Name = p.VoucherNumber + "-" + p.Product + " (" + p.Currency + ":" + p.Price + ")",
                Product = p.Product,
                ProductId = p.ProductId,
                Division = p.Division,
                ActualProductId = p.ActualProductId,
                ActualProduct = p.ActualProduct,
                ActualProductDivision = p.ActualProductDivision

            }).ToList();
        }

        public List<IdName> GetProjectsByUserTypeId()
        {
            var query = AsQueryable<vw_CRMProjects>(true);
            if (SiteContext.Current.UserTypeId == CRMUserType.SalesExecutive)
            {
                query = query.Where(p => p.SalePersonId == SiteContext.Current.User.Id);
            }
            else if (SiteContext.Current.UserTypeId == CRMUserType.RSM)
            {
                query = query.Where(p => p.RegionId == SiteContext.Current.RegionId);
            }
            else if (SiteContext.Current.UserTypeId == CRMUserType.DivisionalHead)
            {
                query = query.Where(p => p.DivisionId == SiteContext.Current.DivisionId);
            }

            return query.Select(p => new IdName()
            {
                Id = p.Id,
                Name = p.VoucherNumber + "-" + p.Product
            }).ToList();
        }

        public List<CRMCustomerProjectExtra> GetProjects(int customerId, int productId)
        {
            return AsQueryable<vw_CRMProjects>(true).Where(p => p.CustomerId == customerId && p.ProductId == productId).Select(p => new CRMCustomerProjectExtra
            {
                Id = p.Id,
                Name = p.VoucherNumber + "-" + p.Product,
                Product = p.Product,
                ProductId = p.ProductId
            }).ToList();
        }
        public List<int> GetPmcItemIds(int customerId, List<int> pmcItemIds)
        {
            return AsQueryable<vw_CRMProjects>(true).Where(p => p.CustomerId == customerId && pmcItemIds.Contains(p.PMCItemId)).Select(p => p.PMCItemId).ToList();
        }

        public CRMProject GetByVoucherNumber(int voucherno, VoucherType vtype)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno);
        }
        public CRMProject GetProject(int customerId, int productId, int currencyId)
        {
            return FiscalCollection.FirstOrDefault(p => p.CustomerId == customerId && p.ProductId == productId && p.CurrencyId == currencyId);
        }
        public CRMProject GetProject(int customerId, int productId)
        {
            return FiscalCollection.FirstOrDefault(p => p.CustomerId == customerId && p.ProductId == productId);
        }
        public List<CRMProject> GetByVoucherNumber(VoucherType vtype, int voucherno)
        {
            return FiscalCollection.Where(p => p.VoucherNumber == voucherno).ToList();
        }
        public CRMProject GetByVoucherNumber(int voucherno, VoucherType type, int id)
        {
            return FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno && p.Id != id);
        }
        public bool IsVoucherExits(int voucherno, int id)
        {
            return FiscalCollection.Any(p => p.VoucherNumber == voucherno && p.Id != id);
        }
        public bool IsProjectExits(int customerId, int currencyId, int productId, int id)
        {
            return FiscalCollection.Any(p => p.CustomerId == customerId && p.CurrencyId == currencyId && p.ProductId == productId && p.Id != id);
        }
        public bool IsProjectExits(int customerId, int productId, int id)
        {
            return FiscalCollection.Any(p => p.CustomerId == customerId && p.ProductId == productId && p.Id != id);
        }
        public int GetVoucherNumberbyPMCItem(int pmcItemId)
        {
            var voucherNo = 0;
            if (FiscalCollection.Any(p => p.PMCItemId == pmcItemId))
                voucherNo = FiscalCollection.FirstOrDefault(p => p.PMCItemId == pmcItemId).VoucherNumber;
            return voucherNo;

        }
        public int GetIdbyPMCItem(int pmcItemId)
        {
            var id = 0;
            if (FiscalCollection.Any(p => p.PMCItemId == pmcItemId))
                id = Collection.FirstOrDefault(p => p.PMCItemId == pmcItemId).Id;
            return id;

        }




        public CRMProject GetByVoucherNumber(int voucherno, VoucherType vtype, string key, out bool next, out bool previous)
        {
            CRMProject v = null;
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
                v = new CRMProject();
                v.VoucherNumber = ConfigurationReader.GetConfigKeyValue<int>("VoucherStartNumber", 1001);
                v.Date = DateTime.Now;
            }
            next = FiscalCollection.Any(p => p.VoucherNumber > voucherno);
            previous = FiscalCollection.Any(p => p.VoucherNumber < voucherno);
            return v;
        }

        //overload for service order

        public void DeleteByVoucherNumber(int voucherno)
        {
            var project = FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno);
            Delete(project);
        }


        public override void Delete(int id)
        {
            var project = FiscalCollection.FirstOrDefault(p => p.Id == id);
            Delete(project);
        }
        public void Delete(CRMProject PMC)
        {

            base.Delete(PMC.Id);
        }

        public override void Save(CRMProject PMC)
        {
            var repo = this;
            if (PMC.Id == 0)
            {
                repo.Add(PMC);
            }
            else
            {
                repo.Update(PMC);
            }
        }

        public override void Add(CRMProject PMC)
        {
            base.Add(PMC, true, false);
        }
        public override void Update(CRMProject PMC)
        {
            base.Update(PMC, true, false);
        }




    }
}