using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common
{
    public class GetSecuirtyPossessionAccountStatementExtra
    {
        public int NoOfInstallment { get; set; }

        public decimal DueAmount { get; set; }
        public DateTime DueDate { get; set; }
        public Nullable<DateTime> PaidOn { get; set; }
        public int RcvNo { get; set; }
        public int BillNo { get; set; }
        public decimal OutStanding { get; set; }
    }
    public class GetSecuirtyPossessionAccountStatementExtra1
    {

        public decimal DueAmount { get; set; }
        public DateTime? DueDate { get; set; }
        public Nullable<DateTime> PaidOn { get; set; }
        public int BillNo { get; set; }
        public decimal NetAmount { get; set; }
        public bool IsReceived { get; set; }
    }
}
