using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using Transaction = AccountEx.CodeFirst.Models.Transaction;

namespace AccountEx.BussinessLogic
{
    public static class ServiceManager
    {
        public static void Save(Order order)
        {
            using (var scope = TransactionScopeBuilder.Create())
            {
                var repo = new OrderBookingRepository();
                if (order.Id == 0)
                {
                    order.Status = (byte)TransactionStatus.Pending;
                    repo.Add(order);
                    repo.SaveLog(order, ActionType.Added);
                }
                else
                {

                    repo.Update(order);
                }
                scope.Complete();
            }

        }

    }
}
