
namespace AccountEx.CodeFirst.Models
{
    using System;
    using System.Collections.Generic;
    public class ItemGroup : BaseEntity
    {
        public ItemGroup()
        {
            this.ItemGroupItems = new HashSet<ItemGroupItem>();
        }
        public string Name { get; set; }
        public byte GroupSubType { get; set; }
        public byte GroupType { get; set; }
        public Nullable<decimal> CommisionPercent { get; set; }
        public virtual ICollection<ItemGroupItem> ItemGroupItems { get; set; }
    }
}