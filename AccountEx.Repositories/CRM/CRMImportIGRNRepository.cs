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
    public class CRMImportIGRNRepository : GenericRepository<CRMImportGRN>
    {
        public CRMImportIGRNRepository() : base() { }
        public CRMImportIGRNRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public List<CRMInvoiceListing> GetListing(int startRow, int pageSize, string sortColumn, string sortDirection, string textToSearch)
        {
            var query = string.Format("EXEC [DBO].[CRM_InvoiceListing] @COMPANYID = {0}, @FiscalId = {1},@UserId={2}, @StartRow = {3}, @PageSize={4},@SortCol='{5}',@SortDir='{6}',@TextToSearch='{7}'",
                 SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, SiteContext.Current.User.Id, startRow, pageSize, sortColumn, sortDirection, textToSearch);
            return Db.Database.SqlQuery<CRMInvoiceListing>(query).ToList();
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
        public List<CRMImportGRN> GetByPINumber(string piNumber)
        {
            return FiscalCollection.Where(p => p.PINumber == piNumber).ToList();
        }




    }
}