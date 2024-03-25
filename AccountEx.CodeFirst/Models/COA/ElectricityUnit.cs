using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models.COA
{
    public class ElectricityUnit : BaseEntity
    {
        public int VoucherNo { get; set; }
        public DateTime Date { get; set; }
        public string Remarks { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int FiscalId { get; set; }

        public virtual ICollection<ElectricityUnitItem> ElectricityUnitItems { get; set; }
    }
}
