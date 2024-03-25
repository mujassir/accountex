using AccountEx.CodeFirst.Models.Lab;
using AccountEx.CodeFirst.Models.Nexus;
using AccountEx.Common.Lab;
using AccountEx.DbMapping.Lab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccountEx.Repositories.Lab
{
    public class DepartmentRateListRepository : GenericRepository<DepartmentRateList>
    {
        public DepartmentRateListRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public void AddUpdateRateList(int testId, List<TestDepartmentExtra> testDepartment)
        {

            var departmentIds = testDepartment.Select(p => p.DepartmentAccountId).ToList();
            var dbRateList = Collection.Where(p => departmentIds.Contains(p.DepartmentAccountId)).GroupBy(p => p.DepartmentAccountId).Select(p => new
            {
                DepartmentId = p.Key,
                DepartmentRateListId = p.OrderByDescending(q => q.WithEffectFrom).FirstOrDefault().Id,
                RateListItem = p.OrderByDescending(q => q.WithEffectFrom).FirstOrDefault().DepartmentRateListItems.FirstOrDefault(t => t.TestId == testId)
            }).ToList();
            var newDepartmentRateListItem = new List<DepartmentRateListItem>();
            foreach (var item in testDepartment)
            {


                if (dbRateList.Any(p => p.DepartmentId == item.DepartmentAccountId))
                {
                    var rateList = dbRateList.FirstOrDefault(p => p.DepartmentId == item.DepartmentAccountId);
                    if (rateList.RateListItem != null && rateList.RateListItem.ApprovedRate != item.Price)
                    {
                        newDepartmentRateListItem.Add(new DepartmentRateListItem()
                        {
                            Id = rateList.RateListItem.Id,
                            DepartmentRateListId = rateList.DepartmentRateListId,
                            TestId = testId,
                            DepartmentId = item.DepartmentAccountId,
                            ApprovedRate = item.Price
                        });
                    }
                    else
                    {
                        newDepartmentRateListItem.Add(new DepartmentRateListItem()
                        {

                            DepartmentRateListId = rateList.DepartmentRateListId,
                            TestId = testId,
                            DepartmentId = item.DepartmentAccountId,
                            ApprovedRate = item.Price
                        });
                    }
                }
                else
                {
                    var newDRL = new DepartmentRateList()
                    {
                        DepartmentAccountId = item.DepartmentAccountId,
                        Name = "Rate List(Auto Created) - " + item.DepartmentName + " " + DateTime.Now.Month.ToString("MMM") + " " + DateTime.Now.Month.ToString("yyyy") + " onwards",
                        WithEffectFrom = DateTime.Now
                    };
                    Save(newDRL);
                    newDepartmentRateListItem.Add(new DepartmentRateListItem()
                    {

                        DepartmentRateListId = newDRL.Id,
                        TestId = testId,
                        DepartmentId = item.DepartmentAccountId,
                        ApprovedRate = item.Price
                    });
                }
            }
            new DepartmentRateListItemRepository(this).Save(true, newDepartmentRateListItem);

        }
        public List<LatestPriceExtra> GetLatestPrices(int testId, List<int> departmentIds)
        {
            return Collection.Where(p => departmentIds.Contains(p.DepartmentAccountId)).GroupBy(p => p.DepartmentAccountId).Select(p => new LatestPriceExtra()
            {
                DepartmentAccountId = p.Key,
                Price = p.OrderByDescending(q => q.WithEffectFrom).FirstOrDefault().DepartmentRateListItems.Any(t => t.TestId == testId)
                ? p.OrderByDescending(q => q.WithEffectFrom).FirstOrDefault().DepartmentRateListItems.FirstOrDefault(t => t.TestId == testId).ApprovedRate
                : 0.0M
            }).ToList();
        }
    }
}

