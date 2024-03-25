namespace AccountEx.CodeFirst.Models
{
    public partial class Action : BaseEntity
    {
        //[Key]
        public string Key { get; set; }
        public string Description { get; set; }
        public bool Allowed { get; set; }
        public int SequenceNo { get; set; }
      
    }

  
}
