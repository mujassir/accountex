//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Collections.Generic;

namespace AccountEx.CodeFirst.Models
{
    public partial class WheatPurchase:BaseEntity
    {
        public WheatPurchase()
        {
            this.WheatPurchaseItems = new HashSet<WheatPurchaseItem>();
        }
    
        
        public int InvoiceNumber { get; set; }
        public int FiscalId { get; set; }
        public int VoucherNumber { get; set; }
        public int BagsTotal { get; set; }
        public decimal StdWeightTotal { get; set; }
        public decimal BardanaWeightTotal { get; set; }
        public decimal GrossWeightTotal { get; set; }
        public int KhotTotal { get; set; }
        public int BardanaReturnTotal { get; set; }
        public int FreightTotal { get; set; }
        public decimal NetWeightTotal { get; set; }
        public decimal NetAmountTotal { get; set; }
        public string Comments { get; set; }
        public System.DateTime Date { get; set; }
        public bool IsGovt { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("PurchaseId")]
        public virtual ICollection<WheatPurchaseItem> WheatPurchaseItems { get; set; }
    }
}
