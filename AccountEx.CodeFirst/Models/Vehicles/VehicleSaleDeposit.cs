using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models.Vehicles
{
    public class VehicleSaleDeposit : BaseEntity
    {
        public int SaleId { get; set; }
        public string PaymentMode { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
