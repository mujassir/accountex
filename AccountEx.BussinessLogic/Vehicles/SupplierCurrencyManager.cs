using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.CodeFirst;
using Entities.CodeFirst;
using AccountEx.CodeFirst.Models.Vehicles;
using AccountEx.Common.VehicleSystem;

namespace AccountEx.BussinessLogic
{

    public static class SupplierCurrencyManager
    {


        public static void Save(SupplierCurrency input)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new SupplierCurrencyRepository();
                repo.Save(input);
                repo.SaveChanges();
                scope.Complete();
            }

        }
        public static void Delete(int id)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new SupplierCurrencyRepository();
                repo.Delete(id);
                repo.SaveChanges();
                scope.Complete();
            }
        }



    }
}
