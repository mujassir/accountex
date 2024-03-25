using AccountEx.CodeFirst.Models.COA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.DbMapping
{
    public class RentAgreementExtra
    {
        public RentAgreement RentAgreements { get; set; }
        public List<Challan> ChallanLiability { get; set; }

        public decimal ExtraSecurityAmount { get; set; }
        public decimal ExtraPossessionAmount { get; set; }

        public decimal ExtraPossessionType { get; set; }
        public decimal ExtraSecurityType { get; set; }

        public bool IsRenew { get; set; }
    }
    public class RentMonthYear
    {
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
