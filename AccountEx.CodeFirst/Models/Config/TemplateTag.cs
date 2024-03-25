namespace AccountEx.CodeFirst.Models
{
    public partial class TemplateTag : BaseEntity
    {
        
        public string ColumnName { get; set; }
        public string TableName { get; set; }
        public string Tag { get; set; }
        public string Lable { get; set; }
        public byte TagType { get; set; }
        public string Contents { get; set; }

    }
}