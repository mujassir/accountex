using AccountEx.CodeFirst.Models.Lab;
using AccountEx.Common.Lab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccountEx.Repositories.Lab
{
    public class TestParameterRepository : GenericRepository<TestParameter>
    {
        public List<TestParameter> GetByTestId(int testId)
        {
            return Collection.Where(p => p.TestId == testId).ToList();
        }
        public void HardDelete(int testId)
        {
            foreach (var item in Collection.Where(p => p.TestId == testId).ToList())
            {
                Db.TestParameters.Remove(item);
            }
            Db.SaveChanges();
        }
        public List<TestResultExtra> GetParametersByTestId(int testid)
        {
            //var query = Db.Tests      // source
            //    .Join(Db.TestGroups,         // target
            //      t => t.TestGroupId,// FK
            //     tg => tg.Id,   // PK
            //(t, tg) => new { t, tg })

            var query = Db.Parameters  // source
                .Join(Collection,  // target
                p => p.Id,
                tp => tp.ParameterId,
                (p, tp) => new { p, tp })
                .Where(q => q.tp.TestId == testid)
                .Select(p => new TestResultExtra()
                {
                    Id = p.tp.TestId,
                    ParameterId = p.p.Id,
                    Name = p.p.Name,
                    Unit = p.p.Unit,
                    Value = ""
                }).ToList();
            return query;
        }
    }
}
