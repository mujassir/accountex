using System;

namespace AccountEx.CodeFirst.Models
{
    public partial class vw_RentAgreements
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int FiscalId { get; set; }
        public bool IsDeleted { get; set; }
        public int VoucherNumber { get; set; }
        public int TenantAccountId { get; set; }
        public int ShopId { get; set; }
        public string AccountCode { get; set; }
        public string TenantName { get; set; }
        public string Business { get; set; }
        public string Brand { get; set; }
        public string ContactNumber { get; set; }
        public int NumberOfPartners { get; set; }
        public string ShopNo { get; set; }
        public string ShopCode { get; set; }
        public string Block { get; set; }
        public Nullable<decimal> Area { get; set; }
        public string North { get; set; }
        public string East { get; set; }
        public string West { get; set; }
        public string South { get; set; }
        public byte Status { get; set; }
        public bool IsActive { get; set; }
    }
}
