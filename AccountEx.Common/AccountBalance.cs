using System.Collections.Generic;

namespace AccountEx.Common
{
    public class AccountBalance
    {
        public string AccountTitle { get; set; }
        public string TotalDebit { get; set; }
        public string TotalCredit { get; set; }
        public string Difference { get; set; }
        public List<TrialBalanceLine> Records { get; set; }

    }
    public class TrialBalanceLine
    {
        public string AccountTitle { get; set; }
        public string Debit { get; set; }
        public string Credit { get; set; }
        public decimal Balance { get; set; }
        public decimal OpeningBalance { get; set; }

        
    }

}
