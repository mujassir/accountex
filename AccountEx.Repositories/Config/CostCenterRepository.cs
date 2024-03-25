
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Repositories
{
   public  class CostCenterRepository:GenericRepository<CostCenter>
    {
       public bool IsExist(string name, int id)
       {
           return Collection.Any(p => p.Name.ToLower() == name.ToLower() && p.Id != id);
       }
      
    }
}
