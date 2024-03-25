using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common
{
    public class VehicleFollowUpExtra
    {
        public int SaleId { get; set; }
        public int VehicleId { get; set; }
        public int AccountId { get; set; }
        public string ChassisNo { get; set; }
        public string RegNo { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public string Color { get; set; }
        public string Name { get; set; }
        public Nullable<DateTime> InstallmentDate { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public int TotalInstallment { get; set; }
        public Decimal TotalAmount { get; set; }
        public byte RecoveryStatus { get; set; }
    }
}
