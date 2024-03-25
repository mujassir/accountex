namespace AccountEx.CodeFirst.Models
{
    public partial class Attribute : BaseEntity
    {
        public int AccountTypeId { get; set; }
        public int TypeId { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string TypeName { get; set; }
        public byte SequenceNumber { get; set; }
    }
}
