using System.Collections.Generic;
using AccountEx.Common;
using System.Linq;
using AccountEx.CodeFirst.Models;
using System;
using AccountEx.DbMapping;

namespace AccountEx.Repositories
{
    public class ProductMappingRepository : GenericRepository<ProductMapping>
    {
        public ProductMappingRepository() : base() { }
        public ProductMappingRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public List<CustomerDiscount> GetDiscount(int customer)
        {
            var query = string.Format("EXEC [DBO].[GetDiscountByCustomer] @CompanyId = {0}, @CustomerId = {1}",
                SiteContext.Current.User.CompanyId, customer);
            return Db.Database.SqlQuery<CustomerDiscount>(query).ToList();
        }
        public List<ProdMappingExtra> GetDiscountWithProducts(int customerId)
        {
            var query = string.Format("EXEC [DBO].[GetProductMappings] @CompanyId = {0}, @CustomerId = {1}, @AccountDetailFormId={2}",
                SiteContext.Current.User.CompanyId, customerId, (byte)AccountDetailFormType.Products);
            return Db.Database.SqlQuery<ProdMappingExtra>(query).ToList();
        }
        public List<ProductExtra> GetDiscountWithProducts()
        {
            var query = string.Format("EXEC [DBO].[GetDiscountWithProductsByFormId] @CompanyId = {0}, @AccountDetailFormId={1}",
               SiteContext.Current.User.CompanyId, (byte)AccountDetailFormType.Products);
            return Db.Database.SqlQuery<ProductExtra>(query).ToList();

        }
        public void Save(ProductMappingExtra obj)
        {

            int? customId = null;
            var productMappings = new List<ProductMapping>();
            var dbProductMappingIds = new List<int>();
            var records = obj.ProductMappings.ToList();
            productMappings = records;
            if (records.Any())
                customId = records.FirstOrDefault().CustomerId;


            if (customId == null || customId.Value == 0)
            {
                productMappings = new List<ProductMapping>();
                var customers = AsQueryable<AccountDetail>().
                    Where(p => p.AccountDetailFormId == (byte)AccountDetailFormType.Customers).
                    Select(p => new
                    {
                        p.AccountId,
                        p.Code,
                        p.AccountTitle
                    }).ToList();
                foreach (var customer in customers)
                {
                    // records.ForEach(p => { p.CustomerId = customer.AccountId; p.CustomerTitle = customer.AccountTitle; });
                    var newProductMappings = records.Select(p => new ProductMapping()
                    {
                        Id = p.Id,

                        CustomerId = customer.AccountId,
                        CustomerTitle = customer.Code + "-" + customer.AccountTitle,
                        COAProductId = p.COAProductId,
                        ProductCode = p.ProductCode,
                        ProductTitle = p.ProductTitle,
                        ProductId = p.ProductId,
                        ManualCode = p.ManualCode
                    });
                    productMappings.AddRange(newProductMappings);
                }
                dbProductMappingIds = FiscalCollection.Select(p => p.Id).ToList();
            }
            else
            {
                dbProductMappingIds = FiscalCollection.Where(p => p.CustomerId == customId).Select(p => p.Id).ToList();
            }
            var fiscalId = SiteContext.Current.Fiscal.Id;
            productMappings.ForEach(p => { p.FiscalId = fiscalId; });
            //////////////////////////////////////
            //foreach (var item in productMappings)
            //{
            //    var dbProdMappings = new ProductMappingRepository().AsQueryable().FirstOrDefault(p => p.CustomerId == item.CustomerId && p.ProductId == item.ProductId);
            //    if (dbProdMappings != null)
            //    {
            //        dbProdMappings.ManualCode = item.ManualCode;
            //        if (item.ManualCode == null)
            //            Db.ProductMappings.Remove(dbProdMappings);
            //    }
            //    else if (item.ManualCode != null)
            //        Db.ProductMappings.Add(item);
            //}
            //////////////////////////////////////
            var Ids = productMappings.Select(p => p.Id).ToList();
            var deletedIds = dbProductMappingIds.Where(p => !Ids.Contains(p)).Select(p => p).ToList();
            deletedIds.AddRange(productMappings.Where(p => p.ManualCode == "" || p.ManualCode == null && p.Id > 0).Select(p => p.Id).ToList());
            new ProductMappingRepository(this).Delete(deletedIds);
            var validProductMappings = productMappings.Where(p => p.ManualCode != "" && p.ManualCode != null).ToList();
            new ProductMappingRepository(this).Save(validProductMappings);
        }
    }
}