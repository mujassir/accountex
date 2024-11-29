using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AccountEx.BussinessLogic
{
    public static class OrderBookingManager
    {
        public static void Save(Order input)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var orderbookingRepo = new OrderBookingRepository();
                var stockRequisitionRepo = new StockRequisitionRepository(orderbookingRepo);
                if (input.Id == 0)
                {
                    if (input.OrderType == (byte)OrderType.Production)
                        input.Status = (byte)TransactionStatus.PendingProduction;
                    orderbookingRepo.Add(input, true, false);
                }
                else
                {
                    orderbookingRepo.Update(input);
                }
                if (input.SRN != 0)
                    stockRequisitionRepo.Update(input.SRN, input.AuthLocationId);
                orderbookingRepo.SaveChanges();
                scope.Complete();
            }
        }

        public static void Delete(int id)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var orderbookingRepo = new OrderBookingRepository();
                orderbookingRepo.Delete(id);
                orderbookingRepo.SaveChanges();
                scope.Complete();
            }
        }
    }
}
