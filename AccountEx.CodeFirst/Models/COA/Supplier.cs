namespace AccountEx.CodeFirst.Models
{
    public partial class Supplier : BaseEntity
    {
        public int AccountId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ContactNumber { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string AccountTitle { get; set; }
        public string GST { get; set; }
        public string NTN { get; set; }
        public string Others { get; set; }
    }
}
