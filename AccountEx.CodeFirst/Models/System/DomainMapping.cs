namespace AccountEx.CodeFirst.Models
{
    public partial class DomainMapping
    {
       
        public int Id { get; set; }
        public string Domain { get; set; }
        public string CompanyName { get; set; }
        public int CompanyId { get; set; }
        
    }
}
