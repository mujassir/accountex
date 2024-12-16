namespace AccountEx.CodeFirst.Models
{
    public class Machine : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int LocationId { get; set; }
        public bool IsDefault { get; set; }
    }
}
