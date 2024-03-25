using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.Common;

namespace AccountEx.CodeFirst.Models.COA
{
    public class Challan : BaseEntity
    {
        public Challan()
        {
            this.ChallanItems = new HashSet<ChallanItem>();
        }

        public int FiscalId { get; set; }
        public VoucherType TransactionType { get; set; }
        public byte EntryType { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int ToMonth { get; set; }
        public int ToYear { get; set; }
        public Nullable<int> ChargeId { get; set; }
        public Nullable<int> TenantAccountId { get; set; }
        public Nullable<DateTime> DueDate { get; set; }
        public Nullable<DateTime> ReceiveDate { get; set; }
        public Nullable<int> RentAgreementId { get; set; }
        public string FileUrl { get; set; }

        public int VoucherNumber { get; set; }

        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }

        public int InvoiceNumber { get; set; }
        public int NumberOfInstallment { get; set; }
        public bool IsReceived { get; set; }
        public bool IsOpening { get; set; }
        public bool IsAuto { get; set; }

        public int RcvNo { get; set; }
        public decimal NetAmount { get; set; }
        public byte AdjustmentType { get; set; }
        public decimal AdjustmentAmount { get; set; }
        public decimal GrandTotal { get; set; }


        public virtual ICollection<ChallanItem> ChallanItems { get; set; }

    }
}


