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
    public static class DiscountManager
    {



        public static void Save(CustomerDiscountExtra discounts)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {

                var repo = new CustomerDiscountRepository();
                repo.Save(discounts);
                repo.SaveChanges();
                scope.Complete();
            }

        }

        
    }
}
