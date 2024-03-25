namespace AccountEx.CodeFirst.Models
{
    public partial class TenantPartner : BaseEntity
    {


        public int TenantId { get; set; }
        public string Name { get; set; }
        public string ContactNumber { get; set; }
        public string CNIC { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }
}
