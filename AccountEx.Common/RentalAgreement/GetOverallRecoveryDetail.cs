using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common.RentalAgreement
{
    public partial class BillsIssuesToTenants
    {
        public string ShopNo { get; set; }
        public string Tenant { get; set; }
        public string BlockName { get; set; }

        public decimal MonthlyRent { get; set; }
        public decimal UCPercent { get; set; }
        public decimal ElectricityCharges { get; set; }
        public decimal MiscCharges { get; set; }

        public decimal PossessionCharges { get; set; }
        public decimal SecurityMoney { get; set; }



    }

    public partial class GetOverallRecoveryDetail
    {
        public string ShopNo { get; set; }
        public string Tenant { get; set; }
        public string BlockName { get; set; }

        public decimal RentUCEArrears { get; set; }
        public decimal PossessionArrears { get; set; }
        public decimal SecurityArrears { get; set; }
        public decimal RentUCE { get; set; }

        public decimal PossessionCharges { get; set; }
        public decimal SecurityMoney { get; set; }
        public decimal RentUCEReceived { get; set; }
        public decimal PossessionReceived { get; set; }
        public decimal SecurityReceived { get; set; }
    }
}
