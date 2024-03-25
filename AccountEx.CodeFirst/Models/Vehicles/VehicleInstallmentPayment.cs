using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models.Vehicles
{
    public class VehicleInstallmentPayment : BaseEntity
    {
        public int VehicleSaleId { get; set; }
        public int VehicleSaleDetailId { get; set; }
        public int VehicleId { get; set; }
        public int InstalmentNo { get; set; }
        public decimal Amount { get; set; }
        public decimal Discount { get; set; }
        public DateTime RecievedDate { get; set; }
        public Nullable<int> VehiclePaymentId { get; set; }
        public Nullable<int> RcvAccountId { get; set; }
        public int VoucherNo { get; set; }
        public string PaymentMode { get; set; }
        public string ChequeNo { get; set; }
        public string Remarks { get; set; }
    }
}
