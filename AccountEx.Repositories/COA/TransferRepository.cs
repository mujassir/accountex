using AccountEx.CodeFirst.Models.COA;
using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Repositories
{
    public class TransferRepository : GenericRepository<Transfer>
    {
        public int GetNextVoucherNumber()
        {
            var maxnumber = ConfigurationReader.GetConfigKeyValue<int>("VoucherStartNumber", 1001);
            if (!FiscalCollection.Any())
                return maxnumber;
            return FiscalCollection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
        }
        public Transfer GetByVoucherNumber(int voucherno, string key, out bool next, out bool previous)
        {
            Transfer v = null;
            switch (key)
            {
                case "first":
                    v = FiscalCollection.OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "last":
                    v = FiscalCollection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "next":
                    v = FiscalCollection.Where(p => p.VoucherNumber > voucherno).OrderBy(p => p.VoucherNumber).FirstOrDefault();
                    break;
                case "previous":
                    v = FiscalCollection.Where(p => p.VoucherNumber < voucherno).OrderByDescending(p => p.VoucherNumber).FirstOrDefault(); ;
                    break;
                case "same":
                    v = FiscalCollection.FirstOrDefault(p => p.VoucherNumber == voucherno);
                    break;

            }

            if (v != null)
                voucherno = v.VoucherNumber;
            else if (key != "nextvouchernumber")
            {
                v = FiscalCollection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault();
            }
            next = FiscalCollection.Any(p => p.VoucherNumber > voucherno);
            previous = FiscalCollection.Any(p => p.VoucherNumber < voucherno);
            return v;
        }

    }
}
