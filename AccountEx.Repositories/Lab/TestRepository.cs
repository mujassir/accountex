using AccountEx.CodeFirst.Models.Lab;
using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccountEx.Repositories.Lab
{
    public class TestRepository : GenericRepository<Test>
    {

        public TestRepository() : base() { }
        public TestRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public List<Test> GetInvestigationByGroupId(int groupId)
        {
            try
            {
                List<Test> list = new List<Test>();
                list = Collection.Where(p => p.MainCategoryId == groupId).ToList();
                return list;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public new void Save(Test invest)
        {
            if (invest.Id == 0)
            {
                Add(invest);
            }
            else
            {
                Update(invest);

            }
            //AddTransaction(sale, sale.CashSale);

        }
        public Test GetByName(string name)
        {
            name = name.ToLower();
            return Collection.FirstOrDefault(p => p.Name.ToLower() == name);
        }
        public List<Test> GetAll(TestType type)
        {
            var testType = Convert.ToByte(type);
            return Collection.ToList();
        }
        public override void Update(Test p)
        {
            var query = "Delete from TestParameters where TestId=" + p.Id + " and CompanyId=" + SiteContext.Current.User.CompanyId;
            Db.Database.ExecuteSqlCommand(query);
            foreach (var item in p.TestParameters)
            {
                Db.TestParameters.Add(item);
            }
            p.TestParameters = null;
            base.Update(p);
            SaveChanges();
        }
        public void DeleteById(int id)
        {
            string query = "Update TestIs Set IsDeleted='" + true + "' where Id='" + id + "'";
            Db.Database.ExecuteSqlCommand(query);
            Db.SaveChanges();
        }
    }
}
