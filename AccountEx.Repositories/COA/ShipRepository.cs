using AccountEx.CodeFirst.Models.COA;
using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Repositories
{
    public class ShipRepository : GenericRepository<Ship>
    {
        public List<IdName> GetByShipperId(int shipperId)
        {
            return Collection.Where(p => p.ShipperId == shipperId).Select(p => new { p.Id, p.Name, p.VESSEL })
                .ToList().Select(p => new IdName()
                {
                    Id = p.Id,
                    Name = p.Name + " (" + p.VESSEL + ")"
                })
                .ToList();
        }
        public bool CheckIfVessleExists(string vessleNo, string name, int shipperId, int id)
        {
            return Collection.Any(p => p.VESSEL.ToLower() == vessleNo.ToLower() && p.Name.ToLower() == name.ToLower() && p.ShipperId == shipperId && p.Id != id);
        }

    }
}
