using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common.RentalAgreement
{
    public partial class GetRentAccountStatement
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public Nullable<DateTime> DueDate { get; set; }


        public decimal DueAmount { get; set; }
        public decimal SurCharge { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal LatePayment { get; set; }

    }
    public partial class GetRentAccountStatementV1
    {
        public DateTime Date { get; set; }
        public Nullable<DateTime> DueDate { get; set; }
        public Nullable<DateTime> ReceiveDate { get; set; }
        public decimal NetAmount { get; set; }
        public decimal SurCharge { get; set; }
        public bool IsReceived { get; set; }
        public decimal LatePayment { get; set; }

    }
}
