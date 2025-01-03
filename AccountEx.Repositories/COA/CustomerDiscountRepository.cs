using System.Collections.Generic;
using AccountEx.Common;
using System.Linq;
using AccountEx.CodeFirst.Models;
using System;
using AccountEx.DbMapping;

namespace AccountEx.Repositories
{
    public class CustomerDiscountRepository : GenericRepository<CustomerDiscount>
    {
        public CustomerDiscountRepository() : base() { }
        public CustomerDiscountRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public List<DiscountEx> GetDiscountByProduct()
        {
            return Collection.Where(p => p.Discount > 0).Select(p => new DiscountEx()
            {
                CustomerId = p.CustomerId.Value,
                COAProductId = p.COAProductId.Value,
                Discount = p.Discount
            }).ToList();
        }
        public List<CustomerDiscount> GetDiscount(int customer)
        {
            var query = string.Format("EXEC [DBO].[GetDiscountByCustomer] @CompanyId = {0}, @CustomerId = {1}",
                SiteContext.Current.User.CompanyId, customer);
            return Db.Database.SqlQuery<CustomerDiscount>(query).ToList();
        }
        public List<ProductExtra> GetDiscountWithProducts(int customerId)
        {
            var query = string.Format("EXEC [DBO].[GetDiscountWithProducts] @CompanyId = {0}, @CustomerId = {1}, @AccountDetailFormId={2}",
                SiteContext.Current.User.CompanyId, customerId, (byte)AccountDetailFormType.Products);
            return Db.Database.SqlQuery<ProductExtra>(query).ToList();
        }
        public List<ProductExtra> GetDiscountWithProducts()
        {
            var query = string.Format("EXEC [DBO].[GetDiscountWithProductsByFormId] @CompanyId = {0}, @AccountDetailFormId={1}",
               SiteContext.Current.User.CompanyId, (byte)AccountDetailFormType.Products);
            return Db.Database.SqlQuery<ProductExtra>(query).ToList();

        }
        public DateTime GetLastActivityDate()
        {
            try
            {
                var query = string.Format("EXEC [DBO].[SP_GetLastUpdatedDate] @TableName = {0}", "CustomerDiscounts");
                var date = Db.Database.SqlQuery<DateTime>(query).FirstOrDefault();


                return date;

            }
            catch (Exception)
            {

                return DateTime.Now;
            }

        }
        public void Save(CustomerDiscountExtra obj)
        {

            int? customId = null;
            var discounts = new List<CustomerDiscount>();
            var dbdiscountIds = new List<int>();
            var records = obj.Discounts.ToList();
            discounts = records;
            if (records.Any())
                customId = records.FirstOrDefault().CustomerId;


            if (customId == null || customId.Value == 0)
            {
                discounts = new List<CustomerDiscount>();
                var customers = AsQueryable<AccountDetail>().
                    Where(p => p.AccountDetailFormId == (byte)AccountDetailFormType.Customers
                    || p.AccountDetailFormId == (byte)AccountDetailFormType.Suppliers
                    ).
                    Select(p => new
                    {
                        p.AccountId,
                        p.Code,
                        p.AccountTitle
                    }).ToList();
                foreach (var customer in customers)
                {
                    // records.ForEach(p => { p.CustomerId = customer.AccountId; p.CustomerTitle = customer.AccountTitle; });
                    var newDiscounts = records.Select(p => new CustomerDiscount()
                    {
                        Id = p.Id,

                        CustomerId = customer.AccountId,
                        CustomerTitle = customer.Code + "-" + customer.AccountTitle,
                        COAProductId = p.COAProductId,
                        Enable = p.Enable,
                        ProductCode = p.ProductCode,
                        ProductTitle = p.ProductTitle,
                        ProductId = p.ProductId,
                        Discount = p.Discount
                    });
                    discounts.AddRange(newDiscounts);
                }
                dbdiscountIds = FiscalCollection.Select(p => p.Id).ToList();
            }
            else
            {
                dbdiscountIds = FiscalCollection.Where(p => p.CustomerId == customId).Select(p => p.Id).ToList();
            }
            var fiscalId = SiteContext.Current.Fiscal.Id;
            discounts.ForEach(p => { p.FiscalId = fiscalId; });
            //foreach (var item in discounts)
            //{
            //    var dbDiscount = dbdiscounts.FirstOrDefault(p => p.CustomerId == item.CustomerId && p.ProductId == item.ProductId);
            //    if (dbDiscount != null)
            //    {
            //        dbDiscount.Discount = item.Discount;
            //        if (item.Discount <= 0)
            //            Db.CustomerDiscounts.Remove(dbDiscount);
            //    }
            //    else if (item.Discount > 0)
            //        Db.CustomerDiscounts.Add(item);


            //}

            var Ids = discounts.Select(p => p.Id).ToList();
            var deletedIds = dbdiscountIds.Where(p => !Ids.Contains(p)).Select(p => p).ToList();
            deletedIds.AddRange(discounts.Where(p => p.Discount == 0 && p.Id > 0).Select(p => p.Id).ToList());
            new CustomerDiscountRepository(this).Delete(deletedIds);
            var validDiscounts = discounts.Where(p => p.Discount > 0 || p.Enable == true).ToList();
            new CustomerDiscountRepository(this).Save(validDiscounts);
        }
    }
}