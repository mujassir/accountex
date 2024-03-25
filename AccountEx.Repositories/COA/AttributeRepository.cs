using System.Collections.Generic;
using System.Linq;
using AccountEx.CodeFirst.Models;

namespace AccountEx.Repositories
{
    public class AttributeRepository : GenericRepository<Attribute>
    {
        public List<Attribute> GetByAccountTypeId(int accountTypeId)
        {
            return Collection.Where(p => p.AccountTypeId == accountTypeId).ToList();
        }


    }
}
