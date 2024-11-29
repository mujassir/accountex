using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using Transaction = AccountEx.CodeFirst.Models.Transaction;

namespace AccountEx.BussinessLogic
{
    public static class StockRequisitionManager
    {
       public static void Save(StockRequisition input) 
       {
           using (var scope = TransactionScopeBuilder.Create())
           {
               var repo = new StockRequisitionRepository();
               if (input.Id == 0)
               {
                    input.Status = (byte)TransactionStatus.Pending;

                    foreach (var item in input.StockRequisitionItems)
                    {
                        item.Status = input.Status;
                        item.QuantityDelivered = 0;
                    }
                    repo.Add(input,true,false);
               }
               else
               {
                   repo.Update(input);
               }
               repo.SaveChanges();
               scope.Complete();
           }
       
       }

    }
}
