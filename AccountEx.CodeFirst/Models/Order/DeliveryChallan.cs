//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using AccountEx.Common;
namespace AccountEx.CodeFirst.Models
{
    public partial class DeliveryChallan : BaseEntity
    {
        public DeliveryChallan()
        {
            this.DCItems = new HashSet<DCItem>();
        }
        public int FiscalId { get; set; }
        public Nullable<int> CompanyPartnerId { get; set; }
        public int InvoiceNumber { get; set; }
        public int VoucherNumber { get; set; }
        public string VoucherCode { get; set; }
        public string OBNo { get; set; }
        public int OrderNo { get; set; }
        public Nullable<int> AccountId { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public string PartyAddress { get; set; }
        public Nullable<int> OrderId { get; set; }
        public Nullable<int> GatePassNo { get; set; }
        //public System.DateTime OrderDate { get; set; }
        public Nullable<int> ReceivingVoucher { get; set; }
        public VoucherType InvoiceTransactionType { get; set; }
        public VoucherType TransactionType { get; set; }
        public Nullable<int> QuantityTotal { get; set; }
        public decimal NTQtyTotal { get; set; }
        public string Comments { get; set; }
        public string BiltyNo { get; set; }
        public Nullable<System.DateTime> BiltyDate { get; set; }
        public string ShipVia { get; set; }
        public Nullable<int> ShipQty { get; set; }
        public string SaleType { get; set; }
        public byte Status { get; set; }
        public int RequisitionNo { get; set; }
        public Nullable<int> RequisitionId { get; set; }
        //public Nullable<System.DateTime> RequisitionDate { get; set; }
        public int WPNo { get; set; }
        public Nullable<System.DateTime> WPDate { get; set; }
        public System.DateTime Date { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string DriverName { get; set; }
        public string VehicleNo { get; set; }
        public string Instructions { get; set; }
        public string PartyPONumber { get; set; }
        public decimal GrossTotal { get; set; }
        public decimal GstAmountTotal { get; set; }
        public bool CashSale { get; set; }
        public string VehicleCode { get; set; }
        public int VehicleId { get; set; }
        public decimal TotalFreight { get; set; }
        public int SalesmanId { get; set; }
        public string SalesmanCode { get; set; }
        public string SalesmanName { get; set; }
        public string OrderedBy { get; set; }
        public string OrderedByContact { get; set; }
        public string DriverContact { get; set; }
        public string MOT { get; set; }
        public decimal Discount { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal NetTotal { get; set; }
        public int AuthLocationId { get; set; }
        public int WareHouseId { get; set; }
        public virtual ICollection<DCItem> DCItems { get; set; }
    }
}
