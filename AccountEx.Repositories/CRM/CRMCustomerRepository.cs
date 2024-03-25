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
    public class CRMCustomerRepository : GenericRepository<CRMCustomer>
    {
        public CRMCustomerRepository() : base() { }
        public CRMCustomerRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }

        public List<CRMCustomerListing> GetListing(int startRow, int pageSize, string sortColumn, string sortDirection, string textToSearch)
        {
            var query = string.Format("EXEC [DBO].[CRM_CustomerListing] @COMPANYID = {0},@UserId={1}, @StartRow = {2}, @PageSize={3},@SortCol='{4}',@SortDir='{5}',@TextToSearch='{6}'",
                 SiteContext.Current.User.CompanyId, SiteContext.Current.User.Id, startRow, pageSize, sortColumn, sortDirection, textToSearch);
            return Db.Database.SqlQuery<CRMCustomerListing>(query).ToList();
        }

        public List<IdName> GetByUserTypeId(CRMUserType userTypeId, CRMCustomerType type = CRMCustomerType.All)
        {
            if (userTypeId == CRMUserType.SalesExecutive)
            {
                return new CRMCustomerRepository().GetCustomerSalesPersonId(SiteContext.Current.User.Id);
            }
            else if (userTypeId == CRMUserType.RSM)
            {
                return new CRMCustomerRepository().GetCustomerByRegionId(SiteContext.Current.RegionId,type);
            }
            else
            {
                return GetNames(type);
            }
        }

        public List<IdName> GetNames(CRMCustomerType type = CRMCustomerType.All)
        {
            return GetNames(SiteContext.Current.UserTypeId,type);
        }
        public List<IdName> GetNames(CRMUserType userTypeId,CRMCustomerType type = CRMCustomerType.All)
        {

            var query = Collection.AsQueryable();
            if (type == CRMCustomerType.Customer)
            {
                query = query.Where(p => !p.IsTrader);
            }
            else if (type == CRMCustomerType.Trader)
            {
                query = query.Where(p => p.IsTrader);
            }


            return query.OrderBy(p => p.Name).Select(p => new IdName
            {
                Id = p.Id,
                Name = p.Name,
            }).ToList();
        }

        public List<IdName> GetSalesPersonByCustomerId(int customerId)
        {
            var Ids = AsQueryable<CRMCustomerSalePerson>().Where(p => p.CRMCustomerId == customerId).Select(p => p.UserId).ToList();
            return new UserRepository().GetIdNames(Ids);
        }

        public int GetSalesPersonId(int customerId, int productId)
        {
            var categoryId = new CRMProductRepository(this).GetCategoryId(productId);
            if (AsQueryable<CRMCustomerSalePerson>().Any(p => p.CRMCustomerId == customerId && p.CategroyId == categoryId))
                return AsQueryable<CRMCustomerSalePerson>().FirstOrDefault(p => p.CRMCustomerId == customerId && p.CategroyId == categoryId).UserId;
            else return 0;

        }
        public List<IdName> GetCustomerSalesPersonId(int salePersonId, CRMCustomerType type = CRMCustomerType.All)
        {
            var Ids = AsQueryable<CRMCustomerSalePerson>().Where(p => p.UserId == salePersonId).Select(p => p.CRMCustomerId).ToList();
            return GetByIds(Ids,type);
        }
        public dynamic GetCategoriesWithCustomer(int salePersonId)
        {
            return AsQueryable<CRMCustomerSalePerson>().Where(p => p.UserId == salePersonId).Select(p =>
            new
            {
                p.CRMCustomerId,
                p.CategroyId
            }
            ).ToList();

        }
        public dynamic GetCategories(int customerId)
        {
            return AsQueryable<CRMCustomerSalePerson>().Where(p => p.CRMCustomerId == customerId).Select(p =>
            new
            {
                p.CRMCustomerId,
                p.CategroyId
            }
            ).ToList();

        }
        public List<IdName> GetCustomerIdName(int salePersonId)
        {
            var customerIds = AsQueryable<CRMCustomerSalePerson>().Where(p => p.UserId == salePersonId).Select(p => p.CRMCustomerId).Distinct().ToList();
            return Collection.Where(p => customerIds.Contains(p.Id)).OrderBy(p => p.Name).Select(p => new IdName
            {
                Id = p.Id,
                Name = p.Name,

            }).ToList();
        }
        public List<IdName> GetCustomerByRegionId(int regionId, CRMCustomerType type = CRMCustomerType.All)
        {
            var query = AsQueryable<vw_CRMCustomers>();
            if (type == CRMCustomerType.Customer)
            {
                query = query.Where(p => !p.IsTrader);
            }
            else if (type == CRMCustomerType.Trader)
            {
                query = query.Where(p => p.IsTrader);
            }

            return query.Where(p => p.RegionId == regionId).OrderBy(p => p.Name).Select(p => new IdName
            {
                Id = p.Id,
                Name = p.Name,

            }).ToList();
        }
        public int GetIdByName(string name)
        {

            if (Collection.Any(p => p.Name.ToLower() == name.ToLower()))
                return Collection.FirstOrDefault(p => p.Name.ToLower() == name.ToLower()).Id;
            else return 0;
        }
        public List<IdName> GetByIds(List<int> Ids, CRMCustomerType type = CRMCustomerType.All)
        {
            var query = Collection.AsQueryable();
            if (type == CRMCustomerType.Customer)
            {
                query = query.Where(p => !p.IsTrader);
            }
            else if (type == CRMCustomerType.Trader)
            {
                query = query.Where(p => p.IsTrader);
            }

            return query.Where(p => Ids.Contains(p.Id)).OrderBy(p => p.Name).Select(p => new IdName
            {
                Id = p.Id,
                Name = p.Name,

            }).ToList();
        }
        public override void Save(CRMCustomer customer)
        {
            if (customer.Id == 0)
            {
                base.Add(customer, true, false);
            }
            else
            {
                Update(customer);
            }
            SaveChanges();
        }

        public override void Update(CRMCustomer customer)
        {
            var spRepo = new CRMCustomerSalePersonRepository(this);
            var dbPMC = GetById(customer.Id, true);

            //add,update & remove services items
            var Ids = customer.CRMCustomerSalePersons.Select(p => p.Id).ToList();
            var deletedIds = dbPMC.CRMCustomerSalePersons.Where(p => !Ids.Contains(p.Id)).Select(p => p.Id).ToList();
            spRepo.Delete(deletedIds);
            spRepo.Save(customer.CRMCustomerSalePersons.ToList());
            //SaveChanges();
            customer.CRMCustomerSalePersons = null;
            base.Update(customer, true, false);
            SaveChanges();
        }




    }
}