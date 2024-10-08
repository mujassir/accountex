//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AccountEx.CodeFirst.Models.CRM
{
    using Common;
    using System;
    using System.Collections.Generic;

    public partial class CRMSaleInvoiceItem : BaseEntity
    {
        public int CRMSaleInvoiceId { get; set; }
        public int SRNo { get; set; }
        public int CurrencyId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public CRMSaleType SaleType { get; set; }
        public int SalePersonId { get; set; }
        public int DivisionId { get; set; }

        
        public Nullable<int> ProjectId { get; set; }
    }
}
