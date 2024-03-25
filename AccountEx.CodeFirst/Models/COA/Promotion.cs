
namespace AccountEx.CodeFirst.Models
{
    using System;
    using System.Collections.Generic;
    public class Promotion:BaseEntity
    {
        public Promotion() 
        {
            this.PromotionItems = new HashSet<PromotionItem>();
        }
        public string Name { get; set; }
        public int GroupId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int FiscalId { get; set; }
        public virtual ICollection<PromotionItem> PromotionItems { get; set; }
    }
}
