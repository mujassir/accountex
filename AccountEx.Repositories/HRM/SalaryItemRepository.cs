using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.CodeFirst;
using AccountEx.Common;
using AccountEx.CodeFirst.Models;

namespace AccountEx.Repositories
{
    public class SalaryItemRepository : GenericRepository<SalaryItem>
    {
        public SalaryItemRepository() : base() { }
        public SalaryItemRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public List<SalaryItem> GetByIds(List<int> salaryItemids)
        {
            return Collection.Where(p => salaryItemids.Contains(p.Id)).ToList();
        }

        public List<SalaryItem> GetByESalaryId(int eSalaryId)
        {
            return Collection.Where(p => p.ESalaryId == eSalaryId).ToList();
        }

    }
}
