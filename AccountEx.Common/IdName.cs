namespace AccountEx.Common
{
   public class IdName
    {
       public int Id { get; set; }
       public string Name { get; set; }
    }
    public class KlassVoucherExtra
    {
        public int VoucherNumber { get; set; }
        public VoucherType TransactionType { get; set; }
    }
    public class TemplateTagExtra
    {
        public string Name { get; set; }
        public string Tag { get; set; }
        public string Lable { get; set; }
        public byte TagType { get; set; }
    }

}
