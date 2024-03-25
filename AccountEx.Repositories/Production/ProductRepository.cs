using System.Collections.Generic;
using System.Linq;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;

namespace AccountEx.Repositories
{
    public class ProductRepository : GenericRepository<Product>
    {
        public void SyncIds(List<int> ids)
        {
            foreach (var account in Db.Accounts.Where(p => ids.Contains(p.Id)).ToList())
            {
                var item = Collection.FirstOrDefault(p => p.AccountId == account.Id);
                if (item != null)
                    account.ReferenceId = item.Id;
            }
            Db.SaveChanges();
        }
        public override List<IdName> GetNames()
        {
            return Collection.Select(p => new { p.Id, p.Code, p.Name }).ToList().Select(p => new IdName { Id = p.Id, Name = p.Code + "-" + p.Name }).ToList();
        }
        public void MergeProducts(int fromProdutId,int toProductId)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                
                string query = string.Format("EXEC [DBO].[CRM_MergeProducts] @FromProductId={0},@ToProductId={1},@COMPANYID={2}",fromProdutId,toProductId, SiteContext.Current.User.CompanyId);
                ExecuteQuery(query);
                SaveChanges();
                scope.Complete();

            }
        }


        
    }
}
