
using AccountEx.CodeFirst.Models.Lab;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AccountEx.DbMapping.Lab;


namespace AccountEx.Repositories.Lab
{
    public class TestCategoryRepository : GenericRepository<TestCategory>
    {
        public List<InvestigationGroupExtra> GetGroup()
        {
            var query = AsQueryable<Test>()      // source
                .Join(AsQueryable<TestCategory>(),         // target
                  i => i.MainCategoryId,// FK
                 ig => ig.Id,   // PK
            (i, ig) => new
            {
                Investigation = i,
                InvestigationGroup = ig
            }).GroupBy(p => p.Investigation.MainCategoryId) // project result
                             .ToList().Select(p => new DbMapping.Lab.InvestigationGroupExtra()
                             {
                                 GroupId = p.Key,
                                 GroupName = p.FirstOrDefault().InvestigationGroup.Name,
                                 Investigations = p.Select(q => q.Investigation).ToList()
                             }).ToList();
            return query;
        }

       
    }
}
