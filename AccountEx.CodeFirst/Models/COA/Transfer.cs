using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models.COA
{
    public class Transfer : BaseEntity
    {
        public int TransferByTenantAccountId { get; set; }
        public int TransferToTenantAccountId { get; set; }
        public int ShopId { get; set; }
        public int FiscalId { get; set; }
        public int VoucherNumber { get; set; }
        public decimal AgreementPD { get; set; }
        public decimal YearlyInc { get; set; }
        public string TransferChargesName { get; set; }
        public decimal TransferChargesAmount { get; set; }
        public decimal SellerFee { get; set; }
        public decimal BuyerFee { get; set; }
    }
}
