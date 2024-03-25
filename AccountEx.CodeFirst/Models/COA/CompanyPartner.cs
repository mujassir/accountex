namespace AccountEx.CodeFirst.Models
{
    public partial class CompanyPartner : BaseEntity
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string NTNNo { get; set; }
        public string GSTNo { get; set; }
    }
}
