using AccountEx.CodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Repositories
{
    public class FuelRepository : GenericRepository<Fuel>
    {

        public bool IsExist(string name, int id)
        {
            return Collection.Any(p => p.Name.ToLower() == name.ToLower() && p.Id != id);
        }
    }
}
