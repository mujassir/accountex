using System.Collections.Generic;
using AccountEx.Common;
using System.Linq;
using AccountEx.CodeFirst.Models;
using AccountEx.DbMapping;

namespace AccountEx.Repositories
{
    public class StockObRepository : GenericRepository<AccountDetail>
    {
        public List<CustomerDiscount> GetDiscount(int customer)
        {
            var query = string.Format("EXEC [DBO].[GetDiscount] @CUSTOMERID = {0}, @COMPANYID = {1}, @FISCALID={2}",
              customer, SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id);

//            var query = @"SELECT
//                            ISNULL(cd.Id,0) as Id,
//							cd.CustomerId,
//                            cd.CustomerTitle,
//	                        p.Id as ProductId,
//                            p.Name AS ProductTitle,
//                            p.AccountId as COAProductId,
//	                        p.Code as ProductCode,
//                            p.IsDeleted,
//                           ISNULL(cd.Discount,0) as Discount
//	                        
//                        FROM Products p LEFT OUTER JOIN (Select * from CustomerDiscounts where CustomerId =" + customer + @") cd ON cd.COAProductId = p.AccountId
//                        WHERE p.IsDeleted = 0";


            return Db.Database.SqlQuery<CustomerDiscount>(query).ToList();
        }
        public List<ProductExtra> GetDiscountWithProducts(int customer)
        {
            var query = string.Format("EXEC [DBO].[GetDiscountWithProductsAD] @CUSTOMERID = {0}, @COMPANYID = {1}, @FISCALID={2}",
            customer, SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id);
//            var query = @"SELECT
//                            ISNULL(cd.Id,0) as Id,
//							cd.CustomerId,
//                            cd.CustomerTitle,
//	                        ad.Id as ProductId,
//                            ad.Name AS ProductTitle,
//                            ad.AccountId as COAProductId,
//	                        ad.Code as ProductCode,
//                             ISNULL(ad.PurchasePrice,0) as PurchasePrice,
//                             ISNULL(ad.SalePrice,0) as SalePrice,
//                             ad.IsDeleted,
//                           ISNULL(cd.Discount,0) as Discount
//	                        
//                        FROM AccountDetails ad LEFT OUTER JOIN (Select * from CustomerDiscounts where CustomerId =" + customer + @") cd ON cd.COAProductId = ad.AccountId
//                        WHERE ad.IsDeleted = 0";


            return Db.Database.SqlQuery<ProductExtra>(query).ToList();
        }
        public void Save(CustomerDiscountExtra obj)
        {
            foreach (var item in obj.Discounts)
            {
                var currentrecord = Db.CustomerDiscounts.FirstOrDefault(p => p.CustomerId == item.CustomerId && p.ProductId == item.ProductId);
                if (currentrecord != null)
                    currentrecord.Discount = item.Discount;
                else
                    Db.CustomerDiscounts.Add(item);
            }
            SaveChanges();
        }
        //public List<ActivityEntry> GetDailyActivity(DateTime date1, DateTime date2)
        //{
        //    var fromDate = date1.ToString("yyyy-MM-dd");
        //    var toDate = date2.ToString("yyyy-MM-dd");

        //    var query = string.Format("EXEC [DBO].[GetDailyActivity] @COMPANYID = {0}, @FROMDATE = {1}, @TODATE={2}",
        //        SiteContext.Current.User.CompanyId, fromDate, toDate);
        //    return Db.Database.SqlQuery<ActivityEntry>(query).ToList();

        //}
    }
}