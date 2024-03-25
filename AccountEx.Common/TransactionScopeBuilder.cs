using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace AccountEx.Common
{


    public static class TransactionScopeBuilder
    {
        /// <summary>
        /// Creates a transactionscope with ReadUncommitted Isolation, the same level as sql server
        /// </summary>
        /// <returns>A transaction scope</returns>
        /// 
        public static TransactionScope Create()
        {
            return Create(IsolationLevel.ReadUncommitted, TransactionManager.DefaultTimeout);
        }
        public static TransactionScope Create(TimeSpan timeout)
        {
            return Create(IsolationLevel.ReadUncommitted, timeout);
        }
        public static TransactionScope Create(IsolationLevel transactionType, TimeSpan timeout)
        {
            var options = new TransactionOptions
            {
                IsolationLevel = transactionType,
                Timeout = timeout
            };

            return new TransactionScope(TransactionScopeOption.Required, options);
        }
    }
}
