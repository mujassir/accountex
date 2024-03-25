

namespace AccountEx.CodeFirst.Models.CRM
{
    using System;
    using System.Collections.Generic;

    public partial class CRMImportRequisitionItem : BaseEntity
    {
        public int CRMImportRequisitionId { get; set; }
        public int SRNo { get; set; }
        public string HSCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int ItemId { get; set; }
        public Nullable<int> UOMId { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public Nullable<decimal> Size { get; set; }
        public Nullable<decimal> Nos { get; set; }
        public int CurrencyId { get; set; }
        public decimal ExcRate { get; set; }

        public decimal Rate { get; set; }
        public Nullable<decimal> Amount { get; set; }

    }
}
