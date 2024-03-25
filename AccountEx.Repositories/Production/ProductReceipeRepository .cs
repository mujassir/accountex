using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using System.Linq;

namespace AccountEx.Repositories
{
    public class ProductReceipeRepository : GenericRepository<ProductReceipe>
    {
        public ProductReceipeRepository() : base() { }
        public ProductReceipeRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }


        public override void Update(ProductReceipe receipe)
        {
            var dbRecord = GetById(receipe.Id, true);
            var itemRepo = new ProductReceipeitemRepository(this);
            var ids = receipe.ProductReceipeitems.Select(q => q.Id).ToList();
            var deletedIds = dbRecord.ProductReceipeitems.Where(q => !ids.Contains(q.Id)).Select(q => q.Id).ToList();
            itemRepo.Delete(deletedIds);
            itemRepo.Save(receipe.ProductReceipeitems.ToList());
            base.Update(receipe, true, false);
        }
        public bool IsReceipeExits(int productId, int id)
        {
            return Collection.Any(p => p.ProductId == productId && p.Id != id);
        }
        public ProductReceipe GetByProductId(int productId)
        {
            return Collection.FirstOrDefault(p => p.ProductId == productId);
        }
    }
}