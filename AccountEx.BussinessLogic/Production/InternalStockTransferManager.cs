using AccountEx.CodeFirst.Models.Stock;
using AccountEx.Common;
using AccountEx.Repositories;

namespace AccountEx.BussinessLogic
{
    public static class InternalStockTransferManager
    {
       public static void Save(InternalStockTransfer input, bool received = false) 
       {
           using (var scope = TransactionScopeBuilder.Create())
           {
               var repo = new InternalStockTransferRepository();
               if (input.Id == 0)
               {
                    input.Status = (byte)TransactionStatus.Pending;

                    foreach (var item in input.InternalStockTransferItems)
                    {
                        item.Status = input.Status;
                        item.QuantityDelivered = 0;
                        item.Type = (int)StockType.Active;
                    }
                    repo.Add(input,true,false);
               }
               else
               {
                   repo.Update(input, received);
               }
               repo.SaveChanges();
               scope.Complete();
           }
       
       }

    }
}
