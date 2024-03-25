using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models.Vehicles
{
    public class VehicleSaleDetail : BaseEntity
    {
        public int VehicleSaleId { get; set; }
        public int InstalmentNo { get; set; }
        public decimal Amount { get; set; }
        public decimal RecievedAmount { get; set; }
        public DateTime InstallmentDate { get; set; }
        public Nullable<DateTime> ChequeDate { get; set; }
        public string ChequeNo { get; set; }
        public string BankName { get; set; }
        public bool IsRecieved { get; set; }
       
    }
}
