
namespace AccountEx.CodeFirst.Models
{
    using System;
    using System.Collections.Generic;
    using AccountEx.Common;

    public partial class Requisition : BaseEntity
    {
        public Requisition()
        {
            this.RequisitionItems = new HashSet<RequisitionItem>();
        }
        public int VoucherNumber { get; set; }
        public int FiscalId { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        //public Nullable<System.DateTime> OrderDate { get; set; }
        //public Nullable<System.DateTime> DeliveryDate { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public VoucherType TransactionType { get; set; }
        public byte Status { get; set; }
        public virtual ICollection<RequisitionItem> RequisitionItems { get; set; }
    }
}
