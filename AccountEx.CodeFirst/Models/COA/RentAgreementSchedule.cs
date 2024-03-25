using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models.COA
{
    public class RentAgreementSchedule : BaseEntity
    {
        public int RentAgreementId { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public Nullable<DateTime> ToDate { get; set; }
        public decimal Area { get; set; }
        public decimal Rate { get; set; }
        public string Remarks { get; set; }
        public decimal IncPercent { get; set; }
        public decimal IncAmount { get; set; }
        public decimal NetRent { get; set; }
        public decimal UCPercent { get; set; }
        public decimal UCAmount { get; set; }
        public decimal Total { get; set; }

        public bool IsRenewed { get; set; }
    }
}
