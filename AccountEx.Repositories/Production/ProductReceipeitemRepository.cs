using AccountEx.CodeFirst.Models;
using System.Linq;
namespace AccountEx.Repositories
{
    public class ProductReceipeitemRepository : GenericRepository<ProductReceipeitems>
    {
        public ProductReceipeitemRepository() : base() { }
        public ProductReceipeitemRepository(BaseRepository repo) 
        {
            base.Db = repo.GetContext();
        }

    }
}
