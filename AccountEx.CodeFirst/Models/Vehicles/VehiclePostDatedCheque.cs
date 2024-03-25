using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountEx.CodeFirst.Models;

namespace AccountEx.CodeFirst.Models.Vehicles
{

    public partial class VehiclePostDatedCheque : BaseEntity
    {


        public int VoucherNumber { get; set; }
        public string ChequeNo { get; set; }
        public DateTime ChequeDate { get; set; }
        public Nullable<int> VehicleSaleId { get; set; }
        public Nullable<DateTime> Date { get; set; }
        public string BankId { get; set; }
        public Nullable<Byte> Status { get; set; }
        public string Remarks { get; set; }
        public decimal Amount { get; set; }
        public int FiscalId { get; set; }


    }
}
