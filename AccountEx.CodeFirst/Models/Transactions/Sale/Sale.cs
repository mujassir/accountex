using System;
using System.Collections.Generic;
using AccountEx.Common;

namespace AccountEx.CodeFirst.Models
{
    public partial class Sale : BaseEntity
    {
        public Sale()
        {
            SaleItems = new HashSet<SaleItem>();
            ServiceExpenses = new HashSet<ServiceExpense>();
            SaleServicesItems = new HashSet<SaleServicesItem>();
            InvoiceDcs = new HashSet<InvoiceDc>();
        }
        public int AccountId { get; set; }
        public int FiscalId { get; set; }
        public string AccountTitle { get; set; }
        public int InvoiceNumber { get; set; }
        public int VoucherNumber { get; set; }
        public int ManualVoucherNumber { get; set; }
        public string PONo { get; set; }
        public int DCNo { get; set; }
        public Nullable<int> DCId { get; set; }
        public Nullable<int> ReceivingVoucher { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public string PartyAddress { get; set; }
        public VoucherType TransactionType { get; set; }
        public decimal GrossTotal { get; set; }
        public decimal QuantityTotal { get; set; }
        public decimal NTQtyTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal PromotionTotal { get; set; }
        
        public decimal NetTotal { get; set; }
        public decimal ServicesExpencesTotal { get; set; }
        public string Comments { get; set; }
        public string BiltyNo { get; set; }
        public Nullable<DateTime> BiltyDate { get; set; }
        public string ShipViaCode { get; set; }
        public string ShipViaName { get; set; }
        public Nullable<int> ShipViaId { get; set; }
        public Nullable<int> ShipQty { get; set; }
        public byte SaleType { get; set; }
        public bool CashSale { get; set; }
        public int OtherAccountId { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<int> OldId { get; set; }
        public decimal GST { get; set; }
        public decimal GstAmountTotal { get; set; }
        public decimal Cutting { get; set; }
        public decimal Loading { get; set; }
        public decimal Carriage { get; set; }
        public decimal WHT { get; set; }
        public string VehicleNo { get; set; }
        public string VehicleCode { get; set; }
        public int VehicleId { get; set; }
        public decimal TotalFreight { get; set; }
        public bool Xmill { get; set; }
        public int SalesmanId { get; set; }
        public string SalesmanCode { get; set; }
        public string SalesmanName { get; set; }
        public string ContactPerson { get; set; }
        public string ContactTime { get; set; }
        public string Problem { get; set; }
        public int MachineId { get; set; }
        public string MachineCode { get; set; }
        public string MachineName { get; set; }
        public string ServicesRequired { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public int EmployeeId { get; set; }
        public string InitialObservation { get; set; }
        public string ServicesNeeded { get; set; }
        public string EstimatedTime { get; set; }
        public byte BehaviourLevel { get; set; }
        public byte ServicesLevel { get; set; }
        public string DelayTime { get; set; }
        public string TimeOfSupply { get; set; }
        public string SrNo { get; set; }
        public Nullable<decimal> PreviousReading { get; set; }
        public Nullable<decimal> CurrentReading { get; set; }
        public Nullable<decimal> Consumption { get; set; }
        public Nullable<int> SalemanId { get; set; }
        public Nullable<int> OrderTakerId { get; set; }
        public Nullable<int> TerritoryManagerId { get; set; }
        public decimal AdvanceTaxPercent { get; set; }
        public decimal AdvanceTaxTotal { get; set; }
        public string CustomerName { get; set; }
        public int ServiceSaleTaxNo { get; set; }
        public Nullable<int> CompanyPartnerId { get; set; }
        public bool IsCleared { get; set; }

        public Nullable<int> WarrantyNo { get; set; }
        public Nullable<int> TotalCartons { get; set; }
        public decimal NetOtherTotal { get; set; }
        public string PatientID { get; set; }
        public decimal WHTaxTotal { get; set; }
        public Nullable<DateTime> RequisitionDate { get; set; }
        public string RequisitionNo { get; set; }
        public Nullable<int> ReferenceId { get; set; }
        public bool IsLock { get; set; }
        public string Type { get; set; }
        public string PatientVoucherNumber { get; set; }
        public string PatientNo { get; set; }
        public virtual ICollection<SaleItem> SaleItems { get; set; }
        public virtual ICollection<SaleServicesItem> SaleServicesItems { get; set; }
        public virtual ICollection<ServiceExpense> ServiceExpenses { get; set; }
        public virtual ICollection<InvoiceDc> InvoiceDcs { get; set; }

    }
}
