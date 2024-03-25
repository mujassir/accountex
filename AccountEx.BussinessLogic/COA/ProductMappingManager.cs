using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.DbMapping;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using Transaction = AccountEx.CodeFirst.Models.Transaction;

namespace AccountEx.BussinessLogic
{
    public static class ProductMappingManager
    {



        public static void Save(ProductMappingExtra productmappings)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new ProductMappingRepository();
                repo.Save(productmappings);
                repo.SaveChanges();
                scope.Complete();
            }

        }

        
    }
}
