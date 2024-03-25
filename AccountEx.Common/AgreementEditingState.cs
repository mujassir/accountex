using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common
{
    public class AgreementEditingState
    {
        public int LastRentMonth { get; set; }
        public int LastRentYear { get; set; }
        public int SecurityAmountReceived { get; set; }
        public int SecurityInstallmentsReceived { get; set; }
        public int PossessionAmountReceived { get; set; }
        public int PossessionInstallmentsReceived { get; set; }

    }
}
