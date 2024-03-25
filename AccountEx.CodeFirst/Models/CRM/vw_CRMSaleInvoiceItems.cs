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
    using System;
    using System.Collections.Generic;
    
    public partial class vw_CRMSaleInvoiceItems
    {
        public int Id { get; set; }
        public System.DateTime Date { get; set; }
        public int CustomerId { get; set; }
        public string Customer { get; set; }
        public int ProductId { get; set; }
        public string Product { get; set; }
        public int DivisionId { get; set; }
        public string Division { get; set; }

        public int CategoryId { get; set; }
        public string Category { get; set; }
        public int InvoiceNumber { get; set; }
        public int VoucherNumber { get; set; }
        public Nullable<int> OGPNo { get; set; }
        public int RegionId { get; set; }
        public int SalePersonId { get; set; }
        public string SalePerson { get; set; }
        public int SN { get; set; }
        public int CurrencyId { get; set; }
        public string Currency { get; set; }

        
        public decimal Rate { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public string SaleType { get; set; }
        public byte SaleTypeId { get; set; }
        public int ProjectId { get; set; }
        public string Project { get; set; }
        public string DeliveryType { get; set; }

        
        public int CompanyId { get; set; }
        public int FiscalId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
