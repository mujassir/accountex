using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AccountEx.Repositories
{
    public class TransRepository : GenericRepository<Sale>
    {
        public TransRepository() :base() { }
        public TransRepository(BaseRepository repo) 
        {
            base.Db = repo.GetContext();
        }
   
        public override void Update(Sale s)
        {
            ////add,update & remove saleitems items
            var saleItemRepo = new SaleItemRepository(this);
            var saleServicesItemRepo = new SaleServicesItemRepository(this);
            var servExpRepo = new ServiceExpensesRepository(this);

            var dbSale = GetById(s.Id, true);
            var ids = s.SaleItems.Select(p => p.Id).ToList();
            var deletedIds = dbSale.SaleItems.Where(p => !ids.Contains(p.Id)).Select(p => p.Id).ToList();
            saleItemRepo.Delete(deletedIds);
            saleItemRepo.Save(s.SaleItems.ToList());

            //add,update & remove services items
            ids = s.SaleServicesItems.Select(p => p.Id).ToList();
            deletedIds = dbSale.SaleServicesItems.Where(p => !ids.Contains(p.Id)).Select(p => p.Id).ToList();
            saleServicesItemRepo.Delete(deletedIds);
            saleServicesItemRepo.Save(s.SaleServicesItems.ToList());

            //add,update & remove expenses items
            ids = s.ServiceExpenses.Select(p => p.Id).ToList();
            deletedIds = dbSale.ServiceExpenses.Where(p => !ids.Contains(p.Id)).Select(p => p.Id).ToList();
            servExpRepo.Delete(deletedIds);
            servExpRepo.Save(s.ServiceExpenses.ToList());
            base.Update(s, true, false);
        }
    }
}