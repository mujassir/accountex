using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.Common;

namespace AccountEx.CodeFirst.Models.Vehicles
{
    public class VehicleVoucher : BaseEntity
    {
        public int InvoiceNumber { get; set; }
        public int VoucherNumber { get; set; }
        public int AccountId { get; set; }
        public string BookNo { get; set; }
        public int AccountId1 { get; set; }
        public Nullable<int> SaleId { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public VoucherType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public decimal NetTotal { get; set; }
        public string Comments { get; set; }
        public DateTime Date { get; set; }
        public int FiscalId { get; set; }
        public int VehicleId { get; set; }
        public string ChequeNumber { get; set; }
        public Nullable<DateTime> ChequeDate { get; set; }
        public decimal ForexPrice { get; set; }
        public decimal ExcRate { get; set; }
        public decimal AdjustmentAmount { get; set; }
        public Nullable<int> CurrencyId { get; set; }
        public string PaymentMode { get; set; }

    }
}
