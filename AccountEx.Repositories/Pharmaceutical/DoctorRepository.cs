using AccountEx.CodeFirst.Models.Pharmaceutical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Repositories.Pharmaceutical
{
    public class DoctorRepository:GenericRepository<Doctor>
    {
        public bool IsExist(string name, int id)
        {
            return Collection.Any(p => p.Name.ToLower() == name.ToLower() && p.Id != id);
        }
    }
}
