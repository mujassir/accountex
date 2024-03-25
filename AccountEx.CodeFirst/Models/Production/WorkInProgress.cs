using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models
{
    using Common;
    using System;
    using System.Collections.Generic;

    public partial class WorkInProgress : BaseEntity
    {
        public int OrderNo { get; set; }
        public int? ProductId { get; set; }
        public VoucherType VoucherType { get; set; }
        public decimal Quantity { get; set; }
        public int AccountId { get; set; }
        public int FiscalId { get; set; }
        public string AccountTitle { get; set; }
        public int InvoiceNumber { get; set; }
        public int VoucherNumber { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public int QuantityTotal { get; set; }
        public decimal NetTotal { get; set; }
        public int FinishedQuantityTotal { get; set; }
        public decimal FinishedNetTotal { get; set; }
        public string Comments { get; set; }
        public System.DateTime Date { get; set; }
        public System.DateTime CreatedDate { get; set; }
        //public Nullable<DateTime> OrderDate { get;set; }
        public Nullable<int> OrderId { get; set; }
        public decimal TotalPorductionRecpeit { get; set; }
        public decimal RemaingInProcess { get; set; }
        public decimal TotalCharges { get; set; }
        public decimal Difference { get; set; }
        public virtual ICollection<WPItem> WPItems { get; set; }
    }
}