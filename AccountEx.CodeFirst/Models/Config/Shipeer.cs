namespace AccountEx.CodeFirst.Models
{
    public partial class Shipeer : BaseEntity
    {
        
        public string Name { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        
    }
}