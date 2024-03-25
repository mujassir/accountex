using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common.VehicleSystem
{

    public partial class PrintVehicleVoucherById
    {
        public DateTime Date { get; set; }
        public int VoucherNumber { get; set; }
        public string CustomerName { get; set; }
        public string ChassisNo { get; set; }
        public string RegNo { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string PaymentMode { get; set; }
        public byte TransactionType { get; set; }
        public decimal Amount { get; set; }

    }

}
