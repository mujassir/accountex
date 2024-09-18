namespace AccountEx.CodeFirst.Models
{
    public class WareHouse : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
    }
}
