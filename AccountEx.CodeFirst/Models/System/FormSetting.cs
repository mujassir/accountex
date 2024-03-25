namespace AccountEx.CodeFirst.Models
{
    public partial class FormSetting : BaseEntity
    {
       
        public string VoucherType { get; set; }
        public string KeyName { get; set; }
        public string Value { get; set; }
        public bool UseCOA { get; set; }
        public string AccountTitle { get; set; }
    }
}
