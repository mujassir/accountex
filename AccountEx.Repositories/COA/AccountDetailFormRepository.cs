using AccountEx.Common;
using System.Linq;
using AccountEx.CodeFirst.Models;

namespace AccountEx.Repositories
{
    public class AccountDetailFormRepository : GenericRepository<AccountDetailForm>
    {
        public void UpdateId(AccountDetailFormType type, int headId)
        {
            var form = Collection.FirstOrDefault (p => p.Id == (int)type);
            if (form == null) return;
            form.HeadAccountId = headId;
            SaveChanges();
        }
    }
}