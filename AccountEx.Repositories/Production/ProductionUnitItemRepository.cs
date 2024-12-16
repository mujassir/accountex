using AccountEx.CodeFirst.Models.Production;

namespace AccountEx.Repositories.Production
{
    public class ProductionUnitItemRepository : GenericRepository<ProductionUnitItem>
    {
        public ProductionUnitItemRepository() : base() { }
        public ProductionUnitItemRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
    }
}
