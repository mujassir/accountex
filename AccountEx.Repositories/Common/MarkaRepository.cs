using AccountEx.Common;
using System.Collections.Generic;
using System.Linq;
using AccountEx.CodeFirst.Models;

namespace AccountEx.Repositories
{
    public class MarkaRepository : GenericRepository<Marka>
    {

        public List<IdName> GetIdNameMarka()
        {
            return Collection.Select(p => new IdName
            {
                Id = p.Id,
                Name = p.Name,

            }).ToList();
        }
       


    }

}