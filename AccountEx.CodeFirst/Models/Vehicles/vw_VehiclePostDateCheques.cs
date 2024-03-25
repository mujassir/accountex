using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models.Vehicles
{
    public class vw_VehiclePostDateCheques 
    {
        public int Id { get; set; }
        public string ChequeNo { get; set; }
        public System.DateTime ChequeDate { get; set; }
        public int VoucherNumber { get; set; }
        public string AccountName { get; set; }
        public string RegNo { get; set; }
        public decimal Amount { get; set; }
        public string ChassisNo { get; set; }
        public string Bank { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public int CompanyId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
