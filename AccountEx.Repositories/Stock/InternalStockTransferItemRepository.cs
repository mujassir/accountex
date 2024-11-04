using AccountEx.CodeFirst.Models.Stock;

namespace AccountEx.Repositories
{
    public class InternalStockTransferItemRepository : GenericRepository<InternalStockTransferItem>
    {
       public InternalStockTransferItemRepository() : base() { }
       public InternalStockTransferItemRepository(BaseRepository repo) 
       {
           base.Db = repo.GetContext();
       }
    }
}
