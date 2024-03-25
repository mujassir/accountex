using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Repositories.Config
{
    public class ProductGroupRepository : GenericRepository<ProductGroup>
    {
        public List<IdName> GetIdNameByDivisionId(int divisionId)
        {
            return Collection.Where(p => p.DivisionId == divisionId && p.DivisionId > 0).Select(p => new IdName
            {
                Id = p.Id,
                Name = p.Name,

            }).ToList();
        }
    }
}
