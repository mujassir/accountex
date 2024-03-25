namespace AccountEx.CodeFirst.Models
{
    public partial class AttributeType : BaseEntity
    {
        
        public string Name { get; set; }
        public string ControlType { get; set; }
        public string CssClass { get; set; }
        public string SizeId { get; set; }
        public string SizeName { get; set; }
        public string Data { get; set; }
       
    }
}
