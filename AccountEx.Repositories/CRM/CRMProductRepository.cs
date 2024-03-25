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
    public class CRMProductRepository : GenericRepository<CRMProduct>
    {

        public CRMProductRepository() : base() { }
        public CRMProductRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public override List<IdName> GetNames()
        {
            return GetNames(SiteContext.Current.UserTypeId);
        }
        public new List<vw_CRMPRoducts> GetAll()
        {
            return GetAll(SiteContext.Current.UserTypeId);
        }
        public int GetIdByName(string name)
        {

            if (Collection.Any(p => p.Name.ToLower() == name.ToLower()))
                return Collection.FirstOrDefault(p => p.Name.ToLower() == name.ToLower()).Id;
            else return 0;
        }
        public bool IsOwnProduct(int id)
        {
            return Collection.Any(p => p.Id == id && p.IsOwnProduct);


        }
        public bool IsOwnProduct(string name)
        {
            return Collection.Any(p => p.Name.ToLower() == name.ToLower() && p.IsOwnProduct);


        }
        public bool IsExistOwnProduct(string name)
        {
            return Collection.Any(p => p.Name.ToLower() == name.ToLower() && p.IsOwnProduct);


        }
        public vw_CRMPRoducts GetByName(string name)
        {


            return AsQueryable<vw_CRMPRoducts>().FirstOrDefault(p => p.Name.ToLower() == name.ToLower());

        }
        public vw_CRMPRoducts GetOwnProductByName(string name)
        {


            return AsQueryable<vw_CRMPRoducts>().FirstOrDefault(p => p.Name.ToLower() == name.ToLower() && p.IsOwnProduct);

        }
        public List<vw_CRMPRoducts> GetAll(CRMUserType userTypeId)
        {

            var query = AsQueryable<vw_CRMPRoducts>();
            if (userTypeId == CRMUserType.DivisionalHead)
            {
                query = query.Where(p => p.DivisionId == SiteContext.Current.DivisionId);
            }
            return query.OrderBy(p => p.Name).ThenBy(p => p.Division).ToList();
        }
        public List<IdName> GetNames(CRMUserType userTypeId)
        {

            var query = AsQueryable<vw_CRMPRoducts>();
            if (userTypeId == CRMUserType.DivisionalHead)
            {
                query = query.Where(p => p.DivisionId == SiteContext.Current.DivisionId);
            }
            return query.OrderBy(p => p.Name).ThenBy(p => p.Division).Select(p => new IdName
            {
                Id = p.Id,
                Name = p.Name,
            }).ToList();
        }
        public List<vw_CRMPRoducts> GetProducts(List<int> ids)
        {
            var userTypeId = SiteContext.Current.UserTypeId;
            var query = AsQueryable<vw_CRMPRoducts>();
            if (userTypeId == CRMUserType.DivisionalHead)
            {
                query = query.Where(p => p.DivisionId == SiteContext.Current.DivisionId);
            }
            return query.Where(p => ids.Contains(p.Id)).ToList();
        }
        public List<IdName> GetProductIdName(List<int> ids)
        {
            return Collection.Where(p => ids.Contains(p.Id)).Select(p => new IdName
            {
                Id = p.Id,
                Name = p.Name,
            }).ToList();
        }
        public List<int> GetProductIdsByDivision(List<int> ids, int DivisionId)
        {

            return AsQueryable<vw_CRMPRoducts>().Where(p => ids.Contains(p.Id) && p.DivisionId == DivisionId).Select(p => p.Id).ToList();
        }
        public int GetCategoryId(int id)
        {
            if (Collection.Any(p => p.Id == id))
                return Collection.FirstOrDefault(p => p.Id == id).CategoryId.Value;
            else return 0;
        }

        public List<IdName> GetOwnProductsIdName()
        {
            var userTypeId = SiteContext.Current.UserTypeId;
            var query = AsQueryable<vw_CRMPRoducts>();
            if (userTypeId == CRMUserType.DivisionalHead)
            {
                query = query.Where(p => p.DivisionId == SiteContext.Current.DivisionId);
            }

            return query.Where(p => p.IsOwnProduct).Select(p => new IdName
            {
                Id = p.Id,
                Name = p.Name,
            }).ToList();
        }
        public List<CRMProductWithOwnerShip> GetProductsIdName(CRMUserType userTypeId)
        {

            var query = AsQueryable<vw_CRMPRoducts>();
            if (userTypeId == CRMUserType.DivisionalHead)
            {
                query = query.Where(p => p.DivisionId == SiteContext.Current.DivisionId);
            }
            return query.Where(p => p.IsOwnProduct).Select(p => new CRMProductWithOwnerShip
            {
                Id = p.Id,
                Name = p.Name,
                IsOwnProduct = p.IsOwnProduct
            }).ToList();
        }

        public List<vw_CRMPRoducts> GetOwnProducts()
        {
            var userTypeId = SiteContext.Current.UserTypeId;
            var query = AsQueryable<vw_CRMPRoducts>();
            if (userTypeId == CRMUserType.DivisionalHead)
            {
                query = query.Where(p => p.DivisionId == SiteContext.Current.DivisionId);
            }
            return query.Where(p => p.IsOwnProduct).ToList();
        }


    }
}
