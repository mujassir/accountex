using AccountEx.CodeFirst.Models;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using System.Linq.Dynamic;
using System.Linq;

namespace AccountEx.Repositories
{
    public class UserCompanyRepository : GenericRepository<UserCompany>
    {
        public UserCompanyRepository() : base() { }
        public UserCompanyRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
        public virtual List<UserCompany> GetAll(Expression<Func<UserCompany, bool>> predicate)
        {
            return Db.UserCompanies.Where(predicate).ToList();
        }
    }
}
