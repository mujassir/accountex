using AccountEx.CodeFirst.Models.Lab;
using AccountEx.CodeFirst.Models.Nexus;
using AccountEx.Common.Lab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccountEx.Repositories.Lab
{
    public class DepartmentTestRepository : GenericRepository<DepartmentTest>
    {
        public DepartmentTestRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public List<DepartmentTest> GetByTestId(int testId)
        {
            return Collection.Where(p => p.TestId == testId).ToList();
        }
    }

}
