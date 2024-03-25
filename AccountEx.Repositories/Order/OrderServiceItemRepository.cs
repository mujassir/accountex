using AccountEx.CodeFirst.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Core.Objects;
using AccountEx.Common;
using AccountEx.CodeFirst.Context;

namespace AccountEx.Repositories
{
    public class OrderServiceItemRepository : GenericRepository<OrderExpenseItem>
    {
        public OrderServiceItemRepository() : base() { }
        public OrderServiceItemRepository( BaseRepository repo)
        {
            base.Db = repo.GetContext();
        }
    }
}
