using System;
using System.ComponentModel.DataAnnotations;
namespace AccountEx.CodeFirst.Models
{
    public partial class ServiceItem : BaseEntity
    {
        public int ServiceId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int ItemId { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        
    }
}