namespace AccountEx.CodeFirst.Models
{
    public partial class Asset : BaseEntity
    {
       
        public int AccountId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string AssetDetail { get; set; }
        public string Value { get; set; }
        public string Others { get; set; }
        public string AssetType { get; set; }
    }
}
