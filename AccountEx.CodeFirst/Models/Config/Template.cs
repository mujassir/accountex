namespace AccountEx.CodeFirst.Models
{
    public partial class Template : BaseEntity
    {
        
        public string Name { get; set; }
        public byte Type { get; set; }
        public string Contents { get; set; }

    }
}