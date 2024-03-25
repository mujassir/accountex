using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models.Vehicles
{
    public class VehiclePayment : BaseEntity
    {
        public int VehicleSaleId { get; set; }
        public int VehicleId { get; set; }
        public decimal Amount { get; set; }
        public decimal Discount { get; set; }
        public DateTime RecievedDate { get; set; }
        public Nullable<int> RcvAccountId { get; set; }
        public int VoucherNo { get; set; }
        public string PaymentMode { get; set; }
        public string ChequeNo { get; set; }
        public string Remarks { get; set; }
    }
}
