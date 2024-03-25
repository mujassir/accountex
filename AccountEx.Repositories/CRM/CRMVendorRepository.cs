using AccountEx.CodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Core.Objects;
using AccountEx.Common;
using AccountEx.CodeFirst.Models.CRM;

namespace AccountEx.Repositories
{
    public class CRMVendorRepository : GenericRepository<CRMVendor>
    {
        public CRMVendorRepository() : base() { }
        public CRMVendorRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public List<IdName> GetIdNameByType(CRMVendorType type)
        {
            return Collection.Where(p => p.Type == type).Select(p => new IdName { Id = p.Id, Name = p.Name }).ToList();
        }

        public string GetAddress(int id)
        {
            if (Collection.Any(p => p.Id == id))
                return Collection.FirstOrDefault(p => p.Id == id).Address;
            else return "";
        }
    }
}