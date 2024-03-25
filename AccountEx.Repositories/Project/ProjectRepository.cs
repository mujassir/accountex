using System.Linq;
using AccountEx.CodeFirst.Models;

namespace AccountEx.Repositories
{
    public class ProjectRepository : GenericRepository<Project>
    {
        public int GetNextProjectNumber()
        {
            const int maxnumber = 1;
            if (!FiscalCollection.Any())
                return maxnumber;
            return FiscalCollection.OrderByDescending(p => p.Number).FirstOrDefault().Number + 1;
        }
       
    }
}
