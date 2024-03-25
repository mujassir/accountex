using System.Linq;
using AccountEx.CodeFirst.Models;
using System.Collections.Generic;
using AccountEx.Common;

namespace AccountEx.Repositories
{
    public class OrderItemRepository : GenericRepository<OrderItem>
    {
        public OrderItemRepository() : base() { }
        public OrderItemRepository(BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
    }
}