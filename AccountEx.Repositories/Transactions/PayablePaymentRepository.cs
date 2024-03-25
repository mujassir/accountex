using AccountEx.CodeFirst.Models.Transactions;
using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Repositories.Transactions
{
    public class PayablePaymentRepository : GenericRepository<PayablePayment>
    {
        public int GetNextVoucherNumber(VoucherType vouchertype)
        {
            var maxnumber = ConfigurationReader.GetConfigKeyValue<int>("VoucherStartNumber", 1);
            if (!FiscalCollection.Any(p => p.TransactionType == vouchertype))
                return maxnumber;
            return FiscalCollection.Where(p => p.TransactionType == vouchertype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
        }
        public PayablePayment GetByVoucherNumber(int voucherno, VoucherType vtype, string key, out bool next, out bool previous)
        {
            PayablePayment v = null;
            switch (key)
            {
                case "first":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "last":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "next":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype && p.VoucherNumber > voucherno).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "previous":
                    v = FiscalCollection.Where(p => p.TransactionType == vtype && p.VoucherNumber < voucherno).OrderByDescending(p => p.VoucherNumber).FirstOrDefault(); ;
                    break;
                case "same":
                    v = FiscalCollection.FirstOrDefault(p => p.TransactionType == vtype && p.VoucherNumber == voucherno);
                    break;

            }

            if (v != null)
                voucherno = v.VoucherNumber;
            else if (key != "nextvouchernumber")
            {
                v = FiscalCollection.Where(p => p.TransactionType == vtype).OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
            }
            if (v == null && !FiscalCollection.Any(p => p.TransactionType == vtype))
            {
                v = new PayablePayment
                {
                    VoucherNumber = 1001,
                    InvoiceNumber = 1,
                    Date = DateTime.Now,
                    CreatedDate = DateTime.Now
                };
            }
            next = FiscalCollection.Any(p => p.TransactionType == vtype && p.VoucherNumber > voucherno);
            previous = FiscalCollection.Any(p => p.TransactionType == vtype && p.VoucherNumber < voucherno);
            return v;
        }
        public decimal GetTotalPayments(int voucherId, VoucherType type)
        {
            if (FiscalCollection.Any(p => p.TransactionType == type && p.VoucherId == voucherId))
                return FiscalCollection.Where(p => p.TransactionType == type && p.VoucherId == voucherId).Sum(p => p.Amount);
            else return 0.0M;
        }
        public bool IsPaymentExist(int voucherId)
        {
            return FiscalCollection.Any(p => p.VoucherId == voucherId);

        }
        public bool IsPaymentExist(int voucherno, VoucherType transactionType)
        {
            var voucherId = new VoucherTransRepository().GetVocuherId(voucherno, transactionType);
            return IsPaymentExist(voucherId);

        }

    }
}
