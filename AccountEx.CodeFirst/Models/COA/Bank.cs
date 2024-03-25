namespace AccountEx.CodeFirst.Models
{
    public partial class Bank : BaseEntity
    {
       
        public int AccountId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string AccountNumber { get; set; }
        public string AccountTitle { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public string ContactPerson { get; set; }
        public string Branch { get; set; }
        public string BranchCode { get; set; }
        public string IBN { get; set; }
        public string SwiftCode { get; set; }
        public string Others { get; set; }
       
    }
}
