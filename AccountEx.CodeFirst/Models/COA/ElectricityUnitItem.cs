using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models.COA
{
    public class ElectricityUnitItem : BaseEntity
    {
        public int ElectricityUnitId { get; set; }
        public int RentAgreementId { get; set; } 
        public int ShopId { get; set; }
        public string ShopNo { get; set; }
        public int TenantAccountId { get; set; }
        public string TenantAccountName { get; set; }  
        public Nullable<int> Unit { get; set; }
        public Nullable<int> PreviousReading { get; set; }
        public Nullable<int> CurrentReading { get; set; }
        public Nullable<decimal> ElectriciyUnitCharges { get; set; }
        public Nullable<decimal> Amount { get; set; }
    }
}
