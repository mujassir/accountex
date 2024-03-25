using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.CodeFirst.Models.Views
{
    public class vw_TenantAgreements
    {
        public int Id { get; set; }
        public string TenantCode { get; set; }
        public string TenantAccountName { get; set; }
        public string BlockName { get; set; }
        public int TenantAccountId { get; set; }
        public int ShopId { get; set; }
        public string ShopCode { get; set; }
        public string ShopNo { get; set; }
        public Nullable<int> RentAgreementId { get; set; }
        public int CompanyId { get; set; }
        public byte Status { get; set; }
        public bool IsDeleted { get; set; }
    }
}
