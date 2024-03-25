using System;
using System.Collections.Generic;
using AccountEx.Common;

namespace AccountEx.CodeFirst.Models
{
    public partial class Voucher : BaseEntity
    {
        public Voucher()
        {
            VoucherItems = new HashSet<VoucherItem>();
        }

        public Nullable<int> AccountId { get; set; }
        public int FiscalId { get; set; }
        public string AccountTitle { get; set; }
        public int InvoiceNumber { get; set; }
        public int VoucherNumber { get; set; }
        public string BookNo { get; set; }
        public Nullable<int> ReceivingVoucher { get; set; }
        public Nullable<int> TerritoryId { get; set; }
        public Nullable<int> StationId { get; set; }
        public Nullable<int> BillingMonth { get; set; }
        public Nullable<int> BillingYear { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<int> CostCenterId { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public string PartyAddress { get; set; }
        public VoucherType TransactionType { get; set; }
        public decimal GrassTotal { get; set; }
        public int QuantityTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal NetTotal { get; set; }
        public Nullable<int> CompanyPartnerId { get; set; }
        public Nullable<int> VehicleId { get; set; }
        public string Comments { get; set; }
        public DateTime Date { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public bool IsPaid { get; set; }
        public bool IsFinal { get; set; }
        public decimal TotalPaid { get; set; }
        public virtual ICollection<VoucherItem> VoucherItems { get; set; }
    }
}
