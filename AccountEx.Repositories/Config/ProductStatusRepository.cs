using AccountEx.CodeFirst.Models;
using AccountEx.CodeFirst.Models.CRM;
using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Repositories
{
    public class ProductStatusRepository : GenericRepository<ProductStatus>
    {

        public List<IdName> GetIdName()
        {

            return Collection.OrderBy(p => p.SN).Select(p => new IdName
            {
                Id = p.Id,
                Name = p.Name,
            }).ToList();
        }
    }
}
