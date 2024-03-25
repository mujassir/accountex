using System;

namespace AccountEx.CodeFirst.Models
{
     [Serializable]
    public partial class MenuItem : BaseEntity
    {
        public int ParentMenuItemId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string IconClass { get; set; }
        public bool IsMegaMenu { get; set; }
        public bool HasChild { get; set; }
        public byte SequenceNumber { get; set; }
        public bool IsVisible { get; set; }
        public string DataType { get; set; }
     
    }
}
