using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.Common;

namespace AccountEx.CodeFirst.Models.Vehicles
{
    public class VehicleSale : BaseEntity
    {
        public VehicleSale()
        {
            VehicleSaleDetails = new HashSet<VehicleSaleDetail>();
            VehicleSaleDeposits = new HashSet<VehicleSaleDeposit>();
            SaleDocuments = new HashSet<SaleDocument>();
        }
        public int InvoiceNumber { get; set; }
        public int VoucherNumber { get; set; }
        public DateTime Date { get; set; }
        public Nullable<DateTime> InstalmentStartDate { get; set; }
        public int AccountId { get; set; }
        public int VehicleId { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public string FileUrl { get; set; }
        public int FiscalId { get; set; }
        public bool IsFinal { get; set; }
        public VoucherType SaleType { get; set; }
        public VoucherType TransactionType { get; set; }
        public decimal SalePrice { get; set; }
        public bool IsTrackerAdded { get; set; }
        public Nullable<int> TrackerSupplierId { get; set; }
        public decimal TrackerSellingPrice { get; set; }
        public decimal TrackerSalePrice { get; set; }
        public decimal TrackerReceivedAmount { get; set; }
        public decimal TrackerBalanceAmount { get; set; }
        public string TrackerPaymentMode { get; set; }
        public Nullable<int> TrackerAccountId { get; set; }
        public bool IsInsurenceAdded { get; set; }
        public Nullable<int> InsurenceSupplierId { get; set; }
        public decimal InsurenceValue { get; set; }
        public decimal InsurencePercent { get; set; }
        public decimal InsurenceSellingPrice { get; set; }
        public decimal InsurenceSalePrice { get; set; }
        public decimal InsurenceReceivedAmount { get; set; }
        public decimal InsurenceBalanceAmount { get; set; }
        public string InsurancePaymentMode { get; set; }
        public Nullable<int> InsuranceAccountId { get; set; }
        public decimal LogBookFee { get; set; }
        public decimal LevyPercent { get; set; }
        public decimal StampDutyAmount { get; set; }
        public decimal Received { get; set; }
        public decimal Advance { get; set; }
        public decimal Balance { get; set; }
        public Nullable<int> NoOfInstallments { get; set; }
        public string PaymentMode { get; set; }
        public Nullable<int> RcvAccountId { get; set; }
        public string ChequeNo { get; set; }
        public byte RecoveryStatus { get; set; }
        public Nullable<int> TradeInVehicleId { get; set; }
        public decimal TradeInPrice { get; set; }
        public bool IsTradeIn { get; set; }
        public string Comments { get; set; }
        public string Notes { get; set; }
        public bool IsDelivered { get; set; }
        public Nullable<System.DateTime> DeliveredDate { get; set; }
        public int BranchId { get; set; }
        public Nullable<int> DeliveredBy { get; set; }
        public string AgreementRemarks { get; set; }
        public string DeliveredRemarks { get; set; }
        public bool IsVoid { get; set; }
        public Nullable<System.DateTime> VoidDate { get; set; }
        public Nullable<int> VoidBy { get; set; }
        public bool IsTradeUnitReturned { get; set; }
        public decimal DepreciationAmount { get; set; }
        public decimal ReturnAmount { get; set; }
        public string ReturnPaymentMode { get; set; }
        public Nullable<int> ReturnAccountId { get; set; }
        public string CancellRemakrs { get; set; }
        public Nullable<DateTime> CancellationDate { get; set; }
        public virtual ICollection<VehicleSaleDetail> VehicleSaleDetails { get; set; }
        [ForeignKey("SaleId")]
        public virtual ICollection<VehicleSaleDeposit> VehicleSaleDeposits { get; set; }
        [ForeignKey("SaleId")]
        public virtual ICollection<SaleDocument> SaleDocuments { get; set; }
    }
}
