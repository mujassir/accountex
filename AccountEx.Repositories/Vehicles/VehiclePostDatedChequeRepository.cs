using AccountEx.CodeFirst.Models.Vehicles;
using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Repositories.Vehicles
{
    public class VehiclePostDatedChequeRepository : GenericRepository<VehiclePostDatedCheque>
    {
        public int GetNextVoucherNumber()
        {
            var maxnumber = 1001;
            if (!FiscalCollection.Any())
                return maxnumber;
            return FiscalCollection.OrderByDescending(p => p.VoucherNumber).FirstOrDefault().VoucherNumber + 1;
        }
    }
}
