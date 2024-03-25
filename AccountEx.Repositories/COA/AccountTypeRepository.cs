using System.Linq;
using AccountEx.CodeFirst.Models;

namespace AccountEx.Repositories
{
    public class AccountTypeRepository : GenericRepository<AccountType>
    {
        public AccountType GetByName(string name)
        {
            return Collection.FirstOrDefault(p => p.Name == name);
        }
    }
}
