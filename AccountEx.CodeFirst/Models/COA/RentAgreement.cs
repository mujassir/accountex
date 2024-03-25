using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models.COA
{
    public class RentAgreement : BaseEntity
    {
        public RentAgreement()
        {
            this.RentAgreementSchedules = new HashSet<RentAgreementSchedule>();
        }

        public int TenantAccountId { get; set; }
        public int ShopId { get; set; }
        public int FiscalId { get; set; }
        public int VoucherNumber { get; set; }
        public Nullable<DateTime> StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public string Single { get; set; }
        public string Partnership { get; set; }
        public decimal AgreementPD { get; set; }
        public decimal YearlyInc { get; set; }

        public decimal PossessionTotal { get; set; }
        public int PossessionInstallment { get; set; }
        public decimal PossessionPerInstallment { get; set; }
        public decimal AlreadyPaidPossessionAmount { get; set; }
        public decimal NotPaidPossessionAmount { get; set; }
        public decimal TotalPossessionAmount { get; set; }
        public decimal PossessionReceived { get; set; }
        public decimal PossessionBalance { get; set; }

        public decimal SecurityTotal { get; set; }
        public int SecurityInstallment { get; set; }
        public decimal SecurityPerInstallment { get; set; }
        public decimal SecurityMoneyAmount { get; set; }
        public decimal ReceivedSecurityAmount { get; set; }
        public decimal SecurityBalance { get; set; }

        public decimal RentInstallments { get; set; }
        public decimal MiscChangesInstallments { get; set; }
        public int ArrearMonth { get; set; }
        public int ArrearYear { get; set; }
        public decimal ArrearAmount { get; set; }
        public byte Status { get; set; }
        public decimal TransfeerFee { get; set; }
        public decimal ProcessingFee { get; set; }
        public Nullable<DateTime> TransfeerDate { get; set; }
        public string TransfeerRemarks { get; set; }
        public Nullable<int> TransfeerAgreementId { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<RentAgreementSchedule> RentAgreementSchedules { get; set; }

    }
}
